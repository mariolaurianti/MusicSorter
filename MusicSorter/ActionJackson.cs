using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TagLib;
using File = System.IO.File;

namespace MusicSorter
{
    public class ActionJackson
    {
        public List<Song> ListOfSongs { get; set; }
        public string MessyFolderPath { get; set; }
        public string CleanNewFolderPath { get; set; }
        
        private static string SterilizeAlbumName(string albumName)
        {
            var regex = new Regex("[^a-zA-Z0-9 -]");
            albumName = regex.Replace(albumName, "");
            return albumName;
        }

        public void CreateFolders(List<Song> cachedListOfAlbums = null)
        {
            var listOfSongs = cachedListOfAlbums ?? ListOfSongs;

            foreach (var song in listOfSongs)
            {
                if (song.Album == null) continue;
                var albumName = SterilizeAlbumName(song.Album);
                var newFolderPath = Path.Combine(CleanNewFolderPath, albumName);

                Directory.CreateDirectory(newFolderPath);
            }
        }
        
        private void CheckForDuplicate(string songTitle)
        {
            var listOfDuplicates = ListOfSongs
                .Where(x => x.Name != null && x.Name.Equals(songTitle)).ToList();

            if (listOfDuplicates.Count == 1) return; 

            foreach (var duplicate in listOfDuplicates.ToList())
            {
                if (!string.IsNullOrEmpty(duplicate.Artist) || listOfDuplicates.Count <= 1) continue;

                ListOfSongs.Remove(duplicate);
                listOfDuplicates.Remove(duplicate);
            }

            if(listOfDuplicates.Count > 1)
                listOfDuplicates.RemoveRange(1, listOfDuplicates.Count - 1);
        }


        public void AddSongsToFolders()
        {
            foreach (var song in ListOfSongs.ToList())
            {
                if (song.Album == null) continue;
                var albumName = SterilizeAlbumName(song.Album);

                if (song.FilePath == null) continue;
                var combinedName = Path.Combine(CleanNewFolderPath, albumName);
                var combinedPath = Path.Combine(combinedName, Path.GetFileName(song.FilePath));

                CheckForDuplicate(song.Name);

                if (!File.Exists(combinedPath))
                    File.Copy(song.FilePath, combinedPath);

                HelperMethods.DrawTextProgressBar(song.SongId, ListOfSongs.Count);
            }
        }

        public void ImportSongInformation(string folderPath)
        {
            foreach (var folder in Directory.GetDirectories(folderPath))
            {
                var hasLength = Directory.GetDirectories(Path.GetFullPath(folder)).Length == 0;
                if (hasLength)
                    GetSongInformationForAllFilesInFolder(folder);
                else
                    ImportSongInformation(folder);
            }
        }

        private void GetSongInformationForAllFilesInFolder(string folder)
        {
            var factory = new EntityIdFactory();

            foreach (var file in Directory.GetFiles(folder))
            {
                try
                {
                    var fileInformation = TagLib.File.Create(file);

                    var songInformation = new Song
                    {
                        SongId = factory.Create(ListOfSongs),
                        Album = fileInformation.Tag.Album,
                        Artist = fileInformation.Tag.FirstAlbumArtist,
                        Name = fileInformation.Tag.Title,
                        FilePath = Path.GetFullPath(file)
                    };

                    ListOfSongs.Add(songInformation);
                }
                catch (UnsupportedFormatException) {}
                catch (CorruptFileException) { }
            }
        }
    }
}