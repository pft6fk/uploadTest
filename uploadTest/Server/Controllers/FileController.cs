﻿using SolrNet;
using System.Net;
using uploadTest.Shared;
using Microsoft.AspNetCore.Mvc;
using SolrNet.Commands.Parameters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace uploadTest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly string _solrInstanceDir = "D:\\dev\\solr\\solr\\server\\solr\\";
        private readonly string _solrCore = "NewCore";
        private readonly string _solrUri = "http://localhost:8983/solr";
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
            Suggest(q);
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
            indexFields.Id = Guid.NewGuid().ToString();
            indexFields.DocName = new List<string> { fileName };
            indexFields.DocContent = new List<string> { docContent };
            indexFields.DocMetaData = metaData;
            indexFields.Path = path;
            _solr.Add(indexFields);
            _solr.Commit();
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public void Delete(string id)
        {
            var doc = _solr.Query(new SolrQuery(id));
            var path = doc.Single().Path;
            try
            {
                System.IO.File.Delete(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            _solr.Delete(id);
            _solr.Commit();
        }

        [HttpGet]
        [Route("AddSynonynms/{synonyms}")]
        public async Task AddSynonynms(string synonyms)
        {
            try
            {
                System.IO.File.AppendAllText( _solrInstanceDir + _solrCore + "\\conf\\synonyms.txt", synonyms + Environment.NewLine);
                //after adding updating synonyms, it is required to reload core
                using (var client = new HttpClient())
                {
                    client.GetAsync($"{_solrUri}/admin/cores?action=RELOAD&core={_solrCore}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        [HttpGet]
        [Route("AddProtwords/{protwords}")]
        public async Task AddProtwords(string protwords)
        {
            try
            {
                System.IO.File.AppendAllText( _solrInstanceDir + _solrCore + "\\conf\\protwords.txt", protwords + Environment.NewLine);
                //after adding updating synonyms, it is required to reload core
                using (var client = new HttpClient())
                {
                    client.GetAsync($"{_solrUri}/admin/cores?action=RELOAD&core={_solrCore}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        [HttpGet]
        [Route("AddStopwords/{stopwords}")]
        public async Task AddStopwords(string stopwords)
        {
            try
            {
                System.IO.File.AppendAllText( _solrInstanceDir + _solrCore + "\\conf\\stopwords.txt", stopwords + Environment.NewLine);
                //after adding updating synonyms, it is required to reload core
                using (var client = new HttpClient())
                {
                    client.GetAsync($"{_solrUri}/admin/cores?action=RELOAD&core={_solrCore}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [HttpGet]
        [Route("Download/{id}")]
        public async Task<IActionResult> DownloadFile(string id)
        {
            var solrDoc = _solr.Query(new SolrQuery(id));
            var path = solrDoc.Single().Path;
            var docName = solrDoc.Single().DocName.Single();
            var docType = solrDoc.Single().DocDataType.Single();
            var memory = new MemoryStream();

            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, docType, path);
        }

        [HttpGet]
        [Route("Suggest/{input}")]
        public async Task<List<string>> Suggest(string term)
        {
            Suggest terms;

            var termsList = new List<string>();

            using (var client = new HttpClient())
            {
                 
                var response = JObject.Parse(
                    await client.GetStringAsync(
                        _solrUri + $"/NewCore/suggest?suggest=true&suggest.dictionary=mySuggester&suggest.q={term}"
                        ));

                var suggestions = response["suggest"];
                var AnalyzingInfixLookupFactory = suggestions["mySuggester"];
                var AnalyzingInfixLookupFactoryTerm = AnalyzingInfixLookupFactory[term];
                terms = JsonConvert.DeserializeObject<Suggest>(AnalyzingInfixLookupFactoryTerm.ToString());

                Console.WriteLine(terms);
                foreach (var item in terms.suggestions)
                {
                    termsList.Add(item.term);
                }
            }
            return termsList;
        }
    }
}
