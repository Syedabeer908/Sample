using System.Net;
using System.Text.RegularExpressions;

namespace WebApplication1.Common.Validators
{
    public static class InputValidators
    {
        // Detect full HTML tags
        private static readonly Regex HtmlTagRegex =
            new("<.*?>", RegexOptions.Compiled | RegexOptions.Singleline);

        // Detect attribute injection patterns (your missing case)
        private static readonly Regex AttributeInjectionRegex =
            new(@"(\s(on\w+)\s*=)|(\""\s*on\w+\s*=)|(' \s*on\w+\s*=)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Detect javascript: protocol
        private static readonly Regex JsProtocolRegex =
            new(@"javascript\s*:", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool ContainsHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return HtmlTagRegex.IsMatch(input);
        }

        public static bool ContainsEncodedHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var decoded = WebUtility.UrlDecode(input);
            return HtmlTagRegex.IsMatch(decoded);
        }

        // NEW: detects your failing case (" onmouseover=...")
        public static bool ContainsAttributeInjection(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return AttributeInjectionRegex.IsMatch(input);
        }

        // NEW: detects javascript: payloads
        public static bool ContainsJsProtocol(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return JsProtocolRegex.IsMatch(input);
        }

        // FINAL COMBINED CHECK (use this everywhere)
        public static bool IsUnsafeForPlainText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var decoded = WebUtility.UrlDecode(input);

            return
                ContainsHtml(decoded) ||
                ContainsEncodedHtml(input) ||
                ContainsAttributeInjection(decoded) ||
                ContainsJsProtocol(decoded);
        }
    }
}