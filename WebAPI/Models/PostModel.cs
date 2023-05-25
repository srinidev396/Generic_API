using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FusionWebApi.Models
{
    public class UIPostModel
    {
        public UIPostModel()
        {
            PostColomn = new List<PostColumns>();
            PostColumnsMulti = new List<List<PostColumns>>();
        }
        [Required]
        public string TableName { get; set; }
        public string keyValue { get; set; }
        public string FieldName { get; set; }
        public bool IsMultyupdate { get; set; }
        public List<PostColumns> PostColomn { get; set; }
        public List<List<PostColumns>> PostColumnsMulti { get; set; }
    }
    //public class UIEditModel : UIPostModel
    //{
    //    [Required]
    //    public string keyValue { get; set; }
    //}
    //public class UIEditBycolumnModel : UIPostModel
    //{
    //    [Required]
    //    public string keyValue { get; set; }
    //    [Required]
    //    public string FieldName { get; set; }
    //    [Required]
    //    public bool IsMultyupdate { get; set; }
    //}


    public class PostColumns
    {
        public string Value { get; set; }
        [Required]
        public string ColumnName { get; set; }
        [HiddenInput]
        public string DataTypeFullName { get; set; }
        //public string DataTypeFullName { get; set; }
    }
}
