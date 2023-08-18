using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using ConsoleApp1;
using System.Linq;


//Get token
string url = "http://localhost:5000"; //"https://restapi.tabfusionrms.com";
string DatabaseName = "cfg";
string UserName = "administrator";
string Password = "password$";
string token = String.Empty;

var client = new HttpClient();
client.Timeout = TimeSpan.FromMinutes(100);
var response = await client.GetAsync($"{url}/GenerateToken?userName={UserName}&passWord={Password}&database={DatabaseName}");
if (response.IsSuccessStatusCode)
{
    token = await response.Content.ReadAsStringAsync();
    response.EnsureSuccessStatusCode();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
}
else
{
    Console.WriteLine("Not Authorize!");
    return;
}


//Test Cases
//CallApi.TestException(client, url);

//###EDIT_IF_NOT_EXIST_ADD###
//var start = DateTime.Now;
//var call = await CallApi.EditIfNotExistAdd(client, url);
//var end = DateTime.Now;
//var hours = Math.Round((end - start).TotalHours);
//var minutes = Math.Round((end - start).TotalMinutes);
//var seconds = Math.Round((end - start).TotalSeconds);
//Console.WriteLine($"Msg from API: {call.ToString()} in {hours}:{minutes}:{seconds}");
//Console.ReadLine();

//###EDIT_RECORD_BY_COLUMN###
//var start = DateTime.Now;
//var call = await CallApi.EditRecordByColumn(client, url);
//var end = DateTime.Now;
//var hours = Math.Round((end - start).TotalHours);
//var minutes = Math.Round((end - start).TotalMinutes);
//var seconds = Math.Round((end - start).TotalSeconds);
//Console.WriteLine($"Msg from API: {call.ToString()} in {hours}:{minutes}:{seconds}");
//Console.ReadLine();


//###EDIT_RECORD###
//var start = DateTime.Now;
//var call = await CallApi.EditRecord(client, url);
//var end = DateTime.Now;
//var hours = Math.Round((end - start).TotalHours);
//var minutes = Math.Round((end - start).TotalMinutes);
//var seconds = Math.Round((end - start).TotalSeconds);
//Console.WriteLine($"Msg from API: {call.ToString()} in {hours}:{minutes}:{seconds}");
//Console.ReadLine();

//###ADD_NEW_RECORD###
//var start = DateTime.Now;
//var call = await CallApi.AddNewRecord(client, url);
//var end = DateTime.Now;
//var hours = Math.Round((end - start).TotalHours);
//var minutes = Math.Round((end - start).TotalMinutes);
//var seconds = Math.Round((end - start).TotalSeconds);
//Console.WriteLine($"Msg from Moti:  Evan WE DID IT {call.ToString()} in {hours}:{minutes}:{seconds}");
//Console.ReadLine();

//##ADD_MULTIPL_RECORD
//var start = DateTime.Now;
//var call = await CallApi.AddNewRecoredMulti(client, url);
//var end = DateTime.Now;
//var hours = Math.Round((end - start).TotalHours);
//var minutes = Math.Round((end - start).TotalMinutes);
//var seconds = Math.Round((end - start).TotalSeconds);

//Console.WriteLine($"Msg from API:  EVAN - WE DID IT {call.ToString()} in {hours}:{minutes}:{seconds} AMAZING!");
//Console.ReadLine();

//###GET_DB_SCHEMEA_SYNC###
var getschema = await CallApi.GetDbSchemeAsync(client, url);
Console.WriteLine(getschema.ToString());
Console.ReadLine();

//###GET_TABLE_SCHEMA###
//var getTableSchema = await CallApi.GetTableSchema(client, url, "Deals");
//Console.WriteLine(getTableSchema.ToString());
//Console.ReadLine();

//###GET_VIEW-DATA
//var start = DateTime.Now;
//var getviewdata = await CallApi.GetViewdata(client, url, 101, 1);
//var end = DateTime.Now;
//var seconds = Math.Round((end - start).TotalSeconds);
//Console.WriteLine($"Total second: {seconds}");
//Console.WriteLine(getviewdata.ToString());
//Console.ReadLine();






