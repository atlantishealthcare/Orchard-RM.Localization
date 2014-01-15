using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using System.Globalization;
using RM.Localization.Services;
using Orchard.Localization.Records;
using Orchard.Data;

namespace RM.Localization.Providers
{
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class CookieCultureSelector : ICultureSelector
    {
        private readonly ICookieCultureService _cookieCultureService;
        private readonly IRepository<CultureRecord> _cultureRepository;

        public CookieCultureSelector(ICookieCultureService cookieCultureService, IRepository<CultureRecord> cultureRepository)
        {
            _cookieCultureService = cookieCultureService;
            _cultureRepository = cultureRepository;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context) {
            
            var cultureCookie = _cookieCultureService.GetCulture();
            if (cultureCookie == null || cultureCookie == "Browser") return null;

            var cultureName = CultureHelper.GetSpecificOrNeutralCulture(ListCultures(), cultureCookie);

            return cultureName == null ? null : new CultureSelectorResult { Priority = 0, CultureName = cultureName };
        }

        private IEnumerable<string> ListCultures()
        {
            var query = from culture in _cultureRepository.Table select culture.Culture;
            return query.ToList();
        }
    }
}
