using System.Linq;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Orchard.Autoroute.Settings;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement.MetaData.Models;

namespace RM.Localization
{
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            ContentDefinitionManager.AlterPartDefinition(
                "CookieCulturePickerPart",
                builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(
                "CookieCulturePicker",
                cfg => cfg
                           .WithPart("CookieCulturePickerPart")
                           .WithPart("CommonPart")
                           .WithPart("WidgetPart")
                           .WithSetting("Stereotype", "Widget")
                );
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.CreateTable("CookieCulturePickerPartRecord",
                                        table => table.ContentPartRecord()
                                            .Column<string>("Style", c => c.WithLength(255))
                                        );

            ContentDefinitionManager.AlterTypeDefinition(
                "CookieCulturePicker",
                cfg => cfg
                           .WithPart("CommonPart")
                           .WithPart("IdentityPart")
                           .WithPart("WidgetPart")
                           .WithPart("CookieCulturePickerPart")
                           .WithSetting("Stereotype", "Widget")
                );
            return 2;
        }

        const string LocalizationPartTypeName = "LocalizationPart";
        const string AutoroutePartTypeName = "AutoroutePart";
        const string LocalizedTitleName = "LocalizedTitle";
        static readonly string[] _contentTypesToUpdate = new[] { "Page", "BlogPost", "ProjectionPage", "ContentMenuItem", "MenuItem", "HtmlMenuItem", "NavigationQueryMenuItem", "ShapeMenuItem", "TaxonomyNavigationMenuItem" };

        public int UpdateFrom2()
        {
            foreach (var contentTypeName in _contentTypesToUpdate)
            {
                var typeDef = ContentDefinitionManager.GetTypeDefinition(contentTypeName);
                if (typeDef == null) continue;

                var localizationPart = typeDef.Parts.Where(p=>p.PartDefinition.Name == LocalizationPartTypeName).FirstOrDefault();
                if (localizationPart == null)
                {
                    ContentDefinitionManager.AlterTypeDefinition(contentTypeName, c=>c.WithPart(LocalizationPartTypeName));
                    typeDef = ContentDefinitionManager.GetTypeDefinition(contentTypeName);
                }

                var autoroutePart = typeDef.Parts.Where(p => p.PartDefinition.Name == AutoroutePartTypeName).FirstOrDefault();
                if (autoroutePart == null) continue;

                var autoroutePartSettings = LoadAutorouteSettings(autoroutePart.Settings);

                var routePattern = autoroutePartSettings.Patterns.Where(p => p.Name == LocalizedTitleName).FirstOrDefault();
                if (routePattern == null)
                {
                    var patterns = autoroutePartSettings.Patterns.ToList();
                    var baseRoute = patterns.FirstOrDefault();
                    routePattern = new RoutePattern
                    { 
                        Name = LocalizedTitleName,
                        Pattern = baseRoute != null ? string.Format("{{Content.Culture}}/{0}", baseRoute.Pattern) : "{Content.Culture}/{Content.Slug}",
                        Description = baseRoute != null ? string.Format("en-us/{0}", baseRoute.Description) : "en-us/my-content-item"
                    };
                    patterns.Add(routePattern);
                    autoroutePartSettings.Patterns = patterns;
                }
                var defaultIndex = autoroutePartSettings.Patterns.IndexOf(routePattern);
                autoroutePartSettings.DefaultPatternIndex = defaultIndex;

                SetAutorouteSettings(autoroutePartSettings, autoroutePart.Settings);

                ContentDefinitionManager.StoreTypeDefinition(typeDef);
            }
            return 3;
        }

        private AutorouteSettings LoadAutorouteSettings(SettingsDictionary settings)
        {
            bool b;
            int i;
            string s;

            return new AutorouteSettings 
            {
                AllowCustomPattern = settings.TryGetValue("AutorouteSettings.AllowCustomPattern", out s) && bool.TryParse(s, out b) && b,
                AutomaticAdjustmentOnEdit = settings.TryGetValue("AutorouteSettings.AutomaticAdjustmentOnEdit", out s) && bool.TryParse(s, out b) && b,
                DefaultPatternIndex = settings.TryGetValue("AutorouteSettings.DefaultPatternIndex", out s) && int.TryParse(s, out i) ? i : -1,
                PatternDefinitions = settings.TryGetValue("AutorouteSettings.PatternDefinitions", out s) ? s : string.Empty,
                PerItemConfiguration = settings.TryGetValue("AutorouteSettings.PerItemConfiguration", out s) && bool.TryParse(s, out b) && b
            };
        }

        private void SetAutorouteSettings(AutorouteSettings autorouteSettings, SettingsDictionary settings)
        {
            settings["AutorouteSettings.AllowCustomPattern"] = autorouteSettings.AllowCustomPattern.ToString();
            settings["AutorouteSettings.AutomaticAdjustmentOnEdit"] = autorouteSettings.AutomaticAdjustmentOnEdit.ToString();
            settings["AutorouteSettings.DefaultPatternIndex"] = autorouteSettings.DefaultPatternIndex.ToString();
            settings["AutorouteSettings.PerItemConfiguration"] = autorouteSettings.PerItemConfiguration.ToString();
            settings["AutorouteSettings.PatternDefinitions"] = autorouteSettings.PatternDefinitions;
        }
    }

    [OrchardFeature("RM.Localization.ShadowCultureManager")]
    public class ShadowCultureManagerMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("ShadowCulturePartRecord",
                table => table.ContentPartRecord()
                    .Column<string>("Rules", c=>c.WithLength(2048))
                );
            return 1;
        }
    }

    [OrchardFeature("RM.Localization.LocalizedTags")]
    public class LocalizedTagCloudMigrations : DataMigrationImpl
    {
        public int Create()
        {
            ContentDefinitionManager.AlterPartDefinition("LocalizedTagCloudPart", builder => builder.Attachable());
            ContentDefinitionManager.AlterTypeDefinition("LocalizedTagCloud",
                cfg => cfg
                    .WithPart("LocalizedTagCloudPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );
            return 1;
        }
    }
}
