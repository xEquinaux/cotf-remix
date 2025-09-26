using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf.Base;
using cotf.World;
using cotf.Collections;

namespace cotf.ID
{
    public sealed class TrapID
    {
        public sealed class Sets
        {
            public const int Total = 11;
            public static bool[] Damaging 
            {
                get
                {
                    bool[] result = new bool[Total];
                    for (int i = 0; i < Total; i++)
                    {
                        switch (i)
                        {
                            case 2:
                            case 4:
                            case 6:
                            case 7:
                            case 9: 
                            case 10:
                                result[i] = true;
                                break;
                        }
                    }
                    return result;
                }
            }
            public static bool[] IsTurret
            { 
                get
                {
                    bool[] result = new bool[Total];
                    for (int i = 0; i < Total; i++)
                    {
                        switch (i)
                        {
                            case 4:
                            case 7:
                            case 10:
                                result[i] = true;
                                break;
                        }
                    }
                    return result;
                }
            }
            public static bool[] Effect
            {
                get
                {
                    bool[] result = new bool[Total];
                    for (int i = 0; i < Total; i++)
                    {
                        switch (i)
                        {
                            case 1:
                            case 3:
                            case 5:
                            case 8:
                                result[i] = true;
                                break;
                        }
                    }
                    return result;
                }
            }
        }
        public const short
            None = 0,
            Trapdoor = 1,
            Spikes = 2,
            Teleport = 3,
            CrossbowTurret = 4,
            WoodenCage = 5,
            RockFall = 6,
            FlameGeyser = 7,
            FogMachine = 8,
            AcidPatch = 9,
            MagicTurret = 10;
    }
    public sealed class DebuffID
    {
        public const short
            None = 0,
            Fire = 1,
            Poison = 2;
    }
}
