using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Localization.ViewModels;
using RM.Localization.Services;

namespace RM.Localization.Drivers
{
    [UsedImplicitly]
    public class LocalizationPartDriver : ContentPartDriver<LocalizationPart>
    {
        private const string TemplatePrefix = "Localization";
        private readonly ICultureService _cultureService;

        public LocalizationPartDriver(ICultureService cultureService) 
        {
            _cultureService = cultureService;
        }

        protected override DriverResult Display(LocalizationPart part, string displayType, dynamic shapeHelper) {
            var localizations = _cultureService.GetLocalizations(part, VersionOptions.Latest).ToList();
            var siteCulture = _cultureService.GetSiteCulture();
            var selectedCulture = part.Culture != null ? part.Culture.Culture : (part.Id == 0 ? siteCulture : null);
            return ContentShape("Parts_RMLocalization_ContentTranslations_SummaryAdmin",
                             () => shapeHelper.Parts_RMLocalization_ContentTranslations_SummaryAdmin(MasterId: part.MasterContentItem != null ? part.MasterContentItem.Id : part.Id,
                                                                                                     MasterContentItem: part.MasterContentItem,
                                                                                                     ShowAddTranslation: _cultureService.ListCultures().Select(c => c.Culture).Where(s => s != siteCulture && !localizations.Select(l => l.Culture.Culture).Contains(s)).Any(),
                                                                                                     SelectedCulture: selectedCulture,
                                                                                                     Localizations: localizations.Where(c => c.Culture.Culture != selectedCulture)));
        }

        protected override DriverResult Editor(LocalizationPart part, dynamic shapeHelper) {
            var localizations = _cultureService.GetLocalizations(part, VersionOptions.Latest).ToList();
            var siteCulture = _cultureService.GetSiteCulture();
            var selectedCulture = part.Culture != null ? part.Culture.Culture : (part.Id == 0 ? siteCulture : null);
            var model = new EditLocalizationViewModel
            {
                SelectedCulture = selectedCulture,
                SiteCultures = _cultureService.ListCultures().Select(c=>c.Culture).Where(s => s != siteCulture && !localizations.Select(l => l.Culture.Culture).Contains(s)),
                ContentItem = part,
                MasterContentItem = part.MasterContentItem,
                ContentLocalizations = new ContentLocalizationsViewModel(part) { Localizations = localizations.Where(c => c.Culture.Culture != selectedCulture) }
            };

            return ContentShape("Parts_RMLocalization_ContentTranslations_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts.RMLocalization.ContentTranslations.Edit", Model: model, Prefix: TemplatePrefix));
        }
    }
}