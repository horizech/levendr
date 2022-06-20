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
    public class UserAccessLevelsService : BaseService
    {

        public UserAccessLevelsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetUserAccessLevels()
        {
            APIResult cacheResult = await ServiceManager.Instance.GetService<MemoryCacheService>().Get("UserAccessLevels");
            if (cacheResult != null)
            {
                return cacheResult;
            }

            string tablePath = FileSystem.GetPathInConfigurations("Tables/Definitions/" + TableNames.UserAccessLevels.ToString() + ".json");
            string tableJson = FileSystem.ReadFile(tablePath);
            TableDefinition table = FileSystem.ReadJsonString<TableDefinition>(tableJson);


            List<ColumnInfo> columnDefinitions = await ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().GetTableColumns(Schemas.Levendr, TableNames.UserAccessLevels.ToString());

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.UserAccessLevels.ToString())
                .AddColumnDefinitions(columnDefinitions)
                .AddForeignTables(table.ForeignTables)
                .RunSelectQuery();

            APIResult newCacheResult = new APIResult()
            {
                Success = true,
                Message = "UserAccessLevels loaded successfully!",
                Data = result
            };

            ServiceManager.Instance.GetService<MemoryCacheService>().Set("UserAccessLevels", newCacheResult);

            return newCacheResult;
        }

        public async Task<APIResult> AddUserAccessLevel(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.UserAccessLevels.ToString())
                .AddRow(data)
                .RunInsertQuery();

            await ServiceManager.Instance.GetService<MemoryCacheService>().Remove("UserAccessLevels");
            
            return new APIResult()
            {
                Success = true,
                Message = "UserAccessLevel added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdateUserAccessLevel(string name, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.UserAccessLevels.ToString())
                .WhereEquals("Name", name)
                .AddRow(data)
                .RunUpdateQuery();

            await ServiceManager.Instance.GetService<MemoryCacheService>().Remove("UserAccessLevels");
            
            return new APIResult()
            {
                Success = true,
                Message = "UserAccessLevel updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteUserAccessLevel(string name)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.UserAccessLevels.ToString())
                .WhereEquals("Name", name)
                .RunDeleteQuery();

            await ServiceManager.Instance.GetService<MemoryCacheService>().Remove("UserAccessLevels");
            
            return new APIResult()
            {
                Success = true,
                Message = "UserAccessLevel deleted successfully!",
                Data = result
            };
        }
    }
}