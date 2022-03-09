using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OngProject.Core.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OngProject.Core.Helper
{
    public class S3AwsHelper : IS3AwsHelper
    {
        private readonly IAmazonS3 _amazonS3;
        public S3AwsHelper()
        {
            var chain = new CredentialProfileStoreChain("C:\\Users\\matil\\source\\repos\\OT134-SERVER\\OngProject\\App_data\\credentials.ini");
            AWSCredentials awsCredentials;
            RegionEndpoint uSEast1 = RegionEndpoint.USEast1;
            if (chain.TryGetAWSCredentials("default", out awsCredentials))
            {
                _amazonS3 = new AmazonS3Client(awsCredentials.GetCredentials().AccessKey, awsCredentials.GetCredentials().SecretKey, uSEast1);
            }
        }

        public async Task<AwsManagerResponse> AwsUploadFile(string key, IFormFile file)
        {
            try
            {
                var putRequest = new PutObjectRequest()
                {
                    BucketName = "cohorte-enero-835eb560",
                    Key = key,
                    InputStream = file.OpenReadStream(),
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicReadWrite
                };
                var result = await this._amazonS3.PutObjectAsync(putRequest);
                var response = new AwsManagerResponse
                {
                    Message = "File upload successfully",
                    Code = (int)result.HttpStatusCode,
                    NameImage = key,
                    Url = $"https://cohorte-enero-835eb560.s3.amazonaws.com/{key}",
                };
                return response;
            }
            catch (AmazonS3Exception e)
            {
                return new AwsManagerResponse
                {
                    Message="Error encountered when writting an object",
                    Code=(int)e.StatusCode,
                    Errors=e.Message
                };
            }
            catch(Exception e)
            {
                return new AwsManagerResponse
                {
                    Message = "Unknown server error when writting an object",
                    Code = 500,
                    Errors = e.Message
                };
            }
        }
        public async Task<FileStreamResult> AwsGetFile([FromQuery] string imageName)
        {
            try
            {
                var request = new GetObjectRequest()
                {
                    BucketName = "cohorte-enero-835eb560",
                    Key = imageName,
                };
                using GetObjectResponse response = await this._amazonS3.GetObjectAsync(request);
                using Stream responseStream = response.ResponseStream;
                var stream = new MemoryStream();
                await responseStream.CopyToAsync(stream);
                stream.Position = 0;

                return new FileStreamResult(stream, response.Headers["Content-Type"])
                {
                    FileDownloadName = imageName
                };
            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<AwsManagerResponse> AwsDeleteFile([FromQuery] string imageName)
        {
            try
            {
                var request = new DeleteObjectRequest()
                {
                    BucketName = "cohorte-enero-835eb560",
                    Key = imageName,
                };
                var result = await this._amazonS3.DeleteObjectAsync(request);
                var response = new AwsManagerResponse
                {
                    Message = "File deleted successfully",
                    Code = (int)result.HttpStatusCode,
                    NameImage = imageName,
                    Url = "",
                };
                return response;
            }
            catch (AmazonS3Exception e)
            {
                return new AwsManagerResponse
                {
                    Message = "Error encountered when deleting an object",
                    Code = (int)e.StatusCode,
                    Errors = e.Message
                };
            }
            catch (Exception e)
            {
                return new AwsManagerResponse
                {
                    Message = "Unknown server error when deleting an object",
                    Code = 500,
                    Errors = e.Message
                };
            }
        }
    }
}