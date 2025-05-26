namespace MerchStore.WebUI.Helpers
{
    public static class HoverImageHelper
    {
        public static string GetHoverImageUrl(string productName)
        {
            if (productName.Contains("Litter cute Mug", StringComparison.OrdinalIgnoreCase))
                return "/img/merchimug02.png";

            if (productName.Contains("Laptop Sticker Pack", StringComparison.OrdinalIgnoreCase))
                return "/img/sticker01.png";

            if (productName.Contains("Hoodie", StringComparison.OrdinalIgnoreCase))
                return "/img/hoodie.png";

            return "/img/canvas01.png";
        }
    }
}
