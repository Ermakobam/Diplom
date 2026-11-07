using System;
using Microsoft.AspNetCore.Mvc;

namespace IPriziv.Server.Sanitizer
{
    public class AllowedIpAttribute : TypeFilterAttribute
    {
        public AllowedIpAttribute() : base(typeof(AllowedIpFilter))
        {
        }
    }

}
