﻿using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

// ReSharper disable All

namespace MusicSorter
{
    public static class CacheXml
    {
        private const string _cacheXmlFile = "CachedListOfSongs.xml";
        public static bool DoesCacheExists => File.Exists(CacheFilePath);
        public static bool IsCacheInSync => ValidateCache();
        private static string TempFolderPath = Path.GetTempPath();
        private static string CacheFilePath = $@"{TempFolderPath}\{_cacheXmlFile}";
        
        public static void SaveToCache(ActionJackson ribs)
        {
            SaveData(ribs.ListOfSongs, CacheFilePath);
        }

        private static void SaveData(object obj, string filename)
        {
            var serializer = new XmlSerializer(obj.GetType());
            var writer = new StreamWriter(filename);
            serializer.Serialize(writer, obj);
            writer.Close();
        }

        public static List<Song> LoadCachedData()
        {
            if (!DoesCacheExists) return null;

            var serializer = new XmlSerializer(typeof(List<Song>));
            using (FileStream stream = File.OpenRead(CacheFilePath))
            {
                List<Song> dezerializedList = (List<Song>) serializer.Deserialize(stream);
                return dezerializedList;
            }
        }

        public static void DeleteCache()
        {
            File.Delete(CacheFilePath);
        }

        private static bool ValidateCache()
        {
            //TODO
            return true;
        }

        public static void UpdateCache()
        {
            
        }
    }
}