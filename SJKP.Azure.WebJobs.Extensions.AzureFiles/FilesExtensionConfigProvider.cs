using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SJKP.Azure.WebJobs.Extensions.AzureFiles
{
    public class FilesExtensionConfigProvider : IExtensionConfigProvider
    {
        private StorageAccountProvider _accountProvider;        
        private readonly INameResolver _nameResolver;
        public FilesExtensionConfigProvider(StorageAccountProvider accountProvider, INameResolver nameResolver)
        {
            _nameResolver = nameResolver;
            _accountProvider = accountProvider;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            var rule = context.AddBindingRule<FileAttribute>();
            var v = System.Environment.GetEnvironmentVariables();
            rule.BindToStream(CreateStreamAsync, System.IO.FileAccess.Read);
            
        }

        private async Task<Stream> CreateStreamAsync(
            FileAttribute fileAttribute,
            ValueBindingContext context)
        {
            var cancellationToken = context.CancellationToken;
            var blob = await GetFileAsync(fileAttribute, cancellationToken);

            return await blob.OpenReadAsync();
        }

        private async Task<CloudFile> GetFileAsync(
               FileAttribute fileAttribute,
               CancellationToken cancellationToken,
               Type requestedType = null)
        {
            var client = await GetClientAsync(fileAttribute, cancellationToken);
            //BlobPath boundPath = BlobPath.ParseAndValidate(fileAttribute.BlobPath);
            var filePath = fileAttribute.FilePath.Split(new[] { '/' });
            var share = client.GetShareReference(filePath.First());

            var exists = await share.ExistsAsync();

            // Call ExistsAsync before attempting to create. This reduces the number of 
            // 40x calls that App Insights may be tracking automatically
            //if (fileAttribute.Access != FileAccess.Read && !await container.ExistsAsync())
            //{
            //    await container.CreateIfNotExistsAsync(cancellationToken);
            //}



            var dir = share.GetRootDirectoryReference();
             exists = await dir.ExistsAsync();
            if (filePath.Length - 2 > 0)
            {
                dir = dir.GetDirectoryReference(string.Join("/", filePath.Skip(1).Take(filePath.Length - 2)));
                exists = await dir.ExistsAsync();
            }

            var blob = dir.GetFileReference(filePath.Last());
            exists = await blob.ExistsAsync();

            return blob;
        }

        private async Task<CloudFileShare> GetContainerAsync(
           FileAttribute fileAttribute,
           CancellationToken cancellationToken)
        {
            var client = await GetClientAsync(fileAttribute, cancellationToken);
            
            //BlobPath boundPath = BlobPath.ParseAndValidate(fileAttribute.BlobPath, isContainerBinding: true);
            var share = client.GetShareReference(fileAttribute.FilePath);
                        
            return share;
        }

        private Task<CloudFileClient> GetClientAsync(
         FileAttribute blobAttribute,
         CancellationToken cancellationToken)
        {
            var account = _accountProvider.Get(blobAttribute.Connection, _nameResolver);

            var client = account.SdkObject.CreateCloudFileClient();
            return Task.FromResult(client);
        }
    }
}
