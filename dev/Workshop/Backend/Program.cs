using Workshop.ServiceLayer;
using Moq;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add builder.Services to the container.
            IService service;
            Mock<IExternalSystem> externalSystem = new Mock<IExternalSystem>();
            externalSystem.Setup(x => x.Supply(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Random().Next(10000, 100000));
            externalSystem.Setup(x => x.Cancel_Supply(It.IsAny<int>())).Returns(1);
            externalSystem.Setup(x => x.Pay(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Random().Next(10000, 100000));
            externalSystem.Setup(x => x.Cancel_Pay(It.IsAny<int>())).Returns(1);
            externalSystem.Setup(x => x.IsExternalSystemOnline()).Returns(true);

            if (args.Length == 1)
            {
                using (StreamReader streamReader = File.OpenText(args[0]))
                {

                    service = new Service(externalSystem.Object, streamReader.ReadToEnd());
                }
            }
            else if (args.Length == 0)
            {
                service = new Service(externalSystem.Object, "admin~admin~admin~22/08/1972\nport~8800");
            }
            else
            {
                throw new ArgumentException("API needs to receive at most one command line arguments.");
            }
            int WEBSOCKET_PORT = service.GetPort();
            StaisticsViewingServer statistics_server = new StaisticsViewingServer(System.Net.IPAddress.Parse("127.0.0.1"), WEBSOCKET_PORT);
            statistics_server.Start();
            Console.WriteLine($"WS server started on ws://127.0.0.1:{WEBSOCKET_PORT}");

            builder.Services.AddSingleton(service);
            builder.Services.AddSingleton(statistics_server);
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
