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
                    if(!File.Exists(Properties.Constants.TELEGRAM_SETTINGS_PATH)) {
                        throw new Exception(
                            $"YOU SHOULD MAKE YOUR OWN {Properties.Constants.TELEGRAM_SETTINGS_PATH} FILE\n" +
                            $"CHECK {Properties.Constants.TELEGRAM_SETTINGS_DEFAULT_PATH} FILE");
                    }
                    config.AddJsonFile(Properties.Constants.TELEGRAM_SETTINGS_PATH);
                })
                .UseStartup<Startup>();
    }
}
