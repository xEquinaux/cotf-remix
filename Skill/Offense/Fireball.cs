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
    public class Fireball : Skill
    {
        public override ToolTip SetToolTip()
        {
            return toolTip = new ToolTip("Fire ball", "", Color.Red);
        }
        public override void SetDefaults()
        {
            this.damage = 10;
            this.manaCost = 7;
            this.friendly = true;
            this.hostile = true;
            this.type = SkillID.FireBolt;
            this.useTime = 120;
            this.speed = 8f;
        }
        Lamp lamp;
        int ai = 0;
        public override void Update()
        {
            if (projectile != null && projectile.active)
            { 
                lamp.parent = projectile;
                this.Lighting(lamp);
            }
            else
            {
                ai = 0;
                lamp?.Dispose();
            }
        }
        public override bool PreCast(Player player)
        {
            switch (ai)
            {
                case 0:
                    lamp = Main.lamp[Lamp.NewLamp(0, 0, 40f, false, player.whoAmI)];
                    lamp.lampColor = Color.Red;
                    goto case 1;
                case 1:
                    ai = 1;
                    break;
            }
            return base.PreCast(player);
        }
        public override void Cast(Player player)
        {
            if (!PreCast(player))
                return;
            var angle = player.UseAngle();
            int proj = Projectile.NewProjectile(angle, player.AngleToSpeed(player.AngleTo(Main.MouseWorld), speed), 0f, ProjectileID.Fireball, player);
            projectile = Main.projectile[proj];
            base.Cast(player);
        }
    }
}
