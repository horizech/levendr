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
    public class PermissionGroupsService : BaseService
    {

        public PermissionGroupsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetPermissionGroups()
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get("PermissionGroups");
            if (cacheResult != null)
            {
                return cacheResult;
            }

            string tablePath = FileSystem.GetPathInConfigurations("Tables/Definitions/" + TableNames.PermissionGroups.ToString() + ".json");
            string tableJson = FileSystem.ReadFile(tablePath);
            TableDefinition table = FileSystem.ReadJsonString<TableDefinition>(tableJson);


            List<ColumnInfo> columnDefinitions = await ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().GetTableColumns(Schemas.Levendr, TableNames.PermissionGroups.ToString());

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.PermissionGroups.ToString())
                .AddColumnDefinitions(columnDefinitions)
                .AddForeignTables(table.ForeignTables)
                .RunSelectQuery();


            APIResult newCacheResult = new APIResult()
            {
                Success = true,
                Message = "PermissionGroups loaded successfully!",
                Data = result
            };

            ServiceManager.Instance.GetService<MemoryCacheService>().Set("PermissionGroups", newCacheResult);

            return newCacheResult;
        }

        public async Task<APIResult> AddPermissionGroup(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.PermissionGroups.ToString())
                .AddRow(data)
                .RunInsertQuery();

            await ServiceManager.Instance.GetService<MemoryCacheService>().Remove("PermissionGroups");
            
            return new APIResult()
            {
                Success = true,
                Message = "PermissionGroup added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdatePermissionGroup(string name, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.PermissionGroups.ToString())
                .WhereEquals("Name", name)
                .AddRow(data)
                .RunUpdateQuery();

            await ServiceManager.Instance.GetService<MemoryCacheService>().Remove("PermissionGroups");
            
            return new APIResult()
            {
                Success = true,
                Message = "PermissionGroup updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeletePermissionGroup(string name)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.PermissionGroups.ToString())
                .WhereEquals("Name", name)
                .RunDeleteQuery();

            await ServiceManager.Instance.GetService<MemoryCacheService>().Remove("PermissionGroups");
            
            return new APIResult()
            {
                Success = true,
                Message = "PermissionGroup deleted successfully!",
                Data = result
            };
        }
    }
}