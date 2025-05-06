using System.Text;
using System.Text.Json;

namespace MerchStore.WebUI.Infrastructure;


// A JSON naming policy that converts property names to snake_case.

public class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
{
    
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        var builder = new StringBuilder();

        for (int i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]))
            {
                builder.Append('_');
            }

            builder.Append(char.ToLowerInvariant(name[i]));
        }

        return builder.ToString();
    }
}