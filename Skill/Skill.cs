using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf.Base;
using cotf.World;
using System.Drawing.Imaging;
using ToolTip = cotf.Base.ToolTip;

namespace cotf
{
    public class Skill
    {
        public Condition condition;
        public ToolTip toolTip;
        public Projectile projectile;
        public int type = SkillID.None;
        public int manaCost = 0;
        public int damage;
        public int useTime;
        public bool hostile;
        public bool friendly;
        public bool defense;
        public bool offense;
        public float speed;
        private int useTicks = 0;
        public Skill()
        {
            Initialize();
        }
        public static Skill None 
            => new Skill() 
            {
                damage = 0,
                friendly = false,
                hostile = false,
                manaCost = 0,
                type = SkillID.None,
                toolTip = new ToolTip("None", "", Color.White)
            };
        public virtual ToolTip SetToolTip() 
            => toolTip = new ToolTip();
        protected void Initialize()
        {
            SetDefaults();
            SetToolTip();
        }
        public virtual void SetDefaults()
        {
        }
        public virtual void OnCooldown()
        {
            if (useTicks > 0)
                useTicks--;
        }
        public virtual void Cast(Player player)
        {
            useTicks = useTime;
            player.statMana -= manaCost;
        }
        public virtual bool PreCast(Player player)
        {
            return player.statMana >= manaCost && useTicks == 0;
        }
        public virtual void Lighting(Lamp lamp)
        {
            lamp.Update();
        }
        public virtual void Update()
        {
        }
        public static Skill SetActive(short type)
        {
            switch (type)
            {
                default:
                case SkillID.None:
                    return Skill.None;
                case SkillID.Light:
                    return new Light();
                case SkillID.FireBolt:
                    return new Firebolt();
                case SkillID.FireBall:
                    return new Fireball();
                case SkillID.MinorHeal:
                    return new MinorHeal();
            }
        }
    }
    public sealed class SkillID
    {
        public const int Total = 6;
        public const short
            None = 0,
            Light = 1,
            FireBolt = 2,
            FireBall = 3,
            MinorHeal = 4,
            Melee = 5;
    }
}
