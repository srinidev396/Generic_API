using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class CallApi
    {
        public static async Task<string> GetDbSchemeAsync(HttpClient client, string url)
        {
            string model = string.Empty;
            var call = await client.GetAsync($"{url}/Data/GetDbSchema");
            if (call.IsSuccessStatusCode)
            {
                model = await call.Content.ReadAsStringAsync();
            }
            return model;
        }

        public static async Task<string> GetTableSchema(HttpClient client, string url, string tableName)
        {
            string model = string.Empty;
            var call = await client.GetAsync($"{url}/Data/getTableSchema?tablename={tableName}");
            if (call.IsSuccessStatusCode)
            {
                model = await call.Content.ReadAsStringAsync();
            }
            return model;
        }
        public async static Task<string> AddNewRecoredMulti(HttpClient client, string url)
        {
            string msg = String.Empty;
            var post = new PostModel();
            post.TableName = "Boxes";

            //string[] data = { "test1", "test2", "test3", "test4", "test5" };
            for (int i = 0; i < 5000; i++)
            {
                string data = $"test{i}";
                post.PostColomn.Add(new PostColumns { ColumnName = "Description", Value = data });
                post.PostColomn.Add(new PostColumns { ColumnName = "OffSiteNo", Value = data });
                post.PostColomn.Add(new PostColumns { ColumnName = "Contents", Value = data });
                post.PostColumnsMulti.Add(post.PostColomn);
                post.PostColomn = new List<PostColumns>();
            }
            



            var content = JsonConvert.SerializeObject(post);
            var requestContent = new StringContent(content, Encoding.UTF8, "application/json");

            var getdata = await client.PostAsync($"{url}/Data/NewRecordMulti", requestContent);

            var success = getdata.EnsureSuccessStatusCode();
            if (success.IsSuccessStatusCode)
            {
                msg = await getdata.Content.ReadAsStringAsync();
            }

            return msg;
        }
        public async static Task<string> AddNewRecord(HttpClient client, string url)
        {
            string data = String.Empty;

            var post = new PostModel();
            post.TableName = "Boxes";
            for (int i = 1; i < 1000; i++)
            {
                string dealname = $"Client-Deal";
                post.PostColomn.Add(new PostColumns { ColumnName = "Description", Value = dealname });
                post.PostColomn.Add(new PostColumns { ColumnName = "OffSiteNo", Value = dealname });
                post.PostColomn.Add(new PostColumns { ColumnName = "Contents", Value = dealname });

                var content = JsonConvert.SerializeObject(post);
                var requestContent = new StringContent(content, Encoding.UTF8, "application/json");

                var getdata = await client.PostAsync($"{url}/Data/NewRecord", requestContent);

                var success = getdata.EnsureSuccessStatusCode();
                if (success.IsSuccessStatusCode)
                {
                    data = await getdata.Content.ReadAsStringAsync();
                }
                Console.WriteLine($"added {i}");

            }
            return data;
        }

        public async static Task<string> EditRecord(HttpClient client, string url)
        {
            string data = String.Empty;

            var post = new UIEditModel();
            int id = 1022;
            post.TableName = "Deals";
            post.PostColomn.Add(new PostColumns { ColumnName = "DealName", Value = "motimash" });
            post.PostColomn.Add(new PostColumns { ColumnName = "ClosingDate", Value = "2022-10-08" });
            post.PostColomn.Add(new PostColumns { ColumnName = "DealAmount", Value = "55555" });
            post.keyValue = id.ToString();
            id++;
            var content = JsonConvert.SerializeObject(post);
            var requestContent = new StringContent(content, Encoding.UTF8, "application/json");

            var getdata = await client.PostAsync($"{url}/Data/EditRecord", requestContent);

            var success = getdata.EnsureSuccessStatusCode();
            if (success.IsSuccessStatusCode)
            {
                data = await getdata.Content.ReadAsStringAsync();
            }
            return data;
        }
        public async static Task<string> EditRecordByColumn(HttpClient client, string url)
        {
            string data = String.Empty;

            var post = new UIEditModel();

            post.TableName = "Deals";
            post.FieldName = "DealName";
            post.keyValue = "motimadsfash";
            post.IsMultyupdate = true;

            post.PostColomn.Add(new PostColumns { ColumnName = "DealName", Value = "motimash" });
            post.PostColomn.Add(new PostColumns { ColumnName = "ClosingDate", Value = "2022-10-08" });
            post.PostColomn.Add(new PostColumns { ColumnName = "DealAmount", Value = "55555" });


            var content = JsonConvert.SerializeObject(post);
            var requestContent = new StringContent(content, Encoding.UTF8, "application/json");

            var getdata = await client.PostAsync($"{url}/Data/EditRecordByColumn", requestContent);

            var success = getdata.EnsureSuccessStatusCode();
            if (success.IsSuccessStatusCode)
            {
                data = await getdata.Content.ReadAsStringAsync();
            }
            return data;
        }
        public async static Task<string> EditIfNotExistAdd(HttpClient client, string url)
        {
            string data = String.Empty;

            var post = new UIEditModel();

            post.TableName = "Deals";
            post.FieldName = "DealName";
            post.keyValue = "test";
            post.IsMultyupdate = false;

            post.PostColomn.Add(new PostColumns { ColumnName = "DealName", Value = "newRecord" });
            post.PostColomn.Add(new PostColumns { ColumnName = "ClosingDate", Value = "2022-10-08" });
            post.PostColomn.Add(new PostColumns { ColumnName = "DealAmount", Value = "1010" });


            var content = JsonConvert.SerializeObject(post);
            var requestContent = new StringContent(content, Encoding.UTF8, "application/json");

            var getdata = await client.PostAsync($"{url}/Data/EditIfNotExistAdd", requestContent);

            var success = getdata.EnsureSuccessStatusCode();
            if (success.IsSuccessStatusCode)
            {
                data = await getdata.Content.ReadAsStringAsync();
            }
            return data;
        }

        public async static void TestException(HttpClient client, string url)
        {
            string data = String.Empty;
            var getdata = await client.GetAsync($"{url}/Data/TestExceptionMethod");

            var success = getdata.EnsureSuccessStatusCode();
            if (success.IsSuccessStatusCode)
            {
                data = await getdata.Content.ReadAsStringAsync();
            }
        }
    }
}

public class PostModel
{
    public PostModel()
    {
        PostColomn = new List<PostColumns>();
        PostColumnsMulti = new List<List<PostColumns>>();
    }
    [Required]
    public string TableName { get; set; }
    [Required]
    public List<PostColumns> PostColomn { get; set; }
    public List<List<PostColumns>> PostColumnsMulti { get; set; }
}

//public class PostMultiRecordModel : PostModel
//{
//    public List<List<PostColumns>> PostColomnList { get; set; }
//}
public class UIEditModel : PostModel
{
    [Required]
    public string keyValue { get; set; }
    public string FieldName { get; set; }
    public bool IsMultyupdate { get; set; }

}

public class PostColumns
{
    [Required]
    public string Value { get; set; }
    [Required]
    public string ColumnName { get; set; }
    [Required]
    public string DataTypeFullName { get; set; }
}


