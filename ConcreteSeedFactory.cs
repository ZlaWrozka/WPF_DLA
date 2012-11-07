using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLA
{
    class ConcreteSeedFactory : SeedFactory
    {
        public override Seed GetSeed(SeedKind seedKind)
        {
            switch (seedKind)
            {
                case SeedKind.PointSeed:
                    {
                        return new PointSeed();
                    }
                case SeedKind.LineSeed:
                    {
                        return new LineSeed();
                    }
                case SeedKind.CircleSeed:
                    {
                        return new CircleSeed();
                    }
                default:
                    {
                        return new PointSeed();
                    }
            }
        }
    }
}
