using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model.Momo;
using ShoppingStore.Client.Services.Momo;
using ShoppingStore.Client.Services.Vnpay;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(configure =>
        configure.JsonSerializerOptions.PropertyNamingPolicy = null);

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear(); // clear dictionaries to avoid mapping claim type - get same claim name from IDP and client

builder.Services.AddAccessTokenManagement();// add service from identyModel package to manage token send to API BE

// register for session (use for storing cart information)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120); // time for session exists - 30 minutes will expires
    options.Cookie.IsEssential = true;
});

//builder.Services.AddHttpContextAccessor(); // old .net which have Configure and Program seperate
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // new .Net 6 get cookies by constructor injection ShippingService.cs 

// API HttpClient Backend
builder.Services.AddHttpClient<ProductService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<BrandService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<CategoryService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<RoleService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<UserService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<RatingService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<OrderService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<OrderDetailService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<SliderService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<ContactService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<WishlistService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<CompareService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<ProductQuantityService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<ShippingService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<CouponService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<StatisticService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<StatisticProductOrderService>().AddUserAccessTokenHandler();
builder.Services.AddHttpClient<ExternalPaymentService>().AddUserAccessTokenHandler();

// IDP HttpClient
builder.Services.AddHttpClient("IDPClient", client => // add client Idp to revoke reference token in AuthenticationController.cs
{
    //client.BaseAddress = new Uri("https://localhost:5001/");
    client.BaseAddress = new Uri(builder.Configuration["IDPServerRoot"]);
});

builder.Services.AddAuthentication(options => // authentication middleware
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; //when part of our application requires authentication, OpenIdConnet will be trigger as default
})
    /* enable cookie scheme for default scheme, once an identity token is validated and transformed into claims identiy
     * it will be store in an encryted cookie and that cookie is then used on subsequent requests to check 
     * whether or not we are making an authenticated request
    */
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.AccessDeniedPath = "/Authentication/AccessDenied";
        options.LoginPath = "/Authentication/Login";
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        // the successful result of authentication will be stored in the cookie matching this scheme
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = builder.Configuration["IDPServerRoot"]; // idp uri
        //options.RequireHttpsMetadata = false;
        //options.Authority = "https://localhost:5001/"; // idp uri
        // clientid+clientsecret allow the client application to execute an authenticated call to the token endpoint(authorize btn in swagger)
        options.ClientId = "shoppingstoreclient";
        options.ClientSecret = "secret";
        options.ResponseType = "code"; // corresponds to flow -> code = Authorization Code Flow
        //options.Scope.Add("openid");
        //options.Scope.Add("profile");
        //options.CallbackPath = new PathString("signin-oidc"); // if not use forward header then IDP will recieve http request redirectUrl because render reverse proxy
        // SignedOutCallbackPath: default = host:port/signout-callback-oidc.
        // Must match with the post logout redirect URI at IDP client config if
        // you want to automatically return to the application after logging out
        // of IdentityServer.
        // To change, set SignedOutCallbackPath
        // eg: options.SignedOutCallbackPath = new PathString("pathaftersignout");
        options.SaveTokens = true; // save token from idp to use afterward (to cookie here)
        options.GetClaimsFromUserInfoEndpoint = true; // add Profile claim to User object
        options.ClaimActions.Remove("aud");
        options.ClaimActions.DeleteClaim("sid");
        options.ClaimActions.DeleteClaim("idp");
        options.Scope.Add("roles"); // add custom scope
        options.Scope.Add("email"); // add custom scope
        //options.Scope.Add("imagegalleryapi.fullaccess"); // api scope
        options.Scope.Add("shoppingstoreapi.read"); // api scope
        options.Scope.Add("shoppingstoreapi.write"); // api scope
        options.Scope.Add("country"); // custom scope - from this point the country claim will be return from IDP UserInfo endpoint, still need to mapping to use in our claims identity
        options.Scope.Add("offline_access"); // support refresh token
        options.ClaimActions.MapJsonKey("role", "role"); // mapping claim in custom scope
        options.ClaimActions.MapUniqueJsonKey("country", "country"); // mapping claim in custom scope
        options.TokenValidationParameters = new() // setup how the token should do when validate
        {
            NameClaimType = "given_name", // set NameClaimType to the given_name from return claim
            RoleClaimType = "role",
        };
    });

// Connect MomoAPI
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();

// Connect VnPay
builder.Services.AddScoped<IVnPayService, VnPayService>();

var app = builder.Build();

// setup 404 page
app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

// use session for cart
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // add authenticate pipeline, place beetween UseRouting, MapControllerRoute
app.UseAuthorization();

// setup route for all area
app.MapControllerRoute(
	name: "Areas",
	pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");

//// setup route for /category?slug=... -> /category/slug
//app.MapControllerRoute(
//    name: "category",
//    pattern: "/category/{Slug?}",
//    defaults: new { controller = "Category", action = "Index" });

//// setup route for /brand?slug=... -> /brand/slug
//app.MapControllerRoute(
//    name: "brand",
//    pattern: "/brand/{Slug?}",
//    defaults: new { controller = "Brand", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
