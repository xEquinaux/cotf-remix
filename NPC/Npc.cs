using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using cotf.Assets;
using cotf.Base;
using cotf.World;
using Microsoft.Xna.Framework;

namespace cotf
{
    public class Npc : Entity
    {
        public Lamp light;
        public const int sightTileRange = 8;
        public bool inHallway = false;
        public bool idle = false;
        public bool bodied = false;
        public IdleBehavior behavior = IdleBehavior.None;
        public float speed = 1f;
        protected byte moveType;
        int ticks2 = 0, ticks3 = 0;
        bool flag;
        Direction direction;
        public List<Npc> collideObj = new List<Npc>();
        public Npc()
        {
            SetDefaults();
        }
        public virtual void SetDefaults()
        {
            TextureName = $"npc{type}";
            texture = preTexture = Asset<Bitmap>.Request(TextureName);
        }
        protected virtual void Init()
        {
            switch (type)
            {
                case NpcType.Kobold:
                    //texture = Main.texture;
                    break;
                default:
                    break;
            }
            life = lifeMax;
        }
        public bool PreUpdate()
        {
            Background bg = Background.GetSafely((int)Center.X / Tile.Size, (int)Center.Y / Tile.Size);
            if (bg != null && bg.active)
            {
                inShadow = !bg.lit;
            }   
            else inShadow = false;
            discovered = Distance(Main.myPlayer.Center) < Sight || (NpcSight(Main.myPlayer) && !inShadow);
            return true;
        }
        public virtual void AI()
        {
            if (!active)
                return;

            //  Player hit check
            Main.myPlayer.NpcHit(this);

            //  Collision checks
            collideObj.Clear();
            foreach (Tile t in Main.tile)
            {
                if (t.Active && InProximity(t, 300f))
                {
                    t.Collision(this);
                }
            }
            foreach (Npc n in Main.npc)
            {
                if (n != null && n != this && n.active && InProximity(n, 100f))
                {
                    if (n.Collision(this))
                        collideObj.Add(n);
                }
            }
            foreach (WorldObject obj in Main.wldObject)
            {
                if (obj != null && obj.active && InProximity(obj, 100f))
                {
                    obj.Collision(this);
                }
            }
            foreach (Scenery s in Main.scenery)
            {
                if (s != null && s.active && s.solid && InProximity(s, 100f))
                {
                    s.Collision(this);
                }
            }
            Collision(Main.myPlayer);

            //  Collision reaction
            if (velocity != Vector2.Zero)
            {
                if (!colLeft && velocity.X < 0f)
                    position.X += velocity.X;
                if (!colRight && velocity.X > 0f)
                    position.X += velocity.X;
                if (!colUp && velocity.Y < 0f)
                    position.Y += velocity.Y;
                if (!colDown & velocity.Y > 0f)
                    position.Y += velocity.Y;
            }

            //  Idling behaviors
            IdleActions();
        }
        public void RemoteAI(Floor floor)
        {
            if (!active)
                return;

            //  Collision checks
            collideObj.Clear();
            foreach (Tile t in floor.tile)
            {
                if (t.Active && InProximity(t, 300f))
                {
                    t.Collision(this);
                }
            }
            foreach (Npc n in floor.npc)
            {
                if (n != null && n != this && n.active && InProximity(n, 100f))
                {
                    if (n.Collision(this))
                        collideObj.Add(n);
                }
            }
            
            //  Collision reaction
            if (velocity != Vector2.Zero)
            {
                if (!colLeft && velocity.X < 0f)
                    position.X += velocity.X;
                if (!colRight && velocity.X > 0f)
                    position.X += velocity.X;
                if (!colUp && velocity.Y < 0f)
                    position.Y += velocity.Y;
                if (!colDown & velocity.Y > 0f)
                    position.Y += velocity.Y;
            }

            //  Idling behaviors
            IdleActions();
        }
        public virtual void Draw(Graphics graphics)
        {
            if (active && discovered)
            {
                this.PostFX();
                if (iFrames > 0)
                {
                    iFrames--;
                    color = color.Transparency(iFrames % 4 == 0 ? 1f : 0f);
                }
                Drawing.LightmapHandling(texture, this, gamma, graphics);
            }
        }
        public virtual bool NpcItemHit(Item item)
        {
            return Main.TimeScale > 0 && iFrames == 0 && item.inUse && item.equipped && item.type != ItemID.Torch && item.friendly && item.InProximity(this, 100f) && item.hurtbox.IntersectsWith(hitbox);
        }
        public virtual bool NpcProjHit(Projectile projectile)
        {
            return Main.TimeScale > 0 && iFrames == 0 && projectile.InProximity(this, 100f) && projectile.hitbox.IntersectsWith(hitbox) && projectile.friendly;
        }
        public virtual void NpcHurt(int damage, float knockBack, float angle)
        {
            CombatText.NewText(damage, this);
            this.iFrames = iFramesMax;
            this.life -= damage;
            velocity += Helper.AngleToSpeed(angle, knockBack);
            if (life <= 0)
                Dispose();
        }
        public virtual void Loot()
        {
        }
        public virtual void IdleActions()
        {
            if (!idle && ticks2++ % 180 == 0 && Main.rand.NextFloat() < 0.33f)
            {
                idle = true;
            }
            if (idle)
            { 
                if (behavior == IdleBehavior.None)
                {
                    bool rand = Main.rand.NextBool();
                    switch (rand)
                    {
                        case true:
                            behavior = IdleBehavior.MoveAtRandom;
                            break;
                        case false:
                            behavior = IdleBehavior.ChangeRooms;
                            break;
                    }
                }
            }

            if (!idle)
                return;

            if (Main.rand.NextFloat() < 0.01f)
                idle = false;

            switch (behavior)
            {
                case IdleBehavior.None:
                    goto default;
                case IdleBehavior.MoveAtRandom:
                    if (ticks3++ % 180 == 0)
                        moveType = (byte)Main.rand.Next(5);
                    if (moveType != 0)
                        velocity = Helper.AngleToSpeed((float)Math.PI / 4f * moveType, speed);
                    else velocity = Vector2.Zero;
                    break;
                case IdleBehavior.ChangeRooms:
                    if (!flag)
                    {
                        var list = Main.door.ToList();
                        list.RemoveAll(t => t == null);
                        var door = list.OrderBy(t => t.Distance(Center)).FirstOrDefault();
                        if (door != default(Door))
                        {
                            velocity = Helper.AngleToSpeed(AngleTo(door.Center), speed);
                            if (door.hitbox.Contains(new System.Drawing.Point((int)Center.X, (int)Center.Y)))
                            {
                                direction = door.direction;
                                flag = true;
                            }
                        }
                    }
                    else
                    {
                        int n = Tile.Size / 2;
                        bool top = !Tile.GetSafely(Center.X, Center.Y - n).Active;
                        bool right = !Tile.GetSafely(Center.X + n, Center.Y).Active;
                        bool bottom = !Tile.GetSafely(Center.X, Center.Y + n).Active;
                        bool left = !Tile.GetSafely(Center.X - n, Center.Y).Active;
                        if (!top && !right && !bottom && !left)
                        {
                            direction = Direction.None;
                            behavior = IdleBehavior.None;
                            flag = false;
                            idle = false;
                        }
                        switch (direction)
                        {
                            case Direction.Top:
                                if (top)
                                    velocity = Helper.AngleToSpeed((float)Math.PI, speed);
                                if (right)
                                    velocity = Helper.AngleToSpeed((float)Math.PI / 4f, speed);
                                else if (left)
                                    velocity = Helper.AngleToSpeed((float)Math.PI / 1.5f, speed);
                                break;
                            case Direction.Right:
                                if (right)
                                    velocity = Helper.AngleToSpeed((float)Math.PI / 4f, speed);
                                else if (top)
                                    velocity = Helper.AngleToSpeed((float)Math.PI, speed);
                                else if (bottom)
                                    velocity = Helper.AngleToSpeed((float)Math.PI / 2f, speed);
                                break;
                            case Direction.Bottom:
                                if (bottom)
                                    velocity = Helper.AngleToSpeed((float)Math.PI / 2f, speed);
                                if (right)
                                    velocity = Helper.AngleToSpeed((float)Math.PI / 4f, speed);
                                else if (left)
                                    velocity = Helper.AngleToSpeed((float)Math.PI / 1.5f, speed);
                                break;
                            case Direction.Left:
                                if (left)
                                    velocity = Helper.AngleToSpeed((float)Math.PI / 1.5f, speed);
                                else if (top)
                                    velocity = Helper.AngleToSpeed((float)Math.PI, speed);
                                else if (bottom)
                                    velocity = Helper.AngleToSpeed((float)Math.PI / 2f, speed);
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public static int NewNPC(float x, float y, short type)
        {
            int num = Main.npc.Length - 1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i] == null || !Main.npc[i].active)
                {
                    num = i;
                    break;
                }
            }
            switch (type)
            {
                case NpcType.None:
                    goto default;
                case NpcType.Kobold:
                    Main.npc[num] = new Kobold();
                    break;
                default:
                    Main.npc[num] = new Npc();
                    break;
            }
            Main.npc[num].active = true;
            Main.npc[num].position = new Vector2(x, y);
            Main.npc[num].type = type;
            Main.npc[num].whoAmI = num;
            Main.npc[num].Init();
            Main.npc[num].SetDefaults();
            return num;
        }
        public bool NpcSight(Entity target)
        {
            for (int n = 0; n < Distance(target.Center); n += Tile.Size / 5)
            {
                var v2 = Center + Helper.AngleToSpeed(Helper.AngleTo(Center, target.Center), n);
                if (Tile.GetSafely((int)v2.X / Tile.Size, (int)v2.Y / Tile.Size).Active)
                {
                    return false;
                }
            }
            return true;
        }
        public bool Colliding()
        {
            return collide || colUp || colRight || colDown || colLeft;
        }
        public override void Dispose()
        {
            if (Main.npc[whoAmI] != null)
            { 
                Main.npc[whoAmI].active = false;
                Main.npc[whoAmI].position = Vector2.Zero;
                Main.npc[whoAmI].light?.Dispose();
                Main.npc[whoAmI] = null;
            }
        }
    }
    public sealed class NpcType
    {
        public const short
            None = 0,
            Kobold = 1;
    }
    public enum IdleBehavior
    {
        None,
        MoveAtRandom,
        ChangeRooms
    }
}
