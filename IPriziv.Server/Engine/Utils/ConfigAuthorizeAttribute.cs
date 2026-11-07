using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Athena.Service.Analytic.Engine.Utils
{
    public class ConfigAuthorizeAttribute : TypeFilterAttribute
    {
        public ConfigAuthorizeAttribute(bool AllowUserAuth = false) : base(typeof(ConfigAuthorizeAttributeImpl))
        {
            Arguments = new object[] { AllowUserAuth };
        }

        private class ConfigAuthorizeAttributeImpl : ActionFilterAttribute
        {
            private readonly IConfiguration _configuration;
            private readonly bool _allowUserAuth;
            public ConfigAuthorizeAttributeImpl(IConfiguration configuration, bool allowUserAuth)
            {
                _configuration = configuration;
                _allowUserAuth = allowUserAuth;
            }
            public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (_allowUserAuth && context.HttpContext.User.Identity.IsAuthenticated)
                { await next(); return; }
                var requiredTokenValue = _configuration.GetValue<string>("ConfigAuthToken");
                context.HttpContext.Request.Headers.TryGetValue("X-Auth-Token", out var token);
                if (requiredTokenValue != token)
                    context.Result = new UnauthorizedObjectResult(new { Errors = $"X-Auth-Token=({token}) is not valid" });
                else
                    await next();
            }
        }
    }
}
