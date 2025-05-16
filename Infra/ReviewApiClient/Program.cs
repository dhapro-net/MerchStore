using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            // Use today's date to construct the Function App name
            
            string functionAppName = $"ReviewApiv2250506";
            string functionKey = "ItHurtWhenIPee";

            // You can either use a random GUID or a specific product ID
            // For a random GUID:
            string productId = Guid.NewGuid().ToString();

            // Or use a specific product ID to get consistent results:
            // string productId = "3fa85f64-5717-4562-b3fc-2c963f66afa6";

            // Create HttpClient
            using var client = new HttpClient();

            Console.WriteLine("Review API Client Test");
            Console.WriteLine("=====================");
            Console.WriteLine($"Product ID: {productId}");
            Console.WriteLine();

            // METHOD 1: API Key in Query String
            string urlWithQueryParam = $"https://{functionAppName}.azurewebsites.net/api/products/{productId}/reviews?code={functionKey}";
            Console.WriteLine("METHOD 1: API Key in Query String");
            Console.WriteLine($"Requesting from: {urlWithQueryParam}");
            Console.WriteLine();

            // Make the first request with query string
            var responseWithQueryParam = await client.GetStringAsync(urlWithQueryParam);

            // Output the raw response
            Console.WriteLine("Response from query param method:");
            Console.WriteLine(responseWithQueryParam);
            Console.WriteLine();

            // Parse and display the response in a more readable format
            var options = new JsonSerializerOptions { WriteIndented = true };
            var formattedJsonQueryParam = JsonSerializer.Serialize(
                JsonSerializer.Deserialize<JsonElement>(responseWithQueryParam),
                options);

            Console.WriteLine("Formatted response (query param):");
            Console.WriteLine(formattedJsonQueryParam);
            Console.WriteLine();
            Console.WriteLine("===========================================");
            Console.WriteLine();

            // METHOD 2: API Key in Header
            string urlWithHeader = $"https://{functionAppName}.azurewebsites.net/api/products/{productId}/reviews";
            Console.WriteLine("METHOD 2: API Key in Header");
            Console.WriteLine($"Requesting from: {urlWithHeader}");
            Console.WriteLine($"With header: x-functions-key: {functionKey}");
            Console.WriteLine();

            // Create a new request message with the header
            var request = new HttpRequestMessage(HttpMethod.Get, urlWithHeader);
            request.Headers.Add("x-functions-key", functionKey);

            // Send the request and get the response
            var headerResponse = await client.SendAsync(request);
            headerResponse.EnsureSuccessStatusCode();

            var responseWithHeader = await headerResponse.Content.ReadAsStringAsync();

            // Output the raw response
            Console.WriteLine("Response from header method:");
            Console.WriteLine(responseWithHeader);
            Console.WriteLine();

            // Parse and display the response in a more readable format
            var formattedJsonHeader = JsonSerializer.Serialize(
                JsonSerializer.Deserialize<JsonElement>(responseWithHeader),
                options);

            Console.WriteLine("Formatted response (header):");
            Console.WriteLine(formattedJsonHeader);
            Console.WriteLine();
            Console.WriteLine("===========================================");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner error: {ex.InnerException.Message}");
            }
        }
    }
}