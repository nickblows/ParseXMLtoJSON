using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ParseXMLtoJSON
{
    public static class XMLtoJSON
    {
        [FunctionName("xml_to_json")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string name = req.Query["name"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            Root root = new();
            string jsonResponse = JsonConvert.SerializeObject(root);
            return new OkObjectResult(jsonResponse);
        }
    }

    public class Document
    {
        public Properties properties { get; set; }
    }

    public class Properties
    {
        public Properties()
        {
            UDIFromDG = new UDIFromDG();
            Document = new Document();
            DocumentType = "BP5PP";
        }

        public UDIFromDG UDIFromDG { get; set; }
        public Document Document { get; set; }
        public string DocumentType { get; set; }
    }

    public class Root
    {
        public Root()
        {
            this.properties = new Properties();
        }

        public Properties properties { get; set; }
    }

    public class UDIFromDG
    {
        public UDIFromDG()
        {
            this.properties = null;
        }

        public Properties properties { get; set; }
    }


}
