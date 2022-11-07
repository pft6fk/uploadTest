using Microsoft.AspNetCore.Mvc;
using SolrNet;
using System.Net;
using uploadTest.Shared;


namespace uploadTest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ISolrOperations<IndexFields> _solr;
        public FileController(IWebHostEnvironment env, ISolrOperations<IndexFields> solr)
        {
            _env = env;
            _solr = solr;
        }

       /// <summary>
       /// Sends Query to Solr Server
       /// </summary>
       /// <param name="q">Query</param>
       /// <returns>List of IndexFields objects</returns>
        [HttpGet]
        [Route("Query/{q}")]
        public async Task<SolrQueryResults<IndexFields>> Query(string q)
        {
            var results = _solr.Query(new SolrQuery(q));

            return results;
        }
        
        /// <summary>
        /// Uploads files to local "uploads" directory
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<List<UploadResult>>> UploadFile(List<IFormFile> files)
        {
            List<UploadResult> uploadResults = new();
            foreach (var file in files)
            {
                var uploadResult = new UploadResult();
                string trustedFileNameForFileStorage;
                var untrustedFileName = file.Name;
                //uploadResult.FileName = file.Name;
                uploadResult.FileName = untrustedFileName;
                //var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);
                var trustedFileNameForDisplay = WebUtility.HtmlEncode(file.Name);
                trustedFileNameForFileStorage = file.FileName;
                //trustedFileNameForFileStorage = Guid.NewGuid().ToString();
                //trustedFileNameForFileStorage = Path.GetRandomFileName();

                var path = Path.Combine(_env.ContentRootPath, "uploads", trustedFileNameForFileStorage);

                await using FileStream fs = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(fs);

                uploadResult.StoredFileName = trustedFileNameForFileStorage;
                uploadResults.Add(uploadResult);

            }

            return Ok(uploadResults);
        }
    }
}
