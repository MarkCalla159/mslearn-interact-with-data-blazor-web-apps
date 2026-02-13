using BlazingPizza.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using BlazingPizza.Services;
using BlazingPizza;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddScoped<OrderState>();
builder.Services.AddControllers();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddSqlite<PizzaStoreContext>("Data Source=pizza.db");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapRazorPages();
app.MapBlazorHub();
/*app.MapGet("/specials", async (PizzaStoreContext db) =>
{
    return await db.Specials.ToListAsync();
});*/
app.MapFallbackToPage("/_Host");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

// Initialize the database
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PizzaStoreContext>();
    if (db.Database.EnsureCreated())
    {
        SeedData.Initialize(db);
    }
}

var cultureInfo = new CultureInfo("es-PE");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.Run();