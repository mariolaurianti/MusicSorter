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
            if (string.IsNullOrEmpty(property)) return string.Empty;

            var regex = new Regex("[^a-zA-Z0-9 ]").Replace(property, string.Empty).Trim();
            regex = regex.Replace(".", string.Empty);
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(regex);
        }

        private void CreateArtistFolder(Song song)
        {
            var newFolderPath = Path.Combine(CleanNewFolderPath, song.Artist);

            Directory.CreateDirectory(newFolderPath);

            if (!string.IsNullOrEmpty(song.Album))
                CreateAlbumFolder(song, song.HasArtist, song.Artist);
        }

        private void CreateAlbumFolder(Song song, bool fromArtist, string artistClean = null)
        {
            var newFolderPath = fromArtist
                ? Path.Combine($@"{CleanNewFolderPath}\", $@"{artistClean}\", song.Album)
                : Path.Combine($@"{CleanNewFolderPath}\Unknown Artist", song.Album);

            Directory.CreateDirectory(newFolderPath);
        }

        public void CreateFolders()
        {
            foreach (var song in ListOfSongs)
            {
                if (song.HasArtist)
                    CreateArtistFolder(song);
                else if (!string.IsNullOrEmpty(song.Album))
                    CreateAlbumFolder(song, false);
            }
        }

        private void CheckForDuplicate(string songTitle)
        {
            var listOfDuplicates =
                ListOfSongs.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Equals(songTitle)).ToList();

            if (listOfDuplicates.Count.Equals(1)) return;

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
            const string unknownArtist = "Unknown Artist";
            foreach (var song in ListOfSongs.ToList())
            {
                if (string.IsNullOrEmpty(song.FilePath)) continue;
                if (string.IsNullOrEmpty(song.Album)) continue;

                var albumName = SterilizeString(song.Album);
                var combinedName = song.HasArtist
                    ? Path.Combine(CleanNewFolderPath, song.Artist, albumName)
                    : Path.Combine(CleanNewFolderPath, unknownArtist, albumName);

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
                var hasLength = Directory.GetDirectories(Path.GetFullPath(folder)).Length.Equals(0);
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
                        Album = SterilizeString(fileInformation.Tag.Album),
                        Artist = SterilizeString(fileInformation.Tag.FirstAlbumArtist),
                        Name = SterilizeString(fileInformation.Tag.Title),
                        FilePath = Path.GetFullPath(file),
                        HasArtist = !string.IsNullOrEmpty(fileInformation.Tag.FirstAlbumArtist)
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