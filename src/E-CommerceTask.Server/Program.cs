var builder = WebApplication.CreateBuilder(args);

var mongoDbSettings = Guard.Against.Null(
    builder.Configuration.GetSection(nameof(MongoDbSettings))
        .Get<MongoDbSettings>());
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(nameof(MongoDbSettings)));

builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseMongoDB(Guard.Against.NullOrEmpty(mongoDbSettings.AtlasUri),
        Guard.Against.NullOrEmpty(mongoDbSettings.DatabaseName)));

builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<IProductCategoryService,ProductCategoryService>();
builder.Services.AddScoped<IPurchaseService,PurchaseService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<ILibraryService,LibraryService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });
builder.Services
    .AddAuthorization()
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Set to true in production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();


#region Database

using (var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
    await DataSeeding.SeedDefaultData(context);
}

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("E-CommerceTask Api Reference")
            .WithTheme(ScalarTheme.Mars)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .WithDarkMode(true)
            .WithBaseServerUrl("https://localhost:7150");
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecurityScheme = "Bearer"
        };
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();