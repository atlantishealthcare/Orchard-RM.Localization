using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Themes;
using RM.Localization.Services;
using Orchard.Alias.Implementation.Holder;

namespace RM.Localization.Controllers
{
    [HandleError, Themed]
    [OrchardFeature("RM.Localization")]
    public class LocalizedHomeController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICultureService _cultureService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public LocalizedHomeController(IOrchardServices orchardServices, ICultureService cultureService, IWorkContextAccessor workContextAccessor)
        {
            _cultureService = cultureService;
            _orchardServices = orchardServices;
            _workContextAccessor = workContextAccessor;
        }

        public ActionResult RedirectToCulture()
        {
            // Add check for existing translation and redirect to nottranslated page
            var currentCulture = _cultureService.GetCurrentCulture();
            if (string.IsNullOrWhiteSpace(currentCulture)) currentCulture = _cultureService.GetSiteCulture();

            var context = _workContextAccessor.GetContext(HttpContext);
            var aliasHolder = context.Resolve<IAliasHolder>();
            var aliasMap = aliasHolder.GetMap("Contents");

            currentCulture = currentCulture.ToLower();
            IDictionary<string, string> routeValues = null;
            if (aliasMap.TryGetAlias(currentCulture, out routeValues)) return Redirect(Url.Content("~/" + currentCulture));

            return RedirectToRoute(new { Area = "RM.Localization", Action = "NotTranslated", Controller = "LocalizedHome", Culture = currentCulture });
        }

        public ActionResult NotTranslated(string culture, int? id)
        {
            ViewBag.Culture = culture ?? _cultureService.GetCurrentCulture();
            ViewBag.ContentItem = id.HasValue ? _orchardServices.ContentManager.Get(id.Value) : null;
            ViewBag.Localizations = ViewBag.ContentItem != null ? _cultureService.GetLocalizations((ViewBag.ContentItem as IContent).As<LocalizationPart>(), VersionOptions.Latest).ToArray() : new LocalizationPart[0];
            return View("NotTranslated");
        }
    }
}
