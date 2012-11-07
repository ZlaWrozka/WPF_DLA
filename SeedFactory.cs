using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLA
{
    abstract class SeedFactory
    {
        public abstract Seed GetSeed(SeedKind seedKind);
    }
}
