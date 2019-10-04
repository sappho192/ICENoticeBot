namespace ICENoticeBot.Core
{
    public delegate void NoticeUpdatedEventHandler(object sender, int recentDB, int recentWeb);
    public interface NoticeUpdatedEvent
    {
        event NoticeUpdatedEventHandler OnNoticeUpdated;
    }
}
