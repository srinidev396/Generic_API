using Smead.RecordsManagement;
using Smead.Security;
using System.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using Microsoft.VisualBasic;
using System.Data.SqlClient;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace FusionWebApi.Models
{

    public class RecordsActions
    {
        //private string KeyValue { get; set; }
        //private string FieldName { get; set; }
        public RecordsActions(Passport passport)
        {
            this.passport = passport;
        }
        public Passport passport { get; set; }
        public bool AddNewRow(UIPostModel PostData)
        {
            bool ispass = false;
            var param = new Parameters(PostData.TableName, passport);
            if (passport.CheckPermission(PostData.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Add))
            {
                param.Scope = ScopeEnum.Table;
                param.NewRecord = true;
                //var col = PostData.PostColomn[0];
                // Columns = PostData.PostColomn;
                param.AfterData = GetAfterData(PostData.PostRow);
                //param.RequestedRows = 1;

                //param.Culture = Keys.GetCultureCookies(_httpContext);
                ScriptReturn result = null;

                Query.Save(param, "", param.KeyField, "", DataFieldValues(PostData.PostRow), passport, result);

                var withBlock = AuditType.WebAccess;
                withBlock.TableName = param.TableName;
                withBlock.TableId = param.KeyValue;
                withBlock.ClientIpAddress = "RestAPI Call";
                withBlock.ActionType = AuditType.WebAccessActionType.AddRecord;
                withBlock.AfterData = param.AfterDataTrimmed;
                withBlock.BeforeData = string.Empty;

                Auditing.AuditUpdates(AuditType.WebAccess, passport);

                string retentionCode = Query.SetRetentionCode(param.TableName, param.TableInfo, param.KeyValue, passport);
                DataRow row = Navigation.GetSingleRow(param.TableInfo, param.KeyValue, param.KeyField, passport);
                Tracking.SetRetentionInactiveFlag(param.TableInfo, row, retentionCode, passport);
                ispass = true;
            }
            return ispass;
        }
        public string AddNewRowMulti(UIPostModel PostData)
        {

            return Query.AddNewMultiRecords(passport, PostData.TableName, DataFieldValuesMulti(PostData.PostMultiRows));
        }
        public bool EditRow(UIPostModel EditData)
        {
            bool ispass = false;
            var param = new Parameters(EditData.TableName, passport);
            if (passport.CheckPermission(param.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Edit))
            {
                param.Scope = ScopeEnum.Table;
                param.KeyValue = EditData.keyValue;
                param.NewRecord = false;
                param.BeforeData = "";
                // Columns = EditData.PostColomn;
                param.AfterData = GetAfterData(EditData.PostRow);
                //param.Culture = Keys.GetCultureCookies(_httpContext);

                // linkscript before
                ScriptReturn result = null;
                var BeforeDataTrimmed = GetBeforeDataTrimmedEdit(EditData.PostRow, passport, param)[0];
                // save row
                Query.Save(param, "", param.KeyField, param.KeyValue, DataFieldValues(EditData.PostRow), passport, result);

                //save audit
                {
                    var withBlock = AuditType.WebAccess;
                    withBlock.TableName = param.TableName;
                    withBlock.TableId = param.KeyValue;
                    withBlock.ClientIpAddress = "RestAPI Call";
                    withBlock.ActionType = AuditType.WebAccessActionType.UpdateRecord;
                    withBlock.AfterData = param.AfterDataTrimmed;
                    withBlock.BeforeData = BeforeDataTrimmed;
                }

                Auditing.AuditUpdates(AuditType.WebAccess, passport);

                string retentionCode = Query.SetRetentionCode(param.TableName, param.TableInfo, param.KeyValue, passport);
                DataRow row = Navigation.GetSingleRow(param.TableInfo, param.KeyValue, param.KeyField, passport);
                Smead.RecordsManagement.Tracking.SetRetentionInactiveFlag(param.TableInfo, row, retentionCode, passport);
                ispass = true;
            }
            return ispass;

        }
        private List<string> GetBeforeDataTrimmedEditBycolumn(UIPostModel model, Passport pass, string fieldnametype)
        {
            string columns = string.Empty;
            string sql = string.Empty;
            string beforedata = string.Empty;
            var listofcolumns = new List<string>();
            var listofrows = new List<string>();
            var table = new DataTable();

            var counter = 1;
            foreach (PostColumns col in model.PostRow)
            {
                listofcolumns.Add(col.ColumnName);

                if (model.PostRow.Count == counter)
                {
                    columns += $"[{col.ColumnName}]";
                }
                else
                {
                    columns += $"[{col.ColumnName}],";
                }
                counter++;
            }

            if (model.keyValue.ToLower() == "null")
            {
                sql = $"SELECT {columns} FROM {model.TableName} WHERE {model.FieldName} is {model.keyValue}";
            }
            else if (fieldnametype == "text")
            {
                sql = $"SELECT {columns} FROM {model.TableName} WHERE CAST({model.FieldName} as nvarchar) = '{model.keyValue}'";
            }
            else
            {
                sql = $"SELECT {columns} FROM {model.TableName} WHERE {model.FieldName} = '{model.keyValue}'";
            }

            var conn = pass.Connection();
            var cmd = new SqlCommand(sql, conn);
            var adp = new SqlDataAdapter(cmd);
            adp.Fill(table);
            for (int j = 0; j < table.Rows.Count; j++)
            {
                for (int i = 0; i < listofcolumns.Count; i++)
                {
                    beforedata += $"{listofcolumns[i].ToString()}: {table.Rows[j][listofcolumns[i].ToString()]}";
                }
                listofrows.Add(beforedata);
                beforedata = string.Empty;
            }


            return listofrows;
        }
        private List<string> GetBeforeDataTrimmedEdit(List<PostColumns> lst, Passport pass, Parameters param)
        {
            string columns = string.Empty;
            string sql = string.Empty;
            string beforedata = string.Empty;
            var listofcolumns = new List<string>();
            var listofrows = new List<string>();
            var table = new DataTable();

            var counter = 1;
            foreach (PostColumns col in lst)
            {
                listofcolumns.Add(col.ColumnName);

                if (lst.Count == counter)
                {
                    columns += $"[{col.ColumnName}]";
                }
                else
                {
                    columns += $"[{col.ColumnName}],";
                }
                counter++;
            }

            sql = $"SELECT {columns} FROM {param.TableName} WHERE {param.KeyField} = '{param.KeyValue}'";
            var conn = pass.Connection();
            var cmd = new SqlCommand(sql, conn);
            var adp = new SqlDataAdapter(cmd);
            adp.Fill(table);
            for (int j = 0; j < table.Rows.Count; j++)
            {
                for (int i = 0; i < listofcolumns.Count; i++)
                {
                    beforedata += $"{listofcolumns[i].ToString()}: {table.Rows[j][listofcolumns[i].ToString()]}";
                }
                listofrows.Add(beforedata);
                beforedata = string.Empty;
            }


            return listofrows;
        }
        public string EditRecordByColumn(UIPostModel Ed)
        {
            //var param = new Parameters(Ed.TableName, passport);
            //param.Scope = ScopeEnum.Table;
            //param.KeyValue = Ed.keyValue;
            //param.NewRecord = false;
            //param.KeyField = Ed.FieldName;
            //param.BeforeData = "";
            if (passport.CheckPermission(Ed.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Edit))
            {
            var FieldNameType = DatabaseSchema.GetTableSchema(Ed.TableName, passport).ListOfColumns.Where(a => a.ColumnName.ToLower() == Ed.FieldName.ToLower()).FirstOrDefault().DataType;
            var ListbeforeDataTrim = GetBeforeDataTrimmedEditBycolumn(Ed, passport, FieldNameType);
            var data = DataFieldValues(Ed.PostRow);
            return Query.UpdateRecordsByColumn(Ed.keyValue, Ed.FieldName, Ed.TableName, passport, data, Ed.IsMultyupdate, FieldNameType, ListbeforeDataTrim);
            
            }
            else
            {
                return "nopermission";
            }

        }
        private List<FieldValue> DataFieldValues(List<PostColumns> ListOfcolumns)
        {
            var lst = new List<FieldValue>();
            foreach (var row in ListOfcolumns)
            {
                if (!string.IsNullOrEmpty(row.ColumnName) && !string.IsNullOrEmpty(row.DataTypeFullName))
                {
                    // If param.KeyField = row.columnName Then
                    // param.KeyValue = row.value
                    // End If
                    var field = new FieldValue(row.ColumnName, row.DataTypeFullName);
                    if (row.Value is null)
                    {
                        field.value = "";
                    }
                    else if (row.DataTypeFullName == "System.DateTime")
                    {
                        field.value = row.Value;//Keys.get_ConvertCultureDate(row.value, "E", _httpContext);
                    }
                    else
                    {
                        field.value = row.Value;

                    }
                    lst.Add(field);
                }
            }
            return lst;
        }
        private List<List<FieldValue>> DataFieldValuesMulti(List<List<PostColumns>> listoflistcolumns)
        {

            var lstof = new List<List<FieldValue>>();
            for (int i = 0; i < listoflistcolumns.Count; i++)
            {
                var lst = new List<FieldValue>();
                for (int j = 0; j < listoflistcolumns[i].Count; j++)
                {
                    var row = listoflistcolumns[i][j];
                    if (!string.IsNullOrEmpty(row.ColumnName) && !string.IsNullOrEmpty(row.DataTypeFullName))
                    {
                        var field = new FieldValue(row.ColumnName, row.DataTypeFullName);
                        if (row.Value is null)
                        {
                            field.value = "";
                        }
                        else if (row.DataTypeFullName == "System.DateTime")
                        {
                            field.value = row.Value;//Keys.get_ConvertCultureDate(row.value, "E", _httpContext);
                        }
                        else
                        {
                            field.value = row.Value;
                        }
                        lst.Add(field);

                    }
                }
                lstof.Add(lst);
            }
            return lstof;
        }
        public static string GetAfterData(List<PostColumns> lst)
        {
            string afteradd = string.Empty;
            foreach (var item in lst)
            {
                afteradd += $"{item.ColumnName}: {item.Value} ";
            }
            return afteradd;
        }
        //return view 
        public Viewmodel GetviewData(int viewid, int pageNumber)
        {
            var v = new Viewmodel();
            var query = new Query(passport);
            var param = new Parameters(viewid, passport);
            if (passport.CheckPermission(param.ViewName, Smead.Security.SecureObject.SecureObjectType.View, Permissions.Permission.View))
            {
                param.Paged = true;
                param.PageIndex = pageNumber;
                query.FillData(param);
                v.TotalRowsQuery = TotalQueryRowCount(param.TotalRowsQuery, passport.Connection());
                v.RowsPerPage = RowPerpage(passport, param.ViewId);
                v.TableName = param.TableName;
                v.ViewName = param.ViewName;
                v.Viewid = param.ViewId;
                v.PageNumber = pageNumber;
                v.ListOfHeaders = BuildNewTableHeaderData(param);
                v.ListOfDatarows = Buildrows(param);
                int RowperPage = v.RowsPerPage == 0 ? 100 : v.RowsPerPage;
                v.RowsPerPage = RowperPage;
                decimal totpages = (decimal)v.TotalRowsQuery / RowperPage;
                v.TotalPages = Math.Ceiling(totpages);
                if (pageNumber > v.TotalPages)
                {
                    v.ErrorMessages.FusionCode = (int)EventCode.WrongValue;
                    v.ErrorMessages.FusionMessage = $"That page number {pageNumber} is incorrect";
                }
            }
            else
            {
                v.ErrorMessages.FusionCode = (int)EventCode.insufficientpermissions;
                v.ErrorMessages.FusionMessage = "Insufficient permission";
            }

            return v;
        }
        private int RowPerpage(Passport pass, int viewid)
        {
            var conn = pass.Connection();
            var cmd = new SqlCommand($"SELECT MaxRecsPerFetch FROM Views WHERE Id = {viewid}", conn);
            return (int)cmd.ExecuteScalar();
        }
        private List<TableHeadersProperty> BuildNewTableHeaderData(Parameters param)
        {
            int columnOrder = 0;
            var ListOfHeaders = new List<TableHeadersProperty>();
            foreach (DataColumn col in param.Data.Columns)
            {
                if (ShowColumn(col, 0, param.ParentField))
                {
                    string dataType = col.DataType.Name;
                    var headerName = col.ExtendedProperties["heading"];
                    var isSortable = col.ExtendedProperties["sortable"];
                    var isdropdown = col.ExtendedProperties["dropdownflag"];
                    var isEditable = col.ExtendedProperties["editallowed"];
                    var editmask = col.ExtendedProperties["editmask"];
                    int MaxLength = col.MaxLength;
                    bool isCounterField = false;
                    if (dataType == "Int16")
                    {
                        MaxLength = 5;
                    }
                    else if (dataType == "Int32")
                    {
                        MaxLength = 10;
                    }
                    else if (dataType == "Double")
                    {
                        MaxLength = 53;
                    }

                    var dataTypeFullName = col.DataType.FullName;
                    string ColumnName = col.ColumnName;
                    columnOrder = columnOrder + 1;
                    // build dropdown table
                    bool PrimaryKey = false;
                    if ((param.PrimaryKey ?? "") == (ColumnName ?? ""))
                    {
                        isCounterField = !string.IsNullOrEmpty(param.TableInfo["CounterFieldName"].ToString());
                        ListOfHeaders.Add(new TableHeadersProperty(Convert.ToString(headerName).ToString(), Convert.ToString(isSortable), dataType, Convert.ToString(isdropdown), Convert.ToString(isEditable), columnOrder, Convert.ToString(editmask), col.AllowDBNull, dataTypeFullName, ColumnName, true, MaxLength, isCounterField));
                        PrimaryKey = true;
                    }
                    else
                    {
                        ListOfHeaders.Add(new TableHeadersProperty(Convert.ToString(headerName), Convert.ToString(isSortable), dataType, Convert.ToString(isdropdown), Convert.ToString(isEditable), columnOrder, Convert.ToString(editmask), col.AllowDBNull, dataTypeFullName, ColumnName, false, MaxLength, isCounterField));
                    }
                }
            }
            return ListOfHeaders;
        }
        private List<List<string>> Buildrows(Parameters param)
        {
            var ListOfColumn = new List<string>();
            var ListOfDatarows = new List<List<string>>();
            // build rows
            foreach (DataRow dr in param.Data.Rows)
            {
                // 'get the pkey
                string dataColumn = "";
                foreach (DataColumn col in param.Data.Columns)
                {
                    // If Not dr(col.ColumnName).GetType.ToString.ToLower = "system.boolean" And Not dr(col.ColumnName).GetType.ToString.ToLower = "system.datetime" Then
                    if (ShowColumn(col, 0, param.ParentField) & col.ColumnName.ToString().Length > 0)
                    {
                        if (Convert.ToString(col.ColumnName) is not null)
                        {

                            if (!string.IsNullOrEmpty(dr[col.ColumnName].ToString()))
                            {
                                if (col.DataType.Name == "DateTime")
                                {
                                    dataColumn = Convert.ToString(dr[col.ColumnName.ToString()]).Split(" ")[0];
                                }
                                else
                                {
                                    dataColumn = Convert.ToString(dr[col.ColumnName.ToString()]);
                                }
                            }
                            else
                            {
                                dataColumn = "";
                            }
                        }
                        ListOfColumn.Add(dataColumn);
                    }
                }
                ListOfDatarows.Add(ListOfColumn);
                ListOfColumn = new List<string>();
            }
            return ListOfDatarows;
        }
        private static bool ShowColumn(DataColumn col, int crumblevel, string parentField)
        {
            switch (Convert.ToInt32(col.ExtendedProperties["columnvisible"]))
            {
                case 3:  // Not visible
                    {
                        return false;
                    }
                case 1:  // Visible on level 1 only
                    {
                        if (crumblevel != 0)
                            return false;
                        break;
                    }
                case 2:  // Visible on level 2 and below only
                    {
                        if (crumblevel < 1)
                            return false;
                        break;
                    }
                case 4:  // Smart column- not visible in a drill down when it's the parent.
                    {
                        if (crumblevel > 0 & (parentField.ToLower() ?? "") == (col.ColumnName.ToLower() ?? ""))
                        {
                            return false;
                        }

                        break;
                    }
            }

            if (col.ColumnName.ToLower() == "formattedid")
                return false;
            // If col.ColumnName.ToLower = "id" Then Return False
            if (col.ColumnName.ToLower() == "attachments")
                return false;
            if (col.ColumnName.ToLower() == "slrequestable")
                return false;
            if (col.ColumnName.ToLower() == "itemname")
                return false;
            if (col.ColumnName.ToLower() == "pkey")
                return false;
            if (col.ColumnName.ToLower() == "dispositionstatus")
                return false;
            if (col.ColumnName.ToLower() == "processeddescfieldnameone")
                return false;
            if (col.ColumnName.ToLower() == "processeddescfieldnametwo")
                return false;
            if (col.ColumnName.ToLower() == "rownum")
                return false;
            return true;
        }

        private static int TotalQueryRowCount(string sql, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT COUNT(*) " + Strings.Right(sql, sql.Length - Strings.InStr(sql, " FROM ", CompareMethod.Text)), conn))
            {
                cmd.CommandTimeout = 60;

                try
                {
                    return (int)(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return 0;
                }
            }
        }

        public bool DataValidation<T>(List<T> objlist) where T : IHasValue
        {
            string pattern = @"<script>.*?</script>";
            Regex regex = new Regex(pattern);
            if (objlist.Count == 0) return true;

            foreach (T obj in objlist)
            {
                var x = regex.Match(obj.Value);
                if (x.Success)
                {
                    return false;
                }
            }
            return true;
        }
    }
    //Interfaces
    public interface IHasValue
    {
        string Value { get; }
    }
}
