using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using Orchard.Localization.Records;
using Orchard.Localization.Services;
using RM.Localization.Models;

namespace RM.Localization.Services
{
    public class CultureService : ICultureService
    {
        private readonly ICultureManager _cultureManager;
        //private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IOrchardServices _orchardServices;

        public CultureService(IWorkContextAccessor workContextAccessor, IOrchardServices orchardServices, ICultureManager cultureManager, ILocalizationService localizationService)
        {
            _orchardServices = orchardServices;
            _cultureManager = cultureManager;
            //_workContextAccessor = workContextAccessor;
            _localizationService = localizationService;
        }

        public IEnumerable<CultureItemModel> ListCultures()
        {
            return _cultureManager.ListCultures().Select(x => new CultureInfo(x)).Select(x => new CultureItemModel { Culture = x.Name, LocalizedName = x.NativeName, ShortName = x.TwoLetterISOLanguageName, FullName = x.DisplayName });
        }

        public string GetCurrentCulture()
        {
            return _cultureManager.GetCurrentCulture(_orchardServices.WorkContext.HttpContext);
        }

        public string GetSiteCulture()
        {
            return _cultureManager.GetSiteCulture();
        }

        public IContent GetCurrentContentItem()
        {
            var values = _orchardServices.WorkContext.HttpContext.Request.RequestContext.RouteData.Values;

            object v = values.TryGetValue("area", out v) && string.Equals("Contents", v as string, StringComparison.OrdinalIgnoreCase) ? v : null;
            v = v != null && values.TryGetValue("controller", out v) && string.Equals("Item", v as string, StringComparison.OrdinalIgnoreCase) ? v : null;
            v = v != null && values.TryGetValue("action", out v) && string.Equals("Display", v as string, StringComparison.OrdinalIgnoreCase) ? v : null;

            int id = v != null && values.TryGetValue("id", out v) && int.TryParse(v as string, out id) ? id : 0;
            if (id > 0) return _orchardServices.ContentManager.Get(id);
            return null;
        }

        public IEnumerable<LocalizationPart> GetLocalizations(LocalizationPart part, VersionOptions versionOptions)
        {
            CultureRecord siteCulture = null;
            return new[] { (part.MasterContentItem ?? part.ContentItem).As<LocalizationPart>() }
                .Union(part.Id > 0 ? _localizationService.GetLocalizations(part.MasterContentItem ?? part.ContentItem, versionOptions) : new LocalizationPart[0])
                .Select(c =>
                {
                    var localized = c.ContentItem.As<LocalizationPart>();
                    if (localized.Culture == null)
                        localized.Culture = siteCulture ?? (siteCulture = _cultureManager.GetCultureByName(GetSiteCulture()));
                    return c;
                });
        }
    }
}
