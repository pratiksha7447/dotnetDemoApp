using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Middlewares
{
    public class ActivityLoggingMiddleware
    {

        private readonly RequestDelegate _next;

        public ActivityLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Collect necessary data
            var dbContext = context.RequestServices.GetRequiredService<EmployeeContext>();

            var userId = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Guest";
            var action = $"{context.Request.Method} {context.Request.Path}";
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            // Log the activity
            var log = new UserActivityLog
            {
                UserId = userId,
                Action = action,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            dbContext.UserActivityLogs.Add(log);
            await dbContext.SaveChangesAsync();
            // Proceed to the next middleware
            await _next(context);
        }
    }
}
