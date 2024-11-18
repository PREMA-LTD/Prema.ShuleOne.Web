namespace Prema.Services.UnifiedNotifier.MasstransitContracts
{
    public class SendSms
    {
        public string message { get; set; }
        public string recipient_name { get; set; }
        public string recipient_contact { get; set; }
        public string sender { get; set; }
    }
}
