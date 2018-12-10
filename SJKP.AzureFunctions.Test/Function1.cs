
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;

namespace SJKP.AzureFunctions.Test
{
    public static class Function1
    {
        [FunctionName("Function1")]        
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, [SJKP.Azure.WebJobs.Extensions.AzureFiles.File("acishare/browsertime-results/github.com/2018-11-14T205432+0000/browsertime.json", FileAccess.Read, Connection = "AzureWebJobsStorage")] Stream file, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var ms = new MemoryStream();
            file.CopyTo(ms);
            ms.Position = 0;
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentLength = file.Length;
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            //response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            //{
            //    FileName = "dataFile.zip",
            //    Size = file.Length
            //};


            return response;
        }
    }
}
