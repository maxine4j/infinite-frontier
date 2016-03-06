using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    public interface IOwnable
    {
        Empire Owner { get; set; }
    }
}
