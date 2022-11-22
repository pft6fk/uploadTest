using SolrNet;
using System.Net;
using uploadTest.Shared;
using Microsoft.AspNetCore.Mvc;

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
                
                fs.Dispose();

                IndexFile(path, trustedFileNameForFileStorage);

                uploadResult.StoredFileName = trustedFileNameForFileStorage;
                uploadResults.Add(uploadResult);

            }

            return Ok(uploadResults);
        }

        /// <summary>
        /// Retrieves all data from document using Apache Tika
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns></returns>
        public ExtractResponse? RetrieveDataFromFile(string path)
        {
            try
            {
                var f = new FileStream(path, FileMode.Open);

                var response = _solr.Extract(new ExtractParameters(f, Guid.NewGuid().ToString())
                {
                    ExtractOnly = true,
                    ExtractFormat = ExtractFormat.Text,
                });

                f.Dispose();
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Indexes provided file
        /// </summary>
        /// <param name="path">Path for file</param>
        /// <param name="fileName">Name of file</param>
        public void IndexFile(string path, string fileName)
        {
            var doc = RetrieveDataFromFile(path);

            var docContent = doc.Content;
            List<Dictionary<string,string>> metaData = new List<Dictionary<string,string>>();
            foreach (var item in doc.Metadata)
            {
                var tmp = new Dictionary<string, string>();
                tmp.Add(item.FieldName, item.Value);
                metaData.Add(tmp);
            }
            var indexFields = new IndexFields();
            indexFields.  Id = Guid.NewGuid().ToString();
            indexFields.DocName = new List<string> { fileName };
            indexFields.DocContent = new List<string> { docContent };
            indexFields.DocMetaData = metaData;
            indexFields.Path = path;
            _solr.Add(indexFields);
            _solr.Commit();
        }
    }
}
