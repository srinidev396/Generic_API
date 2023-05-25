using Smead.RecordsManagement;
using Smead.Security;
using System.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Drawing.Imaging;

namespace FusionWebApi.Models
{

    public class RecordsActions
    {
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
                param.AfterData = GetAfterData(PostData.PostColomn);
                //param.RequestedRows = 1;

                //param.Culture = Keys.GetCultureCookies(_httpContext);
                ScriptReturn result = null;

                Query.Save(param, "", param.KeyField, "", DataFieldValues(PostData.PostColomn), passport, result);

                var withBlock = AuditType.WebAccess;
                withBlock.TableName = param.TableName;
                withBlock.TableId = param.KeyValue;
                withBlock.ClientIpAddress = "RestAPI Call";
                withBlock.ActionType = AuditType.WebAccessActionType.AddRecord;
                withBlock.AfterData = param.AfterDataTrimmed;
                withBlock.BeforeData = string.Empty;


                //Auditing.AuditUpdates(AuditType.WebAccess, passport);

                string retentionCode = Query.SetRetentionCode(param.TableName, param.TableInfo, param.KeyValue, passport);
                DataRow row = Navigation.GetSingleRow(param.TableInfo, param.KeyValue, param.KeyField, passport);
                Tracking.SetRetentionInactiveFlag(param.TableInfo, row, retentionCode, passport);
                ispass = true;
            }
            return ispass;
        }
        public string AddNewRowMulti(UIPostModel PostData)
        {
            return Query.AddNewMultiRecords(passport, PostData.TableName, DataFieldValuesMulti(PostData.PostColumnsMulti)); 
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
                param.AfterData = GetAfterData(EditData.PostColomn);
                //param.Culture = Keys.GetCultureCookies(_httpContext);

                // linkscript before
                ScriptReturn result = null;
                // save row
                Query.Save(param, "", param.KeyField, param.KeyValue, DataFieldValues(EditData.PostColomn), passport, result);

                //save audit
                {
                    var withBlock = AuditType.WebAccess;
                    withBlock.TableName = param.TableName;
                    withBlock.TableId = param.KeyValue;
                    withBlock.ClientIpAddress = "RestAPI Call";
                    withBlock.ActionType = AuditType.WebAccessActionType.UpdateRecord;
                    withBlock.AfterData = param.AfterDataTrimmed;
                    withBlock.BeforeData = param.BeforeDataTrimmed;
                }

                Auditing.AuditUpdates(AuditType.WebAccess, passport);

                string retentionCode = Query.SetRetentionCode(param.TableName, param.TableInfo, param.KeyValue, passport);
                DataRow row = Navigation.GetSingleRow(param.TableInfo, param.KeyValue, param.KeyField, passport);
                Smead.RecordsManagement.Tracking.SetRetentionInactiveFlag(param.TableInfo, row, retentionCode, passport);
                ispass = true;
            }
            return ispass;

        }
        public string EditRecordByColumn(UIPostModel Ed)
        {
            var data = DataFieldValues(Ed.PostColomn);
            return Query.UpdateRecordsByColumn(Ed.keyValue, Ed.FieldName, Ed.TableName, passport, data, Ed.IsMultyupdate);
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
    }
}
