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
using Levendr.Mappings;
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
            // query.ColumnsDefinitions
            string queryString = "";
            
            if (query.Action == QueryAction.Custom)
            {
                return new QueryBuilderOutput(query.CustomQuery, query.CustomParameters);
            }
            else if (query.Action == QueryAction.SelectRows)
            {
                queryString = GetTabsByLevel(level) + "SELECT";
                queryString += "\n" + GetTabsByLevel(level + 1);

                List<string> columnsToSelect = new();

                if ((query.SelectedOutputColumns?.Count ?? 0) > 0)
                {
                    columnsToSelect.AddRange(
                        query.SelectedOutputColumns.Select(x => x.Key == x.Value ?
                            string.Format("\"{0}\".\"{1}\"", query.TableName + level, x.Key) :
                            string.Format("\"{0}\".\"{1}\" AS \"{2}\"", query.TableName + level, x.Key, x.Value)
                        )
                    );
                }
                else
                {
                    columnsToSelect.Add(string.Format("\"{0}\".*", query.TableName + level));
                }

                if((query.AddForeignTables?.Count ?? 0) > 0)
                {
                    if((query.ColumnsDefinitions?.Count ?? 0) > 0)
                    {
                        int joinCounter = 0;
                        foreach(AddForeignTables foreignTableinfo in query.AddForeignTables)
                        {
                            if((foreignTableinfo.ForeignColumns?.Count ?? 0) > 0)
                            {
                                ColumnInfo columnInfo = query.ColumnsDefinitions.Where( x => x.Name.ToLower().Equals(foreignTableinfo.Name.ToLower())).FirstOrDefault();
                                if(columnInfo != null) 
                                {
                                    string foreignTable = columnInfo.ForeignTable;
                                    string foreignKeyColumn = columnInfo.ForeignName;

                                    foreach(AddForeignTableColumns foreignColumnInfo in foreignTableinfo.ForeignColumns )
                                    {                                        
                                        columnsToSelect.Add(
                                            (string.IsNullOrEmpty(foreignColumnInfo.OutputName)?
                                                string.Format("\"{0}\".\"{1}\"", foreignTable + level + joinCounter, foreignColumnInfo.ForeignName) :
                                                string.Format("\"{0}\".\"{1}\" AS \"{2}\"", foreignTable + level + joinCounter, foreignColumnInfo.ForeignName, foreignColumnInfo.OutputName)
                                            )
                                        );
                                    }
                                    joinCounter++;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Cannot add foreign tables because Columns definitions were not provided
                        ServiceManager.Instance.GetService<LogService>().Print("Cannot add foreign tables because Columns definitions were not provided!", LoggingLevel.Errors);
                    }
                }
                queryString += string.Join(",\n" + GetTabsByLevel(level + 1), columnsToSelect);
            }
            else if (query.Action == QueryAction.SelectCount)
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

            if (query.Action == QueryAction.SelectRows)
            {
                queryString += "\n" + GetTabsByLevel(level) + "FROM";
                queryString += string.Format("{0}\"{1}\".\"{2}\" \"{3}\"", "\n" + GetTabsByLevel(level + 1), query.SchemaName, query.TableName, query.TableName + level);
            
                if((query.AddForeignTables?.Count ?? 0) > 0)
                {
                    if((query.ColumnsDefinitions?.Count ?? 0) > 0)
                    {
                        int joinCounter = 0;
                        foreach(AddForeignTables foreignTableinfo in query.AddForeignTables)
                        {
                            if((foreignTableinfo.ForeignColumns?.Count ?? 0) > 0)
                            {
                                ColumnInfo columnInfo = query.ColumnsDefinitions.Where( x => x.Name.ToLower().Equals(foreignTableinfo.Name.ToLower())).FirstOrDefault();
                                if(columnInfo != null) 
                                {
                                    string foreignTable = columnInfo.ForeignTable;
                                    string foreignKeyColumn = columnInfo.ForeignName;

                                    queryString += "\n" + GetTabsByLevel(level) + "LEFT JOIN";
                                    queryString += string.Format("{0}\"{1}\".\"{2}\" \"{3}\"", "\n" + GetTabsByLevel(level + 1), columnInfo.ForeignSchema, columnInfo.ForeignTable, columnInfo.ForeignTable + level + joinCounter);
                                    queryString += "\n" + GetTabsByLevel(level) + "ON";
                                    queryString += string.Format("{0}\"{1}\".\"{2}\" =  \"{3}\".\"{4}\"", "\n" + GetTabsByLevel(level + 1), columnInfo.ForeignTable + level + joinCounter, columnInfo.ForeignName, query.TableName + level, columnInfo.Name);
                                    
                                    joinCounter++;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Cannot add foreign tables because Columns definitions were not provided
                        ServiceManager.Instance.GetService<LogService>().Print("Cannot add foreign tables because Columns definitions were not provided!", LoggingLevel.Errors);
                    }
                }
            }
            else if (query.Action == QueryAction.SelectCount || query.Action == QueryAction.DeleteRows)
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