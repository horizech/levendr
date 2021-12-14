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
    public class LevendrAuthorizedAttribute : TypeFilterAttribute
    {
        public LevendrAuthorizedAttribute() : base(typeof(LevendrAuthorizedFilter))
        {            
        }
    }

    public class LevendrAuthorizedFilter : IAuthorizationFilter
    {

        public LevendrAuthorizedFilter()
        {
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var currentAction = context.HttpContext.Request.RouteValues["action"];
            var currentController = context.HttpContext.Request.RouteValues["controller"];
            
            string permission = currentController + "." + currentAction;

            var hasClaim = context.HttpContext.User.Claims.Any(c => 
                c.Type == ClaimTypes.Authentication &&
                c.Value.Contains(permission));
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
