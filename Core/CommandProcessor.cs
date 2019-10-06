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
                answerText = $"명령어는 / 로 시작해요.";
            }

            answerUrl = $"https://api.telegram.org/{APIKey}/sendMessage?chat_id={userID}&reply_to_message_id={messageID}&text={answerText}";
            return answerUrl;
        }

        private string ExecuteCommand(int messageID, int userID, string command)
        {
            switch (command)
            {
                case "sub":
                    return Subscribe(userID);
                case "unsub":
                    return Unsubscribe(userID);
                default:
                    return @"그런 명령은 없어요.";
            }
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
