using System.Diagnostics;
using System;
using System.Web;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Levendr.Filters
{
    public class LevendrClaimAuthorizedAttribute : TypeFilterAttribute
    {
        public LevendrClaimAuthorizedAttribute(string claimType, string claimValue) : base(typeof(LevendrClaimAuthorizedFilter))
        {
            Arguments = new object[] {new Claim(claimType, claimValue) };
        }
    }

    public class LevendrClaimAuthorizedFilter : IAuthorizationFilter
    {
        readonly Claim _claim;

        public LevendrClaimAuthorizedFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var currentAction = context.HttpContext.Request.RouteValues["action"];
            var currentController = context.HttpContext.Request.RouteValues["controller"];
            
            string permission = currentController + "." + currentAction;

            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
            if (!hasClaim)
            {
                context.Result = new ForbidResult();
                
                // context.Result = new RedirectToRouteResult(
                //     new RouteValueDictionary {
                //         {"controller", "Errors"}, {"action", "Error"}, { "errorCode", "AUTH001"}, {"errorMessage", "Unauthorized! " }
                //     }
                // );

                // context.Result = new JsonResult( new 
                // {
                //     Success = false,
                //     Data = (string)null,
                //     ErrorCode = "AUTH001",
                //     Message = "Unauthorized: " + permission
                // });
            }
        }
    }
}
