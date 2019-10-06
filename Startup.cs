using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using ICENoticeBot.Core;
using ICENoticeBot.Properties;
using ICENoticeBot.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ICENoticeBot
{
    public class Startup
    {
        public readonly NoticeCrawler noticeCrawler;
        public readonly CommandReceiver commandReceiver;

        private static readonly HttpClientHandler handler = new HttpClientHandler();
        private static readonly HttpClient client = new HttpClient(handler);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Properties.Constants.CONFIGURATION = configuration;

            InitHttpClient();
            noticeCrawler = new NoticeCrawler();
            commandReceiver = new CommandReceiver();

            noticeCrawler.OnNoticeUpdated += NoticeCrawler_OnNoticeUpdated;
        }

        private void NoticeCrawler_OnNoticeUpdated(object sender, int recentDB, int recentWeb)
        {
            int articlesCount = recentWeb - recentDB;
            for(int i = 0; i < articlesCount; i++)
            {
                var recentNoticeHeader = Globals.articleList[recentDB + i];
                string message = $"*{recentNoticeHeader.Index}. {recentNoticeHeader.Title}*%0A{recentNoticeHeader.Date}{(recentNoticeHeader.HasAttachment ? " (첨부파일 있음) " : " ")}[공지 링크](http://dept.inha.ac.kr{Regex.Replace(recentNoticeHeader.Url, "\\&", "%26")})";

                // From TelegramSettings.json
                string APIKey = Configuration["APIKey"];

                // To all subscribed users
                foreach (int userID in UserManager.Instance().Get())
                {
                    //string chatId = UserManager.Instance().Get().First().ToString();
                    string messageUrl = $"https://api.telegram.org/{APIKey}/sendMessage?text={message}&parse_mode=markdown&chat_id={userID.ToString()}";
                    var response = Synchronizer.RunSync(new Func<Task<string>>
                        (async () => await VisitAsync(messageUrl)));
                    Console.WriteLine($"Message response: {response}");
                }
            }
        }

        private async Task<string> VisitAsync(string url)
        {
            return await client.GetStringAsync(url);
        }

        private void InitHttpClient()
        {
#if DEBUG
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.17.1");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Host", "dept.inha.ac.kr");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
