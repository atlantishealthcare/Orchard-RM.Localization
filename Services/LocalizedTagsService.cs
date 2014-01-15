using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Localization.Models;
using Orchard.Localization.Records;
using Orchard.Localization.Services;
using Orchard.Tags.Models;
using RM.Localization.Models;
using Orchard.Caching;
using Orchard.Environment.Extensions;

namespace RM.Localization.Services
{
    [OrchardFeature("RM.Localization.LocalizedTags")]
    public class LocalizedTagsService : ILocalizedTagsService
    {
        class LocalizationBucket
        {
            public CultureRecord CurrentCulture { get; private set; }
            public CultureRecord DefaultCulture { get; private set; }
            public bool IsCurrentCultureDefault { get; private set; }

            public LocalizationBucket(ICultureManager cultureManager, IWorkContextAccessor workContextAccessor)
            {
                var currentCultureName = cultureManager.GetCurrentCulture(workContextAccessor.GetContext().HttpContext);
                var defaultCultureName = cultureManager.GetSiteCulture();
                CurrentCulture = cultureManager.GetCultureByName(currentCultureName);
                DefaultCulture = cultureManager.GetCultureByName(defaultCultureName);
                IsCurrentCultureDefault = CurrentCulture != null && CurrentCulture.Id == DefaultCulture.Id;
            }
        }

        internal const string _localizedTagcloudTagsChanged = "RM.Localization.LocalizedTagCloud.TagsChanged";

        const int Buckets = 6;
        private readonly IRepository<ContentTagRecord> _contentTagRepository;
        private readonly IRepository<LocalizationPartRecord> _localizationPartRepository;
        private readonly ICultureManager _cultureManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        public LocalizedTagsService(IContentManager contentManager,
                                    IRepository<ContentTagRecord> contentTagRepository, 
                                    IRepository<LocalizationPartRecord> localizationPartRepository, 
                                    ICultureManager cultureManager,
                                    IWorkContextAccessor workContextAccessor,
                                    ICacheManager cacheManager,
                                    ISignals signals) 
        {
            _contentManager = contentManager;
            _contentTagRepository = contentTagRepository;
            _localizationPartRepository = localizationPartRepository;
            _cultureManager = cultureManager;
            _workContextAccessor = workContextAccessor;
            _cacheManager = cacheManager;
            _signals = signals;
        }

        public IEnumerable<LocalizedTag> GetLocalizedTags()
        {
            var localizationBucket = new LocalizationBucket(_cultureManager, _workContextAccessor);
            if (localizationBucket.CurrentCulture == null || localizationBucket.CurrentCulture.Id == 0) return new List<LocalizedTag>();

            var cacheKey = "RM.Localization.TagCloud." + localizationBucket.CurrentCulture.Culture;

            return _cacheManager.Get(cacheKey, ctx =>
            {
                ctx.Monitor(_signals.When(_localizedTagcloudTagsChanged));
                var tags = _contentManager
                    .Query<TagsPart, TagsPartRecord>(VersionOptions.Published)
                    .List()
                    .Select(x => x.As<LocalizationPart>())
                    .Where(x => x.Culture == null && localizationBucket.IsCurrentCultureDefault || x.Culture != null && x.Culture.Id == localizationBucket.CurrentCulture.Id)
                    .Select(x => x.As<TagsPart>())
                    .SelectMany(x => x.CurrentTags)
                    .GroupBy(x => x.TagName)
                    .Select(x => new LocalizedTag { TagName = x.Key, Count = x.Count() }).ToList();

                if (tags.Any())
                {
                    var maxCount = tags.Max(tc => tc.Count);
                    var minCount = tags.Min(tc => tc.Count);
                    var delta = maxCount - minCount;
                    if (delta != 0)
                    {
                        foreach (var tag in tags)
                        {
                            tag.Bucket = (tag.Count - minCount) * (Buckets - 1) / delta + 1;
                        }
                    }
                }
                return tags;
            });
        }


        public IEnumerable<IContent> GetTaggedContentItems(int tagId, int skip, int take)
        {
            var localizationBucket = new LocalizationBucket(_cultureManager, _workContextAccessor);
            if (localizationBucket.CurrentCulture == null || localizationBucket.CurrentCulture.Id == 0) return new TagsPart[0];

            return _contentManager
                .Query<TagsPart, TagsPartRecord>(VersionOptions.Published)
                .Where(tpr => tpr.Tags.Any(tr => tr.TagRecord.Id == tagId))
                .Join<CommonPartRecord>().OrderByDescending(x => x.CreatedUtc)
                .List()
                .Select(x=>x.As<LocalizationPart>())
                .Where(x=>x.Culture == null && localizationBucket.IsCurrentCultureDefault || x.Culture != null && x.Culture.Id == localizationBucket.CurrentCulture.Id)
                .Skip(skip).Take(take);
        }

        public int GetTaggedContentItemCount(int tagId)
        {
            var localizationBucket = new LocalizationBucket(_cultureManager, _workContextAccessor);
            if (localizationBucket.CurrentCulture == null || localizationBucket.CurrentCulture.Id == 0) return 0;

            return _contentManager
                .Query<TagsPart, TagsPartRecord>(VersionOptions.Published)
                .Where(tpr => tpr.Tags.Any(tr => tr.TagRecord.Id == tagId))
                .List()
                .Select(x => x.As<LocalizationPart>())
                .Where(x => x.Culture == null && localizationBucket.IsCurrentCultureDefault || x.Culture != null && x.Culture.Id == localizationBucket.CurrentCulture.Id)
                .Count();
        }
    }
}
