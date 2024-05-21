using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rectangle = System.Drawing.Rectangle;

namespace cotf.World
{
    internal class Slope
    {
        int x, y, width, height;
        public void s(Player player, int rise, int run)
        {
            Vector2 corner = new Vector2(player.hitbox().Right, player.hitbox().Bottom);
            Vector2 origin = new Vector2(x, y);
            Rectangle tile = new Rectangle(x, y, width, height);
            int slope = height / width;
            if (tile.Contains((int)corner.X, (int)corner.Y))
            {
                player.position.Y = slope; 
            }
        }
    }
}
