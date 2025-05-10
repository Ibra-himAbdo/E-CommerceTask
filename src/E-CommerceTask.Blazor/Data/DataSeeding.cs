using E_CommerceTask.Shared.Models;

namespace E_CommerceTask.Blazor.Data;

public static class DataSeeding
{
    public static async Task SeedDefaultData(ApplicationDbContext context)
    {
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<ProductCategory>
            {
                new() { Name = "PlayStation", Id = ObjectId.GenerateNewId()},
                new() { Name = "Xbox", Id = ObjectId.GenerateNewId() },
                new() { Name = "Nintendo Switch" , Id = ObjectId.GenerateNewId()},
                new() { Name = "PC" , Id = ObjectId.GenerateNewId()},
                new() { Name = "Mobile" , Id = ObjectId.GenerateNewId()},
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        if (!await context.Products.AnyAsync())
        {
            var random = new Random();
            var categories = await context.Categories.ToListAsync();

            var products = new List<Product>();
            for (var i = 1; i <= 120; i++)
            {
                var category = categories[random.Next(categories.Count)];

                var productName = $"Game {i}";

                products.Add(new Product
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = productName,
                    NormalizedName = productName.ToUpperInvariant(),
                    Description = $"Description for {productName}",
                    Price = Math.Round((decimal)(random.NextDouble() * 100), 2),
                    ImageUrl = $"https://fakeimg.pl/600x400/?retina=1&text={productName}",
                    Quantity = random.Next(1, 10),
                    Rate = Math.Round(random.NextDouble() * 5, 1),
                    DateAdded = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                    CategoryId = category.Id
                });
            }

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync())
        {
            var admin = new User()
            {
                Id = ObjectId.GenerateNewId(),
                Email = "admin@me.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@me.com"),
                Role = "Admin"
            };

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();

        }
    }
}