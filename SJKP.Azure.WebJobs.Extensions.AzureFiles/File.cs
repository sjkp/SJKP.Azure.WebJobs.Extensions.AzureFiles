using Microsoft.Azure.WebJobs.Description;
using System;
using System.IO;

namespace SJKP.Azure.WebJobs.Extensions.AzureFiles
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public class FileAttribute : Attribute
    {
        private FileAccess? _access;
        public FileAttribute(string filePath)
        {
            FilePath = filePath;
        }
        public FileAttribute(string filePath, FileAccess fileAccess)
        {
            FilePath = filePath;
            this.Access = fileAccess;
        }



        [AutoResolve]
        public string FilePath { get; set; }

        public string Connection { get; set; }

        /// <summary>
        /// Gets the kind of operations that can be performed on the blob.
        /// </summary>
        public FileAccess? Access
        {
            get { return _access; }
            set { _access = value; }
        }
    }
}
