using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ICENoticeBot.Util;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICENoticeBot.Core
{
    public class CommandReceiver
    {
        private readonly Timer commandRefreshTimer;
        private readonly Timer commandReadTimer;

        private static readonly HttpClientHandler handler = new HttpClientHandler();
        private static readonly HttpClient client = new HttpClient(handler);
        private static UniqueQueue<JToken> commandsList = new UniqueQueue<JToken>();

        private readonly CommandProcessor commandProcessor = new CommandProcessor();

        private static int lastUpdateId;

        public CommandReceiver()
        {
            LoadLastUpdateId();

            InitHttpClient();

            //RefreshCommand(null);
            commandRefreshTimer = new Timer(RefreshCommand, null,
                Properties.Constants.COMMAND_REFRESH_DUETIME,
                // Timer will be repeated over 2 second
                Properties.Constants.COMMAND_REFRESH_PERIOD);

            //ReadCommand(null);
            commandReadTimer = new Timer(ReadCommand, null,
                Properties.Constants.COMMAND_READ_DUETIME,
                // Timer will be repeated over 1 second
                Properties.Constants.COMMAND_READ_PERIOD);
        }

        private void LoadLastUpdateId()
        {
            if (File.Exists(Properties.Constants.LAST_UPDATE_ID))
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader sr = new StreamReader(Properties.Constants.LAST_UPDATE_ID))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    lastUpdateId = serializer.Deserialize<int>(reader);
                    Console.WriteLine($"Loaded '{Properties.Constants.LAST_UPDATE_ID}'");
                }
            }
            else
            {
                File.WriteAllText(Properties.Constants.LAST_UPDATE_ID, JsonConvert.SerializeObject(lastUpdateId));
                Console.WriteLine($"Created '{Properties.Constants.LAST_UPDATE_ID}'");
            }
        }

        private void ReadCommand(object state)
        {
            if(commandsList.Count != 0)
            {
                var command = commandsList.Dequeue();
                var message = command.Value<JObject>("message");

                var messageID = message.Value<int>("message_id");
                var userID = message.Value<JObject>("chat").Value<int>("id");
                var text = message.Value<string>("text");

                string answerUrl = commandProcessor.Process(messageID, userID, text);
                string response = Synchronizer.RunSync(new Func<Task<string>>
                        (async () => await VisitAsync(answerUrl)));
            }
        }

        private void RefreshCommand(object state)
        {
            string APIKey = Properties.Constants.CONFIGURATION["APIKey"];
            string url = $"https://api.telegram.org/{APIKey}/getUpdates";
            if (lastUpdateId != 0)
            {
                url += $"?offset={lastUpdateId.ToString()}";
            }

            //Console.WriteLine($"Offset: {lastUpdateId} Request URL: {url}");
            try
            {
                string response = Synchronizer.RunSync(new Func<Task<string>>
                    (async () => await VisitAsync(url)));
                var responseJson = JObject.Parse(response);
                if (responseJson.Value<bool>("ok"))
                {
                    var Commands = responseJson["result"] as JArray;
                    //Console.WriteLine($"{Commands.Count} results");
                    foreach (JObject command in Commands)
                    {
                        //Console.WriteLine($"Received command: {command}");
                        int offset = command.Value<int>("update_id");

                        if (lastUpdateId == offset)
                        {// already processed message and next loop can be update
                            continue;
                        }
                        lastUpdateId = offset;
                        // update file too
                        File.WriteAllText(Properties.Constants.LAST_UPDATE_ID, JsonConvert.SerializeObject(lastUpdateId));

                        commandsList.Enqueue(command);
                    }
                }
            }
            catch (HttpRequestException ex)
            {//TODO: Will this resolve timeout exception?
                Console.WriteLine($"Web visit failed with {ex.ToString()}");
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
    }
}
