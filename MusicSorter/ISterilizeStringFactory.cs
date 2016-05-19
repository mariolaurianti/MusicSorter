using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSorter
{
    public interface ISterilizeStringFactory
    {
        string CleanString(string property);
    }
}
