using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Services;
using Orchard;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.AppData;
using Orchard.Indexing;
using Orchard.Logging;

namespace Hazza.RelatedContent.Services {
    [OrchardSuppressDependency("Lucene.Services.LuceneIndexProvider")]
    public class LuceneIndexProvider : Lucene.Services.LuceneIndexProvider, IIndexProvider {
        private readonly ILuceneAnalyzerProvider _analyzerProvider;
        public LuceneIndexProvider(IAppDataFolder appDataFolder, ShellSettings shellSettings, ILuceneAnalyzerProvider analyzerProvider) 
            : base(appDataFolder, shellSettings, analyzerProvider) {
            _analyzerProvider = analyzerProvider;
        }

        public new void Store(string indexName, IDocumentIndex indexDocument) {
            Store(indexName, new[] { (LuceneDocumentIndexTermVector)indexDocument });
        }

        public new void Store(string indexName, IEnumerable<IDocumentIndex> indexDocuments) {
            Store(indexName, indexDocuments.Cast<LuceneDocumentIndexTermVector>());
        }

        public void Store(string indexName, IEnumerable<LuceneDocumentIndexTermVector> indexDocuments) {
            indexDocuments = indexDocuments.ToArray();

            if (!indexDocuments.Any()) {
                return;
            }

            // Remove any previous document for these content items
            Delete(indexName, indexDocuments.Select(i => i.ContentItemId));

            using (var writer = new IndexWriter(GetDirectory(indexName), _analyzerProvider.GetAnalyzer(indexName), false, IndexWriter.MaxFieldLength.UNLIMITED)) {
                foreach (var indexDocument in indexDocuments) {
                    var doc = CreateDocument(indexDocument);

                    writer.AddDocument(doc);
                    Logger.Debug("Document [{0}] indexed", indexDocument.ContentItemId);
                }
            }
        }

        public new IDocumentIndex New(int documentId) {
            return new LuceneDocumentIndexTermVector(documentId, T);
        }

        private static Document CreateDocument(LuceneDocumentIndexTermVector indexDocument) {
            var doc = new Document();

            indexDocument.PrepareForIndexing();
            foreach (var field in indexDocument.Fields) {
                doc.Add(field);
            }
            return doc;
        }
    }
}