using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Tags.Models;
using RM.Localization.Models;
using RM.Localization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;

namespace RM.Localization.Handlers
{
    [OrchardFeature("RM.Localization.LocalizedTags")]
    public class LocalizedTagCloudHandler : ContentHandler
    {
        private readonly ILocalizedTagsService _tagsService;
        private readonly ISignals _signals;

        public LocalizedTagCloudHandler(
            ILocalizedTagsService tagsService,
            ISignals signals) {

            _tagsService = tagsService;
            _signals = signals;
        }

        protected override void Loading(LoadContentContext context)
        {
            if (context.ContentType == "LocalizedTagCloud")
            {
                SetupTagCloudLoader(context.ContentItem);
            }
            base.Loading(context);
        }

        protected override void Versioning(VersionContentContext context)
        {
            if (context.ContentType == "LocalizedTagCloud")
            {
                SetupTagCloudLoader(context.BuildingContentItem);
            }
            base.Versioning(context);
        }

        protected override void Published(PublishContentContext context)
        {
            var contentTags = context.ContentItem.As<TagsPart>();
            if (contentTags != null)
            {
                _signals.Trigger(LocalizedTagsService._localizedTagcloudTagsChanged);
            }
            base.Published(context);
        }

        protected override void Unpublished(PublishContentContext context)
        {
            var contentTags = context.ContentItem.As<TagsPart>();
            if (contentTags != null)
            {
                _signals.Trigger(LocalizedTagsService._localizedTagcloudTagsChanged);
            }
            base.Unpublished(context);
        }

        protected override void Removed(RemoveContentContext context)
        {
            var contentTags = context.ContentItem.As<TagsPart>();
            if (contentTags != null)
            {
                _signals.Trigger(LocalizedTagsService._localizedTagcloudTagsChanged);
            }
            base.Removed(context);
        }

        private void SetupTagCloudLoader(ContentItem item)
        {
            var cloudPart = (LocalizedTagCloudPart)item.Get(typeof(LocalizedTagCloudPart));
            cloudPart._tags.Loader(() => _tagsService.GetLocalizedTags().ToList());
        }

    }
}
