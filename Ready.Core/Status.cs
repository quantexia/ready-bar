using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ready.Core
{
    public enum Status
    {
        None,

        Launching,
        Available,
        Reserved,

        WithUser,
        Exited,
        Killed
    }
}
