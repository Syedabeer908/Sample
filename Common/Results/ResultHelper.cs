namespace WebApplication1.Common.Results
{
    public class ResultHelper
    {
        public ResultT<List<T>> Success<T>(List<T> data) where T : class
        {
            return ResultT<List<T>>.Success(data);
        }

        public ResultT<T> Success<T>(T obj) where T : class
        {
            return ResultT<T>.Success(obj);
        }

        public Result Success()
        {
            return Result.Success();
        }

    }
}
