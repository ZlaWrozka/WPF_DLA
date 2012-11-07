using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DLA
{
    abstract class Seed
    {
        protected const byte GREEN = 255;
        protected int myX;
        protected int myY;
        protected int mapSizeX;
        protected int mapSizeY;

        internal int XPos
        {
            get
            {
                return myX;
            }
            set
            {
                if (value >= 0 && value < mapSizeX)
                    myX = value;
            }
        }
        internal int YPos
        {
            get
            {
                return myY;
            }
            set
            {
                if (value >= 0 && value < mapSizeY)
                    myY = value;
            }
        }
        internal int MapSizeX
        {
            set { mapSizeX = value; }
        }
        internal int MapSizeY
        {
            set { mapSizeY = value; }
        }

        internal abstract void Draw(ref byte[, ,] p);


    }


}
