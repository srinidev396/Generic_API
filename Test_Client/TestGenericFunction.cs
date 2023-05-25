using ConsoleApp1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class TestGenericFunction
    {

    }

    public class TablesSchema
    {
        public TablesSchema()
        {
            ListOfColumns = new List<Columns>();
            ListOfColumn = new List<List<Columns>>();
        }
        public string? TableName { get; set; }
        public int ColumsCount { get; set; }
        //public string ColumnName { get; set; }
        //public string DataType { get; set; }
        //public string IsNullable { get; set; }
        public List<Columns> ListOfColumns { get; set; }
        public List<List<Columns>> ListOfColumn { get; set; }
        public static async Task<List<GetDbSchema>> GetAllTablesSchema(HttpClient client, string url)
        {
            var lst = new List<GetDbSchema>();
            try
            {
                var getdata = await client.GetAsync($"{url}/Data/GetDbSchema");
                var success = getdata.EnsureSuccessStatusCode();
                if (success.IsSuccessStatusCode)
                {
                    string data = await getdata.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<List<GetDbSchema>>(data);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return lst;
        }
        public static async Task<TablesSchema> GetTableSchema(HttpClient client, string url)
        {
            var lst = new TablesSchema();
            try
            {
                var getdata = await client.GetAsync($"{url}/Data/GetTableSchema?Tablename=boxes");
                var success = getdata.EnsureSuccessStatusCode();
                if (success.IsSuccessStatusCode)
                {
                    string data = await getdata.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<TablesSchema>(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return lst;
        }
    }
    public class Columns
    {
        public string? ColumnName { get; set; }
        public string? DataType { get; set; }
        public string? IsNullable { get; set; }

    }

    public class GetDbSchema
    {
        public GetDbSchema()
        {
            ListOfColumns = new List<Columns>();
        }
        public List<Columns>? ListOfColumns { get; set; }
        public string? TableName { get; set; }
        public int ColumnCount { get; set; }
    }
}

//var listoftables = await TablesSchema.GetAllTablesSchema(client, url);
//File.WriteAllText(@"E:\Temp\json.txt", "");
////create html result:
//using StreamWriter myfile = new(@"E:\Temp\json.txt", append: true);
////await myfile.WriteLineAsync(JsonConvert.SerializeObject(listoftables));
//await myfile.WriteLineAsync("<html><body>");

//foreach (GetDbSchema item in listoftables)
//{
//    await myfile.WriteLineAsync($"<h3><span style='color:blue'>TableName:</span> {item.TableName}</h3>");
//    await myfile.WriteLineAsync($"<h3><span style='color:blue'>Column Count:</span> {item.ColumnCount.ToString()}</h3>");
//    foreach (Columns col in item.ListOfColumns)
//    {
//        await myfile.WriteLineAsync($"<p>ColName: {col.ColumnName} ColType: {col.DataType} IsNull: ;{col.IsNullable}</p>");
//    }
//    await myfile.WriteLineAsync("<p>------------------------------------------------------------------------</p>");
//}
//await myfile.WriteLineAsync("</body></html>");
////var table = await TablesSchema.GetTableSchema(client, url);
////File.WriteAllText(@"D:\TempFiles\json.txt", JsonConvert.SerializeObject(table));
//string xxx = "";
