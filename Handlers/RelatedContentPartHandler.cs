﻿using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.ContentManagement;
using Orchard.Localization;
using Hazza.RelatedContent.Models;

// This code was generated by Orchardizer

namespace Hazza.RelatedContent.Handlers {
    public class RelatedContentPartHandler : ContentHandler {
        public RelatedContentPartHandler() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
    }
}