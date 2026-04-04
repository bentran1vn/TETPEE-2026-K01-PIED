namespace TetPee.Service.MailService;

public interface IService
{
    public Task SendMail(MailContent mailContent);
}

public class MailContent
{
    public required string To { get; set; }              // Địa chỉ gửi đến
    public required string Subject { get; set; }         // Chủ đề (tiêu đề email)
    public required string Body { get; set; }            // Nội dung (hỗ trợ HTML) của email
}