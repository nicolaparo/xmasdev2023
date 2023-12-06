
using Microsoft.Data.SqlClient;

namespace CookieFactory.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped(s => new SqlConnection(builder.Configuration.GetConnectionString("SqlConnectionString")));
            builder.Services.AddScoped<CookieFactoryMetricsService>();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapCookieFactoryEndpoints();

            app.Run();
        }
    }
}
