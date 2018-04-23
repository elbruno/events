using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Cognitive.LUIS;

namespace ConsoleApp5
{
    class Program
    {
        private const string SubscriptionKey = "< LUIS subscription key goes here >";
        private const string LuisAppId = "< LUIS Application Id goes here >";
        private const string QueryString = "";

        static void Main(string[] args)
        {
            GetLuisUsingLuis();
            Console.ReadLine();
        }

       private static async void GetLuisUsingLuis()
        {
            var luisClient = new LuisClient(LuisAppId, SubscriptionKey, true);
            var res = await luisClient.Predict(QueryString);
            var outputMsg = $@"Intent [{res.Intents.FirstOrDefault().Name}, {res.Intents.FirstOrDefault().Score}] {Environment.NewLine}";
            foreach (var resEntity in res.Entities)
            {
                outputMsg += $@"Entity: [{resEntity.Value.FirstOrDefault().Name}, {resEntity.Value.FirstOrDefault().Score} ] {Environment.NewLine}";
            }
            Console.Write(outputMsg);
        }
        private static async void  GetLuisIntents()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
            queryString["q"] = "myname is bruno capuano. I want to report an accident";
            queryString["timezoneOffset"] = "0";
            queryString["verbose"] = "false";
            queryString["spellCheck"] = "false";
            queryString["staging"] = "false";

            var uri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/" + LuisAppId + "?" + queryString;
            var response = await client.GetAsync(uri);

            var strResponseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine(strResponseContent);
        }
    }
}
