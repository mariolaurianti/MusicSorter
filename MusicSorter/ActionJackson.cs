using System.Collections.Generic;
using System.Globalization;
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
        public string CleanNewFolderPath { private get; set; }

        private static string SterilizeString(string property)
        {
            var regex = new Regex("[^a-zA-Z0-9 ]").Replace(property, string.Empty).Trim();
            regex = regex.Replace(".", string.Empty);
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(regex);
        }

        private void CreateArtistFolder(Song song)
        {
            var artistClean = SterilizeString(song.Artist);
            var newFolderPath = Path.Combine(CleanNewFolderPath, artistClean);

            Directory.CreateDirectory(newFolderPath);
            if(song.Album != null)
                CreateAlbumFolder(song, true, artistClean);
        }

        private void CreateAlbumFolder(Song song, bool fromArtist, string artistClean = null)
        {
            var newFolderPath = "";
            var albumClean = SterilizeString(song.Album);
            newFolderPath = fromArtist ? 
                Path.Combine(CleanNewFolderPath + @"\" + artistClean + @"\", albumClean) : 
                Path.Combine(CleanNewFolderPath + @"\" + @"Unknown Artist", albumClean);
            Directory.CreateDirectory(newFolderPath);
        }

        public void CreateFolders()
        {
            foreach (var song in ListOfSongs)
            {
                if (song.Artist != null)
                    CreateArtistFolder(song);
                else if (song.Album != null)
                    CreateAlbumFolder(song, false);
            }
        }

        private void CheckForDuplicate(string songTitle)
        {
            var listOfDuplicates = ListOfSongs.Where(x => x.Name != null && x.Name.Equals(songTitle)).ToList();

            if (listOfDuplicates.Count == 1) return;

            foreach (var duplicate in listOfDuplicates.ToList())
            {
                if (!string.IsNullOrEmpty(duplicate.Artist) || listOfDuplicates.Count <= 1) continue;

                ListOfSongs.Remove(duplicate);
                listOfDuplicates.Remove(duplicate);
            }

            if (listOfDuplicates.Count > 1)
                listOfDuplicates.RemoveRange(1, listOfDuplicates.Count - 1);
        }


        public void AddSongsToFolders(int count)
        {
            foreach (var song in ListOfSongs.ToList())
            {
                if (song.FilePath == null) continue;
                if (song.Album == null) continue;

                var albumName = SterilizeString(song.Album);
                var combinedName = Path.Combine(CleanNewFolderPath, albumName);
                var combinedPath = Path.Combine(combinedName, Path.GetFileName(song.FilePath));

                CheckForDuplicate(song.Name);

                if (!File.Exists(combinedPath))
                    File.Copy(song.FilePath, combinedPath);

                HelperMethods.DrawTextProgressBar(song.SongId, count);
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
                catch (UnsupportedFormatException)
                {
                }
                catch (CorruptFileException)
                {
                }
            }
        }
    }
}