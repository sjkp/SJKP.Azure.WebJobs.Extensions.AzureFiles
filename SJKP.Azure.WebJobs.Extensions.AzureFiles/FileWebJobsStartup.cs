using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Hosting;
using SJKP.Azure.WebJobs.Extensions.AzureFiles;

[assembly: WebJobsStartup(typeof(FileWebJobsStartup))]

namespace SJKP.Azure.WebJobs.Extensions.AzureFiles
{
    public class FileWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<FilesExtensionConfigProvider>();
        }
    }
}