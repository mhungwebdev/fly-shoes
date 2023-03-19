using FlyShoes.Common.Enums;
using FlyShoes.Common.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Core.Interfaces
{
    public interface IStorageService
    {
        Task<string?> UploadFile(FSFile fsFile, BucketEnum bucketEnum, StorageType storageType = StorageType.Product);
        
        Task<List<string>> UploadFileMulti(List<FSFile> fsFiles, BucketEnum bucketEnum, StorageType storageType = StorageType.Product);

        Task DeleteFile(Guid fileID, BucketEnum bucketEnum = BucketEnum.MainBucket, StorageType storageType = StorageType.Product );
     
        Task DeleteFileMulti(List<Guid> fileIDs, BucketEnum bucketEnum = BucketEnum.MainBucket, StorageType storageType = StorageType.Product);

        Task<string?> MoveFileFromTemToMain(Guid fileID, StorageType storageType = StorageType.Product );

        Task<List<string>> MoveFileFromTemToMainMulti(List<Guid> fileIDs, StorageType storageType = StorageType.Product );
    }
}
