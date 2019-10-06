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
                answerText = $"You requested {command}";

            }
            else
            {// Message
                answerText = $"You said {text}";
            }

            answerUrl = $"https://api.telegram.org/{APIKey}/sendMessage?chat_id={userID}&reply_to_message_id={messageID}&text={answerText}";
            return answerUrl;
        }
    }
}
