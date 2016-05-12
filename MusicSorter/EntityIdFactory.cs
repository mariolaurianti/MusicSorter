using System.Collections.Generic;
using System.Linq;

namespace MusicSorter
{
    public class EntityIdFactory
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
