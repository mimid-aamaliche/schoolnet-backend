using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using OnlineSchool_Back_end.Services;
using OnlineSchoolBusniss;
using OnlineSchoolDataAccess;
using Stripe;
using OnlineSchool_Back_end.Middlewares;

AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
{
    Console.WriteLine($"FirstChanceException: {eventArgs.Exception.GetType()}: {eventArgs.Exception.Message}");
};


var builder = WebApplication.CreateBuilder(args);




//**************************             For Debuging        **************************************
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Trace);









//***************************                DB Connection                      *********************************************
// Get connection string from appsettings.json
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Pass it to the static DataAccess class
DataAccessConfiguration.Initialize(connectionString);













//***************************             Stripe Configuration                   ***********************************************
builder.Services.Configure<clsStripeModel>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<clsStripeService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AccountLinkService>();







builder.Services.AddScoped<HomePageService>();








//**************************               Video Service Configuration (MUX)             ****************************************
//Transient: meaning that each time we want to use clsMUXVideoService, we gonna make a new one 
//but that is far more efficient then creating a new httpclient each time with a one scoped clsMUXVideoService service
builder.Services.AddTransient<clsMUXVideoService>(sp =>
{
    var Factory = sp.GetRequiredService<IHttpClientFactory>();

    var client = Factory.CreateClient("muxService");

    return new clsMUXVideoService(client);

});


//for now we gonna store the signing key and its id in the config file
builder.Services.Configure<clsMUXSigningServiceModel>(builder.Configuration.GetSection("MUXSigningService"));
//we add a scoped signing service (alwayse using the same object )
builder.Services.AddSingleton<clsMUXSigningService>();





//mux event service 
builder.Services.AddTransient<MUXEventService>();








//Named httpclient
//to optimise the process because creating a new httpclient from scratch every time is very costy
builder.Services.AddHttpClient("muxService", client =>
{


    //Base address configuration
    client.BaseAddress = new Uri("https://api.mux.com/");

    //Mux token configuration
    IConfiguration muxConfig = builder.Configuration.GetSection("MUXVideoService");

    //add the authentication header
    var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{muxConfig.GetValue<string>("muxTokenID")}:{muxConfig.GetValue<string>("muxTokenSecret")}"));
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

});











builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        );
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyHeader());
});




// Add services to the container.

builder.Services.AddControllers();


//Authentication related middleware and configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Use SameAsRequest for production
        options.Cookie.SameSite = SameSiteMode.Lax; // Critical for cross-origin

        options.Cookie.IsEssential = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(1);

        // Prevent redirect to login page (API behavior)
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthService, AuthService>();





// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();






builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);









var app = builder.Build();
//app.UseCors("AllowAll");

app.UseCors("AllowFrontend");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.Headers["Cache-Control"] = "max-age=300, stale-while-revalidate=60";
        }
        return Task.CompletedTask;
    });
    await next();
});


app.UseMiddleware<RequestLoggingMiddleware>();  // ← logs every request/response
app.UseMiddleware<muxTraficValidatorMiddleware>();

//app.UseHttpsRedirection();

app.UseDeveloperExceptionPage(); // Add this temporarily (for dev)


app.UseAuthentication();



app.UseAuthorization();




app.MapControllers();

app.Run();
