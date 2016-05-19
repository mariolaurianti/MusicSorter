using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Bootstrap;
using Bootstrap.Ninject;
using MusicSorter.Factories.Interfaces;
using Ninject;
using static System.Console;
using static MusicSorter.CacheXml;

namespace MusicSorter
{
    public static class MusicSorterMain
    {
        private static void Main()
        {
            Bootstrapper.With.Ninject().Start();
            var kernel = (IKernel)Bootstrapper.Container;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            const string fromPath = @"D:\Music\";
            const string toPath = @"D:\Sorted Music 3";

            var ribs = new ActionJackson(kernel.Get<ISterilizeStringFactory>(), kernel.Get<IEntityIdFactory>())
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