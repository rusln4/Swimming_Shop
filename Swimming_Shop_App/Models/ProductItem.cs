namespace Swimming_Shop_App
{
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
}