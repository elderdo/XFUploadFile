using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace XFUploadFile.Server.Controllers.v1
{
    /// <summary>
    /// The Controller used to manage the health of the Databases
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UploadFileController : ControllerBase
    {
        //CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("<Your Connection String Here>");

        private readonly ILogger<UploadFileController> _logger;
        private readonly IWebHostEnvironment _environment;

        public UploadFileController(ILogger<UploadFileController> logger,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }
        /// <summary>
        /// Defines the Web API for Post
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(IFormFileCollection files, CancellationToken cancellationtoken)
        {
            try
            {

                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var filePath = Path.Combine(_environment.ContentRootPath, "uploads");

                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream); System.IO.File.WriteAllBytes(Path.Combine(filePath, file.FileName), memoryStream.ToArray());

                            await UploadToAzureAsync(file);
                        }

                    }
                    return Ok();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                return new StatusCodeResult(500);
            }

            return BadRequest();
        }

        private async Task UploadToAzureAsync(IFormFile file)
        {
            //var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            //var cloudBlobContainer = cloudBlobClient.GetContainerReference("testcontainer");

            // if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                // await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Off });
            }

            //     var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
            //    cloudBlockBlob.Properties.ContentType = file.ContentType;

            //   await cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream());
        }
    }
}
