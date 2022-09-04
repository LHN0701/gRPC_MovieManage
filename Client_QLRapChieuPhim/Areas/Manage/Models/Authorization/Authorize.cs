using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QLRapChieuPhim.Areas.Manage.Models.Authorization
{
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute(params string[] claim) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { claim };
        }
    }

    public class AuthorizeFilter : IAuthorizationFilter
    {
        readonly string[] _claim;

        public AuthorizeFilter(params string[] claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var IsAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            if (IsAuthenticated)
            {
                bool flagClaim = false;
                if (_claim.Length > 0)
                {
                    foreach (var item in _claim)
                    {
                        if (context.HttpContext.User.HasClaim("QUYENHAN", item.ToUpper()))
                            flagClaim = true;
                    }
                }
                else
                    flagClaim = true;
                if (!flagClaim)
                {
                    if (context.HttpContext.Request.IsAjaxRequest())
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized; //Set HTTP 401
                    else
                        context.Result = new RedirectResult("~/Manage/Account/AccessDenied");
                }
            }
            else
            {
                if (context.HttpContext.Request.IsAjaxRequest())
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden; //Set HTTP 403
                }
                else
                {
                    var returnURL = context.HttpContext.Request.Path.Value;
                    if (!string.IsNullOrEmpty(returnURL))
                        context.Result = new RedirectResult("~/Manage/Account/Login?ReturnUrl=" + returnURL);
                    else
                        context.Result = new RedirectResult("~/Manage/Account/Login");
                }
            }
            return;
        }
    }
}
