using Workshop.ServiceLayer;
using System.Net;
using System.Text;
using System.IO;

namespace API.Controllers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add builder.Services to the container.
            IService service;
            if (args.Length == 0)
            {
                service = new Service();
            }
            else
            {
                using (StreamReader streamReader = File.OpenText(args[0]))
                {
                    service = new Service(streamReader.ReadToEnd());
                }
            }
            builder.Services.AddSingleton(service);
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
        }
    }
}
