using System;
using System.Linq;
using System.Web;
using System.Globalization;
using Orchard;
using Orchard.Localization.Services;
using Orchard.Environment.Extensions;
using Orchard.Data;
using Orchard.Localization.Records;
using System.Collections.Generic;

namespace RM.Localization.Providers 
{
    [OrchardFeature("RM.Localization.BrowserCultureSelector")]
    public class BrowserCultureSelector : ICultureSelector {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IRepository<CultureRecord> _cultureRepository;

        public BrowserCultureSelector(IWorkContextAccessor workContextAccessor, IRepository<CultureRecord> cultureRepository)
        {
            _cultureRepository = cultureRepository;
            _workContextAccessor = workContextAccessor;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context) {
            var workContext = _workContextAccessor.GetContext();
            if (workContext == null || workContext.HttpContext == null || workContext.HttpContext.Request == null || workContext.HttpContext.Request.UserLanguages == null) return null;
            
            var browserCultures =  workContext.HttpContext.Request.UserLanguages.Select(x => x.Split(';').FirstOrDefault()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (browserCultures.Length == 0) return null;

            var cultureName = SelectSuitableCulture(ListCultures(), browserCultures);

            return cultureName == null ? null : new CultureSelectorResult { Priority = -4, CultureName = cultureName };
        }

        private string SelectSuitableCulture(IEnumerable<string> supportedCultures, IEnumerable<string> browserCultures)
        {
            var supportedCultureInfos = supportedCultures.Select(x=>CultureHelper.ParseCultureInfo(x)).Where(x=>x != null).ToArray();
            Tuple<CultureInfo, int> best = null;
            foreach (var browserCultureInfo in browserCultures.Select(x=>CultureHelper.ParseCultureInfo(x)).Where(x=>x != null))
            {
                var localBest = supportedCultureInfos.Select(x => new Tuple<CultureInfo, int>(x, GetRank(x, browserCultureInfo))).Where(x => x.Item2 > 0).OrderByDescending(x => x.Item2).FirstOrDefault();
                if (localBest != null && (best == null || localBest.Item2 > best.Item2)) best = localBest;
            }
            return best != null ? best.Item1.Name : null;
        }

        private int GetRank(CultureInfo supportedCulture, CultureInfo browserCulture)
        {           
                    // Certain match has highest rank
            return  (supportedCulture.Name == browserCulture.Name ? 8 : 0) +
                    // if supported culture is neutral 'en' and browser culture has same parent: 'en-US' or 'en-GB' have 'en' as parent (neutral) THEN select 'en'
                    (supportedCulture.IsNeutralCulture && !browserCulture.IsNeutralCulture && supportedCulture.Name == browserCulture.Parent.Name ? 4 : 0) +
                    // if supported culture is 'en-US' or 'en-GB' that has 'en' as parent (neutral) and browser culture is neutral 'en' THEN select 'en-US' as possibly matched
                    (browserCulture.IsNeutralCulture && !supportedCulture.IsNeutralCulture && supportedCulture.Parent.Name == browserCulture.Name ? 2 : 0) +
                    // if supported culture is 'en-US' that has 'en' as neutral and browser culture is 'en-GB' that has 'en' as neutral THEN select 'en-US' as possibly matched
                    (!browserCulture.IsNeutralCulture && !supportedCulture.IsNeutralCulture && supportedCulture.Parent.Name == browserCulture.Parent.Name ? 1 : 0);
        }

        private IEnumerable<string> ListCultures()
        {
            var query = from culture in _cultureRepository.Table select culture.Culture;
            return query.ToList();
        }
    }
}
