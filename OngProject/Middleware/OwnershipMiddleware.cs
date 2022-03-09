using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OngProject.Middleware
{
    public class OwnershipMiddleware
    {

        private readonly RequestDelegate _next;
        public OwnershipMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {   

            if (context.Request.Method == HttpMethod.Put.Method) 
            {
                var role = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                var claimId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);                
                var paramId = context.Request.Query["id"].FirstOrDefault();

                if (paramId != null && paramId != "")
                {                    
                    var excludePaths = new List<string>() { "/users" };

                    var currentPath = context.Request.Path.ToString();
                    if (excludePaths.Contains(currentPath.Substring(0, currentPath.LastIndexOf("/"))))
                    {
                        if (Int32.Parse(claimId.Value) != Int32.Parse(paramId) && role.Value != "Administrator")
                        {
                            context.Response.StatusCode = 403;
                            return;
                        }
                    }                    
                }
            }

            await _next.Invoke(context);
        }
    }
}
