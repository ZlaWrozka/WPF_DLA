using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DLA
{
    enum SeedKind { PointSeed, LineSeed, CircleSeed, CustomSeed }

    class LineSeed : Seed
    {
        internal override void Draw(ref byte[, ,] p)
        {
            for (int i = 0; i < mapSizeX; ++i)
            {
                p[0, i, (int)mapSizeY / 2] = GREEN;
                p[1, i, (int)mapSizeY / 2] = GREEN;
            }
        }
    }

    class PointSeed : Seed
    {
        internal override void Draw(ref byte[, ,] p)
        {
            p[0, XPos, YPos] = Seed.GREEN;
            p[1, XPos, YPos] = Seed.GREEN;
        }
    }

    class CircleSeed : Seed
    {
        private int myRadius = 30;
        internal int Radius
        {
            set
            {
                myRadius = value;
            }
        }

        // Bresenham Algorithm of full circle
        internal override void Draw(ref byte[, ,] p)
        {
            int f = 1 - myRadius;
            int ddF_x = 1;
            int ddF_y = -2 * myRadius;
            int x = 0;
            int y = myRadius;

            p[0, XPos, YPos + myRadius] = Seed.GREEN;
            p[0, XPos, YPos - myRadius] = Seed.GREEN;
            p[0, XPos + myRadius, YPos] = Seed.GREEN;
            p[0, XPos - myRadius, YPos] = Seed.GREEN;

            while (x < y)
            {
                // ddF_x == 2 * x + 1;
                // ddF_y == -2 * y;
                // f == x*x + y*y - radius*radius + 2*x - y + 1;
                if (f >= 0)
                {
                    y--;
                    ddF_y += 2;
                    f += ddF_y;
                }
                x++;
                ddF_x += 2;
                f += ddF_x;
                p[0, XPos + x, YPos + y] = Seed.GREEN;
                p[0, XPos - x, YPos + y] = Seed.GREEN;
                p[0, XPos + x, YPos - y] = Seed.GREEN;
                p[0, XPos - x, YPos - y] = Seed.GREEN;
                p[0, XPos + y, YPos + x] = Seed.GREEN;
                p[0, XPos - y, YPos + x] = Seed.GREEN;
                p[0, XPos + y, YPos - x] = Seed.GREEN;
                p[0, XPos - y, YPos - x] = Seed.GREEN;
            }
            // copy green seed
            for (int i = 0; i < mapSizeX; ++i)
            {
                for (int j = 0; j < mapSizeY; ++j)
                {
                    p[1, i, j] = p[0, i, j];
                }
            }
        }
    }
}
