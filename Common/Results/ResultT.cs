namespace WebApplication1.Common.Results
{
    public class ResultT<T> : Result
    {
        public T? Result { get; }

        private ResultT(T? result, bool isSuccess, int statusCode, List<Error> errors)
            : base(isSuccess, statusCode, errors) 
        {
            Result = result;
        }

        public static ResultT<T> Success(T result)
        {
            return new ResultT<T>(result, true, 200, new List<Error>());
        }

        public static new ResultT<T> Failure(int statusCode, List<Error> errors)
        {
            return new ResultT<T>(default, false, statusCode, errors);
        }

    }
}
