using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MusicSorter.Factories.Interfaces;
using TagLib;
using File = System.IO.File;

namespace MusicSorter
{
    public class ActionJackson
    {
        public List<Song> ListOfSongs { get; set; }
        public string MessyFolderPath { get; set; }
        public string NewFolderDestinationPath { private get; set; }
        private readonly IEntityIdFactory _entityIdFactory;
        private readonly ISterilizeStringFactory _sterilizeStringFactory;
        private readonly ICreateFolderFactory _createFolderFactory;

        public ActionJackson(
            ISterilizeStringFactory sterilizeStringFactory, 
            IEntityIdFactory entityIdFactory, 
            ICreateFolderFactory createFolderFactory)
        {
            _sterilizeStringFactory = sterilizeStringFactory;
            _entityIdFactory = entityIdFactory;
            _createFolderFactory = createFolderFactory;
        }

        public void CreateFolders()
        {
            foreach (var song in ListOfSongs)
            {
                _createFolderFactory.CreateFolder(song, NewFolderDestinationPath);
            }
        }

        private void CheckForDuplicate(string songTitle)
        {
            var listOfDuplicates = ListOfSongs.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Equals(songTitle)).ToList();

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

                var combinedPath = ConstructNewPath(song, unknownArtist);

                CheckForDuplicate(song.Name);
                CopyFileToFolder(combinedPath, song);

                HelperMethods.DrawTextProgressBar(song.SongId, count);
            }
        }

        private string ConstructNewPath(Song song, string unknownArtist)
        {
            var combinedName = song.HasArtist
                ? Path.Combine(NewFolderDestinationPath, song.Artist, song.Album)
                : Path.Combine(NewFolderDestinationPath, unknownArtist, song.Album);

            return song.FilePath != null ? Path.Combine(combinedName, Path.GetFileName(song.FilePath)) : null;
        }

        private static void CopyFileToFolder(string combinedPath, Song song)
        {
            if (!File.Exists(combinedPath))
                File.Copy(song.FilePath, combinedPath);
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
            foreach (var file in Directory.GetFiles(folder))
            {
                try
                {
                    var fileInformation = TagLib.File.Create(file);
                    if(fileInformation.PossiblyCorrupt) continue;

                    var songInformation = new Song
                    {
                        SongId = _entityIdFactory.Create(ListOfSongs),
                        Album = _sterilizeStringFactory.CleanString(fileInformation.Tag.Album),
                        Artist = _sterilizeStringFactory.CleanString(fileInformation.Tag.FirstAlbumArtist),
                        Name = _sterilizeStringFactory.CleanString(fileInformation.Tag.Title),
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