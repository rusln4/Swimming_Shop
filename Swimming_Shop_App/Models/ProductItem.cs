using System.ComponentModel;

namespace Swimming_Shop_App
{
    public class ProductItem : INotifyPropertyChanged
    {
        public int IdProduct { get; set; }
        public string NameProduct { get; set; } = string.Empty;
        public string DescriptionProduct { get; set; } = string.Empty;
        public decimal PriceProduct { get; set; }
        public int CountProduct { get; set; }
        public int IdCategoryProduct { get; set; }
        public string PictureProduct { get; set; } = string.Empty;

        private int _cartQuantity;
        public int CartQuantity
        {
            get => _cartQuantity;
            set
            {
                if (_cartQuantity != value)
                {
                    _cartQuantity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CartQuantity)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}