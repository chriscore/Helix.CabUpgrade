using Helix.CabUpgrade.Utils;
using Helix.CabUpgrade.Utils.Interfaces;
using System.Runtime.CompilerServices;
using Azure.Identity;
using Helix.CabUpgrade.Web;

var builder = WebApplication.CreateBuilder(args);

// Add Azure App Configuration to the container.
var azAppConfigConnection = builder.Configuration["AppConfig"];
if (!string.IsNullOrEmpty(azAppConfigConnection))
{
    // Use the connection string if it is available.
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(azAppConfigConnection)
        .ConfigureRefresh(refresh =>
        {
            // All configuration values will be refreshed if the cabmapping key changes.
            refresh.Register("CabUpgrade:Settings:CabMapping", refreshAll: true);
        });
    });
}
else if (Uri.TryCreate(builder.Configuration["Endpoints:AppConfig"], UriKind.Absolute, out var endpoint))
{
    // Use Azure Active Directory authentication.
    // The identity of this app should be assigned 'App Configuration Data Reader' or 'App Configuration Data Owner' role in App Configuration.
    // For more information, please visit https://aka.ms/vs/azure-app-configuration/concept-enable-rbac
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(endpoint, new DefaultAzureCredential())
        .ConfigureRefresh(refresh =>
        {
            // All configuration values will be refreshed if the sentinel key changes.
            refresh.Register("CabUpgrade:Settings:CabMapping", refreshAll: true);
        });
    });
}
builder.Services.AddAzureAppConfiguration();

// bind settings configuration
builder.Services.Configure<Settings>(builder.Configuration.GetSection("CabUpgrade:Settings"));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddLogging();
builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();
builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
builder.Services.AddScoped<IPropertyMapper, PropertyMapper>();
builder.Services.AddScoped<ICabMapper, CabMapper>();
builder.Services.AddScoped<IPresetUpdater, PresetUpdater>();
builder.Services.AddScoped<IMicMapper, MicMapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAzureAppConfiguration();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();