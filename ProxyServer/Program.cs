using ProxyServer.Services;
using StackExchange.Redis;

namespace ProxyServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<RedisCacheService>();
            builder.Services.AddSingleton<LoadBalancerService>();


            var app = builder.Build();

            if (args.Contains("test-redis"))
            {
                try
                {
                    var redis = ConnectionMultiplexer.Connect("redis:6379,abortConnect=false");
                    Console.WriteLine("REDIS OK");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("REDIS FAIL");
                    Console.WriteLine(ex.Message);
                }
                return;
            }

            // Configure the HTTP request pipeline.

            app.UseSwagger();
                app.UseSwaggerUI();
            
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
