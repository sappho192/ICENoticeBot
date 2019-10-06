using System;
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
                answerText = $"You said {text}";
            }

            answerUrl = $"https://api.telegram.org/{APIKey}/sendMessage?chat_id={userID}&reply_to_message_id={messageID}&text={answerText}";
            return answerUrl;
        }

        private string ExecuteCommand(int messageID, int userID, string command)
        {
            string result;
            switch (command)
            {
                case "sub":
                    if(UserManager.Instance().Add(userID))
                    {
                        result = @"공지사항이 새로 올라오면 알려드릴게요.";
                    } else
                    {
                        result = @"이미 알림 목록에 계셔요.";
                    }
                    break;
                case "unsub":
                    if (UserManager.Instance().Remove(userID))
                    {
                        result = @"이제 알려드리지 않을거에요.";
                    }
                    else
                    {
                        result = @"이미 알림 목록에 없으셔요.";
                    }
                    break;
                default:
                    result = @"그런 명령은 없습니다.";
                    break;
            }
            return result;
        }
    }
}
