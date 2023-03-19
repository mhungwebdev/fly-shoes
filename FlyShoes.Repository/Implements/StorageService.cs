using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using FlyShoes.Common.Constants;
using FlyShoes.Common.Enums;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Core.Implements
{
    public class StorageService : IStorageService
    {
        private AmazonS3Client _amazonS3Client;
        private IConfigurationSection _amazonConfig;

        public StorageService(IConfiguration configuration)
        {
            _amazonConfig = configuration.GetSection("AmazonStorage");
            var accessKey = _amazonConfig["AccessKey"];
            var secretKey = _amazonConfig["SecretKey"];

            AmazonS3Config config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.APSoutheast1,
                ForcePathStyle = true
            };

            _amazonS3Client = new AmazonS3Client(accessKey, secretKey, config);
        }

        public async Task DeleteFile(Guid fileID, BucketEnum bucketEnum = BucketEnum.MainBucket, StorageType storageType = StorageType.Product)
        {
            var filePath = GetFilePath(bucketEnum,storageType);
            var response = await _amazonS3Client.DeleteObjectAsync(filePath, fileID.ToString());
        }

        public async Task<string?> UploadFile(FSFile fsFile, BucketEnum bucketEnum, StorageType storageType = StorageType.Product)
        {
            var bucketName = GetBucketName(bucketEnum);
            var bucketDomain = GetBucketDomain(bucketEnum);


            await using var memory = new MemoryStream();
            await fsFile.File.CopyToAsync(memory);

            var extension = Path.GetExtension(fsFile.File.Name);
            var fileName = $"{fsFile.FileID}";
            var filePath = GetFilePath(bucketEnum, storageType);

            PutObjectRequest po = new PutObjectRequest()
            {
                BucketName = filePath,
                InputStream = memory,
                Key = fsFile.FileID.ToString(),
            };

            var response = await _amazonS3Client.PutObjectAsync(po);

            if (response.HttpStatusCode == HttpStatusCode.OK)
                return $"{bucketDomain}{filePath.Replace($"{bucketName}","")}/{fileName}";

            return null;
        }

        private string GetBucketName(BucketEnum bucketEnum)
        {
            switch (bucketEnum)
            {
                case BucketEnum.TempBucket:
                    return BucketNameConstant.TEMP;
                    break;
                case BucketEnum.MainBucket:
                    return BucketNameConstant.MAIN;
                    break;
                default:
                    return BucketNameConstant.TEMP;
            }
        }

        private string GetBucketDomain(BucketEnum bucketEnum)
        {
            switch (bucketEnum)
            {
                case BucketEnum.TempBucket:
                    return _amazonConfig["TempBucket"];
                    break;
                case BucketEnum.MainBucket:
                    return _amazonConfig["MainBucket"];
                    break;
                default:
                    return _amazonConfig["TempBucket"];
            }
        }

        public async Task<string?> MoveFileFromTemToMain(Guid fileID, StorageType storageType = StorageType.Product)
        {
            var filePathTemp = GetFilePath(BucketEnum.TempBucket, storageType);
            var filePathMain = GetFilePath(BucketEnum.MainBucket, storageType);
            var bucketName = GetBucketName(BucketEnum.MainBucket);
            var res = await _amazonS3Client.CopyObjectAsync(filePathTemp, fileID.ToString(), filePathMain, fileID.ToString());

            if (res.HttpStatusCode == HttpStatusCode.OK)
                return $"{_amazonConfig["MainBucket"]}{filePathMain.Replace(bucketName,"")}/{fileID}";
            else return null;
        }

        public string GetFilePath(BucketEnum bucketEnum,StorageType storageType)
        {
            var folder = _amazonConfig[((int)storageType).ToString()];
            var bucketName = BucketNameConstant.TEMP;
            switch (bucketEnum)
            {
                case BucketEnum.TempBucket:
                    bucketName = BucketNameConstant.TEMP;
                    break;
                case BucketEnum.MainBucket:
                    bucketName = BucketNameConstant.MAIN;
                    break;
            }

            return $"{bucketName}/{folder}";
        }

        public async Task<List<string>> UploadFileMulti(List<FSFile> fsFiles, BucketEnum bucketEnum, StorageType storageType = StorageType.Product)
        {
            var result = new List<string>();

            foreach(var fsFile in fsFiles)
            {
                var fileUrl = await UploadFile(fsFile, bucketEnum, storageType);
                result.Add(fileUrl);
            }

            return result;
        }

        public async Task DeleteFileMulti(List<Guid> fileIDs, BucketEnum bucketEnum = BucketEnum.MainBucket, StorageType storageType = StorageType.Product)
        {
            foreach(var fileID in fileIDs)
            {
                await DeleteFile(fileID);
            }
        }

        public async Task<List<string>> MoveFileFromTemToMainMulti(List<Guid> fileIDs, StorageType storageType = StorageType.Product)
        {
            var result = new List<string>();

            foreach(var fileID in fileIDs)
            {
                var fileUrl = await MoveFileFromTemToMain(fileID, storageType);
                result.Add(fileUrl);
            }

            return result;
        }
    }
}
