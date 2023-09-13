using System.Data.SqlClient;
using System.Data;
using Smead.Security;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using System.Runtime.Versioning;
using FusionWebApi.Properties;
using Newtonsoft.Json;

namespace FusionWebApi.Models
{
    public class DatabaseSchema
    {
        public static SchemaModel ReturnDbSchema(Passport passport)
        {
            var model = new SchemaModel();
            int tableCounter = 0;
            var skl = new List<GetDbSchema>();
            
            using (SqlConnection conn = new SqlConnection(passport.ConnectionString))
            {
                using SqlCommand cmd = new SqlCommand("", conn);

                if (!(conn.State == ConnectionState.Open))
                {
                    conn.Open();
                }

                var ListOfColumn = new List<List<Columns>>();
                var lst = new List<Columns>();
                foreach (DataRow item in DatabaseSchema.GetCustomerTablesSchema(cmd).Rows)
                {
                    var tableName = item.Field<string>("tableName");
                    var sqlcmd = new SqlCommand(Resources.GetSchema, conn);
                    var tablescm = new DataTable();
                    if (passport.CheckPermission(tableName, SecureObject.SecureObjectType.Table, Permissions.Permission.View))
                    {
                        sqlcmd.Parameters.AddWithValue("@tablename", tableName);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            da.Fill(tablescm);
                        }
                        foreach (DataRow s in tablescm.Rows)
                        {
                            var columnName = s.Field<string>("COLUMN_NAME");
                            var datatype = s.Field<string>("DATA_TYPE");
                            var maxlength = s.Field<object>("CHARACTER_MAXIMUM_LENGTH");
                            int? maxLengthNullable = (maxlength is DBNull) ? null : (int?)Convert.ToInt32(maxlength);
                            var isnullAble = s.Field<string>("IS_NULLABLE");
                            var isprimarykey = s.Field<string>("IS_PRIMARY_KEY");
                            var isautoincrement = s.Field<string>("IS_AUTO_INCREMENT");
                            var primarykeytype = s.Field<string>("PRIMARY_KEY_TYPE");
                            lst.Add(new Columns
                            {
                                ColumnName = columnName,
                                DataType = datatype,
                                MaxLength = maxLengthNullable,
                                IsNullable = isnullAble,
                                IsPrimaryKey = isprimarykey,
                                IsAutoIncrement = isautoincrement,
                                PrimaryKeyType = primarykeytype

                            });
                        }
                        model.getDbSchemas.Add(new GetDbSchema { TableName = tableName, ColumnCount = lst.Count, ListOfColumns = lst });
                        tableCounter++;
                        lst = new List<Columns>();
                    }
                }
                conn.Close();
            }
            model.TotalSchemaTables = tableCounter;
            model.getDbSchemas.OrderBy(a => a.TableName);
            return model;
        }
        public static TablesSchema GetTableSchema(string TableName, Passport passport)
        {

            TablesSchema tc = new TablesSchema();
            var table = new DataTable();
            // var sqlstring = $"select a.TABLE_NAME, a.COLUMN_NAME, a.DATA_TYPE, a.IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS a WHERE a.TABLE_NAME = @tablename";
            if (passport.CheckPermission(TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.View))
            {
                using (SqlConnection conn = new SqlConnection(passport.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(Resources.GetSchema, conn))
                    {
                        //cmd.CommandText = sqlstring;
                        cmd.Parameters.AddWithValue("@tablename", TableName);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(table);
                        }
                    }

                };

                foreach (DataRow item in table.Rows)
                {
                    var tableName = item.Field<string>("TABLE_NAME").Trim();
                    var columnName = item.Field<string>("COLUMN_NAME");
                    var datatype = item.Field<string>("DATA_TYPE");
                    var maxlength = item.Field<object>("CHARACTER_MAXIMUM_LENGTH");
                    int? maxLengthNullable = (maxlength is DBNull) ? null : (int?)Convert.ToInt32(maxlength);
                    var isnullAble = item.Field<string>("IS_NULLABLE");
                    var isprimarykey = item.Field<string>("IS_PRIMARY_KEY");
                    var isautoincrement = item.Field<string>("IS_AUTO_INCREMENT");
                    var primarykeytype = item.Field<string>("PRIMARY_KEY_TYPE");


                    tc.ListOfColumns.Add(new Columns
                    {
                        ColumnName = columnName,
                        DataType = datatype,
                        MaxLength = maxLengthNullable,
                        IsNullable = isnullAble,
                        IsPrimaryKey = isprimarykey,
                        IsAutoIncrement = isautoincrement,
                        PrimaryKeyType = primarykeytype
                    });

                }
                tc.TableName = TableName.Trim();
                tc.ColumsCount = tc.ListOfColumns.Count;
            }
            return tc;
        }

        public static string GetColumntype(Passport passport, UIPostModel userdata)
        {
            var schema = DatabaseSchema.GetTableSchema(userdata.TableName, passport).ListOfColumns;
            var counter = 0;
            foreach (PostColumns c in userdata.PostRow)
            {
                var col = schema.Where(a => a.ColumnName == c.ColumnName);
                if (col.Count() > 0)
                {
                    switch (col.FirstOrDefault().DataType)
                    {
                        case "varchar":
                            userdata.PostRow[counter].DataTypeFullName = "System.String";
                            break;
                        case "nvarchar":
                            userdata.PostRow[counter].DataTypeFullName = "System.String";
                            break;
                        case "text":
                            userdata.PostRow[counter].DataTypeFullName = "System.String";
                            break;
                        case "smallint":
                            userdata.PostRow[counter].DataTypeFullName = "System.Int16";
                            break;
                        case "int":
                            userdata.PostRow[counter].DataTypeFullName = "System.Int32";
                            break;
                        case "float":
                            userdata.PostRow[counter].DataTypeFullName = " System.Double";
                            break;
                        case "datetime":
                            userdata.PostRow[counter].DataTypeFullName = "System.DateTime";
                            break;
                        case "bit":
                            userdata.PostRow[counter].DataTypeFullName = "System.boolean";
                            break;
                        default:
                            break;
                    }           
                }
                else
                {
                    return $"Column {c.ColumnName} doesn't exist!";
                }
                counter++;
            }
            return "true";
        }

        public static string GetColumntypeMulti(Passport passport, UIPostModel userdata)
        {
            var schema = DatabaseSchema.GetTableSchema(userdata.TableName, passport).ListOfColumns;
            var counter = 0;
            for (int i = 0; i < userdata.PostMultiRows.Count; i++)
            {
                for (int j = 0; j < userdata.PostMultiRows[i].Count; j++)
                {
                    var col = schema.Where(a => a.ColumnName == userdata.PostMultiRows[i][j].ColumnName);
                    if (col.Count() > 0)
                    {
                        switch (col.FirstOrDefault().DataType)
                        {
                            case "varchar":
                                userdata.PostMultiRows[i][j].DataTypeFullName = "System.String";
                                break;
                            case "nvarchar":
                                userdata.PostMultiRows[i][j].DataTypeFullName = "System.String";
                                break;
                            case "text":
                                userdata.PostMultiRows[i][j].DataTypeFullName = "System.String";
                                break;
                            case "smallint":
                                userdata.PostMultiRows[i][j].DataTypeFullName = "System.Int16";
                                break;
                            case "int":
                                userdata.PostMultiRows[i][j].DataTypeFullName = "System.Int32";
                                break;
                            case "float":
                                userdata.PostMultiRows[i][j].DataTypeFullName = " System.Double";
                                break;
                            case "datetime":
                                userdata.PostMultiRows[i][j].DataTypeFullName = "System.DateTime";
                                break;
                            case "bit":
                                userdata.PostMultiRows[i][j].DataTypeFullName = "System.boolean";
                                break;
                            default:
                                break;
                        }
                        counter++;
                    }
                    else
                    {
                        return $"Column {userdata.PostMultiRows[i][j].ColumnName} doesn't exist!";

                    }
                }
            }
            return "true";
        }
        private static DataTable GetCustomerTablesSchema(SqlCommand cmd)
        {
            var table = new DataTable();
            DataTable tablescm = new DataTable();
            cmd.CommandText = "SELECT tableName FROM tables ORDER BY TableName";
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(table);
            }
            return table;
        }

        public class TablesSchema
        {
            public TablesSchema()
            {
                ListOfColumns = new List<Columns>();
                ErrorMessages = new ErrorMessages();
                //ListOfColumn = new List<List<Columns>>();
            }
            public string TableName { get; set; }
            public int ColumsCount { get; set; }
            public List<Columns> ListOfColumns { get; set; }
            public ErrorMessages ErrorMessages { get; set; }
        }
        public class Columns
        {
            public string ColumnName { get; set; }
            public string DataType { get; set; }
            public object MaxLength { get; set; }
            public string IsNullable { get; set; }
            public string IsPrimaryKey { get; set; }
            public string IsAutoIncrement { get; set; }
            public string PrimaryKeyType { get; set; }

        }

        public class GetDbSchema
        {
            public GetDbSchema()
            {
                ListOfColumns = new List<Columns>();
            }
            [JsonProperty(Order = 1)]
            public string TableName { get; set; }
            [JsonProperty(Order = 2)]
            public int ColumnCount { get; set; }
            [JsonProperty(Order = 3)]
            public List<Columns> ListOfColumns { get; set; }
        }

        public class SchemaModel
        {
            public SchemaModel()
            {
                ErrorMessages = new ErrorMessages();
                getDbSchemas = new List<GetDbSchema>();
            }

            [JsonProperty(Order = 1)]
            public int TotalSchemaTables { get; set; }
            public List<GetDbSchema> getDbSchemas { get; set; }
            public ErrorMessages ErrorMessages { get; set; }
        }

    }
}
