using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using cotf;
using cotf.Base;
using cotf.ID;

namespace cotf.World.Traps
{
    public class FlameGeyser : Trap
    {
        double[] angle;
        double _angle;
        public FlameGeyser()
        {
        }
        public override void SetDefaults()
        {
            name = "Flame Geyser";
            damage = 1;
            base.SetDefaults();
        }
        protected override void Init()
        {
            angle = Helper.GetAngle(4);
            base.Init();
        }
        public override void Update()
        {
            if (!base.PreUpdate(true))
                return;
            //rotation = Main.timeSpan.Milliseconds / 1000f * (float)Math.PI;//Helper.NormalizedRadius(Main.GlobalTime, Main.GlobalTime) * (float)(Math.PI * 2);
            //time = Main.timeSpan.TotalMilliseconds % 10000 / 10000f * Math.PI * 2f;

            _angle += Math.PI / 360d * Main.TimeScale;
            if (_angle >= Math.PI)
            {
                _angle *= -1;
            }
        }
        public override void Draw(Graphics graphics)
        {
            if (!base.PreUpdate(true))
                return;
            for (int i = 0; i < angle.Length; i++)
            {
                var v2 = Helper.AngleToSpeed((float)(angle[i] + _angle), 300f);
                var result = Center + v2;
                Entity.DrawChain(Main.chainTexture[0], this, result, graphics, 300, true);
            }
            
            Drawing.DrawRotate(texture, hitbox, Helper.ToDegrees((float)_angle), new PointF(width / 2, height / 2), color, RotateType.GraphicsTransform, graphics);
        }
    }
}
