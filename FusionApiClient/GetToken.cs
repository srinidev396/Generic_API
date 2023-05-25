using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Security.Policy;

namespace FusionApiClient
{
    internal class GetToken
    {
        public static async Task<HttpClient> GetClient(string userName, string password, string databaseName, string url)
        {
            string token = String.Empty;
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(100);
            var response = await client.GetAsync($"{url}/GenerateToken?userName={userName}&passWord={password}&database={databaseName}");
            if (response.IsSuccessStatusCode)
            {
                token = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Console.WriteLine("Not Authorize!");
            }

            return client;
        }
    }
}
