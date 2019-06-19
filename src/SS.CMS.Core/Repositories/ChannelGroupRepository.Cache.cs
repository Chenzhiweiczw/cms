﻿using System.Collections.Generic;
using System.Linq;
using SS.CMS.Models;

namespace SS.CMS.Core.Repositories
{
    public partial class ChannelGroupRepository
    {
        public void ClearCache()
        {
            _cacheManager.Remove(CacheKey);
        }

        public Dictionary<int, List<ChannelGroupInfo>> GetAllChannelGroups()
        {
            var retval = _cacheManager.Get<Dictionary<int, List<ChannelGroupInfo>>>(CacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<Dictionary<int, List<ChannelGroupInfo>>>(CacheKey);
            if (retval == null)
            {
                retval = GetAllChannelGroupsToCache();

                _cacheManager.Insert(CacheKey, retval);
            }

            return retval;
        }

        public bool IsExists(int siteId, string groupName)
        {
            var list = GetChannelGroupInfoList(siteId);
            return list.Any(group => group.GroupName == groupName);
        }

        public ChannelGroupInfo GetChannelGroupInfo(int siteId, string groupName)
        {
            var list = GetChannelGroupInfoList(siteId);
            return list.FirstOrDefault(group => group.GroupName == groupName);
        }

        public List<string> GetGroupNameList(int siteId)
        {
            var list = GetChannelGroupInfoList(siteId);
            return list.Select(group => group.GroupName).ToList();
        }

        public List<ChannelGroupInfo> GetChannelGroupInfoList(int siteId)
        {
            List<ChannelGroupInfo> list = null;
            var dict = GetAllChannelGroupsToCache();

            if (dict != null && dict.ContainsKey(siteId))
            {
                list = dict[siteId];
            }
            return list ?? new List<ChannelGroupInfo>();
        }
    }
}
