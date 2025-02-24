namespace WebApplication1.Middlewares
{
    public class UserException:Exception
    {
        public UserException(string message) : base(message) { }
    }
}
