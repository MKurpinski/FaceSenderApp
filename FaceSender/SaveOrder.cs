
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace FaceSender
{
    public static class SaveOrder
    {
        [FunctionName("SaveOrder")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, [Table("orders", Connection = "StorageConnection")] ICollector<Order> orders, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string name = data?.name;

            if (name == null)
            {
                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

            var order = new Order
            {
                Name = name,
                FileName = $"{name}.jpg",
                PartitionKey = name,
                RowKey = $"{name}.jpg"
            };
            orders.Add(order);

            return new NoContentResult();
        }
    }

    public class Order : TableEntity
    {
        public string Name { get; set; }
        public string FileName { get; set; }
    }
}
