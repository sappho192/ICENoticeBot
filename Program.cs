using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ICENoticeBot.Core;

namespace ICENoticeBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // YOU SHOULD MAKE YOUR OWN TelegramSettings.json FILE
                    // CHECK TelegramSettings.DEFAULT.json FILE
                    if(!File.Exists("TelegramSettings.json")) {
                        throw new Exception(
                            "YOU SHOULD MAKE YOUR OWN TelegramSettings.json FILE\n" +
                            "CHECK TelegramSettings.DEFAULT.json FILE");
                    }
                    config.AddJsonFile("TelegramSettings.json");
                })
                .UseStartup<Startup>();
    }
}
