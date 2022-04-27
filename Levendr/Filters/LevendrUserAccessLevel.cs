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
        public LevendrUserAccessLevelAttribute(string checkPropertyName = "", string insertPropertyName = "", string updatePropertyName = "") : base(typeof(LevendrUserAccessLevelFilter))
        {
            Arguments = new object []{checkPropertyName, insertPropertyName, updatePropertyName};
        }
    }

    public class LevendrUserAccessLevelFilter : IAsyncActionFilter
    {
        string checkPropertyName;
        string insertPropertyName;
        string updatePropertyName;        

        public LevendrUserAccessLevelFilter(string checkPropertyName, string insertPropertyName, string updatePropertyName)
        {
            this.checkPropertyName = checkPropertyName;
            this.insertPropertyName = insertPropertyName;
            this.updatePropertyName = updatePropertyName;
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
            if(((int)actionRolePermissionGroupMapping["UserAccessLevel"]) != 1)
            {
                if(method == "POST" || method == "PUT" || method == "DELETE")
                {
                    List<QuerySearchItem> parameters = null;
                    Dictionary<string, object> singleRowData = null;
                    List<Dictionary<string, object>> MultipleRowsData = null;
                    
                    if(!string.IsNullOrEmpty(checkPropertyName) && context.ActionArguments.ContainsKey("parameters"))
                    {
                        if(context.ActionArguments["parameters"] is List<QuerySearchItem>)
                        {
                            parameters = (List<QuerySearchItem>)context.ActionArguments["parameters"];                    
                        }
                    }

                    if(context.ActionArguments.ContainsKey("data"))
                    {
                        if(context.ActionArguments["data"] is List<Dictionary<string, object>>)
                        {
                            MultipleRowsData = (List<Dictionary<string, object>>)context.ActionArguments["data"];                    
                        }
                    }

                    if(context.ActionArguments.ContainsKey("request"))
                    {
                        if(context.ActionArguments["request"] is UpdateRequest)
                        {
                            parameters = ((UpdateRequest)context.ActionArguments["request"]).Parameters;                    
                            singleRowData = ((UpdateRequest)context.ActionArguments["request"]).Data;                    
                        }
                    }
                    
                    if(parameters != null)
                    {
                        if(parameters.Any( x => x.Name.ToLower() == checkPropertyName.ToLower()))
                        {
                            List<QuerySearchItem> parameterItemsToDelete = parameters.Where( x => x.Name.ToLower() == checkPropertyName.ToLower()).ToList();
                            for(int i = 0; i < parameterItemsToDelete.Count; i++)
                            {
                                parameters.Remove(parameterItemsToDelete[i]);
                            }
                        }
                        parameters.Add(
                            new QuerySearchItem()
                            {
                                Name = checkPropertyName,
                                Value = userId,
                                Condition = Enums.ColumnCondition.Equal,
                                CaseSensitive = false
                            }
                        );
                    }

                    if(singleRowData != null)
                    {                        
                        if(!string.IsNullOrEmpty(insertPropertyName) && singleRowData.Any( x => x.Key.ToLower() == insertPropertyName.ToLower()))
                        {
                            Dictionary<string, object> dataItemsToDelete = singleRowData.Where( x => x.Key.ToLower() == insertPropertyName.ToLower()).ToDictionary(x => x.Key, x => x.Value);
                            foreach(string keyToRemove in dataItemsToDelete.Select(x => x.Key))
                            {
                                singleRowData.Remove(keyToRemove);
                            }
                        }

                        if(!string.IsNullOrEmpty(updatePropertyName) && singleRowData.Any( x => x.Key.ToLower() == updatePropertyName.ToLower()))
                        {
                            Dictionary<string, object> dataItemsToDelete = singleRowData.Where( x => x.Key.ToLower() == updatePropertyName.ToLower()).ToDictionary(x => x.Key, x => x.Value);
                            foreach(string keyToRemove in dataItemsToDelete.Select(x => x.Key))
                            {
                                singleRowData.Remove(keyToRemove);
                            }
                        }

                        if(!string.IsNullOrEmpty(updatePropertyName))
                        {
                            singleRowData.Add(updatePropertyName, userId);
                        }
                        else
                        {
                            singleRowData.Add(insertPropertyName, userId);
                        }

                    }

                    if(MultipleRowsData != null)
                    {
                        for(int i = 0; i < MultipleRowsData.Count; i++)
                        {
                            if(!string.IsNullOrEmpty(insertPropertyName) && MultipleRowsData[i].Any( x => x.Key.ToLower() == insertPropertyName.ToLower()))
                            {
                                Dictionary<string, object> dataItemsToDelete = MultipleRowsData[i].Where( x => x.Key.ToLower() == insertPropertyName.ToLower()).ToDictionary(x => x.Key, x => x.Value);
                                foreach(string keyToRemove in dataItemsToDelete.Select(x => x.Key))
                                {
                                    MultipleRowsData[i].Remove(keyToRemove);
                                }
                            }

                            if(!string.IsNullOrEmpty(updatePropertyName) && MultipleRowsData[i].Any( x => x.Key.ToLower() == updatePropertyName.ToLower()))
                            {
                                Dictionary<string, object> dataItemsToDelete = MultipleRowsData[i].Where( x => x.Key.ToLower() == updatePropertyName.ToLower()).ToDictionary(x => x.Key, x => x.Value);
                                foreach(string keyToRemove in dataItemsToDelete.Select(x => x.Key))
                                {
                                    MultipleRowsData[i].Remove(keyToRemove);
                                }
                            }

                            if(!string.IsNullOrEmpty(updatePropertyName))
                            {
                                MultipleRowsData[i].Add(updatePropertyName, userId);
                            }
                            else
                            {
                                MultipleRowsData[i].Add(insertPropertyName, userId);
                            }
                        }
                    }
                }
                else if(method == "GET")
                {
                    var values = context.RouteData.Values;
                    if(!string.IsNullOrEmpty(checkPropertyName))
                    {
                        if(!string.IsNullOrEmpty(checkPropertyName) && context.ActionArguments.Any( x => x.Key.ToLower() == checkPropertyName.ToLower()))
                        {
                            Dictionary<string, object> argumentsToDelete = context.ActionArguments.Where( x => x.Key.ToLower() == checkPropertyName.ToLower()).ToDictionary(x => x.Key, x => x.Value);
                            foreach(string keyToRemove in argumentsToDelete.Select(x => x.Key))
                            {
                                context.ActionArguments.Remove(keyToRemove);
                            }
                        }
                        context.ActionArguments.Add(checkPropertyName, userId);
                    }

                    if(!string.IsNullOrEmpty(insertPropertyName))
                    {
                        if(!string.IsNullOrEmpty(insertPropertyName) && context.ActionArguments.Any( x => x.Key.ToLower() == insertPropertyName.ToLower()))
                        {
                            Dictionary<string, object> argumentsToDelete = context.ActionArguments.Where( x => x.Key.ToLower() == insertPropertyName.ToLower()).ToDictionary(x => x.Key, x => x.Value);
                            foreach(string keyToRemove in argumentsToDelete.Select(x => x.Key))
                            {
                                context.ActionArguments.Remove(keyToRemove);
                            }
                        }
                        context.ActionArguments.Add(insertPropertyName, userId);
                    }

                    if(!string.IsNullOrEmpty(updatePropertyName))
                    {
                        if(!string.IsNullOrEmpty(updatePropertyName) && context.ActionArguments.Any( x => x.Key.ToLower() == updatePropertyName.ToLower()))
                        {
                            Dictionary<string, object> argumentsToDelete = context.ActionArguments.Where( x => x.Key.ToLower() == updatePropertyName.ToLower()).ToDictionary(x => x.Key, x => x.Value);
                            foreach(string keyToRemove in argumentsToDelete.Select(x => x.Key))
                            {
                                context.ActionArguments.Remove(keyToRemove);
                            }
                        }
                        context.ActionArguments.Add(updatePropertyName, userId);
                    }
                }
            }
            
            await next();
        }
    }

}
