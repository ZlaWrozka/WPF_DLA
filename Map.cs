using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DLA
{
    internal class Map
    {
        // particles colors
        internal const byte BLACK = 0;
        internal const byte GREEN = 255;

        private byte[,,] map;
        private readonly int xSize;
        private readonly int ySize;
        private Random mySeedRandomizer = new Random();
        private Random myParticleVector = new Random();
        private short myDensity;
        

        // properties
        private int ParticleSize { get; set; }
        internal int X
        {
            get { return xSize; }
        }
        internal int Y
        {
            get { return ySize; }
        }
        internal static long Step { get; set; }
        internal short Density
        {
            set
            {
                if (value > 0 && value <= 50)
                {
                    myDensity = value;
                }
            }
        }
        internal int SeedParticlesNumber { get; set; }

        // constructors
        internal Map() { }
        internal Map(int width, int height)
        {
            xSize = width;
            ySize = height;
            CreateMap();
        }

        // creates new map
        private void CreateMap()
        {
            this.map = new byte[2, this.xSize, this.ySize];
        }

        // sets initial green tree seed depending on chosen type of seed
        internal void SetTreeSeed(SeedKind seedKind)
        {
            ConcreteSeedFactory seedFactory = new ConcreteSeedFactory();
            Seed seed = seedFactory.GetSeed(seedKind);
            seed.MapSizeX = xSize;
            seed.MapSizeY = ySize;
            seed.XPos = xSize/2;
            seed.YPos = ySize/2;
            if (seedKind == SeedKind.CircleSeed)
            {
                CircleSeed circleSeed = seed as CircleSeed;
                if (circleSeed != null)
                    circleSeed.Radius = (int)(xSize / 5);
            }
            seed.Draw(ref this.map);
        }

        // generates blue particles
        internal void GenerateSeed()
        {
            for (int i = 0; i < this.xSize; ++i)
            {
                for (int j = 0; j < this.ySize; ++j)
                {
                    int number = mySeedRandomizer.Next();
                    if (number % this.myDensity == 0 && this.map[0, i, j] != GREEN)
                    {
                        ++this.map[0, i, j];
                        ++SeedParticlesNumber;   
                    }

                }
            }
        }

        // decides on new positions of blue particles
        // implements particles stacking - on one posiotion there can be more than one particle
        internal void MoveParticles()
        {
            byte c = (byte)(Step % 2);          // current map
            byte n = (byte)((Step + 1) % 2);    // new map

            for (int i = 0; i < this.xSize; ++i)            
            {
                for (int j = 0; j < this.ySize; ++j)
                {
                    // copy green particle to the new map
                    if (this.map[c, i, j] == GREEN)
                        this.map[n, i, j] = GREEN;

                    // if a particle is blue, calculate it's new position
                    if (this.map[c, i, j] > BLACK && this.map[c, i, j] < GREEN)
                    {
                        int newX;
                        int newY;

                        for (int k = 0; k < this.map[c, i, j]; ++k )
                        {
                            do
                            {
                                // randomly choose vector of movement
                                int vx = myParticleVector.Next(-1, 2);
                                int vy = myParticleVector.Next(-1, 2);
                                newX = i + vx;
                                newY = j + vy;

                            } while (newX < 0 || newX >= this.xSize || newY < 0 || newY >= this.ySize || this.map[c, newX, newY] == GREEN);

                            bool isGreen = WillBecomeGreen(newX, newY);
                            // if new particle is going to be green
                            if (isGreen)
                            {
                                // set it green on the new map
                                this.map[n, newX, newY] = GREEN;
                                // update particle counter
                                --SeedParticlesNumber;
                            }
                            // if stays blue
                            else
                            {
                                // add blue particle to the stack on new position
                                ++this.map[n, newX, newY];
                            }
                        }
                    }
                    // reset old particle
                    if (this.map[c, i, j] != GREEN)
                    {
                        this.map[c, i, j] = BLACK;
                    }
                }
            }
        }
        

        // checks if a blue particle has a green neighbour
        private bool WillBecomeGreen(int x, int y)
        {
            byte c = (byte)(Step % 2);

            if ((x - 1 >= 0 && this.map[c, x - 1, y] == GREEN)
            || (x + 1 < this.xSize && this.map[c, x + 1, y] == GREEN)
            || (y - 1 >= 0 && this.map[c, x, y - 1] == GREEN)
            || (y + 1 < this.ySize && this.map[c, x, y + 1] == GREEN)
                // corners
            || ((x - 1 >= 0 && y - 1 >= 0) && this.map[c, x - 1, y - 1] == GREEN)
            || ((x - 1 >= 0 && y + 1 < this.ySize) && this.map[c, x - 1, y + 1] == GREEN)
            || ((x + 1 < this.xSize && y - 1 >= 0) && this.map[c, x + 1, y - 1] == GREEN)
            || ((x + 1 < this.xSize && y + 1 < this.ySize) && this.map[c, x + 1, y + 1] == GREEN))
            {
                return true;
            }
            return false;
        }

        // indexer
        public byte this[int k, int x, int y]
        {
            get { return (byte)map[k, x, y]; }
            set { map[k, x, y] = value; }
        }
    }
}
