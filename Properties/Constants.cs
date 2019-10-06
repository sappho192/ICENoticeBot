using System;
using System.Text;
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

        public static readonly string MANUAL = new StringBuilder()
            .Append($"*인하대 정보통신 프로젝트 공지 알림봇*%0A")
            .Append($"/list - 공지사항 목록을 5개씩 보여줍니다(n번째 목록: /list n)%0A")
            .Append($"/see n - n번째 공지를 보여줍니다.%0A")
            .Append($"/sub - 새로운 공지사항 알림을 받습니다%0A")
            .Append($"/unsub - 공지사항 알림을 더이상 받지 않습니다%0A")
            .Append("기능 문의: @sappho192").ToString();
    }
}
