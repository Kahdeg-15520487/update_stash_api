using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text;
using update_stash_api.Services;
using update_stash_api.Services.DTOs;

namespace update_stash_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateStashController : ControllerBase
    {
        private readonly ILogger<UpdateStashController> _logger;
        private readonly IBackgroundTaskQueue taskQueue;

        public UpdateStashController(ILogger<UpdateStashController> logger, IBackgroundTaskQueue taskQueue)
        {
            _logger = logger;
            this.taskQueue = taskQueue;
        }

        [HttpGet]
        public async Task<IActionResult> ReceiveTorrentNotification([FromQuery] string name, [FromQuery] string path)
        {
            _logger.LogInformation("get called " + DateTime.Now.ToLongTimeString() + Environment.NewLine + "|\t\t" +
                                    name + Environment.NewLine + "|\t\t" +
                                    path);

            /*
             * torr complete
             * metadata scan path
             * wait for metadata scan complete
             * format jav id
             * scrape jav id
             * update jav id
             * 
             */

            await this.taskQueue.QueueBackgroundWorkItemAsync(async (stashService, cancellationToken) =>
            {
                var stashPath = path.Replace("/downloads", "/data/torr");
                var jobId = await stashService.QueueMetadataScanJob(stashPath);
                _logger.LogInformation($"Queued metadata scan job on path {stashPath} with id: {jobId}");

                var job = await stashService.FindJob(jobId);
                while (job.status != "FINISHED")
                {
                    await Task.Delay(500);
                    job = await stashService.FindJob(jobId);
                }
                _logger.LogInformation("Metadata scan job with id: " + jobId + " finished");

                var scene = (await stashService.GetAllScenes(stashPath)).scenes.FirstOrDefault();

                if (scene != null)
                {
                    scene = JavIdFormatter.FormatJavId(scene.files.FirstOrDefault()?.filename, scene.id);

                    var updated = await stashService.UpdateScene(scene);

                    _logger.LogInformation($"Updated {updated.code} {updated.title}");
                }
                else
                {
                    _logger.LogWarning($"No scenes found with path {stashPath}");
                }
            });

            return Ok("ok");
        }

        public static string DecompressBase64GzipString(string base64GzipString)
        {
            // Step 1: Decode the Base64 string to get the compressed binary data
            byte[] compressedData = Convert.FromBase64String(base64GzipString);

            // Step 2: Create a memory stream with the compressed data
            using (MemoryStream compressedStream = new MemoryStream(compressedData))
            {
                // Step 3: Create a GZipStream for decompression
                using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    // Step 4: Read the decompressed data into a new memory stream
                    using (MemoryStream decompressedStream = new MemoryStream())
                    {
                        gzipStream.CopyTo(decompressedStream);

                        // Step 5: Convert the decompressed binary data back to a string
                        byte[] decompressedData = decompressedStream.ToArray();
                        return Encoding.UTF8.GetString(decompressedData);
                    }
                }
            }
        }
    }
}
