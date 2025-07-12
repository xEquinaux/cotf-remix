using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cotf
{
    public class Default
    {
        public static Player Base => _Base(0.15f, 0.15f, 100, 20, 5);
        private static Player _Base(float stopSpeed, float moveSpeed, int statLifeMax, int statManaMax, int baseDamage)
        {
            return new Player()
            {
                stopSpeed = stopSpeed,
                moveSpeed = moveSpeed,
                lifeMax = statLifeMax,
                statMaxMana = statManaMax,
                baseDamage = baseDamage
            };
        }
    }
}
