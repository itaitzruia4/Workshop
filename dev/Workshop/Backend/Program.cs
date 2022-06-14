using Workshop.ServiceLayer;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add builder.Services to the container.
            IService service;
            if (args.Length == 1)
            {
                using (StreamReader streamReader = File.OpenText(args[0]))
                {
                    service = new Service(new ExternalSystem(), streamReader.ReadToEnd());
                }
            }
            else if (args.Length == 0)
            {
                service = new Service(new ExternalSystem(), "admin~admin~admin~22/08/1972");
            }
            else
            {
                throw new ArgumentException("API needs to receive at most one command line arguments.");
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
