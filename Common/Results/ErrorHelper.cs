namespace WebApplication1.Common.Results
{
    public class ErrorHelper
    {
        public List<Error> CreateErrors(List<string> codes, List<string> messages)
        {
            return messages.Select((message, index) => new Error
            {
                Code = codes[index].ToString(),
                Message = message
            }).ToList();
        }

        public List<Error> CreateErrors(string code, List<string> messages)
        {
            return messages.Select(message => new Error
            {
                Code = code,
                Message = message
            }).ToList();
        }

        public List<Error> CreateErrors(string code, string message)
        {
            return new List<Error>
            {
                new Error
                {
                    Code = code,
                    Message = message
                }
            };
        }
    }
}
