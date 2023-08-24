using System.Data.SqlClient;
using System.Data;
using Smead.Security;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace FusionWebApi.Models
{
    public class DatabaseSchema
    {
        public static List<GetDbSchema> ReturnDbSchema(Passport passport)
        {
            var tablescm = new DataTable();
            TablesSchema tc = new TablesSchema();
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
                var TempTableName = string.Empty;
                foreach (DataRow item in DatabaseSchema.GetCustomerTablesSchema(cmd).Rows)
                {
                    var tableName = item.Field<string>("TABLE_NAME");
                    if (passport.CheckPermission(tableName, SecureObject.SecureObjectType.Table, Permissions.Permission.View))
                    {
                        var columnName = item.Field<string>("COLUMN_NAME");
                        var datatype = item.Field<string>("DATA_TYPE");
                        var isnullAble = item.Field<string>("IS_NULLABLE");

                        if (TempTableName != tableName && TempTableName != String.Empty)
                        {
                            ListOfColumn.Add(lst); 
                            skl.Add(new GetDbSchema { TableName = TempTableName, ColumnCount = lst.Count, ListOfColumns = lst });
                            lst = new List<Columns>();

                            lst.Add(new Columns { ColumnName = columnName, DataType = datatype, IsNullable = isnullAble });
                        }
                        else
                        {
                            lst.Add(new Columns { ColumnName = columnName, DataType = datatype, IsNullable = isnullAble });
                        }

                        TempTableName = tableName;
                    }
                }
                conn.Close();
                skl.Add(new GetDbSchema { TableName = TempTableName, ColumnCount = lst.Count, ListOfColumns = lst });
            }
            skl.OrderBy(a => a.TableName);
            return skl;
        }
        public static TablesSchema GetTableSchema(string TableName, Passport passport)
        {

            TablesSchema tc = new TablesSchema();
            var table = new DataTable();
            var sqlstring = $"select a.TABLE_NAME, a.COLUMN_NAME, a.DATA_TYPE, a.IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS a WHERE a.TABLE_NAME = @tablename";
            if (passport.CheckPermission(TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.View))
            {
                using (SqlConnection conn = new SqlConnection(passport.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlstring, conn))
                    {
                        cmd.CommandText = sqlstring;
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
                    var isnullAble = item.Field<string>("IS_NULLABLE");
                    tc.ListOfColumns.Add(new Columns { ColumnName = columnName, DataType = datatype, IsNullable = isnullAble });

                }
                tc.TableName = TableName.Trim();
                tc.ColumsCount = tc.ListOfColumns.Count;
            }
            return tc;
        }

        public static void GetColumntype(Passport passport, UIPostModel userdata)
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
                counter++;
            }
        }

        public static void GetColumntypeMulti(Passport passport, UIPostModel userdata)
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
                                userdata.PostRow[counter].DataTypeFullName = "System.String";
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
                }
            }
        }
        private static DataTable GetCustomerTablesSchema(SqlCommand cmd)
        {
            var table = new DataTable();
            DataTable tablescm = new DataTable();
            cmd.CommandText = "select tableName from tables order by TableName";
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(table);
            }
            string paramss = string.Empty;
            var counter = 0;
            foreach (DataRow item in table.Rows)
            {
                counter++;
                var tablename = item.Field<string>("tableName");
                if (counter == table.Rows.Count)
                {
                    paramss += $"TABLE_NAME = @{tablename}";
                }
                else
                {
                    paramss += $"TABLE_NAME = @{tablename} or ";
                }
                cmd.Parameters.AddWithValue($"@{tablename}", tablename);

            }

            var sqlstring = $"select a.TABLE_NAME, a.COLUMN_NAME, a.DATA_TYPE, a.IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS a WHERE {paramss}";
            cmd.CommandText = sqlstring;
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(tablescm);
            }

            return tablescm;
        }
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
        public string IsNullable { get; set; }

    }

    public class GetDbSchema
    {
        public GetDbSchema()
        {
            ListOfColumns = new List<Columns>();
        }
        public List<Columns> ListOfColumns { get; set; }
        public string TableName { get; set; }
        public int ColumnCount { get; set; }
    }

    public class SchemaModel
    {
        public SchemaModel()
        {
                ErrorMessages = new ErrorMessages();
        }
        public List<GetDbSchema> getDbSchemas { get; set; }
        public ErrorMessages ErrorMessages { get; set; } 
    }

}
