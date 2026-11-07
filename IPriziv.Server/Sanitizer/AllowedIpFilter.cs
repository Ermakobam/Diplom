using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IPriziv.Server.Sanitizer
{
    public class AllowedIpFilter : IAuthorizationFilter
    {
        private static readonly HashSet<IPAddress> _allowedIps = new();

        public static bool IsAllowedIp(HttpContext context)
        {
            var remoteIp = ResolveClientIp(context);
            if (remoteIp == null)
                return false;
            return _allowedIps.Contains(remoteIp);
        }

        /// <summary>
        /// IP клиента с учётом X-Real-Ip (прокси) — как при логине.
        /// </summary>
        public static IPAddress ResolveClientIp(HttpContext context)
        {
            if (context == null)
                return null;

            var realIpHeader = context.Request.Headers["X-Real-Ip"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(realIpHeader)
                && IPAddress.TryParse(realIpHeader.Trim(), out var fromHeader))
            {
                return fromHeader.IsIPv4MappedToIPv6 ? fromHeader.MapToIPv4() : fromHeader;
            }

            var remoteIp = context.Connection.RemoteIpAddress;
            if (remoteIp == null)
                return null;
            return remoteIp.IsIPv4MappedToIPv6 ? remoteIp.MapToIPv4() : remoteIp;
        }

        public AllowedIpFilter(IConfiguration configuration)
        {
            var ipList = configuration.GetSection("AllowedForumIps").Get<string[]>();

            if (ipList != null)
            {
                foreach (var ip in ipList)
                {
                    if (IPAddress.TryParse(ip, out var parsedIp))
                        _allowedIps.Add(parsedIp);
                }
            }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var remoteIp = ResolveClientIp(context.HttpContext);
            if (remoteIp == null || !_allowedIps.Contains(remoteIp))
                context.Result = new ForbidResult();
        }
    }
}
