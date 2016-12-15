using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hazza.RelatedContent.Models {
    public class RelatedContentContext {
        public string[] FieldNames { get; set; } = new string[0];
        public string ContentType { get; set; }
        public int Count { get; set; } = 4;
    }
}