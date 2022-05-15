using Workshop.ServiceLayer;

namespace API.Controllers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IService service = new Service();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IService>(x => new Service());
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
