using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using ShoppingStore.API;
using ShoppingStore.API.DbContexts;
using ShoppingStore.API.Installers;
using ShoppingStore.API.Services;
using ShoppingStore.Model.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    //options.InputFormatters; // change default for input formatter
    //options.OutputFormatters; // change default for output formatter
    options.ReturnHttpNotAcceptable = true; // return status code 406(Not Acceptable) when Accept key in header which we are not supported (EX: Accept: application/xml in header api postman)
}).AddNewtonsoftJson();

builder.Services.InstallerServicesInAsembly(builder.Configuration); // Setup seperate configurations (here just Redis Configuration now)

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShoppingStoreContext>(options =>
{
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:ShoppingStoreDBConnectionString"]);
});

//builder.Services.AddDbContext<ShoppingStoreContext>(options =>
//{
//    options.UseSqlServer("Data Source=********;Initial Catalog=ShoppingStoreSQLDemo;User ID=sa;Password=*******;Trust Server Certificate=True");
//});

//builder.Services.AddDbContext<ShoppingStoreContext123>(); // test db

builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:IdentityDBConnectionString"]);
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ISliderRepository, SliderRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<ICompareRepository, CompareRepository>();
builder.Services.AddScoped<IProductQuantityRepository, ProductQuantityRepository>();
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
builder.Services.AddScoped<IStatisticProductOrderRepository, StatisticProductOrderRepository>();
builder.Services.AddScoped<IExternalPaymentRepository, ExternalPaymentRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear(); // clear dictionaries to avoid mapping claim type (get original name for claim key)

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      //.AddJwtBearer(options =>
      //{
      //    options.Authority = "https://localhost:5001"; // idp uri
      //    options.Audience = "imagegalleryapi"; // imagegalleryapi is resource name(check claim "Audience" + get resource)
      //    options.TokenValidationParameters = new()
      //    {
      //        NameClaimType = "given_name",
      //        RoleClaimType = "role", // set role Authorize = "role" claim value
      //        ValidTypes = new[] { "at+jwt" } // allow only accesstoken jwt format
      //    };
      //});
      .AddOAuth2Introspection(options => // reference token (not local access token validate anymore - now every request to BE will check with IDP Intropection endpoint first then return claims to BE later)
      {
          //options.Authority = "https://localhost:44300";
          //options.Authority = "https://localhost:5001";
          options.Authority = builder.Configuration["IDPServerRoot"];
          options.ClientId = "shoppingstoreapi";
          options.ClientSecret = "apisecret";
          options.NameClaimType = "given_name";
          options.RoleClaimType = "role";
      });

builder.Services.AddAuthorization(authorizationOptions =>
{
    //authorizationOptions.AddPolicy("UserCanAddImage", AuthorizationPolicies.CanAddImage());
    authorizationOptions.AddPolicy("ClientApplicationCanWrite", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireClaim("scope", "shoppingstoreapi.write");
    });
    //authorizationOptions.AddPolicy("MustOwnImage", policyBuilder => // only make sense at API level (not define policy in shared proj)
    //{
    //    policyBuilder.RequireAuthenticatedUser();
    //    policyBuilder.AddRequirements(new MustOwnImageRequirement());
    //});
});

builder.Services.AddSignalR(); // Add service SignalR
builder.Services.AddCors(opt => // Fix Cors SignalR
{
    opt.AddPolicy("FEApp", corsPolicyBuilder =>
    {
		corsPolicyBuilder.WithOrigins(builder.Configuration["ClientRoot"])
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
//app.UseCors("corsapp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");  // Add service SignalR

app.UseCors("FEApp"); // Fix Cors SignalR
app.Run();
