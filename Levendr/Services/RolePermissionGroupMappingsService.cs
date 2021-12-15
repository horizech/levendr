using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Levendr.Databases;
using Levendr.Models;
using Levendr.Helpers;
using Levendr.Constants;
using Levendr.Enums;

using Microsoft.Extensions.Configuration;

namespace Levendr.Services
{
    public class RolePermissionGroupMappingsService : BaseService
    {

        public RolePermissionGroupMappingsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetRolePermissionGroupMappings()
        {
            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.RolePermissionGroupMappings.ToString())
                .RunSelectQuery();

            return new APIResult()
            {
                Success = true,
                Message = "RolePermissionGroupMappings loaded successfully!",
                Data = result
            };
        }

        public async Task<APIResult> AddRolePermissionGroupMapping(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.RolePermissionGroupMappings.ToString())
                .AddRow(data)
                .RunInsertQuery();

            return new APIResult()
            {
                Success = true,
                Message = "RolePermissionGroupMapping added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateRolePermissionGroupMapping(string key, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.RolePermissionGroupMappings.ToString())
                .WhereEquals("Key", key)
                .AddRow(data)
                .RunUpdateQuery();

            return new APIResult()
            {
                Success = true,
                Message = "RolePermissionGroupMapping updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteRolePermissionGroupMapping(string key)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.RolePermissionGroupMappings.ToString())
                .WhereEquals("Key", key)
                .RunDeleteQuery();

            return new APIResult()
            {
                Success = true,
                Message = "RolePermissionGroupMapping deleted successfully!",
                Data = result
            };
        }
    }
}