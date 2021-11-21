using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Dapper;
using Npgsql;

using Levendr.Services;
using Levendr.Models;
using Levendr.Interfaces;
using Levendr.Enums;
using Levendr.Helpers;
using Levendr.Constants;
using Levendr.Exceptions;


namespace Levendr.Databases.Postgresql
{
    public class QueryBuilder : IQueryBuilder
    {
        public string GetTabsByLevel(int level)
        {
            string result = "";
            for (int i = 0; i < level; i++)
            {
                result += "\t";
            }
            return result;
        }

        public QueryBuilderOutput Build(LevendrQuery query, int level = 0, Dictionary<string, object> parameters = null)
        {
            string queryString = "";

            if (query.Action == QueryAction.Custom)
            {
                return new QueryBuilderOutput(query.CustomQuery, query.CustomParameters);
            }

            if (query.Action == QueryAction.SelectRows)
            {
                queryString = GetTabsByLevel(level) + "SELECT";

                if ((query.SelectedOutputColumns?.Count ?? 0) > 0)
                {
                    queryString += "\n" + GetTabsByLevel(level + 1) + string.Join(",\n" + GetTabsByLevel(level + 1),
                        query.SelectedOutputColumns
                        .Select(x => x.Key == x.Value ?
                            string.Format("\"{0}\".\"{1}\"", query.TableName + level, x.Key) :
                            string.Format("\"{0}\".\"{1}\" AS \"{2}\"", query.TableName + level, x.Key, x.Value)
                        )
                    );
                }
                else
                {
                    queryString += "\n" + GetTabsByLevel(level + 1) + "*";
                }
            }

            if (query.Action == QueryAction.SelectCount)
            {
                queryString = GetTabsByLevel(level) + "SELECT";

                if ((query.SelectedOutputColumns?.Count ?? 0) > 0)
                {
                    queryString += "\n" + GetTabsByLevel(level + 1) + string.Join(",\n" + GetTabsByLevel(level + 1),
                        query.SelectedOutputColumns
                        .Select(x => x.Key == x.Value ?
                            string.Format("COUNT(\"{0}\".\"{1}\")", query.TableName + level, x.Key) :
                            string.Format("COUNT(\"{0}\".\"{1}\") AS \"{2}\"", query.TableName + level, x.Key, x.Value)
                        )
                    );
                }
                else
                {
                    queryString += "\n" + GetTabsByLevel(level + 1) + "COUNT(*)";
                }
            }

            if (query.Action == QueryAction.SelectRows ||
                query.Action == QueryAction.SelectCount ||
                query.Action == QueryAction.DeleteRows)
            {
                queryString += "\n" + GetTabsByLevel(level) + "FROM";
                queryString += string.Format("{0}\"{1}\".\"{2}\" \"{3}\"", "\n" + GetTabsByLevel(level + 1), query.SchemaName, query.TableName, query.TableName + level);
            }

            // List<ColumnInfo> columns = await GetTableColumns("information_schema", "tables");

            if (query.Action == QueryAction.SelectRows ||
            query.Action == QueryAction.SelectCount ||
            query.Action == QueryAction.DeleteRows ||
            query.Action == QueryAction.UpdateRows)
            {

                if ((query.Conditions?.Count ?? 0) > 0)
                {
                    IQueryHelper queryHelper = ServiceManager.Instance.GetService<DatabaseService>().GetQueryHelper();
                    queryString += "\n" + GetTabsByLevel(level) + "WHERE";
                    queryString += "\n" + GetTabsByLevel(level + 1) + string.Join(" AND" + "\n" + GetTabsByLevel(level + 1),
                        query.Conditions.Select(x =>
                            x.Value == null ?
                                string.Format("\"{0}\" IS NULL", x.Name) :
                                string.Format("\"{0}\" {1} (@{0}{2})", x.Name, queryHelper.GetConditionString(x.Condition), level)
                        )
                    );

                    parameters ??= new Dictionary<string, object>();
                    query.Conditions.ForEach(condition =>
                    {
                        parameters.Add(condition.Name + level.ToString(), condition.Value);
                    });

                    // foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(parameters))
                    // {
                    //     string obj = propertyDescriptor.GetValue(parameters);
                    //     parameters.Add("@" + propertyDescriptor.Name, obj);
                    // }
                }
            }


            if (query.Action == QueryAction.SelectRows || query.Action == QueryAction.SelectCount)
            {
                if ((query.OrderBy?.Length ?? 0) > 0)
                {
                    queryString += "\n" + GetTabsByLevel(level) + "ORDER BY";
                    queryString += "\n" + GetTabsByLevel(level + 1) + "\"" + query.OrderBy + "\"";
                    if (query.IsOrderDescending)
                    {
                        queryString += " DESC";
                    }
                }

                if (query.Limit > 0)
                {
                    queryString += "\n" + GetTabsByLevel(level) + "LIMIT";
                    queryString += "\n" + GetTabsByLevel(level + 1) + query.Limit;
                }

                if (query.Offset > 0)
                {
                    queryString += "\n" + GetTabsByLevel(level) + "OFFSET";
                    queryString += "\n" + GetTabsByLevel(level + 1) + query.Offset;
                }

            }

            return new QueryBuilderOutput(queryString, parameters);
        }

    }
}