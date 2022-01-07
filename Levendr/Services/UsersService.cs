using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Levendr.Databases;
using Levendr.Models;
using Levendr.Helpers;
using Levendr.Constants;
using Levendr.Enums;
using Levendr.Mappings;

using Microsoft.Extensions.Configuration;

namespace Levendr.Services
{
    public class UsersService : BaseService
    {

        public UsersService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetUserRole(int Id)
        {
            try
            {
                List<Dictionary<string, object>> userRoles = await QueryDesigner
                    .CreateDesigner(schema: Schemas.Levendr, table: TableNames.UserRoles.ToString())
                    .WhereEquals("User", Id)
                    .RunSelectQuery();

                if ((userRoles?.Count ?? 0) > 0)
                {
                    if ((userRoles[0]["Role"]?.ToString().Length ?? 0) > 0)
                    {
                        List<Dictionary<string, object>> roles = await QueryDesigner
                            .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Roles.ToString())
                            .WhereEquals("Id", userRoles[0]["Role"])
                            .RunSelectQuery();

                        if ((roles?.Count ?? 0) > 0)
                        {
                            if ((roles[0]["Name"]?.ToString().Length ?? 0) > 0)
                            {
                                return new APIResult()
                                {
                                    Success = true,
                                    Message = "Role found successfully!",
                                    Data = roles[0]
                                };
                            }
                            else
                            {
                                return new APIResult()
                                {
                                    Success = false,
                                    Message = "Role not found!",
                                    Data = null
                                };
                            }
                        }
                        else
                        {
                            return new APIResult()
                            {
                                Success = false,
                                Message = "User not found!",
                                Data = null
                            };
                        }
                    }
                    else
                    {
                        return new APIResult()
                        {
                            Success = false,
                            Message = "Role not found!",
                            Data = null
                        };
                    }
                }
                else
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "User not found!",
                        Data = null
                    };
                }
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> GetUserPermissions(int Id)
        {
            try
            {
                List<Dictionary<string, object>> userRoles = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.UserRoles.ToString())
                .WhereEquals("User", Id)
                .RunSelectQuery();

                if ((userRoles?.Count ?? 0) > 0)
                {
                    if ((userRoles[0]["Role"]?.ToString().Length ?? 0) > 0)
                    {
                        List<Dictionary<string, object>> permissionGroups = await QueryDesigner
                            .CreateDesigner(schema: Schemas.Levendr, table: TableNames.RolePermissionGroupMappings.ToString())
                            .WhereEquals("Role", userRoles[0]["Role"])
                            .RunSelectQuery();
                        
                        List<int> permissionGroupIds = permissionGroups.Select( x => Int32.Parse(x["PermissionGroup"].ToString())).ToList();

                        List<Dictionary<string, object>> rolePermissions = await QueryDesigner
                            .CreateDesigner(schema: Schemas.Levendr, table: TableNames.PermissionGroupMappings.ToString())
                            .WhereIncludes("PermissionGroup", permissionGroupIds)
                            .RunSelectQuery();

                        if ((rolePermissions?.Count ?? 0) > 0)
                        {
                            int[] permissionIds = new int[rolePermissions.Count];
                            for (int i = 0; i < rolePermissions.Count; i++)
                            {
                                permissionIds[i] = Int32.Parse(rolePermissions[i]["Permission"].ToString());
                            }

                            List<Dictionary<string, object>> permissions = await QueryDesigner
                                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Permissions.ToString())
                                .WhereIncludes("Id", permissionIds)
                                .RunSelectQuery();

                            if ((permissions?.Count ?? 0) > 0)
                            {
                                return new APIResult()
                                {
                                    Success = true,
                                    Message = "Permissions found successfully!",
                                    Data = permissions
                                };
                            }
                            else
                            {
                                return new APIResult()
                                {
                                    Success = false,
                                    Message = "Permissions not found!",
                                    Data = null
                                };
                            }
                        }
                        else
                        {
                            return new APIResult()
                            {
                                Success = false,
                                Message = "Permissions not found!",
                                Data = null
                            };
                        }
                    }
                    else
                    {
                        return new APIResult()
                        {
                            Success = false,
                            Message = "Role not found!",
                            Data = null
                        };
                    }
                }
                else
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "User not found!",
                        Data = null
                    };
                }
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }
    
        public async Task<APIResult> GetUser(int Id)
        {
            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Users.ToString())
                .WhereEquals("Id", Id, true)
                .RunSelectQuery();

            if ((result?.Count ?? 0) > 0)
            {
                return new APIResult()
                {
                    Success = true,
                    Message = "User loaded successfully!",
                    Data = result[0]
                };
            }
            else
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "User not found!",
                    Data = null
                };
            }
        }

        public async Task<APIResult> GetUsers()
        {
            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Users.ToString())
                .RunSelectQuery();

            return new APIResult()
            {
                Success = true,
                Message = "Users loaded successfully!",
                Data = result
            };
        }

        public async Task<APIResult> AddUser(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Users.ToString())
                .AddRow(data)
                .RunInsertQuery();

            return new APIResult()
            {
                Success = true,
                Message = "User added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateUser(int Id, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Users.ToString())
                .WhereEquals("Id", Id)
                .AddRow(data)
                .RunUpdateQuery();

            return new APIResult()
            {
                Success = true,
                Message = "User updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteUser(int Id)
        {
            // Delete User Role
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.UserRoles.ToString())
                .WhereEquals("User", Id)
                .RunDeleteQuery();

            if(!result)
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Error occured while deleting user!",
                    Data = result
                };
            }

            // Delete User
            result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Users.ToString())
                .WhereEquals("Id", Id)
                .RunDeleteQuery();
            if(!result)
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Error occured while deleting user!",
                    Data = result
                };
            }

            return new APIResult()
            {
                Success = true,
                Message = "User deleted successfully!",
                Data = result
            };
        }
    }
}