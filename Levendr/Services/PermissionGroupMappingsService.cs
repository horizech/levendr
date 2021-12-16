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
    public class PermissionGroupMappingsService : BaseService
    {

        public PermissionGroupMappingsService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetPermissionGroupMappings()
        {
            string tablePath = FileSystem.GetPathInConfigurations("Tables/Definitions/" + TableNames.PermissionGroupMappings.ToString() + ".json");
            string tableJson = FileSystem.ReadFile(tablePath);
            TableDefinition table = FileSystem.ReadJsonString<TableDefinition>(tableJson);


            List<ColumnInfo> columnDefinitions = await ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().GetTableColumns(Schemas.Levendr, TableNames.PermissionGroupMappings.ToString());

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.PermissionGroupMappings.ToString())
                .AddColumnDefinitions(columnDefinitions)
                .AddForeignTables(table.ForeignTables)
                .RunSelectQuery();

            return new APIResult()
            {
                Success = true,
                Message = "PermissionGroupMappings loaded successfully!",
                Data = result
            };
        }

        public async Task<APIResult> AddPermissionGroupMapping(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.PermissionGroupMappings.ToString())
                .AddRow(data)
                .RunInsertQuery();

            return new APIResult()
            {
                Success = true,
                Message = "PermissionGroupMapping added successfully!",
                Data = result
            };
        }

        public async Task<APIResult> UpdatePermissionGroupMapping(string name, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.PermissionGroupMappings.ToString())
                .WhereEquals("Name", name)
                .AddRow(data)
                .RunUpdateQuery();

            return new APIResult()
            {
                Success = true,
                Message = "PermissionGroupMapping updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeletePermissionGroupMapping(string name)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.PermissionGroupMappings.ToString())
                .WhereEquals("Name", name)
                .RunDeleteQuery();

            return new APIResult()
            {
                Success = true,
                Message = "PermissionGroupMapping deleted successfully!",
                Data = result
            };
        }
    }
}