namespace WebApplication1.Common.Results
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        public List<Error> Errors { get; }

        protected Result(bool isSuccess, int statusCode, List<Error> errors)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Errors = errors; 
        }

        public static Result Success()
        {
            return new Result(true, 200, new List<Error>());
        }

        public static Result Failure(int statusCode, List<Error> errors)
        {
            return new Result(false, statusCode, errors);
        }
    }
}
