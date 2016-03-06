using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    public class Production
    {
        public float Cost { get; private set; }
        public float Progress { get; set; }
        public IProducible Result { get; private set; }
        public ProductionType ProductionType { get; private set; }
        
        public Production(IProducible result)
        {
            Result = result;
            Cost = Result.Cost;
            ProductionType = Result.ProductionType;
        }

        // Accessors
        public float GetProgress()
        {
            return Progress / Cost;
        }
    }
}
