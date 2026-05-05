namespace WebApplication1.Data
{
    public class Soft
    {
        public record SoftValues(bool Action, DateTime ActionAt, Guid ActionBy);

        public SoftValues SoftValuesSetter(bool action, DateTime actionAt, Guid actionBy)
        {
            return new SoftValues (
                Action : action,
                ActionAt : actionAt,
                ActionBy : actionBy 
            );
        }
    }
}
