using Microsoft.AspNetCore.Mvc;
using Smead.RecordsManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FusionWebApi.Models
{
    public class UIPostModel
    {
        public UIPostModel()
        {
            PostRow = new List<PostColumns>();
            PostMultiRows = new List<List<PostColumns>>();
        }
        [Required]
        public string TableName { get; set; }
        public string keyValue { get; set; }
        public string FieldName { get; set; }
        public bool IsMultyupdate { get; set; }
        public List<PostColumns> PostRow { get; set; }
        public List<List<PostColumns>> PostMultiRows { get; set; }
    }

    public class PostColumns : IHasValue
    {
        public string Value { get; set; }
        [Required]
        public string ColumnName { get; set; }
        [HiddenInput]
        public string DataTypeFullName { get; set; }
    }

    public class Viewmodel
    {
        public Viewmodel()
        {
            ErrorMessages = new ErrorMessages();
        }
        public List<TableHeadersProperty> ListOfHeaders { get; set; }
        public List<List<string>> ListOfDatarows { get; set; }
        public decimal TotalPages { get; set; }
        public ErrorMessages ErrorMessages { get; set; }
        public int TotalRowsQuery { get; set; }
        public int RowsPerPage { get; set; }
        public int Viewid { get; set; }
        public string TableName { get; set; }
        public string ViewName { get; set; }
        public int PageNumber { get; set; }
    }

    public class TableHeadersProperty
    {
        public TableHeadersProperty(string headername, string issort, string datatype, string isdropdown, string isEditable, int columnOrder, string editmask, bool allownull, string dataTypeFullName, string ColumnName, bool isprimarykey, int maxlength, bool iscounterField)
        {
            HeaderName = headername;
            Issort = Convert.ToBoolean(issort);
            DataType = datatype;
            isDropdown = Convert.ToBoolean(isdropdown);
            this.isEditable = Convert.ToBoolean(isEditable);
            this.columnOrder = columnOrder;
            editMask = editmask;
            Allownull = allownull;
            DataTypeFullName = dataTypeFullName;
            this.ColumnName = ColumnName;
            IsPrimarykey = isprimarykey;
            MaxLength = maxlength;
            isCounterField = iscounterField;
        }
        public string HeaderName { get; set; }
        public bool Issort { get; set; }
        public string DataType { get; set; }
        public bool isDropdown { get; set; }
        public bool isEditable { get; set; }
        public int columnOrder { get; set; }
        public string editMask { get; set; }
        public bool Allownull { get; set; }
        public string DataTypeFullName { get; set; }
        public string ColumnName { get; set; }
        public bool IsPrimarykey { get; set; }
        public int MaxLength { get; set; }
        public bool isCounterField { get; set; }
    }
    public class UserViews
    {
        public UserViews()
        {
            ErrorMessages = new ErrorMessages();
        }
        public List<Navigation.ListOfviews> listOfviews {  get; set; }
        public ErrorMessages ErrorMessages { get; set; }
    }
    public class Records
    {
        public Records()
        {
            ErrorMessages = new ErrorMessages();
        }
        public string FusionMessage { get; set; }
        public ErrorMessages ErrorMessages { get; set; }
    }


    public class ErrorMessages
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public string FusionMessage { get; set; }
        public int FusionCode { get; set; }
        public void LogErrorMessage(string code, string message, DateTime dateTime)
        {

        }
    }

    public enum EventCode
    {
        None = 0,
        LoginFail = -1,
        LoginSuccess = 1,
        WrongValue = 2,
        NewRecordAdded = 3,
        NewRecordsAdded = 4,
        NoColumn = 5,
        NoRow = 6,
        RecordUpdated = 7,
        insufficientpermissions = 8,
        IllegalData = 9,
        ColumnIsnotExist = 10
    }


}
