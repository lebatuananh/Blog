using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders;
using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Meowv.Blog.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                           .ConfigureWebHostDefaults(webBuilder =>
                           {
                               webBuilder.UseStartup<Startup>();
                           }).ConfigureServices(services =>
                           {
                               services.AddRazorPages();
                               services.Configure<WebEncoderOptions>(options =>
                               {
                                   options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
                               });
                               services.AddHttpClient("api", x =>
                               {
                                   x.BaseAddress = new Uri("https://localhost:5001");
                               });
                           });
            await host.Build().RunAsync();
        }
    }
}