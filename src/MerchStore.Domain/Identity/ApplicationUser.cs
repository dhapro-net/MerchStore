using AspNetCore.Identity.Mongo.Model;

namespace MerchStore.Domain.Identity;

public class ApplicationUser : MongoUser
{
    public string? DisplayName { get; set; }
}