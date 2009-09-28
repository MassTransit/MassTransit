namespace MassTransit.Infrastructure.Metadata
{
    using System;
    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Lucene.Net.QueryParsers;
    using Lucene.Net.Search;
    using Lucene.Net.Store;

    public class LuceneMetadataRepository :
        IDisposable
    {
        private IndexWriter _indexWriter;
        private IndexSearcher _indexSearcher;
        private Directory _directory;
        private Analyzer _analyzer;

        public LuceneMetadataRepository()
        {
            _analyzer = new StandardAnalyzer();
            _directory = FSDirectory.GetDirectory("./index", true);
            _indexWriter = new IndexWriter(_directory, _analyzer, true);
            _indexWriter.SetMaxFieldLength(25000);
            _indexSearcher = new IndexSearcher(_directory);
        }

        public void Register(object data)
        {
            var messageName = "";
            var description = "";

            var doc = new Document();
            doc.Add(new Field("messageName", messageName, Field.Store.YES, Field.Index.TOKENIZED));
            doc.Add(new Field("description", description, Field.Store.YES, Field.Index.TOKENIZED));
            _indexWriter.AddDocument(doc);
            _indexWriter.Close();
        }

        public void Find(string query)
        {
            var qp = new QueryParser("description", _analyzer);
            Query q = qp.Parse(query);

            Hits hits = _indexSearcher.Search(q);

            
            // Iterate through the results:
            for (int i = 0; i < hits.Length(); i++)
            {
                Document hitDoc = hits.Doc(i);

                //build up result
                //Console.WriteLine("Message: {0} - Description: {1}", hitDoc.Get("messageName"), hitDoc.Get("description"));
            }
        }

        public void Dispose()
        {
            _indexWriter.Close();
            _indexSearcher.Close();
            _directory.Close();
        }
    }
}