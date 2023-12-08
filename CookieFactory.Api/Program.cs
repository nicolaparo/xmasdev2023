
using Microsoft.Data.SqlClient;

namespace CookieFactory.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.local.json", true);

            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped(s => new SqlConnection(builder.Configuration["SqlConnectionString"]));
            builder.Services.AddScoped<CookieFactoryMetricsService>();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthorization();

            var sharedSecretKey = app.Configuration["SharedSecretKey"];

            app.MapCookieFactoryEndpoints(sharedSecretKey);

            app.Run();
        }
    }
}
