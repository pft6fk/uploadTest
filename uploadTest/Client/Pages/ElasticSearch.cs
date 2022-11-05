using CommonServiceLocator;
using SolrNet;
using uploadTest.Shared;

namespace uploadTest.Client.Pages
{
    public partial class ElasticSearch
    {

        //protected override void OnInitialized()
        //{
        //    Startup.Container.Clear();
        //    Startup.InitContainer();
        //    Startup.Init<IndexFields>("http://localhost:8983/solr/NewCore");
        //    solr = ServiceLocator.Current.GetInstance<ISolrOperations<IndexFields>>();
        //}
        //public async void Query1()
        //{
        ////this part Querying in WASM is impossible because connection is established through the HttpWebRequestFactory.
        ////WASM supports HttpClient instead of HttpWebRequestFactory 
        //    var results = solr.Query(new SolrQueryByField("region", "asia"));
        //    indexFields = results;
        //}
    }
}
