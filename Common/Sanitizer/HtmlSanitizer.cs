using Ganss.Xss;

namespace WebApplication1.Common.Sanitizer
{
    public class HtmlSanitizer
    {
        private readonly Ganss.Xss.HtmlSanitizer _sanitizer;

        public HtmlSanitizer()
        {
            _sanitizer = new Ganss.Xss.HtmlSanitizer();
        }

        public string Sanitize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return _sanitizer.Sanitize(input);
        }
    }
}
