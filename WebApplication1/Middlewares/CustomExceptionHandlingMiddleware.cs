
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
namespace WebApplication1.Middlewares
{
    public class CustomExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next=next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex) {

                httpContext.Response.ContentType = "application/json";
                string result;
                HttpStatusCode httpStatusCode;
                switch (ex)
                {
                    case UserException userException:
                        httpStatusCode = HttpStatusCode.BadRequest;
                        result = JsonConvert.SerializeObject(new { error = (string)userException.Message });
                        break;
                    case HandledException handledException:
                        httpStatusCode = HttpStatusCode.BadRequest;
                        result = JsonConvert.SerializeObject(new { error = (string)handledException.Message });
                        break;
                    case UnauthorizedAccessException unauthorized:
                        httpStatusCode = HttpStatusCode.Unauthorized;
                        result = JsonConvert.SerializeObject(new { error = (string)unauthorized.Message });
                        break;
                    default: 
                        httpStatusCode = HttpStatusCode.InternalServerError;
                        result = JsonConvert.SerializeObject(new { error = "unexpectedErrorOccur" });
                        break;    
               }

                httpContext.Response.StatusCode =(int) httpStatusCode;
                await httpContext.Response.WriteAsync(result);

            }
        }
    }
}
