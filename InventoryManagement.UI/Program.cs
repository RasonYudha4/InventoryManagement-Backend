using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using InventoryManagement.UI;
using Blazored.LocalStorage;
using InventoryManagement.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using InventoryManagement.UI.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddTransient<JwtInterceptor>();

builder.Services.AddHttpClient("InventoryAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5028/"); 
})
.AddHttpMessageHandler<JwtInterceptor>(); 

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("InventoryAPI"));

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IAiService, AiService>();

await builder.Build().RunAsync();