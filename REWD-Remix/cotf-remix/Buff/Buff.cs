using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf.Base;
using cotf.ID;

namespace cotf.Buff
{
    public struct Debuff
    {
        private Debuff(short type, int damage, int frames, bool hostile, Color oldColor = default)
        {
            this.type = type;
            this.frames = frames;
            this.maxFrames = frames;
            this.friendly = !hostile;
            this.hostile = hostile;
            this.damage = damage;
            this.oldColor = oldColor;
        }
        public static Debuff NewDebuff(short type, int time)
        {
            int frames = 0;
            int damage = 0;
            bool hostile = false;
            switch (type)
            {
                case DebuffID.None:
                    break;
                case DebuffID.Fire:
                    damage = 5;
                    frames = 180;
                    hostile = true;
                    break;
                case DebuffID.Poison:
                    damage = 1;
                    frames = time;
                    hostile = true;
                    break;
            }
            return new Debuff(type, damage, frames, hostile);
        }
        public void Update(Entity ent)
        {
            if (frames == maxFrames)
            {
                oldColor = ent.color;
            }
            if (frames-- <= 0)
            {
                ent.color = oldColor;
                ent.RemoveBuff();
            }
            switch (type)
            {
                case DebuffID.None:
                    break;
                case DebuffID.Fire:
                    if (!hostile) break;
                    ent.color = Color.OrangeRed;
                    if (frames % 30 == 0)
                    {
                        if (ent.GetType() == typeof(Player))
                        {
                            ((Player)ent).Hurt(damage, 0, 0);
                        }
                        if (ent.GetType() == typeof(Npc))
                        {
                            ((Npc)ent).NpcHurt(damage, 0, 0);
                        }
                    }
                    break;
            }
        }

        public int damage;
        public int frames;
        private int maxFrames;
        public bool friendly;
        public bool hostile;
        public short type;
        public Color oldColor;
    }
}
