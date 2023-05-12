using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DotLiquid;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Xml;
using ParseXMLtoJSON.Helpers;
using System.Reflection;

namespace ParseXMLtoJSON
{
    public static class XMLtoJSON
    {
        [FunctionName("xml_to_json")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "ParseXMLtoJSON.template.liquid";
                string result;

                log.LogInformation("C# HTTP trigger function processed a request.");
                string name = req.Query["name"];
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                HelperMethods helperMethods = new();

                XElement xmlDocumentWithoutNs = helperMethods.RemoveAllNamespaces(XElement.Parse(requestBody));
                string newxml = xmlDocumentWithoutNs.ToString();
                XDocument doc = XDocument.Parse(newxml);
                string json = JsonConvert.SerializeXNode(doc);

                log.LogInformation("Initial json serialze completed.");

                var modelAsMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(json, new DictionaryConverter());


                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }


                var template = Template.Parse(result);

                var rendered = template.Render(Hash.FromDictionary(modelAsMap));

                log.LogInformation("Data remapped to JSON successfully");

                return new OkObjectResult(rendered);
            }
            catch (Exception ex) 
            {
                log.LogError("Failed to parse JSON from XML [{0}]", ex);
            }
        }
    }
}
