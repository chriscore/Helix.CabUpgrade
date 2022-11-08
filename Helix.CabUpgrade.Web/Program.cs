using Helix.CabUpgrade.Utils;
using Helix.CabUpgrade.Utils.Interfaces;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

var cabConfig = new Dictionary<string, string>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddLogging();
builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();
builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
builder.Services.AddScoped<PresetUpdaterDefaults>();
builder.Services.AddScoped(CreateCabMapConfig);
builder.Services.AddScoped<IPropertyMapper, PropertyMapper>();
builder.Services.AddScoped<ICabMapper, CabMapper>();
builder.Services.AddScoped<IPresetUpdater, PresetUpdater>();

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

static CabMapConfiguration CreateCabMapConfig(IServiceProvider provider)
{
    var config = new CabMapConfiguration();
    config._cabMap = new Dictionary<string, string>(); // TODO: load from app config
    config._cabMap.Add("HD2_Cab4x12XXLV30", "HD2_CabMicIr_4x12MOONT75");
    return config;
}