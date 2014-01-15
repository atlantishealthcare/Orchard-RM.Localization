using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using RM.Localization.Models;

namespace RM.Localization.Services
{
    public interface ILocalizedTagsService : IDependency
    {
        IEnumerable<LocalizedTag> GetLocalizedTags();
        IEnumerable<IContent> GetTaggedContentItems(int tagId, int skip, int take);
        int GetTaggedContentItemCount(int tagId);
    }
}
