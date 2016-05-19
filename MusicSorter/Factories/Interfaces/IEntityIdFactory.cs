using System.Collections.Generic;

namespace MusicSorter.Factories.Interfaces
{
    public interface IEntityIdFactory
    {
        int Create(List<Song> files);
    }
}
