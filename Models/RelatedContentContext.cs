using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hazza.RelatedContent.Models {
    public class RelatedContentContext {
        public string[] FieldNames { get; set; } = new string[] { "title", "body", "tags" };
        public string ContentType { get; set; }
        public int Count { get; set; } = 5;
        public string Index { get; set; } = "search";
    }
}