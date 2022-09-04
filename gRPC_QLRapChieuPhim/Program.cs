using gRPC_QLRapChieuPhim.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace gRPC_QLRapChieuPhim
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfigurationRoot configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", optional: false)
                 .AddJsonFile($"appsettings.{envName}.json", optional: true)
                 .Build();
            Utilities.ConnectionString = configuration.GetConnectionString("QLRapChieuPhimContext");
            Utilities.NumberOfWeekShow = int.Parse(configuration["ShowingInfo:NumberOfWeekShow"]);
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}