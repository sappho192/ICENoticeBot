using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using ICENoticeBot.Model;
using ICENoticeBot.Properties;
using ICENoticeBot.Util;
using Newtonsoft.Json;

namespace ICENoticeBot.Core
{
    public class NoticeCrawler : NoticeUpdatedEvent
    { 
        public event NoticeUpdatedEventHandler OnNoticeUpdated;

        private readonly Timer crawlTimer;
        private static readonly HttpClient client = new HttpClient();

        //= new SortedList<int, ArticleHeader>(Comparer<int>.Create((x, y) => y.CompareTo(x)));

        public NoticeCrawler()
        {
            InitHttpClient();
            InitNoticeDB();

            crawlTimer = new Timer(RefreshNotice, null,
                // Timer will be started 10 seconds after the constructor
                Properties.Constants.NOTICE_REFRESH_DUETIME,
                // Timer will be repeated over 60 seconds
                Properties.Constants.NOTICE_REFRESH_PERIOD);
        }

        private void InitNoticeDB()
        {
            if (!LoadNoticeDB())
            {
                Globals.articleList = new SortedList<int, ArticleHeader>();
                GatherAllNotices();
                SaveNoticeDB();
            }
            else
            {
                //CheckNoticeUpdates();
            }
        }

        private void CheckNoticeUpdates()
        {
            if (NoticeUpdated())
            {
                UpdateNoticeList();
                SaveNoticeDB();
            }
            Console.WriteLine("Notice DB is up to date");
        }

        private void UpdateNoticeList()
        {
            // Recent article number should be largest.
            var recentDB = Globals.articleList.Keys.Max();
            int recentWeb = 0;
            int currentWeb = 0;

            int pageNum = 1;
            while (true)
            {
                var headers = GatherSingleNoticePage(pageNum);
                if(pageNum == 1)
                {
                    recentWeb = headers.First().Index;
                }
                foreach(var header in headers)
                {
                    currentWeb = header.Index;
                    if(currentWeb == recentDB)
                    {
                        break;
                    }

                    Globals.articleList.Add(header.Index, header);
                }
                if (currentWeb == recentDB)
                {
                    break;
                }
                pageNum++;
            }
            OnNoticeUpdated?.Invoke(this, recentDB, recentWeb);
        }

        private bool NoticeUpdated()
        {
            var recentDB = Globals.articleList.Last().Key;
            var recentWeb = GatherSingleNoticePage(1).First().Index;
            return recentWeb != recentDB;
        }

        private void GatherAllNotices()
        {
            int pageNum = 1;
            while (true)
            {
                var headers = GatherSingleNoticePage(pageNum);
                if (headers.Count != 0)
                {
                    foreach (var header in headers)
                    {
                        Globals.articleList.Add(header.Index, header);
                    }
                    pageNum++;
                }
                else
                {
                    break;
                }
            }
        }

        private List<ArticleHeader> GatherSingleNoticePage(int pageNum)
        {
            List<ArticleHeader> headers = new List<ArticleHeader>();

            string url = Properties.Constants.NOTICE_URL;
            string page = Synchronizer.RunSync(new Func<Task<string>>
                (async () => await VisitAsync($"{url}{pageNum}")));
            HtmlDocument pageHtml = new HtmlDocument();
            pageHtml.LoadHtml(page);

            // //*[@id="list_frm"]/table
            var boardNode = pageHtml.DocumentNode.SelectSingleNode(@"//*[@id=""list_frm""]/table/tbody");
            if (boardNode.ChildNodes.Count == 3)
            {// Index 
                if (boardNode.InnerText.Contains("없습니다"))
                {
                    return headers;
                }
            }
            //Console.WriteLine($"\n*************\nRaw Notice data:\n{node.InnerHtml}\n*************\n");
            foreach (var subNode in boardNode.ChildNodes)
            {
                if (subNode.Name.Equals("tr"))
                {// Single article header
                    string content = subNode.InnerHtml;
                    int index = -1;
                    string title = string.Empty;
                    string author = string.Empty;
                    string date = string.Empty;
                    bool hasAttachment = false;
                    string articleUrl = string.Empty;

                    foreach (var articleNode in subNode.ChildNodes)
                    {
                        if (articleNode.Name.Equals("td"))
                        {
                            if (articleNode.HasAttributes)
                            {
                                if (articleNode.Attributes["class"].Value.Equals("no"))
                                {
                                    index = int.Parse(articleNode.InnerText);
                                }
                                else if (articleNode.Attributes["class"].Value.Equals("title"))
                                {
                                    title = Regex.Replace(articleNode.InnerText, @"\t|\n|\r|&nbsp;", "");
                                    articleUrl = HttpUtility.HtmlDecode(articleNode.Descendants("a").First().Attributes["href"].Value);
                                }
                                else if (articleNode.Attributes["class"].Value.Equals("name"))
                                {
                                    author = Regex.Replace(articleNode.InnerText, @"\t|\n|\r|&nbsp;", "");
                                }
                            }
                            else
                            {
                                if (articleNode.Descendants("img").Count() > 0)
                                {
                                    hasAttachment = true;
                                }
                                if (Regex.IsMatch(articleNode.InnerText, @"\d{4}\-\d{2}\-\d{2}"))
                                {
                                    date = Regex.Replace(articleNode.InnerText, @"\t|\n|\r|&nbsp;|\s", "");
                                }
                            }
                        }
                    }

                    ArticleHeader articleHeader = new ArticleHeader(
                        index, title, author, date, hasAttachment, articleUrl);
                    headers.Add(articleHeader);
                }
            }

            return headers;
        }

        private void SaveNoticeDB()
        {
            if (Globals.articleList.Count > 0)
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamWriter sw = new StreamWriter(Properties.Constants.NOTICE_HEADERS_PATH))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, Globals.articleList);
                    Console.WriteLine($"Saved notice db into '{Properties.Constants.NOTICE_HEADERS_PATH}'");
                }
            }
        }

        private void InitHttpClient()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.17.1");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Host", "dept.inha.ac.kr");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
        }

        private async Task<string> VisitAsync(string url)
        {
            return await client.GetStringAsync(url);
        }

        private bool LoadNoticeDB()
        {
            if (File.Exists(Properties.Constants.NOTICE_HEADERS_PATH))
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader sr = new StreamReader(Properties.Constants.NOTICE_HEADERS_PATH))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    Globals.articleList = serializer.Deserialize<SortedList<int, ArticleHeader>>(reader);
                    Console.WriteLine($"Loaded notice db from '{Properties.Constants.NOTICE_HEADERS_PATH}'");
                    return true;
                }
            }
            Console.WriteLine("There's no DB for Notice");
            return false;
        }

        private void RefreshNotice(object state)
        {
            System.Console.WriteLine("Service running...");
            CheckNoticeUpdates();
        }
    }
}
