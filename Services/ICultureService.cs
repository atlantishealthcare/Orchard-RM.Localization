using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RM.Localization.Models;
using Orchard;
using Orchard.Localization.Models;
using Orchard.ContentManagement;

namespace RM.Localization.Services
{
    public interface ICultureService : IDependency {
        IEnumerable<CultureItemModel> ListCultures();
        string GetCurrentCulture();
        string GetSiteCulture();
        IContent GetCurrentContentItem();
        IEnumerable<LocalizationPart> GetLocalizations(LocalizationPart part, VersionOptions versionOptions);
    }
}
