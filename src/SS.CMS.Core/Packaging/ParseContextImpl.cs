﻿using System.Collections.Generic;
using System.Collections.Specialized;
using SS.CMS.Core.StlParser;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Core.Plugin
{
    public class ParseContextImpl : IParseContext
    {
        public ParseContextImpl(string stlOuterHtml, string stlInnerHtml, NameValueCollection stlAttributes, PageInfo pageInfo, ParseContext parseContext)
        {
            SiteId = parseContext.SiteInfo.Id;
            ChannelId = parseContext.ChannelId;
            ContentId = parseContext.ContentId;
            ContentInfo = parseContext.ContentInfo;
            TemplateType = pageInfo.TemplateInfo.Type;
            TemplateId = pageInfo.TemplateInfo.Id;

            HeadCodes = pageInfo.HeadCodes;
            BodyCodes = pageInfo.BodyCodes;
            FootCodes = pageInfo.FootCodes;
            StlOuterHtml = stlOuterHtml;
            StlInnerHtml = stlInnerHtml;
            StlAttributes = stlAttributes;

            IsStlElement = !parseContext.IsStlEntity;
            PluginItems = pageInfo.PluginItems;
        }

        public Dictionary<string, object> PluginItems { get; }

        public void Set<T>(string key, T objectValue)
        {
            if (PluginItems != null && !string.IsNullOrEmpty(key))
            {
                PluginItems[key] = objectValue;
            }
        }

        public T Get<T>(string key)
        {
            object objectValue;
            if (PluginItems.TryGetValue(key, out objectValue))
            {
                if (objectValue is T)
                {
                    return (T)objectValue;
                }
            }

            return default(T);
        }

        public int SiteId { get; }

        public int ChannelId { get; }

        public int ContentId { get; }

        public ContentInfo ContentInfo { get; }

        public TemplateType TemplateType { get; }

        public int TemplateId { get; }

        public SortedDictionary<string, string> HeadCodes { get; }

        public SortedDictionary<string, string> BodyCodes { get; }

        public SortedDictionary<string, string> FootCodes { get; }

        public string StlOuterHtml { get; }

        public string StlInnerHtml { get; }

        public NameValueCollection StlAttributes { get; }

        public bool IsStlElement { get; }
    }
}
