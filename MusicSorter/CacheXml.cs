using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

// ReSharper disable All

namespace MusicSorter
{
    public static class CacheXml
    {
        private const string _cacheXmlFile = "ListOfSongsXML";
        public static bool DoesCacheExists => File.Exists(_cacheXmlFile);

        public static void SaveData(object obj, string filename)
        {
            var serializer = new XmlSerializer(obj.GetType());
            var writer = new StreamWriter(filename);
            serializer.Serialize(writer, obj);
            writer.Close();
        }

        public static List<Song> RetrieveData()
        {
            if (!DoesCacheExists) return null;

            var serializer = new XmlSerializer(typeof(List<Song>));
           
            using (FileStream stream = File.OpenRead(_cacheXmlFile))
            {
                List<Song> dezerializedList = (List<Song>) serializer.Deserialize(stream);
                return dezerializedList;
            }
        }

        public static void DeleteCache()
        {
            //TODO
        }

        public static void RefreshCache()
        {
            //TODO
        }
    }
}