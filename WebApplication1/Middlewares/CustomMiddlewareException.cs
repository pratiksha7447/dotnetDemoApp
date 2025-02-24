using Newtonsoft.Json;
using System.Net;
using WebApplication1.Models;

namespace WebApplication1.Middlewares
{
    public class CustomMiddlewareException
    {
        private readonly RequestDelegate _next;
        public CustomMiddlewareException(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

            }catch(Exception ex)
            {
                HttpStatusCode httpStatusCode;
                string message;
                context.Response.ContentType = "application/json";
                switch (ex)
                {
                    case HandledException handledException:
                        httpStatusCode = HttpStatusCode.BadRequest;
                        message = JsonConvert.SerializeObject(new { error = handledException.Message });
                        break;
                    case UserException userException:
                        httpStatusCode = HttpStatusCode.BadRequest;
                        message = JsonConvert.SerializeObject(new { error = userException.Message });
                        break;
                    case UnauthorizedAccessException unauthorizedAccessException:
                        httpStatusCode = HttpStatusCode.Unauthorized;
                        message = JsonConvert.SerializeObject(new { error = unauthorizedAccessException.Message });
                        break;
                    default:
                        httpStatusCode = HttpStatusCode.InternalServerError;
                        message = JsonConvert.SerializeObject(new { error = "error" });
                        break;

                }
                context.Response.StatusCode = (int) httpStatusCode;
                await context.Response.WriteAsync(message);

                }
            }

        }
    }

