using System.Collections.Generic;
using System.Linq;
using MusicSorter.Factories.Interfaces;

namespace MusicSorter.Factories
{
    public class EntityIdFactory : IEntityIdFactory
    {
        public int Create(List<Song> files)
        {
            if (files.Count > 0)
            {
                return files.Max(x => x.SongId) + 1;
            }
            return 0;
        }
    }
}
