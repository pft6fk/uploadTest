using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uploadTest.Shared
{
    public class SolrResponse
    {
        public Responseheader header { get; set; }
        public Response Response { get; set; }
    }

        public class Responseheader
        {
            public int status { get; set; }
            public int QTime { get; set; }
        }

        public class Response
        {
            public int numFound { get; set; }
            public int start { get; set; }
            public bool numFoundExact { get; set; }
            public List<Doc> docs { get; set; }
        }

        public class Doc
        {
            public string[] name { get; set; }
            public string[] alpha2 { get; set; }
            public string[] alpha3 { get; set; }
            public int[] countrycode { get; set; }
            public string[] iso_31662 { get; set; }
            public string[] region { get; set; }
            public string[] subregion { get; set; }
            public int[] regioncode { get; set; }
            public int[] subregioncode { get; set; }
            public string id { get; set; }
            public long _version_ { get; set; }
        }

}
