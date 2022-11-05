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


            //init the SolrNet library
 
            Startup.Container.Clear();
            Startup.InitContainer();
            Startup.Init<IndexFields>("http://localhost:8983/solr/NewCore");
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<IndexFields>>();
        }

       /// <summary>
       /// Sends Query to Solr Server
       /// </summary>
       /// <param name="q">Query</param>
       /// <returns>List of IndexFields objects</returns>
        [Test]
        [HttpGet]
        [Route("Query/{q}")]
        public async Task<SolrQueryResults<IndexFields>> Query(string q)
        {
            var results = solr.Query(new SolrQuery(q));

            return results;
        }
        
        [HttpGet]
        [Route("Query1/{q}")]
        //public async Task<List<IndexFields>> Query1(string q)
        public async Task<string> Query1(string q)
        {
            var client = new HttpClient();
            //var response = await client.GetFromJsonAsync<SolrResponse>($"http://localhost:8983/solr/NewCore/select?q={q}");
            //var lst = ResponseToIndexFields(response.Response.docs);
            var response = await client.GetStringAsync($"http://localhost:8983/solr/NewCore/select?q={q}");
            client.Dispose();
            return response;

        }

        private List<IndexFields> ResponseToIndexFields(List<Doc> res)
        {
            var lst =  new List<IndexFields>();
            foreach (Doc doc in res)
            {
                var tmp = new IndexFields();
                tmp.Id = doc.id;
                tmp.Name.Add(doc.name.First());
                tmp.Iso3116_2.Add(doc.name.First());
                tmp.Name.Add(doc.iso_31662.First());
                tmp.CountryCode.Add(doc.countrycode.First());
                tmp.Region.Add(doc.region.First());
                tmp.RegionCode.Add(doc.regioncode.First());
                tmp.Alpha_2.Add(doc.alpha2.First());
                tmp.Alpha_3.Add(doc.alpha3.First());
                tmp.SubRegionCode.Add(doc.subregioncode.First());
                lst.Add(tmp);
            }
            return lst;
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
