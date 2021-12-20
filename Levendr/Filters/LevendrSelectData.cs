using System.Diagnostics;
using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Levendr.Helpers;
using Levendr.Enums;
using Levendr.Databases;
using Levendr.Exceptions;
using Levendr.Mappings;
using Levendr.Models;
using Levendr.Services;
using Levendr.Constants;
using Levendr.Interfaces;

namespace Levendr.Filters
{
    public class LevendrSelectDataAttribute : TypeFilterAttribute
    {
        public LevendrSelectDataAttribute() : base(typeof(LevendrSelectDataFilter))
        {            
        }
    }

    public class LevendrSelectDataFilter : IAuthorizationFilter
    {

        public LevendrSelectDataFilter()
        {
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var action = context.HttpContext.Request.RouteValues["action"];
            var controller = context.HttpContext.Request.RouteValues["controller"];
            var method = context.HttpContext.Request.Method;
            
            string permission = string.Format("{0}.{1}.{2}", controller, action, method);

            List<ColumnInfo> columnDefinitions = await ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().GetTableColumns(Schemas.Levendr, TableNames.InsertionOverrides.ToString());

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.InsertionOverrides.ToString())
                .WhereEquals("Action", permission)
                .WhereEquals("Type", "INSERT_COLUMN_VALUE")
                .AddColumnDefinitions(columnDefinitions)
                .RunSelectQuery();


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
