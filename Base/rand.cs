using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cotf
{
    public class rand  : Random
    {
        private Random random;
        public rand()
        {
             random = new Random((int)DateTime.Now.Millisecond);
        }
        public override int Next(int max)
        {
            return random.Next(Math.Max(1, max));
        }
        public override int Next(int min, int max)
        {
            if (min >= max)
            {
                min *= max / min;
            }
            return random.Next(min, max);
        }
        public bool NextBool()
        {
            return random.Next(2) == 1;
        }
        public bool NextBool(int chance)
        {
            return random.Next(chance) == 1;
        }
        public float NextFloat()
        {
            return (float)random.NextDouble();
        }
    }
}
