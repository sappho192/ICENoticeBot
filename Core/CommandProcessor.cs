using System;
using System.Text;
using System.Text.RegularExpressions;
using ICENoticeBot.Properties;

namespace ICENoticeBot.Core
{
    public class CommandProcessor
    {
        public CommandProcessor()
        {
        }

        public string Process(int messageID, int userID, string text)
        {
            string APIKey = Properties.Constants.CONFIGURATION["APIKey"];

            string answerUrl = string.Empty;
            string answerText = string.Empty;
            if (text.IndexOf('/') == 0)
            {// Command
                string command = text.Substring(1);
                //answerText = $"You requested {command}";
                answerText = ExecuteCommand(messageID, userID, command);
            }
            else
            {// Message
                answerText = $"*명령어는 / 로 시작해요.*%0A%0A{Constants.MANUAL}";
            }

            answerUrl = $"https://api.telegram.org/{APIKey}/sendMessage?chat_id={userID}&parse_mode=markdown&reply_to_message_id={messageID}&text={answerText}";
            return answerUrl;
        }

        private string ExecuteCommand(int messageID, int userID, string command)
        {
            string[] commands = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string mainCommand = commands[0];
            switch (mainCommand)
            {
                case "start":
                    return Start();
                case "sub":
                    return Subscribe(userID);
                case "unsub":
                    return Unsubscribe(userID);
                case "list":
                    return List(commands);
                default:
                    return $"{command} 이런 명령은 없어요.";
            }
        }

        private string Start()
        {
            return Constants.MANUAL;
        }

        private string List(string[] commands)
        {
            if (Globals.articleList.Count == 0)
            {
                return "공지사항이 전부 날아갔다구요?";
            }

            int idx = 1;
            if (commands.Length > 1)
            {
                if (!int.TryParse(commands[1], out idx))
                {
                    return $"{commands[1]}는 숫자가 아니네요.";
                }
            }
            // (records + recordsPerPage - 1) / recordsPerPage;
            int groupSize = (Globals.articleList.Count + Constants.NOTICE_SUBLIST_COUNT - 1) / Constants.NOTICE_SUBLIST_COUNT;
            if (idx > groupSize) // Over the bound
            {
                idx = groupSize;
            }
            if (idx < 1) // Under the bound
            {
                idx = 1;
            }

            /* if Sublist size is 5, then
             * ex1)
             * list count: 13, idx: 4
             * => idx: 3 (groupSize)
             * => begin: (list count - (idx - 1) * 5) = 3
             * => end: begin - 5 = -3 => 1
             * 
             * ex2)
             * list count: 13, idx: 2
             * => begin: (list count - (idx - 1) * 5) = 8
             * => end: (begin - 5) + 1 = 4
             */
            int begin = (Globals.articleList.Count - ((idx - 1) * Constants.NOTICE_SUBLIST_COUNT));
            int end = begin - Constants.NOTICE_SUBLIST_COUNT + 1;
            end = end > 0 ? end : 1;

            StringBuilder builder = new StringBuilder();
            builder.Append($"[{idx}번째 목록]%0A");
            for (int i = begin; i >= end; i--)
            {
                builder.Append($"*{Globals.articleList[i].Index}. {Globals.articleList[i].Title}*%0A({Globals.articleList[i].Date})");
                if (Globals.articleList[i].HasAttachment) {
                    builder.Append(" (첨부있음)");
                }
                string link = $"http://dept.inha.ac.kr{Regex.Replace(Globals.articleList[i].Url, "\\&", "%26")}";
                builder.Append($" [공지 링크]({link})%0A%0A");
            }

            return builder.ToString();
        }

        private string Subscribe(int userID)
        {
            if (UserManager.Instance().Add(userID))
            {
                return @"공지사항이 새로 올라오면 알려드릴게요.";
            }
            return @"이미 알림 목록에 계셔요.";
        }

        private string Unsubscribe(int userID)
        {
            if (UserManager.Instance().Remove(userID))
            {
                return @"이제 알려드리지 않을거에요.";
            }
            return @"이미 알림 목록에 없으셔요.";
        }
    }
}
