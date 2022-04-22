using System.Diagnostics;
using System;
using System.Web;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Levendr.Services;
using Levendr.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Levendr.Filters
{
    
    public class LevendrUserAccessLevelAttribute : TypeFilterAttribute
    {
        public LevendrUserAccessLevelAttribute(string propertyName) : base(typeof(LevendrUserAccessLevelFilter))
        {
            Arguments = new object[] {propertyName};
        }
    }

    public class LevendrUserAccessLevelFilter : IAsyncActionFilter
    {
        string propertyName;

        public LevendrUserAccessLevelFilter(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string userIdString = context.HttpContext.User.Claims.FirstOrDefault( x => x.Type == ClaimTypes.SerialNumber)?.Value;
            if(string.IsNullOrEmpty(userIdString)) {
                context.Result = new ForbidResult(); return;
            }
            int userId = Int32.Parse(userIdString);

            string roleIdString = context.HttpContext.User.Claims.FirstOrDefault( x => x.Type == ClaimTypes.Role)?.Value;
            if(string.IsNullOrEmpty(roleIdString)) {
                context.Result = new ForbidResult(); return;
            }
            int roleId = Int32.Parse(roleIdString);
            
            string permissionGroupString = context.HttpContext.User.Claims.FirstOrDefault( x => x.Type == ClaimTypes.Authentication)?.Value;
            if(string.IsNullOrEmpty(permissionGroupString)) {
                context.Result = new ForbidResult(); return;
            }
            List<int> permissionGroupIds = permissionGroupString.Split(',').Select(x => Int32.Parse(x)).ToList();

            APIResult rolePermissionGroupMappings = await ServiceManager.Instance.GetService<RolePermissionGroupMappingsService>().GetRolePermissionGroupMappings();
            List<Dictionary<string, object>> userRolePermissionGroupMapping = ((List<Dictionary<string, object>>)rolePermissionGroupMappings.Data).Where(x => (int)x["Role"] == roleId).ToList();
            if(userRolePermissionGroupMapping.Count == 0) {
                context.Result = new ForbidResult(); return;
            }
            
            APIResult permissionGroupMappings = await ServiceManager.Instance.GetService<PermissionGroupMappingsService>().GetPermissionGroupMappings();
            List<Dictionary<string, object>> userPermissionGroupMappings = ((List<Dictionary<string, object>>)permissionGroupMappings.Data).Where(x => permissionGroupIds.Contains((int)x["PermissionGroup"])).ToList();
            if(userPermissionGroupMappings.Count == 0) {
                context.Result = new ForbidResult(); return;
            }
            List<int> userPermissionIds = userPermissionGroupMappings.Select(x => Int32.Parse(x["Permission"].ToString())).ToList();

            APIResult permissions = await ServiceManager.Instance.GetService<PermissionsService>().GetPermissions();
            List<Dictionary<string, object>> userPermissions = ((List<Dictionary<string, object>>)permissions.Data).Where(x => userPermissionIds.Contains(Int32.Parse(x["Id"].ToString()))).ToList();
            if(userPermissions.Count == 0) {
                context.Result = new ForbidResult(); return;
            }

            var action = context.HttpContext.Request.RouteValues["action"];
            var controller = context.HttpContext.Request.RouteValues["controller"];
            var method = context.HttpContext.Request.Method;
            
            string permission = string.Format("{0}.{1}.{2}", controller, action, method);
            if(!userPermissions.Any(x => x["Name"].ToString() == permission))
            {
                context.Result = new ForbidResult(); return;
            }

            Dictionary<string, object> actionPermission = userPermissions.First(x => x["Name"].ToString() == permission);
            Dictionary<string, object> actionPermissionGroupMapping = userPermissionGroupMappings.First(x => (int)x["Permission"] == (int)actionPermission["Id"]);
            Dictionary<string, object> actionRolePermissionGroupMapping = userRolePermissionGroupMapping.First(x => (int)x["PermissionGroup"] == (int)actionPermissionGroupMapping["PermissionGroup"]);
            if(((int)actionRolePermissionGroupMapping["UserLevelAccess"]) != 1)
            {
                List<QuerySearchItem> parameters = null;
                if(context.ActionArguments.ContainsKey("parameters"))
                {
                    if(context.ActionArguments["parameters"] is List<QuerySearchItem>)
                    {
                        parameters = (List<QuerySearchItem>)context.ActionArguments["parameters"];                    
                    }
                }
                if(parameters != null)
                {
                    parameters.Add(
                        new QuerySearchItem()
                        {
                            Name = propertyName,
                            Value = userId,
                            Condition = Enums.ColumnCondition.Equal,
                            CaseSensitive = false
                        }
                    );
                }
            }
            
            await next();
        }
    }

}
