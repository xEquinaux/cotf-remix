using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cotf.Base
{
    public struct Margin
    {
        public int Top, Right, Bottom, Left;
        public Margin(int size)
        {
            Top = Right = Bottom = Left = size;
        }
    }
}
