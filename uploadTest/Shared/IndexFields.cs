using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uploadTest.Shared
{
    public class IndexFields
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("_nest_path_")]
        public string NestPath { get; set; }
        
        [SolrField("_root_")]
        public string Root { get; set; }
        
        //[SolrField("alpha-2")]
        //public List<string> Alpha_2 { get; set; }
        
        //[SolrField("alpha-3")]
        //public List<string> Alpha_3 { get; set; }
        
        //[SolrField("country-code")]
        //public List<int> CountryCode { get; set; }
        
        //[SolrField("intermediate-region")]
        //public List<string> IntermediateRegion { get; set; }
        
        //[SolrField("intermediate-region-code")]
        //public List<long> IntermediateRegionCode { get; set; }
        
        //[SolrField("iso_3166-2")]
        //public List<string> Iso3116_2 { get; set; }
        
        //[SolrField("name")]
        //public List<string> Name { get; set; }
        
        //[SolrField("region")]
        //public List<string> Region { get; set; }
        
        //[SolrField("region-code")]
        //public List<int> RegionCode { get; set; }
        
        //[SolrField("sub-region")]
        //public List<string> SubRegion { get; set; }
        
        //[SolrField("sub-region-code")]
        //public List<int> SubRegionCode { get; set; }

        [SolrField("docName")]
        public List<string> DocName { get; set; }

        [SolrField("docContent")]
        public List<string> DocContent { get; set; }
        
        [SolrField("docMetaDataCreation-Date")]
        public List<DateTime> DocCreationDate { get; set; }
        [SolrField("docMetaDataLast-Modified")]
        public List<DateTime> DocModifiedDate { get; set; }

        [SolrField("docMetaDataAuthor")]
        public List<string> DocAuthor { get; set; }

        [SolrField("docMetaDatalanguage")]
        public List<string> DocLanguage { get; set; }

        [SolrField("docMetaDataContent-Type")]
        public List<string> DocDataType { get; set; }

        [SolrField("docMetaDatastream_size")]
        public List<long> DocSize { get; set; }
        
        [SolrField("docMetaDataxmpTPg_NPages")]
        public List<long> DocPages { get; set; }

        [SolrField("path")]
        public string Path { get; set; }
        
        [SolrField("docMetaData")]
        public List<Dictionary<string, string>> DocMetaData { get; set; }
    }
//created date
//author
//modified
//language
//data type = content type
//size
//pages


    public class Search1
    {
        public string Text { get; set; } = "";
        public string MyProperty { get; set; }
    }
}
