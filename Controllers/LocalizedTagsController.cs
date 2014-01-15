using System;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.Tags.Services;
using Orchard.Tags.ViewModels;
using Orchard.Themes;
using Orchard.UI.Navigation;
using RM.Localization.Services;
using RM.Localization.ViewModels;

namespace RM.Localization.Controllers
{
    [OrchardFeature("RM.Localization.LocalizedTags")]
    [HandleError, ValidateInput(false), Themed]
    public class LocalizedTagsController : Controller {
        private readonly ITagService _tagService;
        private readonly ILocalizedTagsService _localizedTagsService;
        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;

        public LocalizedTagsController(
            ITagService tagService,
            ILocalizedTagsService localizedTagsService,
            IContentManager contentManager,
            ISiteService siteService,
            IShapeFactory shapeFactory) 
        {
            _tagService = tagService;
            _localizedTagsService = localizedTagsService;
            _contentManager = contentManager;
            _siteService = siteService;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }
        
        public ILogger Logger { get; set; }
        public Localizer T { get; set; }
        public dynamic Shape { get; set; }

        public ActionResult Index() {
            var tags = _localizedTagsService.GetLocalizedTags();
            var model = new LocalizedTagsIndexViewModel { Tags = tags.ToList() };
            return View(model);
        }

        public ActionResult Search(string tagName, PagerParameters pagerParameters) {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var tag = _tagService.GetTagByName(tagName);

            if (tag == null) {
                return RedirectToAction("Index");
            }

            var taggedItems = _localizedTagsService.GetTaggedContentItems(tag.Id, pager.GetStartIndex(), pager.PageSize).ToList();
            var tagShapes = taggedItems.Select(item => _contentManager.BuildDisplay(item, "Summary"));

            var list = Shape.List();
            list.AddRange(tagShapes);

            var totalItemCount = _localizedTagsService.GetTaggedContentItemCount(tag.Id);
            var viewModel = new TagsSearchViewModel {
                TagName = tag.TagName,
                List = list,
                Pager = Shape.Pager(pager).TotalItemCount(totalItemCount)
            };

            return View(viewModel);
        }
    }
}
