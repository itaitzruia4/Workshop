using Microsoft.AspNetCore.Mvc;

namespace Backend.Communication
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("LiberalPolicy",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000/")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });

            builder.Services.AddControllers();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
