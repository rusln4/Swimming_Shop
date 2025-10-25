using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net.Http;
using System.Net.Http.Json;
using System.IO;

namespace Swimming_Shop_App.Pages
{
    public partial class CatalogPage : Page
    {
        

        private readonly ObservableCollection<ProductItem> _allProducts = new ObservableCollection<ProductItem>();
        private readonly ObservableCollection<ProductItem> _searchResults = new ObservableCollection<ProductItem>();

        public ObservableCollection<ProductItem> Products { get; } = new ObservableCollection<ProductItem>();
        private bool _isSearchMode = false;

        public CatalogPage()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += async (s, e) =>
            {
                await LoadProductsAsync();
                ApplyLocalFilters();
                UpdateEmptyState();
            };
        }

        private HttpClient CreateHttp()
        {
            var handler = new HttpClientHandler { UseProxy = false, Proxy = null };
            return new HttpClient(handler) { BaseAddress = new Uri(ApiConfig.BaseUrl) };
        }

        private async Task<bool> CheckServerAsync(HttpClient http)
        {
            try
            {
                var response = await http.GetAsync("api/products");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task LoadProductsAsync()
        {
            ShowLoading(true);
            try
            {
                using var http = CreateHttp();

                if (!await CheckServerAsync(http))
                {
                    MessageBox.Show("Сервер недоступен.");
                    return;
                }

                var items = await http.GetFromJsonAsync<List<ProductItem>>("api/products") ?? new List<ProductItem>();

                _allProducts.Clear();
                foreach (var p in items)
                {
                    p.PictureProduct = BuildImageUrl(p.PictureProduct);
                    _allProducts.Add(p);
                }

                _isSearchMode = false;

                ApplyLocalFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
            finally
            {
                ShowLoading(false);
                UpdateEmptyState();
            }
        }

        private async Task PerformSearchAsync()
        {
            var query = (SearchText?.Text ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(query))
            {
                _isSearchMode = false;
                ApplyLocalFilters();
                UpdateEmptyState();
                return;
            }

            ShowLoading(true);
            try
            {
                using var http = CreateHttp();

                if (!await CheckServerAsync(http))
                {
                    MessageBox.Show("Сервер недоступен.");
                    return;
                }

                var url = $"api/products/search?name={Uri.EscapeDataString(query)}";
                var response = await http.GetAsync(url);

                List<ProductItem> results;
                if (response.IsSuccessStatusCode)
                {
                    results = await response.Content.ReadFromJsonAsync<List<ProductItem>>() ?? new List<ProductItem>();
                }
                else
                {
                    var all = await http.GetFromJsonAsync<List<ProductItem>>("api/products") ?? new List<ProductItem>();
                    results = all
                        .Where(p => p.NameProduct?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                        .ToList();
                }

                _searchResults.Clear();
                foreach (var p in results)
                {
                    p.PictureProduct = BuildImageUrl(p.PictureProduct);
                    _searchResults.Add(p);
                }

                _isSearchMode = true;
                ApplyLocalFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ShowLoading(false);
                UpdateEmptyState();
            }
        }

        // Превращаем путь из БД в пригодный для Image.Source URL
        private string BuildImageUrl(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;

            var raw = path.Trim().Trim('"');
            var normalized = raw.Replace("\\", "/");

            // Абсолютный URL уже пригоден
            if (normalized.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                normalized.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return normalized;

            // Абсолютный путь на диске: если файл существует, вернем file:///
            try
            {
                // Преобразуем обратно к Windows-формату для проверки File.Exists
                var windowsPath = raw;
                if (File.Exists(windowsPath))
                {
                    var uri = new Uri(windowsPath, UriKind.Absolute);
                    return uri.AbsoluteUri; // file:///C:/Users/...
                }
            }
            catch
            {
                // игнорируем ошибки построения Uri для нестандартных путей
            }

            // Попытка отдать через сервер: берем имя файла и обращаемся к /images
            var fileName = Path.GetFileName(raw);
            if (!string.IsNullOrEmpty(fileName))
            {
                return $"{ApiConfig.BaseUrl.TrimEnd('/')}/images/{Uri.EscapeDataString(fileName)}";
            }

            // Если содержит сегмент images/, используем его как относительный путь
            var idx = normalized.IndexOf("images/", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                var rel = normalized.Substring(idx);
                return $"{ApiConfig.BaseUrl.TrimEnd('/')}/{rel}";
            }

            // Ничего подходящего — вернем как есть (WPF попробует интерпретировать локальный путь)
            return raw;
        }

        private void ApplyLocalFilters()
        {
            var source = _isSearchMode ? _searchResults : _allProducts;

            var query = (SearchText?.Text ?? string.Empty).Trim();
            var hasQuery = !string.IsNullOrEmpty(query);

            var onlyAvailable = AvailableOnlyCheck?.IsChecked == true;

            decimal? minPrice = null, maxPrice = null;
            if (decimal.TryParse(MinPriceText?.Text, out var min)) minPrice = min;
            if (decimal.TryParse(MaxPriceText?.Text, out var max)) maxPrice = max;

            var filtered = source.Where(p =>
            {
                if (hasQuery && !p.NameProduct.Contains(query, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (onlyAvailable && p.CountProduct <= 0)
                    return false;

                if (minPrice.HasValue && p.PriceProduct < minPrice.Value)
                    return false;

                if (maxPrice.HasValue && p.PriceProduct > maxPrice.Value)
                    return false;

                return true;
            });

            Products.Clear();
            foreach (var p in filtered)
                Products.Add(p);
        }

        private void ShowLoading(bool isLoading)
        {
            
        }

        private void UpdateEmptyState()
        {
            
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await PerformSearchAsync();
        }

        private async void SearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await PerformSearchAsync();
            }
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            if (SearchText != null)
                SearchText.Text = string.Empty;

            _isSearchMode = false;
            ApplyLocalFilters();
            UpdateEmptyState();
        }

        private void AvailabilityChanged(object sender, RoutedEventArgs e)
        {
            ApplyLocalFilters();
            UpdateEmptyState();
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            ApplyLocalFilters();
            UpdateEmptyState();
        }

        private async void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            if (SearchText != null) SearchText.Text = string.Empty;
            if (AvailableOnlyCheck != null) AvailableOnlyCheck.IsChecked = false;
            if (MinPriceText != null) MinPriceText.Text = string.Empty;
            if (MaxPriceText != null) MaxPriceText.Text = string.Empty;

            _isSearchMode = false;

            await LoadProductsAsync();
            UpdateEmptyState();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.Tag as ProductItem;
            if (item != null)
            {
                MessageBox.Show($"Товар '{item.NameProduct}' добавлен в корзину.");
            }
        }
    }
}