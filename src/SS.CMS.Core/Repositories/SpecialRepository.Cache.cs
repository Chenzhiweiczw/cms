﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Services.IPathManager;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class SpecialRepository
    {
        private readonly object _syncRoot = new object();

        public SpecialInfo GetSpecialInfo(int siteId, int specialId)
        {
            SpecialInfo specialInfo = null;
            var specialInfoDictionary = GetSpecialInfoDictionaryBySiteId(siteId);

            if (specialInfoDictionary != null && specialInfoDictionary.ContainsKey(specialId))
            {
                specialInfo = specialInfoDictionary[specialId];
            }
            return specialInfo;
        }

        public string GetTitle(int siteId, int specialId)
        {
            var title = string.Empty;

            var specialInfo = GetSpecialInfo(siteId, specialId);
            if (specialInfo != null)
            {
                title = specialInfo.Title;
            }

            return title;
        }

        public List<TemplateInfo> GetTemplateInfoList(SiteInfo siteInfo, int specialId, IPathManager pathManager)
        {
            var list = new List<TemplateInfo>();

            var specialInfo = GetSpecialInfo(siteInfo.Id, specialId);
            if (specialInfo != null)
            {
                var directoryPath = pathManager.GetSpecialDirectoryPath(siteInfo, specialInfo.Url);
                var srcDirectoryPath = pathManager.GetSpecialSrcDirectoryPath(directoryPath);

                var htmlFilePaths = Directory.GetFiles(srcDirectoryPath, "*.html", SearchOption.AllDirectories);
                foreach (var htmlFilePath in htmlFilePaths)
                {
                    var relatedPath = PathUtils.GetPathDifference(srcDirectoryPath, htmlFilePath);

                    var templateInfo = new TemplateInfo
                    {
                        Id = 0,
                        Content = GetContentByFilePath(htmlFilePath),
                        CreatedFileExtName = ".html",
                        CreatedFileFullName = PathUtils.Combine(specialInfo.Url, relatedPath),
                        IsDefault = false,
                        RelatedFileName = string.Empty,
                        SiteId = siteInfo.Id,
                        Type = TemplateType.FileTemplate,
                        TemplateName = relatedPath
                    };

                    list.Add(templateInfo);
                }
            }

            return list;
        }

        public List<int> GetAllSpecialIdList(int siteId)
        {
            var list = new List<int>();

            var specialInfoDictionary = GetSpecialInfoDictionaryBySiteId(siteId);
            if (specialInfoDictionary == null) return list;

            foreach (var specialInfo in specialInfoDictionary.Values)
            {
                list.Add(specialInfo.Id);
            }

            return list;
        }

        private Dictionary<int, SpecialInfo> GetSpecialInfoDictionaryBySiteId(int siteId, bool flush = false)
        {
            var dictionary = GetCacheDictionary();

            Dictionary<int, SpecialInfo> specialInfoDictionary = null;

            if (!flush && dictionary.ContainsKey(siteId))
            {
                specialInfoDictionary = dictionary[siteId];
            }

            if (specialInfoDictionary == null)
            {
                specialInfoDictionary = GetSpecialInfoDictionaryBySiteIdToCache(siteId);

                if (specialInfoDictionary != null)
                {
                    UpdateCache(dictionary, specialInfoDictionary, siteId);
                }
            }
            return specialInfoDictionary;
        }

        private void UpdateCache(Dictionary<int, Dictionary<int, SpecialInfo>> dictionary, Dictionary<int, SpecialInfo> specialInfoDictionary, int siteId)
        {
            lock (_syncRoot)
            {
                dictionary[siteId] = specialInfoDictionary;
            }
        }

        public void RemoveCache(int siteId)
        {
            var dictionary = GetCacheDictionary();

            lock (_syncRoot)
            {
                dictionary.Remove(siteId);
            }
        }

        private Dictionary<int, Dictionary<int, SpecialInfo>> GetCacheDictionary()
        {
            var dictionary = _cacheManager.Get<Dictionary<int, Dictionary<int, SpecialInfo>>>(CacheKey);
            if (dictionary != null) return dictionary;

            dictionary = new Dictionary<int, Dictionary<int, SpecialInfo>>();
            _cacheManager.InsertHours(CacheKey, dictionary, 24);
            return dictionary;
        }

        private string GetContentByFilePath(string filePath)
        {
            try
            {
                var content = _cacheManager.Get<string>(filePath);
                if (content != null) return content;

                if (FileUtils.IsFileExists(filePath))
                {
                    content = FileUtils.ReadText(filePath, Encoding.UTF8);
                }

                _cacheManager.Insert(filePath, content, TimeSpan.FromHours(12), filePath);
                return content;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
