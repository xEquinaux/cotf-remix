using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;
using cotf;
using cotf.Base;
using cotf.ID;
using Timer = System.Timers.Timer;

namespace cotf.World.Traps
{
    internal class CrossbowTurret : Trap
    {
        Timer timer;
        float range;
        float speed = 4f;
        int seconds = 3;
        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        protected override void Init()
        {
            name = "Crossbow Turret";
            damage = 10;
            range = 300f;
            timer = new Timer(TimeSpan.FromSeconds(seconds).TotalMilliseconds);
            timer.Elapsed += (sender, e) =>
            {
                float angle = AngleTo(Main.myPlayer.Center);
                Projectile.NewProjectile(Center, Helper.AngleToSpeed(angle, speed), angle, ProjectileID.Arrow, this);
            };
        }
        public override void Update()
        {
            if (base.PreUpdate(true))
                return;
            if (!timer.Enabled && Main.myPlayer.Distance(Center) < range)
            {
                timer.Enabled = true;
                timer.Start();
            }
            else
            {
                timer.Enabled = false;
                timer.Stop();
            }
        }
        public override void Draw(Graphics graphics)
        {
            if (base.PreUpdate(true))
                return;
            color = Ext.Transparency(Ext.Divide(color, lightColor), 0.5f);
            Drawing.TextureLighting(preTexture, hitbox, ref color, defaultColor, ref alpha, graphics, colorTransform);
        }
    }
}
