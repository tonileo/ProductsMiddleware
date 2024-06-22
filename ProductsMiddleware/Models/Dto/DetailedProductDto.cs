namespace ProductsMiddleware.Models.Dto
{
    public class DetailedProductDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public double DiscountPercentage { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public List<string> Tags { get; set; }
        public string Brand { get; set; }
        public string Sku { get; set; }
        public double Weight { get; set; }
        public DetailedDimensionsDto Dimensions { get; set; }
        public string WarrantyInformation { get; set; }
        public string ShippingInformation { get; set; }
        public string AvailabilityStatus { get; set; }
        public List<DetailedProductReviewDto> Reviews { get; set; }
        public string ReturnPolicy { get; set; }
        public int MinimumOrderQuantity { get; set; }
        public DetailedMetaDto Meta { get; set; }
        public List<string> Images { get; set; }
        public string Thumbnail { get; set; }
    }
}
