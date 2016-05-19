using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

            if (!CacheXml.DoesCacheExists)
            {
                Console.WriteLine("Retrieving Song Information. . .");
                ribs.ImportSongInformation(ribs.MessyFolderPath);
                
                CacheXml.SaveData(ribs.ListOfSongs, "ListOfSongsXML");
            }
            else
            {
                Console.WriteLine("Reading From Cache. . .");
                ribs.ListOfSongs = CacheXml.RetrieveData();
            }


            Console.WriteLine("Creating Folders. . .");
            ribs.CreateFolders();

            Console.WriteLine($"Time Elapsed: {stopWatch.Elapsed} seconds");
            
            var count = ribs.ListOfSongs.Count;

            Console.WriteLine("Sorting Songs Into Folders. . . ");
            ribs.AddSongsToFolders(count);

            stopWatch.Stop();


            Console.ReadLine();
        }
    }
}