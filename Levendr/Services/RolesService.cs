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
    public class RolesService : BaseService
    {

        public RolesService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetRoles()
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get("Roles");
            if (cacheResult != null)
            {
                return cacheResult;
            }

            string tablePath = FileSystem.GetPathInConfigurations("Tables/Definitions/" + TableNames.Roles.ToString() + ".json");
            string tableJson = FileSystem.ReadFile(tablePath);
            TableDefinition table = FileSystem.ReadJsonString<TableDefinition>(tableJson);


            List<ColumnInfo> columnDefinitions = await ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().GetTableColumns(Schemas.Levendr, TableNames.Roles.ToString());

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Roles.ToString())
                .AddColumnDefinitions(columnDefinitions)
                .AddForeignTables(table.ForeignTables)
                .RunSelectQuery();

            APIResult newCacheResult = new APIResult()
            {
                Success = true,
                Message = "Roles loaded successfully!",
                Data = result
            };

            ServiceManager.Instance.GetService<MemoryCacheService>().Set("Roles", newCacheResult);

            return newCacheResult;
        }

        public async Task<APIResult> AddRole(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Roles.ToString())
                .AddRow(data)
                .RunInsertQuery();

            await ServiceManager.Instance.GetService<MemoryCacheService>().Remove("Roles");
            
            return new APIResult()
            {
                Success = true,
                Message = "Role added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateRole(string name, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Roles.ToString())
                .WhereEquals("Name", name)
                .AddRow(data)
                .RunUpdateQuery();

            await ServiceManager.Instance.GetService<MemoryCacheService>().Remove("Roles");
            
            return new APIResult()
            {
                Success = true,
                Message = "Role updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteRole(string name)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Roles.ToString())
                .WhereEquals("Name", name)
                .RunDeleteQuery();

            await ServiceManager.Instance.GetService<MemoryCacheService>().Remove("Roles");
            
            return new APIResult()
            {
                Success = true,
                Message = "Role deleted successfully!",
                Data = result
            };
        }
    }
}