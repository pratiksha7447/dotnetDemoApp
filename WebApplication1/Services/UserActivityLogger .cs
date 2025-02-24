using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IUserActivityLogger
    {
        Task LogActivityAsync(string userId, string action, string ipAddress);
    }
    public class UserActivityLogger: IUserActivityLogger
    {
        private readonly EmployeeContext _context;

        public UserActivityLogger(EmployeeContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(string userId, string action, string ipAddress)
        {
            var log = new UserActivityLog
            {
                UserId = userId,
                Action = action,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            _context.UserActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
