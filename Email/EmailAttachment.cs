namespace WebApplication1.Email
{
    public class EmailAttachment
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; } = "application/octet-stream";
    }
}
