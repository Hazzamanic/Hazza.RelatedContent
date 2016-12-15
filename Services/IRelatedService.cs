using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Models;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Similar;
using Lucene.Net.Store;
using Orchard;
using Orchard.Environment.Configuration;
using Orchard.FileSystems.AppData;
using Orchard.Indexing;
using Directory = Lucene.Net.Store.Directory;
using Lucene.Net.Analysis;
using Lucene.Services;
using System.Globalization;
using Hazza.RelatedContent.Models;

namespace Hazza.RelatedContent.Services {
    public interface IRelatedService : IDependency {
        IEnumerable<ISearchHit> GetRelatedItems(int id);
        IEnumerable<ISearchHit> GetRelatedItems(int id, RelatedContentContext context);
    }

    public class RelatedService : IRelatedService {
        private readonly IAppDataFolder _appDataFolder;
        private readonly string _basePath;
        private readonly IIndexManager _indexManager;
        private readonly ILuceneAnalyzerProvider _analyzerProvider;

        public RelatedService(IAppDataFolder appDataFolder, ShellSettings shellSettings, IIndexManager indexManager, ILuceneAnalyzerProvider analyzerProvider) {
            _appDataFolder = appDataFolder;
            _indexManager = indexManager;
            _analyzerProvider = analyzerProvider;
            _basePath = _appDataFolder.Combine("Sites", shellSettings.Name, "Indexes");
        }

        private ISearchBuilder Search(string index) {
            return _indexManager.HasIndexProvider()
                ? _indexManager.GetSearchIndexProvider().CreateSearchBuilder(index)
                : new NullSearchBuilder();
        }

        public IEnumerable<ISearchHit> GetRelatedItems(int id, RelatedContentContext context) {
            var indexName = "search";

            IndexReader reader = IndexReader.Open(GetDirectory("search"), true);
            var indexSearcher = new IndexSearcher(reader);
            var analyzer = _analyzerProvider.GetAnalyzer(indexName);

            var mlt = new MoreLikeThis(reader) {Boost = true, MinTermFreq = 1, Analyzer = analyzer, MinDocFreq = 1};
            if (context.FieldNames.Length > 0) {
                mlt.SetFieldNames(context.FieldNames);
            }
            
            var sReader = new StringReader("dennis");

            mlt.SetFieldNames(new string[] { "title", "body", "tags" });
            var test = mlt.Like(sReader);
            var hitsss = indexSearcher.Search(test, 3);
            var c = hitsss.TotalHits;

            var docid = GetDocumentId(id, indexSearcher);
            Filter filter;
            var d = reader.Document(docid);
            System.String[] text = d.GetValues("title");
            if (text != null) {
                for (int j = 0; j < text.Length; j++) {
                    var x = text[j];
                }
            }

            //BooleanQuery query = (BooleanQuery) mlt.Like(docid);
            var query = mlt.Like(docid);

            if (!String.IsNullOrWhiteSpace(context.ContentType)) {
                var contentTypeQuery = new TermQuery(new Term("type", context.ContentType));
                //query.Add(new BooleanClause(contentTypeQuery, Occur.MUST));
            }

            TopDocs simDocs = indexSearcher.Search(query, context.Count);
            var results = simDocs.ScoreDocs
                .Select(scoreDoc => new LuceneSearchHit(indexSearcher.Doc(scoreDoc.Doc), scoreDoc.Score));

            return results;
        }

        protected virtual Directory GetDirectory(string indexName) {
            var directoryInfo = new DirectoryInfo(_appDataFolder.MapPath(_appDataFolder.Combine(_basePath, indexName)));
            return FSDirectory.Open(directoryInfo);
        }

        public int GetDocumentId(int contentItemId, IndexSearcher searcher) {
            var query = new TermQuery(new Term("id", contentItemId.ToString(CultureInfo.InvariantCulture)));
            var hits = searcher.Search(query, 1);
            return hits.ScoreDocs.Length > 0 ? hits.ScoreDocs[0].Doc : 0;
        }

        public IEnumerable<ISearchHit> GetRelatedItems(int id) {
            return GetRelatedItems(id, new RelatedContentContext());
        }
    }
}
