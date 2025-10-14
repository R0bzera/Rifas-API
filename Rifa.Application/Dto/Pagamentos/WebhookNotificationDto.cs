namespace Rifa.Application.Dto.Pagamentos
{
    public class WebhookNotificationDto
    {
        public string Id { get; set; } = string.Empty;
        public bool Live_mode { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime Date_created { get; set; }
        public string Action { get; set; } = string.Empty;
        public string User_id { get; set; } = string.Empty;
        public string Api_version { get; set; } = string.Empty;
        public WebhookDataDto Data { get; set; } = new();
    }

    public class WebhookDataDto
    {
        public string Id { get; set; } = string.Empty;
    }
}
