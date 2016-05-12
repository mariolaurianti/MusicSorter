using System;
using System.Collections.Generic;
using System.IO;

namespace MusicSorter
{
    public static class MusicSorterMain
    {
        private static void Main()
        {
            const string fromPath = @"D:\Music\";
            const string toPath = @"D:\Sorted Music 2";

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

                Console.WriteLine("Done.");

                CacheXml.SaveData(ribs.ListOfSongs, "ListOfSongsXML");
            }
            else
            {
                Console.WriteLine("Reading From Cache. . .");
                ribs.ListOfSongs = CacheXml.RetrieveData();
                Console.WriteLine("Done.");
            }


            Console.WriteLine("Creating Folders. . .");
            ribs.CreateFolders();
            Console.WriteLine("Done.");

            Console.WriteLine("Sorting Songs Into Folders. . . ");
            ribs.AddSongsToFolders();
            Console.WriteLine("Done.");

            Console.ReadLine();
        }
    }
}