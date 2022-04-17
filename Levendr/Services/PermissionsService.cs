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
    public class PermissionsService : BaseService
    {

        public PermissionsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetPermissions()
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get("Permissions");

            if (cacheResult != null)
            {
                return cacheResult;
            }
            else
            {
                List<Dictionary<string, object>> result = await QueryDesigner
                    .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Permissions.ToString())
                    .RunSelectQuery();

                APIResult newCacheResult = new APIResult()
                {
                    Success = true,
                    Message = "Permissions loaded successfully!",
                    Data = result
                };

                ServiceManager.Instance.GetService<MemoryCacheService>().Set("Permissions", newCacheResult);

                return newCacheResult;
            }
        }

        public async Task<APIResult> AddPermission(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Permissions.ToString())
                .AddRow(data)
                .RunInsertQuery();

            return new APIResult()
            {
                Success = true,
                Message = "Permission added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdatePermission(string name, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Permissions.ToString())
                .WhereEquals("Name", name)
                .AddRow(data)
                .RunUpdateQuery();

            return new APIResult()
            {
                Success = true,
                Message = "Permission updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeletePermission(string name)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Permissions.ToString())
                .WhereEquals("Name", name)
                .RunDeleteQuery();

            return new APIResult()
            {
                Success = true,
                Message = "Permission deleted successfully!",
                Data = result
            };
        }
    }
}