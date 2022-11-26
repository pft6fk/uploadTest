//to run solr
bin/solr start -p 8983

//to add core
bin/solr create -c CORE_NAME


//things to add to solrschemas:

//in solrconfig.conf :
//this is for apache TIKA 

  <lib dir="${solr.install.dir:../../../..}/modules/extraction/lib" regex=".*\.jar" />
  <lib dir="${solr.install.dir:../../../..}/modules/clustering/lib/" regex=".*\.jar" />
  <lib dir="${solr.install.dir:../../../..}/modules/langid/lib/" regex=".*\.jar" />
  <lib dir="${solr.install.dir:../../../..}/modules/ltr/lib/" regex=".*\.jar" />
  <lib dir="${solr.install.dir:../../../..}/modules/scripting/lib/" regex=".*\.jar" />
 

 //this is to use apacheTika
  <requestHandler name="/update/extract" 
                  startup="lazy"
                  class="solr.extraction.ExtractingRequestHandler" >
    <lst name="defaults">
      <str name="lowernames">true</str>
      <str name="uprefix">ignored_</str>
      <str name="captureAttr">true</str>
      <str name="fmap.a">links</str>
      <str name="fmap.div">ignored_</str>
    </lst>
  </requestHandler>

//in "/select" requestHandler 
//this is for global search

      <str name="df">_text_</str>


      
_________________________________________________________________________________________________________________________




things to add to managed-schema:

//add stemm filter for usign text field
//in our case it is for text_general field in index and query analyzer
      <filter name="snowballPorter"/>

//in the end add copyField for global search

  <copyField source="*" dest="_text_"/>  


//after all modification, remember to restart the solr


__________________________________________________________________________________________________________________________

//in dotnet project

//in Program.cs init the solr instance with proper solr core
builder.Services.AddSolrNet<IndexFields>("http://localhost:8983/solr/NewCore");

//in FileController.cs

//edit these variables:
//_solrInstanceDir is path for solr server
private readonly string _solrInstanceDir = "D:\\dev\\solr\\solr\\server\\solr\\";
//_solrCore is name of created core 
private readonly string _solrCore = "NewCore";