﻿using System;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class SiteLogRepository
    {
        public void AddSiteLog(int siteId, int channelId, int contentId, string ipAddress, string adminName, string action, string summary)
        {
            // if (!ConfigManager.Instance.IsLogSite) return;

            if (siteId <= 0)
            {
                _logRepository.AddAdminLog(adminName, action, summary);
            }
            else
            {
                try
                {
                    DeleteIfThreshold();

                    if (!string.IsNullOrEmpty(action))
                    {
                        action = StringUtils.MaxLengthText(action, 250);
                    }
                    if (!string.IsNullOrEmpty(summary))
                    {
                        summary = StringUtils.MaxLengthText(summary, 250);
                    }
                    if (channelId < 0)
                    {
                        channelId = -channelId;
                    }

                    var siteLogInfo = new SiteLogInfo
                    {
                        SiteId = siteId,
                        ChannelId = channelId,
                        ContentId = contentId,
                        UserName = adminName,
                        IpAddress = ipAddress,
                        Action = action,
                        Summary = summary
                    };
                    Insert(siteLogInfo);
                }
                catch (Exception ex)
                {
                    _errorLogRepository.AddErrorLog(ex);
                }
            }
        }
    }
}
