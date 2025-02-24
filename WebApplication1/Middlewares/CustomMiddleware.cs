using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Migrations;
using WebApplication1.Models;

namespace WebApplication1.Middlewares
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("Custom Middleware 1");
            await _next(context);

        }
    }
}
