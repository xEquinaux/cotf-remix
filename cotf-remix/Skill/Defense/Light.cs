using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf.Base;
using cotf.World;
using System.Drawing.Imaging;
using System.Drawing;
using ToolTip = cotf.Base.ToolTip;

namespace cotf
{
    public class Light : Skill
    {
        public override ToolTip SetToolTip()
        {
            return toolTip = new ToolTip("Light", "", Color.White);
        }
        public override void SetDefaults()
        {
            hostile = false;
            friendly = true; 
            manaCost = 5;
            damage = 0;
            type = SkillID.Light;
            useTime = 10;
        }
        public override void Cast(Player player)
        {
            if (!PreCast(player))
                return;
            bool flag = false;
            foreach (Room room in Main.room.Values)
            {
                if (room.bounds.Contains(Main.MouseWorld) && Entity.SightLine(room.bounds.Center(), player, Tile.Size / 4))
                {
                    for (int i = room.bounds.Left; i < room.bounds.Left + room.bounds.Width; i += Tile.Size / 2)
                    {
                        for (int j = room.bounds.Top; j < room.bounds.Top + room.bounds.Height; j += Tile.Size / 2)
                        {
                            Background bg = Background.GetSafely(i / Tile.Size, j / Tile.Size);
                            if (bg != null && bg.active && !bg.lit)
                            {
                                bg.lit = true;
                                flag = true;
                            }
                        }
                    }
                }
            }
            if (flag)
            {
                player.statMana -= manaCost;
                flag = false;
            }
        }
    }
}
