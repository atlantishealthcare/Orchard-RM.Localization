using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using RM.Localization.Models;
using Orchard.Localization.Services;
using RM.Localization.Services;
using Orchard.ContentManagement;
using System.Web.Mvc;
using RM.Localization.ViewModels;
using Orchard.Localization.Models;

namespace RM.Localization.Drivers
{
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class CookieCulturePickerDriver : ContentPartDriver<CookieCulturePickerPart> {

        public static readonly string[] Styles = new[] { "Inline List", "Dropdown" };

        protected override string Prefix { get { return "CookieCulturePickerEdit"; } }

        private readonly ICultureService _cultureService;
        private readonly IOrchardServices _orchardServices;

        public CookieCulturePickerDriver(IOrchardServices orchardServices, ICultureService cultureService)
        {
            _orchardServices = orchardServices;
            _cultureService = cultureService;
        }

        protected override DriverResult Display(CookieCulturePickerPart part, string displayType, dynamic shapeHelper)
        {
            var urlHelper = new UrlHelper(_orchardServices.WorkContext.HttpContext.Request.RequestContext);
            var cookieCultureItems = new List<CookieCultureItemViewModel>();
            // Is content item shown?
            var contentItem = _cultureService.GetCurrentContentItem();
            var currentCulture = _cultureService.GetCurrentCulture();
            var returnUrl = _orchardServices.WorkContext.HttpContext.Request.Url.LocalPath;
            var localizations = contentItem != null ? _cultureService.GetLocalizations(contentItem.As<LocalizationPart>(), VersionOptions.Latest).ToList() : null;

            if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrl.IndexOf("/NotTranslated", StringComparison.OrdinalIgnoreCase) > 0) returnUrl = urlHelper.Content("~/");

            foreach (var cultureItem in _cultureService.ListCultures())
            {
                var cookieCultureItem = new CookieCultureItemViewModel
                {
                    CultureItem = cultureItem,
                    Current = string.Equals(currentCulture, cultureItem.Culture, StringComparison.OrdinalIgnoreCase),
                    ReturnUrl = returnUrl,
                    Rel = contentItem == null ? "nofollow" : null
                };
                if (localizations != null)
                {
                    var localizedContentItem = localizations.Where(p => string.Equals(p.Culture.Culture, cultureItem.Culture, StringComparison.OrdinalIgnoreCase)).Select(p => p.ContentItem).FirstOrDefault();
                    var metadata = localizedContentItem != null ? localizedContentItem.ContentManager.GetItemMetadata(localizedContentItem) : null;
                    if (metadata != null && metadata.DisplayRouteValues != null)
                    {
                        cookieCultureItem.ReturnUrl = urlHelper.Content(urlHelper.RouteUrl(metadata.DisplayRouteValues));
                    }
                    else
                    {
                        cookieCultureItem.ReturnUrl = urlHelper.RouteUrl(new { Area = "RM.Localization", Action = "NotTranslated", Controller = "LocalizedHome", Culture = cultureItem.Culture, Id = contentItem.Id });
                        cookieCultureItem.Rel = "nofollow";
                    }
                }
                cookieCultureItems.Add(cookieCultureItem);
            }

            if (part.Style == Styles[1])
            {
                return ContentShape("Parts_CookieCulturePicker", () => shapeHelper.Parts_DropdownCookieCulturePicker(Cultures: cookieCultureItems, CurrentCulture: currentCulture));
            }
            else
            {
                return ContentShape("Parts_CookieCulturePicker", () => shapeHelper.Parts_InlineListCookieCulturePicker(Cultures: cookieCultureItems, CurrentCulture: currentCulture));
            }
        }

        protected override DriverResult Editor(CookieCulturePickerPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_CookieCulturePicker_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts.CookieCulturePicker.Edit", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(CookieCulturePickerPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}
