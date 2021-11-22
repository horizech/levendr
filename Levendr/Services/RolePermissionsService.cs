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
    public class RolePermissionsService : BaseService
    {

        public RolePermissionsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetRolePermissions()
        {
            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.RolePermissions.ToString())
                .RunSelectQuery();

            return new APIResult()
            {
                Success = true,
                Message = "RolePermissions loaded successfully!",
                Data = result
            };
        }

        public async Task<APIResult> AddRolePermission(Dictionary<string, object> data)
        {
            List<Dictionary<string, object>> existingRows = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.RolePermissions.ToString())
                .WhereEquals("Role", data["Role"])
                .WhereEquals("Permission", data["Permission"])
                .RunSelectQuery();
            if(existingRows != null && existingRows.Count > 0) {
                return new APIResult()
                {
                    Success = false,
                    Message = "RolePermission with Role and Permission already exist!",
                    Data = null
                };
            }

            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.RolePermissions.ToString())
                .AddRow(data)
                .RunInsertQuery();

            return new APIResult()
            {
                Success = true,
                Message = "RolePermission added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateRolePermission(int Id, Dictionary<string, object> data)
        {
            List<Dictionary<string, object>> existingRows = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.RolePermissions.ToString())
                .WhereEquals("Role", data["Role"])
                .WhereEquals("Permission", data["Permission"])
                .RunSelectQuery();
            if(existingRows != null && existingRows.Count > 0 && (int)existingRows[0]["Id"] != Id) {
                return new APIResult()
                {
                    Success = false,
                    Message = "RolePermission with Role and Permission already exist!",
                    Data = null
                };
            }

            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.RolePermissions.ToString())
                .WhereEquals("Id", Id)
                .AddRow(data)
                .RunUpdateQuery();

            return new APIResult()
            {
                Success = true,
                Message = "RolePermission updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteRolePermission(int Id)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.RolePermissions.ToString())
                .WhereEquals("Id", Id)
                .RunDeleteQuery();

            return new APIResult()
            {
                Success = true,
                Message = "RolePermission deleted successfully!",
                Data = result
            };
        }
    }
}