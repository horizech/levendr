using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using Levendr.Services;
using Levendr.Models;
using Levendr.Enums;
using Levendr.Constants;
using Levendr.Helpers;
using Levendr.Filters;
using Amazon.S3.Model;
using System.IO;
using System.Net;
using Amazon.S3;
using Amazon;
using Microsoft.AspNetCore.Http;
using Amazon.S3.Transfer;

namespace Levendr.Plugins
{
    public class S3Plugin : BasePlugin
    {

        public S3Plugin()
        {
            
        }

        public async static Task<List<S3Bucket>> ListBuckets(string region)
        {
            return await ListBuckets(RegionEndpoint.GetBySystemName(region));
        }

        public async static Task<List<S3Bucket>> ListBuckets(RegionEndpoint region)
        {
            string accessKeyId = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3AccessKeyId, null);
            string secretAccessKey = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3SecretAccessKey, null);
            
            try
            {
                AmazonS3Client client = new AmazonS3Client(accessKeyId, secretAccessKey, region);
                ListBucketsResponse buckets = await client.ListBucketsAsync();
                return buckets.Buckets;
            }
            catch( Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
            }
            return null;
        }

        public async static Task<byte[]> DownloadFileAsync(string region, string bucket, string key)
        {
            return await DownloadFileAsync(RegionEndpoint.GetBySystemName(region), bucket, key);
        }

        public async static Task<byte[]> DownloadFileAsync(RegionEndpoint region, string bucket, string key)
        {
            string accessKeyId = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3AccessKeyId, null);
            string secretAccessKey = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3SecretAccessKey, null);
            
            try
            {
                AmazonS3Client client = new AmazonS3Client(accessKeyId, secretAccessKey, region);
                MemoryStream ms = null;
                try
                {
                    GetObjectRequest getObjectRequest = new GetObjectRequest
                    {
                        BucketName = bucket,
                        Key = key
                    };

                    using (var response = await client.GetObjectAsync(getObjectRequest))
                    {
                        if (response.HttpStatusCode == HttpStatusCode.OK)
                        {
                            using (ms = new MemoryStream())
                            {
                                await response.ResponseStream.CopyToAsync(ms);
                            }
                        }
                    }

                    if (ms is null || ms.ToArray().Length < 1)
                        throw new FileNotFoundException(string.Format("The document '{0}' is not found", key));

                    return ms.ToArray();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch( Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
            }
            
            return null;            
        }

        public async Task<bool> UploadFileAsync(string region, string bucket, string key, IFormFile file)
        {
            return await UploadFileAsync(RegionEndpoint.GetBySystemName(region), bucket, key, file);
        }

        public async Task<bool> UploadFileAsync(RegionEndpoint region, string bucket, string key, IFormFile file)
        {
            string accessKeyId = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3AccessKeyId, null);
            string secretAccessKey = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3SecretAccessKey, null);
            
            try
            {
                AmazonS3Client client = new AmazonS3Client(accessKeyId, secretAccessKey, region);
                
                using (var newMemoryStream = new MemoryStream())
                {
                    file.CopyTo(newMemoryStream);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = key,
                        BucketName = bucket,
                        ContentType = file.ContentType
                    };

                    var fileTransferUtility = new TransferUtility(client);

                    await fileTransferUtility.UploadAsync(uploadRequest);

                    return true;
                }
            }
            catch (Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
            }

            return false;
        }

        public async Task<bool> DeleteFileAsync(string region, string bucket, string key)
        {
            return await DeleteFileAsync(RegionEndpoint.GetBySystemName(region), bucket, key);
        }

        public async Task<bool> DeleteFileAsync(RegionEndpoint region, string bucket, string key)
        {
            string accessKeyId = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3AccessKeyId, null);
            string secretAccessKey = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.AWSSESS3SecretAccessKey, null);
            
            try
            {
                AmazonS3Client client = new AmazonS3Client(accessKeyId, secretAccessKey, region);
                await client.DeleteObjectAsync(bucket, key);
                return true;
            }
            catch (Exception e)
            {
                ServiceManager.Instance.GetService<LogService>().Print(e.Message, LoggingLevel.Errors);
            }

            return false;
        }
    }
}
