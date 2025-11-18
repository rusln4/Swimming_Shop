using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Linq;

namespace Swimming_Shop_App
{
    public class ApiClient
    {
        private readonly HttpClient _http;

        public ApiClient()
        {
            var handler = new HttpClientHandler { UseProxy = false, Proxy = null };
            _http = new HttpClient(handler) { BaseAddress = new Uri(ApiConfig.BaseUrl) };
        }

      
        public async Task<List<ProductItem>> GetProductsAsync()
        {
            var items = await _http.GetFromJsonAsync<List<ProductItem>>("api/products");
            return items ?? new List<ProductItem>();
        }

        public async Task<List<ProductItem>> GetProductsByCategoryAsync(int categoryId)
        {
            var items = await _http.GetFromJsonAsync<List<ProductItem>>($"api/products/category/{categoryId}");
            return items ?? new List<ProductItem>();
        }

       
        public async Task<List<CategoryItem>> GetCategoriesAsync()
        {
            var cats = await _http.GetFromJsonAsync<List<CategoryItem>>("api/categories");
            return cats ?? new List<CategoryItem>();
        }

        public async Task<List<ProductItem>> SearchProductsAsync(string name)
        {
            try
            {
                var url = $"api/products/search?name={Uri.EscapeDataString(name)}";
                var response = await _http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var items = await response.Content.ReadFromJsonAsync<List<ProductItem>>();
                    return items ?? new List<ProductItem>();
                }
                else
                {
                    throw new HttpRequestException($"{response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception)
            {
                var all = await GetProductsAsync();
                return all
                    .Where(p => p.NameProduct?.Contains(name, StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();
            }
        }

        public async Task<bool> CheckServerAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/products");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public class ProductItem
    {
        public int IdProduct { get; set; }
        public string NameProduct { get; set; } = string.Empty;
        public string DescriptionProduct { get; set; } = string.Empty;
        public decimal PriceProduct { get; set; }
        public int CountProduct { get; set; }
        public int IdCategoryProduct { get; set; }
        public string PictureProduct { get; set; } = string.Empty;
    }

    
    public class CategoryItem
    {
        public int IdCategories { get; set; }
        public string NameCategory { get; set; } = string.Empty;
    }
}