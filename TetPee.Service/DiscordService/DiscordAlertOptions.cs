namespace TetPee.Service.DiscordService;

public class DiscordAlertOptions
{
    public bool Enabled { get; set; }
    public string WebhookUrl { get; set; }
    public string Mention { get; set; } = "@here";
}
