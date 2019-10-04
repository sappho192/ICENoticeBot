using System;
namespace ICENoticeBot.Model
{
    public class ArticleHeader
    {
        public ArticleHeader(int index, string title, string author, string date, bool hasAttachment, string url)
        {
            Index = index;
            Title = title;
            Author = author;
            Date = date;
            HasAttachment = hasAttachment;
            Url = url;
        }

        public int Index { get; }
        public string Title { get; }
        public string Author { get; }
        public string Date { get; }
        public bool HasAttachment { get; }
        public string Url { get; }
    }
}
