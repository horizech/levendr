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
    public class RolesService : BaseService
    {

        public RolesService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetRoles()
        {
            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Roles.ToString())
                .RunSelectQuery();

            return new APIResult()
            {
                Success = true,
                Message = "Roles loaded successfully!",
                Data = result
            };
        }

        public async Task<APIResult> AddRole(Dictionary<string, object> data)
        {
            List<int> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Roles.ToString())
                .AddRow(data)
                .RunInsertQuery();

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

            return new APIResult()
            {
                Success = true,
                Message = "Role deleted successfully!",
                Data = result
            };
        }
    }
}