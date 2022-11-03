using CommonServiceLocator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SolrNet;
using System.Net;
using uploadTest.Shared;
using System.Text.Json;


namespace uploadTest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ISolrOperations<IndexFields> solr;
        public FileController(IWebHostEnvironment env)
        {
            _env = env;

            Startup.Container.Clear();
            Startup.InitContainer();
            Startup.Init<IndexFields>("http://localhost:8983/solr/NewCore");
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<IndexFields>>();
        }

        //init the SolrNet library
 
        [Test]
        [HttpGet]
        [Route("query")]
        public async Task<SolrQueryResults<IndexFields>> Query()
        {
            var results = solr.Query(new SolrQuery("region:asia"));

            var json = JsonSerializer.Serialize(results);

            return results;
        }

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
