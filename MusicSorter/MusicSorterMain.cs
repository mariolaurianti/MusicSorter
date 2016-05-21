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
            Bootstrapper.With.Ninject().Start();
            var kernel = (IKernel)Bootstrapper.Container;

            var sterilizeStringFactory = kernel.Get<ISterilizeStringFactory>();
            var entityIdFactory = kernel.Get<IEntityIdFactory>();
            var createFolderFactory = kernel.Get<ICreateFolderFactory>();

            const string fromPath = @"D:\Music\";
            const string toPath = @"D:\Sorted Music 3";

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            var ribs = new ActionJackson(
                sterilizeStringFactory, 
                entityIdFactory,
                createFolderFactory)
            {
                MessyFolderPath = Path.GetFullPath(fromPath),
                NewFolderDestinationPath = Path.GetFullPath(toPath),
                ListOfSongs = new List<Song>()
            };
            
            //DeleteCache();

            if (!CacheXml.DoesCacheExists)
            {
                Console.WriteLine("Retrieving Song Information...");
                ribs.ImportSongInformation(ribs.MessyFolderPath);

                CacheXml.SaveToCache(ribs);
            }
            else
            {
                if(!CacheXml.IsCacheInSync)
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