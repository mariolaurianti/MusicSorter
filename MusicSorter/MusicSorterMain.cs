using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static System.Console;
using static MusicSorter.CacheXml;

namespace MusicSorter
{
    public static class MusicSorterMain
    {
        private static void Main()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            const string fromPath = @"D:\Music\";
            const string toPath = @"D:\Sorted Music 3";

            var ribs = new ActionJackson
            {
                MessyFolderPath = Path.GetFullPath(fromPath),
                CleanNewFolderPath = Path.GetFullPath(toPath),
                ListOfSongs = new List<Song>()
            };

            if (!DoesCacheExists)
            {
                WriteLine("Retrieving Song Information. . .");
                ribs.ImportSongInformation(ribs.MessyFolderPath);
                
                SaveData(ribs.ListOfSongs, "ListOfSongsXML");
            }
            else
            {
                if (IsCacheInSync)
                {
                    WriteLine("Reading From Cache. . .");
                    ribs.ListOfSongs = LoadCachedData();
                }
                else
                {
                    UpdateCache();
                    ribs.ListOfSongs = LoadCachedData();
                }
            }

            WriteLine("Creating Folders. . .");
            ribs.CreateFolders();

            WriteLine($"Time Elapsed: {stopWatch.Elapsed} seconds");
            
            var count = ribs.ListOfSongs.Count;

            WriteLine("Sorting Songs Into Folders. . . ");
            ribs.AddSongsToFolders(count);

            stopWatch.Stop();


            ReadLine();
        }
    }
}