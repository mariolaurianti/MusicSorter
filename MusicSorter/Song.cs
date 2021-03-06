using System;
using System.Xml.Serialization;

namespace MusicSorter
{
    [Serializable, XmlRoot("Song")]
    public class Song
    {
        public int SongId { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string FilePath { get; set; }
        public bool HasArtist { get; set; }
    }
}