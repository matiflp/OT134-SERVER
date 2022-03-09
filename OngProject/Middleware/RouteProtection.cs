using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OngProject.Middleware
{
    public class RouteProtection
    {
        private readonly RequestDelegate _next;

        public RouteProtection(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            List<string> methods = new()
            {
                "post",
                "put",
                "delete"
            };

            List<string> paths = new()
            {
                "/activities",
                "/categories",
                "/news",
                "/organizations",
                "/testimonials"
            };

            var method = context.Request.Method.ToLower();
            var path = context.Request.Path.ToString().ToLower();

            bool containsPath = false;
            foreach(string p in paths)
            {
                if (path.Contains(p))
                    containsPath = true;   
            }

            bool containsMethod = false;
            foreach (string m in methods)
            {
                if (method.Contains(m))
                    containsMethod = true;
            }

            if (containsPath && containsMethod)
            {
                if (!context.User.IsInRole("Administrator"))
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}
