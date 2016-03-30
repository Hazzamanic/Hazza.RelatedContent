using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Hazza.RelatedContent.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Themes;


namespace Hazza.RelatedContent.Controllers {
    [Themed]
    public class TestController : Controller {
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IRelatedService _relatedService;

        public TestController(IContentManager contentManager,
            IOrchardServices services, IOrchardServices orchardServices, IRelatedService relatedService) {
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
            Services = services;
            _orchardServices = orchardServices;
            _relatedService = relatedService;
        }

        public Localizer T { get; set; }
        public IOrchardServices Services { get; private set; }

        public ActionResult Item(int id) {
            var contentItem = _contentManager.Get(id, VersionOptions.Published);

            if (contentItem == null) {
                return HttpNotFound();
            }

            var items = _relatedService.GetRelatedItems(id, new[] {"title"});

            return Content("yay");
        }
    }
}