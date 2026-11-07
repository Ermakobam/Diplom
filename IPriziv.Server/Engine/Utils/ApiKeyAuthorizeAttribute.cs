using IPriziv.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Athena.Service.Analytic.Engine.Utils
{
    public class ApiKeyAuthorizeAttribute : TypeFilterAttribute
    {
        public ApiKeyAuthorizeAttribute(bool AllowUserAuth = false) : base(typeof(ConfigAuthorizeAttributeImpl))
        {
            Arguments = new object[] { AllowUserAuth };
        }

        private class ConfigAuthorizeAttributeImpl : ActionFilterAttribute
        {
            private readonly ServiceDbContext _serviceDbContext;
            private readonly IMemoryCache _memoryCache;
            private readonly bool _allowUserAuth;
            public ConfigAuthorizeAttributeImpl(bool allowUserAuth, ServiceDbContext serviceDbContext, IMemoryCache memoryCache)
            {
                _allowUserAuth = allowUserAuth;
                _serviceDbContext = serviceDbContext;
                _memoryCache = memoryCache;
            }
            public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (_allowUserAuth && context.HttpContext.User.Identity.IsAuthenticated)
                { await next(); return; }
                context.HttpContext.Request.Headers.TryGetValue("X-Auth-Token", out var tokenValues);
                var token = tokenValues.FirstOrDefault();
                if (string.IsNullOrEmpty(token))
                {
                    context.Result = new UnauthorizedObjectResult(new { Errors = $"X-Auth-Token header required" });
                    return;
                }
                if (!await _memoryCache.GetOrCreateAsync($"apikey_{token}", async cache =>
                {
                    var result = await _serviceDbContext.ApiKeys.AnyAsync(x => x.Key == token && x.ValidBefore.Date >= DateTime.Now.Date);
                    cache
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                        .SetValue(result);
                    return result;
                }))
                {
                    context.Result = new UnauthorizedObjectResult(new { Errors = $"X-Auth-Token=({token}) is not valid" });
                    return;
                }
                await next();
            }
        }
    }
}
