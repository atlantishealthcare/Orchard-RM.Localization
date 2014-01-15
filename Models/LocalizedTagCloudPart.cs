using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;

namespace RM.Localization.Models
{
    [OrchardFeature("RM.Localization.LocalizedTags")]
    public class LocalizedTagCloudPart : ContentPart<dynamic>
    {
        internal readonly LazyField<IList<LocalizedTag>> _tags = new LazyField<IList<LocalizedTag>>();

        public IList<LocalizedTag> Tags { get { return _tags.Value; } }

    }
}
