using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Bootstrap;
using Bootstrap.Ninject;
using MusicSorter.Factories.Interfaces;
using Ninject;

namespace MusicSorter
{
    public static class MusicSorterMain
    {
        private static void Main()
        {
            var musicFolderPath = GetUserFolderPath(FolderType.Music_Folder);
            var newDestinationFolderPath = GetUserFolderPath(FolderType.New_Destination_Folder);

            Program_Start(musicFolderPath, newDestinationFolderPath);

            //const string fromPath = @"D:\Music\";
            //const string toPath = @"D:\Sorted Music 3";

            //Program_Start(fromPath, toPath);
        }

        private static void VerifyPath(string folderPath, FolderType folderType)
        {
            Console.WriteLine($"Is {folderPath} the correct path? [yes/no]");
            var answer = Console.ReadLine();

            if (answer == null) return;

            if (!answer.Equals("yes") || answer.Equals("Yes") || answer.Equals("YES"))
                GetUserFolderPath(folderType);
        }


        private static string GetUserFolderPath(FolderType folderType)
        {
            Console.Write($"Enter path to [ {folderType} ] folder: ");
            var musicFolderPath = Console.ReadLine();

            VerifyPath(musicFolderPath, folderType);

            return musicFolderPath;
        }

        private static void Program_Start(string musicFolderPath, string newDestinationFolderPath)
        {
            Bootstrapper.With.Ninject().Start();
            var kernel = (IKernel) Bootstrapper.Container;

            var sterilizeStringFactory = kernel.Get<ISterilizeStringFactory>();
            var entityIdFactory = kernel.Get<IEntityIdFactory>();
            var createFolderFactory = kernel.Get<ICreateFolderFactory>();
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var ribs = new ActionJackson(
                sterilizeStringFactory,
                entityIdFactory,
                createFolderFactory)
            {
                MessyFolderPath = Path.GetFullPath(musicFolderPath),
                NewFolderDestinationPath = Path.GetFullPath(newDestinationFolderPath),
                ListOfSongs = new List<Song>()
            };

            CacheXml.DeleteCache();

            if (!CacheXml.DoesCacheExists)
            {
                Console.WriteLine("Retrieving Song Information...");
                ribs.ImportSongInformation(ribs.MessyFolderPath);
                CacheXml.SaveToCache(ribs);
            }
            else
            {
                if (!CacheXml.IsCacheInSync)
                    CacheXml.UpdateCache();

                Console.WriteLine("Reading From Cache...");
                ribs.ListOfSongs = CacheXml.LoadCachedData();
            }

            Console.WriteLine("Creating Folders...");
            ribs.CreateFolders();

            Console.WriteLine($"Time Elapsed: {stopWatch.Elapsed} seconds");

            var count = ribs.ListOfSongs.Count;

            Console.WriteLine("Sorting Songs Into Folders...");
            ribs.AddSongsToFolders(count);

            stopWatch.Stop();

            Console.ReadLine();
        }
    }
}