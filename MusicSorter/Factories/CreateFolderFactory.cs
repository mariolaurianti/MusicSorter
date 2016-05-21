using System.IO;
using MusicSorter.Factories.Interfaces;

namespace MusicSorter.Factories
{
    public class CreateFolderFactory : ICreateFolderFactory
    {
        public void CreateFolder(Song song, string newDestinationPath)
        {
            if (song.HasArtist)
                CreateArtistFolder(song, newDestinationPath);
            else if (!string.IsNullOrEmpty(song.Album))
                CreateAlbumFolder(song, newDestinationPath);
        }

        private static void CreateArtistFolder(Song song, string newDestinationPath)
        {
            var newFolderPath = Path.Combine(newDestinationPath, song.Artist);

            Directory.CreateDirectory(newFolderPath);

            if (!string.IsNullOrEmpty(song.Album))
                CreateAlbumFolder(song, newDestinationPath);
        }

        private static void CreateAlbumFolder(Song song, string newDestinationPath)
        {
            var newFolderPath = song.HasArtist
                ? Path.Combine($@"{newDestinationPath}\", $@"{song.Artist}\", song.Album)
                : Path.Combine($@"{newDestinationPath}\Unknown Artist", song.Album);

            Directory.CreateDirectory(newFolderPath);
        }
    }
}