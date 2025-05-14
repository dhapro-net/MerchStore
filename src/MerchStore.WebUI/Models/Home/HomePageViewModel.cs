namespace MerchStore.WebUI.Models.Home
{
    public class HomePageViewModel
    {
        // Reuse existing model for products
        public List<Catalog.ProductCardViewModel> PopularProducts { get; set; } = new();
        
        // Other sections specific to the home page
        public List<HeroSlideViewModel> HeroSlides { get; set; } = new();
        public NewsletterViewModel Newsletter { get; set; } = new();
        public List<ShippingFeatureViewModel> ShippingFeatures { get; set; } = new();
    }
    
    public class HeroSlideViewModel
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string Heading { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ButtonText { get; set; } = string.Empty;
        public string ButtonUrl { get; set; } = string.Empty;
    }
    
    public class NewsletterViewModel
    {
        public string Heading { get; set; } = string.Empty;
        public string Subheading { get; set; } = string.Empty;
    }
    
    public class ShippingFeatureViewModel
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}


// detta view for pickup all views model also reuse form product cnard view model. 