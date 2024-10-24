namespace Prema.ShuleOne.Web.Server.AppSettings
{
    public class TelegramBotSettings
    {
        public string Username { get; set; }
        public string ApiKey { get; set; }
        public string TelegramToken { get; set; }
        public List<int> TelegramChatId { get; set; }
        public List<string> AdminContacts { get; set; }
    }
}
