using FluentValidation;
using InventoryManagement.Application.Features.Stock.Commands.ReceiveStock;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Infrastructure.Identity;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<InventoryManagement.API.Authentication.TokenService>();
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(ReceiveStockCommand).Assembly);
        cfg.AddOpenBehavior(typeof(InventoryManagement.Application.Common.Behaviors.ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(ReceiveStockCommand).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<InventoryManagement.API.Middleware.ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();