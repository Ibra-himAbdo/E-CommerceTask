using E_CommerceTask.Blazor.Helpers.ApiHelpers;

var builder = WebApplication.CreateBuilder(args);

var apiSettings = Guard.Against.Null(builder.Configuration.GetSection(nameof(ApiSettings))
    .Get<ApiSettings>());
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(nameof(ApiSettings)));

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.Converters.Add(new ObjectIdJsonConverter());
});

builder.Services.AddHttpClient(Guard.Against.NullOrEmpty(apiSettings.ApiName), client =>
{
    client.BaseAddress = new Uri(Guard.Against.NullOrEmpty(apiSettings.BaseUrl));
});


builder.Services.AddMudServices();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<IProductService, ProductService>();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(connectionString!));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddScheme<JwtAuthHandlerOptions,JwtAuthHandler>("jwt", null);
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, AuthService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();


// #region Database
//
// using (var scope = app.Services.CreateScope())
// {
//     var context =
//         scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     //await DataSeeding.SeedDefaultData(context);
// }
// #endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "uploads")),
    RequestPath = "/uploads"
});


app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();