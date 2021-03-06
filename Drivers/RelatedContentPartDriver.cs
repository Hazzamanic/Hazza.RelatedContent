﻿using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Hazza.RelatedContent.Models;
using Orchard.ContentManagement.Handlers;
using Hazza.RelatedContent.Services;
using System.Linq;
using Hazza.RelatedContent.Settings;

// This code was generated by Orchardizer

namespace Hazza.RelatedContent.Drivers {
    public class RelatedContentPartDriver : ContentPartDriver<RelatedContentPart> {

        private readonly IRelatedService _relatedSerive;
        private readonly IContentManager _contentManger;

        public RelatedContentPartDriver(IRelatedService relatedService, IContentManager contentManger) {
            _relatedSerive = relatedService;
            _contentManger = contentManger;
        }

        protected override string Prefix {
            get { return "RelatedContentPart"; }
        }

        protected override DriverResult Display(RelatedContentPart part, string displayType, dynamic shapeHelper) {
            // Put all your driver logic inside this method so that if your part is not being displayed no logic is run. Yay efficiency
            return ContentShape("Parts_RelatedContentPart",
                () => {
                    var settings = part.TypePartDefinition.Settings.GetModel<RelatedContentPartSettings>();

                    var context = new RelatedContentContext { ContentType = settings.ContentType };
                    if (!String.IsNullOrWhiteSpace(settings.Fields))
                        context.FieldNames = settings.Fields.Split(',');

                    if (!String.IsNullOrWhiteSpace(settings.Index))
                        context.Index = settings.Index;

                    if (settings.NoOfItems != 0)
                        context.Count = settings.NoOfItems;

                        var related = _relatedSerive.GetRelatedItems(part.ContentItem.Id, context);
                    var content = _contentManger.GetMany<IContent>(related.Select(e => e.ContentItemId), VersionOptions.Published, QueryHints.Empty); 
                    return shapeHelper.Parts_RelatedContentPart(RelatedContent: content);
                });
        }
    }
}