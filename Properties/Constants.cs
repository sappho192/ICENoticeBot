using System;
using Microsoft.Extensions.Configuration;

namespace ICENoticeBot.Properties
{
    public static class Constants
    {
        public static IConfiguration CONFIGURATION;

        public const string TELEGRAM_SETTINGS_PATH = @"TelegramSettings.json";
        public const string TELEGRAM_SETTINGS_DEFAULT_PATH = @"TelegramSettings.DEFAULT.json";
        public const string SUB_USERS_PATH = @"subscribedUsers.json";
        public const string NOTICE_HEADERS_PATH = @"noticeHeaders.json";
        public const string NOTICE_URL = @"http://dept.inha.ac.kr/user/indexSub.do?codyMenuSeq=6669&siteId=ice&dum=dum&boardId=5396814&page=";
        public const string LAST_UPDATE_ID = @"lastUpdateId.json";

        public const int NOTICE_REFRESH_DUETIME = 10 * 1000; // 10 seconds
        public const int NOTICE_REFRESH_PERIOD = 60 * 1000; // 1 minute

        public const int COMMAND_REFRESH_DUETIME = 5 * 1000;
        public const int COMMAND_REFRESH_PERIOD = 2 * 1000; // 2 second
        public const int COMMAND_READ_DUETIME = 6 * 1000;
        public const int COMMAND_READ_PERIOD = 1 * 1000; // 1 second

        public const int NOTICE_SUBLIST_COUNT = 5;
    }
}
