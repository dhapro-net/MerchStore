namespace MerchStore.WebUI.Helpers
{
    public static class HoverImageHelper
    {
        public static string GetHoverImageUrl(string productName)
        {
            if (productName.Contains("Litter cute Mug", StringComparison.OrdinalIgnoreCase))
                return "https://somethingpicture20250509.blob.core.windows.net/picture/merchimug02.png";

            if (productName.Contains("Laptop Sticker Pack", StringComparison.OrdinalIgnoreCase))
                return "https://somethingpicture20250509.blob.core.windows.net/picture/sticker01.png";

            if (productName.Contains("Canvas for decorating", StringComparison.OrdinalIgnoreCase))
                return "https://somethingpicture20250509.blob.core.windows.net/picture/canvas03.png";

            if (productName.Contains("dragon Coaster", StringComparison.OrdinalIgnoreCase))
                return "https://somethingpicture20250509.blob.core.windows.net/picture/coaster01.png";

            if (productName.Contains("ToteBag Cute bag", StringComparison.OrdinalIgnoreCase))
                return "https://somethingpicture20250509.blob.core.windows.net/picture/totebag08.png";

            if (productName.Contains("Angry ToteBag", StringComparison.OrdinalIgnoreCase))
                return "https://somethingpicture20250509.blob.core.windows.net/picture/totebag04.png";

            return "/img/canvas01.png";
        }
    }
}
