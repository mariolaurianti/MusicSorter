using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSorter
{
    public interface IEntityIdFactory
    {
        int Create(List<Song> files);
    }
}
