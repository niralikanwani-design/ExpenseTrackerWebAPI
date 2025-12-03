
using ET.Application.Contracts;
using ET.Application.DTOs;
using ET.Infrastructure.Persistance.Context;
using ET.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ET.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new KeyNotFoundException("Issue"),
                ValidAudience = builder.Configuration["Jwt:Audience"] ?? throw new KeyNotFoundException("Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new KeyNotFoundException("IssuerSigningKey")))
            };
        });
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy => policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod());
        });
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        builder.Services.AddDbContext<ExpenseTrackerContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddScoped<ITransactionService, TransactionService>();
        builder.Services.AddScoped<IDashboardService, DashboardService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        
        builder.Services.AddAuthorization();
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors("AllowAll");
        // Configure the HTTP request pipeline.
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
    }
}