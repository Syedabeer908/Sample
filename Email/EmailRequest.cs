namespace WebApplication1.Email
{
    public class EmailRequest
    {
        public string ToEmail { get; set; }
        public string ToName { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }

        public List<string>? Cc { get; set; }
        public List<string>? Bcc { get; set; }

        public List<EmailAttachment>? Attachments { get; set; } = new List<EmailAttachment>();
    }
}
