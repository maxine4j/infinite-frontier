using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    public enum ProductionType
    {
        Unit,
        Building
    }

    public interface IProducible
    {
        ProductionType ProductionType { get; set; }
        int Cost { get; set; }
        string Name { get; set; }
    }
}
