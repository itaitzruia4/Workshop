using Workshop.ServiceLayer;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace API.Controllers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add builder.Services to the container.
            builder.Services.AddSingleton<IService, Service>();
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
            var app = builder.Build();
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
            WebSocketServer wssv = new WebSocketServer("ws://127.0.0.1:7890");
            wssv.Start();
        }
    }
}
