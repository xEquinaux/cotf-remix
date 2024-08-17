using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using cotf.Assets;
using cotf.Base;
using cotf.World;
using cotf.Collections.Unused;
using System.Drawing.Imaging;
using System.Diagnostics;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;
using Color = System.Drawing.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using static cotf.Base.TagCompound;
using Point = System.Drawing.Point;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace cotf
{
    public class Player : Entity
    {
        public Player()
        {
            active = true;
            width = 42;
            height = 42;
            lifeMax = 100;
            statMaxMana = 10;
            life = lifeMax;
            statMana = statMaxMana;
            name = "plr";
        }
        public static Player myPlayer => Main.myPlayer;
        public float 
            stopSpeed, 
            moveSpeed = 0.15f;
        public const float maxSpeed = 3f;
        public bool 
            controlUp,
            controlRight,
            controlDown,
            controlLeft;
        public int 
            statMana,
            statMaxMana = 10;
        public int restTicks = 0;
        public int manaRestTicks = 0;
        public const int RestInterval = 60;
        public float lightRange => lamp == null ? Sight : lamp.range;
        public const float PickupRange = 120f;
        public UI.Textbox playerData;
        public int baseDamage = 5;
        internal List<Item> inventory = new List<Item>();
        internal Item[] equipment = new Item[15];
        private System.Drawing.Point mouse => new System.Drawing.Point((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y);
        internal static UI.Textbox itemTextBox;
        public Rectangle lightBox => new Rectangle((int)(position.X - lightRange), (int)(position.Y - lightRange), width + (int)lightRange * 2, height + (int)lightRange * 2);
        public bool hasTorch() => equipment[EquipType.OffHand] != null && Main.myPlayer.equipment[EquipType.OffHand].equipped && Main.myPlayer.equipment[EquipType.OffHand].type == ItemID.Torch;
        int debug = 0;
        public Skill activeSkill = Skill.SetActive(SkillID.Melee);
        public Skill[] skill = new Skill[SkillID.Total];
        public Item Purse => equipment[EquipType.Purse];
        public Item Torch => equipment[EquipType.OffHand];
        public bool cursed;
        private Margin BaseMargin => new Margin(32);
        public Rectangle Proximity(Margin margin) => new Rectangle(box.X - margin.Left, box.Y - margin.Top, box.Width + margin.Right, box.Height + margin.Bottom);
        int travelTicks = 0;
        public int deathCounter = 0;
        public DungeonID dungeonId = DungeonID.Factory;
        bool init;

        public void Init()
        {
            //  TODO -- bypassing floor loading for the time being
            //Map.GenerateFloor(new Margin(3000));
            //goto EndMapInit;
            if (!Main.DoesMapExist($"_{dungeonId}_map", Main.FloorNum))
            {
                Map.GenerateFloor(DungeonID.Castle, new Margin(3000));
            }
            else
            {
                Main.LoadFloor(dungeonId, Main.FloorNum, false);
            }
            //EndMapInit:
            width = 42;
            height = 42;
            TextureName = "Sky_boss";
            texture = preTexture = Asset<Bitmap>.Request(TextureName);
            statMana = statMaxMana;
            defaultColor = Color.Gray;
            iFramesMax = 60;
            color = defaultColor;
            //  Need to reorient draw init positions
            if (!TagCompound.Exists(SaveType.Player, name))
            { 
                FindRandomTile();
            }
            else Load();
            if (hasTorch())
            {
                UnequipTorch(Torch);
            }
            //int item = Item.NewItem(X, Y, 32, 32, ItemID.Torch, (byte)this.whoAmI);
            //EquipTorch(Main.item[item]);
            //PickupItem(ref Main.item[item]);
            //lamp.active = false;
            //inventory[3] = item;
        }
        public void FindRandomTile(bool moveToTile = true)
        {
            Tile t = null;
            do
            {
                t = Main.tile[Main.rand.Next(Main.tile.GetLength(0) - 1), Main.rand.Next(Main.tile.GetLength(1) - 1)];
            } while (t == null || t.Active);
            if (moveToTile)
                position = new Vector2(t.X, t.Y);
        }
        public void Save(bool onExit)
        {
            using (TagCompound tag = new TagCompound(this, SaveType.Player))
            {
                tag.SaveValue("_name", name);
                tag.SaveValue(name + "_life", life);
                tag.SaveValue(name + "_lifeMax", lifeMax);
                tag.SaveValue(name + "_mana", statMana);
                tag.SaveValue(name + "_manaMax", statMaxMana);
                tag.SaveValue(name + "_position", position);
                tag.SaveValue(name + "_count", deathCounter);
                for (int i = 0; i < equipment.Length; i++)
                {
                    tag.SaveValue($"{name}_equip{i}", equipment[i]);
                }
                for (int i = 0; i < inventory.Count; i++)
                {
                    tag.SaveValue($"{name}_inv{i}", inventory[i]);
                }
            }
        }
        public void Load()
        {
            using (TagCompound tag = new TagCompound(this, SaveType.Player))
            {
                name         = tag.GetString("_name");
                life         = tag.GetInt32(name + "_life");
                lifeMax      = tag.GetInt32(name + "_lifeMax");
                statMana     = tag.GetInt32(name + "_mana");
                statMaxMana  = tag.GetInt32(name + "_manaMax");
                position     = tag.GetVector2(name + "_position");
                deathCounter = tag.GetInt32(name + "_count");
                for (int i = 0; i < equipment.Length; i++)
                {
                    var item = tag.GetItem($"{name}_equip{i}");
                    if (item != null && item.active && item.equipped)
                    {
                        item.EquipItem(this);
                    }
                }
                for (int i = 0; i < inventory.Count; i++)
                {
                    var item = tag.GetItem($"{name}_inv{i}");
                    PickupItem(ref item);
                }
            }
        }
        public override void Update()
        {
            base.Update();

            if (!init)
            { 
                if (equipment == null)
                    return;
			    //  Empty equipment slots fix
			    for (int i = 0; i < equipment.Length; i++)
			    {
                    if (equipment[i]?.active is false || equipment[i]?.equipped is false)
                    {
                        equipment[i].equipped = false;
                        equipment[i].active = false;
                        equipment[i] = null;
                    }
                }
                init = true;
            }

            //  DEBUG active skill swapping
            activeSkill = Skill.SetActive(SkillID.Melee);

            //  Stats information
            playerData = new UI.Textbox("", Vector2.Zero, new Rectangle(Main.ScreenWidth / 10 - 5, Main.ScreenHeight / 2 - 80, 0, 0), Main.ScreenWidth / 2, ButtonStyle.None, true, myPlayer.whoAmI);
            playerData.text = StatsText(life, statMana, Main.timeSpan);

            //  Purse methods
            Purse?.Update();

            //  Inventory interaction
            if (Main.open)
            {
                Point offset = new System.Drawing.Point(mouse.X - Main.myPlayer.width, mouse.Y - Main.myPlayer.height);
                if (itemTextBox == null || !itemTextBox.active)
                { 
                    for (int i = 0; i < Item.nearby.Count; i++)
                    {
                        if (Main.mouseLeft && Item.nearby[i].hitbox.Contains(offset))
                        {
                            itemTextBox = new UI.Textbox(Item.nearby[i], Item.nearby[i].texture, Item.nearby[i].ToolTip.name, Item.nearby[i].Text(), Main.InventoryCenter, (int)Main.InventoryCenter.X / 2, ButtonStyle.PickupCancel, true, whoAmI);
                            return;
                        }
                    }
                }
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (string.IsNullOrEmpty(inventory[i].name))
                    {
                        inventory.RemoveAt(i);
                        continue;
                    }
                    //  DEBUG: Fix for innactive items bug
                    else if (!inventory[i].active)
                    {
                        inventory[i].active = true;
                    }
                    inventory[i].Update(this);
                    if (i >= inventory.Count)
                        return;
                    if (Main.mouseLeft)
                    {
                        if (inventory[i].hitbox.Contains(offset))
                        {
                            itemTextBox = new UI.Textbox(inventory[i], inventory[i].texture, inventory[i].ToolTip.name, inventory[i].Text(), Main.InventoryCenter, (int)Main.InventoryCenter.X / 2, ButtonStyle.EquipDropCancel, true, whoAmI);
                            itemTextBox.text = "Damage: " + inventory[i].damage;
                            return;
                        }
                    }
                }
                for (int i = 0; i < Item.nearby.Count; i++)
                {
                    if (string.IsNullOrEmpty(Item.nearby[i].name))
                        Item.nearby.RemoveAt(i);
                }
                if (itemTextBox == null || !itemTextBox.active)
                { 
                    for (int i = 0; i < equipment.Length; i++)
                    {
                        equipment[i]?.Update(this);
                        if (equipment[i] != null && equipment[i].active && equipment[i].equipped && Main.mouseLeft && equipment[i].hitbox.Contains(offset))
                        {
                            itemTextBox = new UI.Textbox(equipment[i], equipment[i].texture, equipment[i].ToolTip.name, equipment[i].Text(), Main.InventoryCenter, (int)Main.InventoryCenter.X / 2, ButtonStyle.UnequipCancel, true, whoAmI);
                            itemTextBox.text = "Damage: " + equipment[i].damage;
                            return;
                        }
                    }
                }
                return;
            }
            foreach (Item t in Item.nearby)
            {
                Item m = t;
                QuickPickupItem(ref m);
            }

            //  Stats dynamics
            if (velocity == Vector2.Zero && KeyDown(Keys.R))
            {
                if (KeyDown(Keys.Space))
                {
                    restTicks += 10;
                }
                if (++restTicks > RestInterval) //++restTicks % RestInterval == 0)
                {
                    if (life < lifeMax)
                        life++;
                    restTicks = 0;    // = 1
                    manaRestTicks++;
                }
                if (manaRestTicks > RestInterval / 6) // manaRestTicks % (RestInterval / 6) == 0
                {
                    if (statMana < statMaxMana)
                        statMana++;
                    manaRestTicks = 0; // = 1
                }
            }
            if (velocity != Vector2.Zero)
            {
                restTicks = 1;
                manaRestTicks = 1;
            }
            if (iFrames < iFramesMax)
                iFrames++;
            
            //  Player lamp
            if (lamp != null && lamp.active)
            {
                lamp.position = Center;
            }

            box = new Rectangle(X, Y, width, height);

            for (int i = 0; i < equipment.Length; i++)
            {
                if (equipment[i] != null && equipment[i].active && equipment[i].equipped && equipment[i].owner == this.whoAmI)
                {
                    equipment[i]?.UseItem(this);
                    equipment[i]?.HoldItem(this);
                }
            }

            //  More collision checks
            foreach (Npc n in Main.npc)
            {
                if (n != null && n.active && InProximity(n, 300f))
                {
                    Collision(n);
                    //  Base attack collision damage
                    if (activeSkill.type == SkillID.None && n.Distance(Center) < 32f)
                    {
                        n.bodied =
                            controlUp    && n.Center.Y < Center.Y ||
                            controlDown  && n.Center.Y > Center.Y ||
                            controlRight && n.Center.X > Center.X ||
                            controlLeft  && n.Center.X < Center.X;
                        if (n.bodied)
                        {
                            n.velocity = Vector2.Zero;
                            if (n.iFrames == 0)
                                n.NpcHurt(baseDamage, 6f, AngleTo(n.Center));
                        }
                    }
                }
            }
            foreach (Scenery s in Main.scenery)
            {
                if (s != null && s.active && s.solid && InProximity(s, 100f))
                {
                    s.Collision(this);
                }
            }

            if (KeyUp(Keys.Space))
            {
                travelTicks = 0;
            }
            //  Staircases
            var stair = Main.staircase.FirstOrDefault(t => t != null && t.InProximity(this, Sight));
            if (travelTicks == 0 && stair != default(Staircase) && KeyDown(Keys.Space) && stair.hitbox.IntersectsWith(this.hitbox()))
            {
                travelTicks = 1;
                switch (stair.direction)
                {
                    case StaircaseDirection.None:
                        break;
                    case StaircaseDirection.LeadingUp:
                        Main.FloorTransition(stair.direction);
                        var entrance = Main.staircase.FirstOrDefault(t => t != null && t.X != 0 && t.Y != 0 && t.active && t.direction == StaircaseDirection.LeadingDown);
                        position = entrance.position;
                        break;
                    case StaircaseDirection.LeadingDown:
                        Main.FloorTransition(stair.direction);
                        var entrance2 = Main.staircase.FirstOrDefault(t => t != null && t.X != 0 && t.Y != 0 && t.active && t.direction == StaircaseDirection.LeadingUp);
                        position = entrance2.position;
                        break;
                    default:
                        break;
                }
            }

            #region movement
            //  Initializing
            stopSpeed = moveSpeed;

            //  Movement mechanic
            if (velocity != Vector2.Zero)
            {
                //  Stopping movement
                if (velocity.X > 0f && !controlRight)
                    velocity.X -= stopSpeed;
                if (velocity.X < 0f && !controlLeft)
                    velocity.X += stopSpeed;
                if (velocity.Y > 0f && !controlDown)
                    velocity.Y -= stopSpeed;
                if (velocity.Y < 0f && !controlUp)
                    velocity.Y += stopSpeed;
            }         

            //  Clamp
            if (velocity.X > maxSpeed)
                velocity.X = maxSpeed;
            if (velocity.X < -maxSpeed)
                velocity.X = -maxSpeed;
            if (velocity.Y > maxSpeed)
                velocity.Y = maxSpeed;
            if (velocity.Y < -maxSpeed)
                velocity.Y = -maxSpeed;

            //  Movement speed set
            if (velocity.X < moveSpeed && velocity.X > -moveSpeed)
                velocity.X = 0f;
            if (velocity.Y < moveSpeed && velocity.Y > -moveSpeed)
                velocity.Y = 0f;

            //  Positioning
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

            //  Controls
            if (controlRight = !KeyDown(Keys.A) && KeyDown(Keys.D))
            {
                // move right
                //Main.TimeScale = 1;
                velocity.X += moveSpeed;
            }
            if (controlLeft = KeyDown(Keys.A) && !KeyDown(Keys.D))
            {
                // move left
                //Main.TimeScale = 1;
                velocity.X -= moveSpeed;
            }
            if (controlDown = !KeyDown(Keys.W) && KeyDown(Keys.S))
            {
                // move down
                //Main.TimeScale = 1;
                velocity.Y += moveSpeed;
            }
            if (controlUp = KeyDown(Keys.W) && !KeyDown(Keys.S))
            {
                // move up
                //Main.TimeScale = 1;
                velocity.Y -= moveSpeed;
            }
            #endregion

            //  DEBUG: possible getting-stuck fix
            Tile _tile = Tile.GetSafely((int)Center.X / Tile.Size, (int)Center.Y / Tile.Size);
            if (_tile != null && _tile.Active)
            {
                FindRandomTile();
            }
            //while (Tile.GetSafely((float)X - 1, (float)Y - 1).Active)
            //{
            //    position.X++;
            //    position.Y++;
            //}
            //while (Tile.GetSafely((float)X + width + 1, (float)Y + height + 1).Active)
            //{   
            //    position.X--;
            //    position.Y--;
            //}

            //  Skill interaction
            activeSkill.Update();
            activeSkill.OnCooldown();
            if (Main.mouseLeft)
            {
                activeSkill.Cast(this);
            }
            //  DEBUG

            if (KeyDown(Keys.Q))
            {
                Item.NewItem(Center.X, Center.Y, 32, 32, ItemID.Torch);
            }
            return;
            Staircase _s = Main.staircase.FirstOrDefault(t => t != null && t.direction == StaircaseDirection.LeadingDown);
            if (_s != default && KeyDown(Keys.D1))
            {
                position = _s.position;
            }
            if (Main.mouseRight)
            {
                //position = Main.lamp[2].position + new Vector2(10, 10);
                //var debug = Main.item.First(t => t != null && t.active && t.type == ItemID.Broadsword);
                var debug = Main.staircase.LastOrDefault(t => t != null && t.active && t.direction == StaircaseDirection.LeadingDown);
                if (debug != null)
                {
                    position = debug.position;
                }
            }
        }
        public void Draw(Graphics graphics)
        {
            if (texture == null)
                return;
            //graphics.FillRectangle(Brushes.White, box);
            playerData?.Init(graphics);
            //playerData?.Draw(graphics);   //  Testing drawing this with higher priority
            //lamp?.WorldLighting();
            //color = Ext.Divide(color, lightColor);
            Drawing.LightmapHandling(texture, this, gamma, graphics);
            //Drawing.TextureLighting(Main.texture, box, this, graphics);
            //  Debug
            //graphics.DrawString(GetControlType().ToString(), Main.DefaultFont, Brushes.White, X - width / 2, Y + height + 10, StringFormat.GenericDefault);
        }
        public void EquipTorch(Item item)
        {
            lamp = item.lamp;
            if (lamp == null) return;
            item.lamp.active = true;
            item.lamp.range = myPlayer.lightRange;
            item.lamp.lampColor = Lamp.TorchLight;
            item.lamp.position = Center;
            if (!Main.lamp.Contains(item.lamp))
            {
                Lamp.AddLamp(item.lamp);
            }
        }
        public void UnequipTorch(Item item)
        {
            if (item.lamp != null)
            { 
                item.lamp.active = false;
                lamp = null;
            }
        }
        public void ApplyCurse(Item item)
        {
            cursed = item.cursed;
        }
        public void Hurt(int damage, float knockback, float angle)
        {
            CombatText.NewText(damage, this);
            life -= damage;
            iFrames = 0;
            velocity += Helper.AngleToSpeed(angle, knockBack);
            if (life <= 0)
            {
                //  TODO Does the player respawn in the Overworld
                Respawn(1);
                deathCounter++;
            }
        }
        public void Respawn(int floornum)
        {
            life = lifeMax;
            Main.LoadFloor(DungeonID.Castle, floornum, true);
        }
        public void Heal(int amount)
        {
            life = Math.Min(life + amount, lifeMax);
        }
        public Vector2 UseAngle()
        {
            int xOffset = 12;
            var plr = myPlayer.position;
            var v2 = AngleToSpeed(AngleTo(Main.MouseWorld), 30f);
            return plr + v2 + new Vector2(-myPlayer.width / 2 + xOffset, 0);
        }
        public Vector2 UseAngle(Projectile proj)
        {
            int xOffset = 12;
            var plr = myPlayer.position;
            var v2 = AngleToSpeed(AngleTo(Main.MouseWorld), 30f);
            return plr + v2 + new Vector2(-myPlayer.width / 2 + xOffset, height / 2 - proj.height / 2);
        }
        public bool ProjHit(Projectile projectile)
        {
            if (projectile != null && projectile.active && (projectile.hostile || !projectile.friendly) && projectile.hitbox.IntersectsWith(hitbox()) && projectile.InProximity(this, Math.Max(height, Math.Max(projectile.width, projectile.height)) * 2) && iFrames == iFramesMax)
            {
                Hurt(projectile.damage, projectile.knockBack, Helper.AngleTo(projectile.Center, Center));
                return true;
            }
            return false;
        }
        public bool ItemHit(Item item)
        {
            if (item != null && item.active && (item.hostile || !item.friendly) && item.hitbox.IntersectsWith(hitbox()) && item.InProximity(this, Math.Max(height, Math.Max(item.width, item.height)) * 2) && iFrames == iFramesMax)
            {
                Hurt(item.damage, item.knockBack, Helper.AngleTo(item.Center, Center));
                return true;
            }
            return false;
        }
        public bool NpcHit(Npc npc)
        {
            if (Main.TimeScale > 0 && npc != null && npc.active && npc.hostile && npc.velocity == Vector2.Zero && npc.InProximity(this, Math.Max(height, Math.Max(npc.width, npc.height)) * 2) && iFrames == iFramesMax)
            {
                Hurt(npc.damage, npc.knockBack, Helper.AngleTo(npc.Center, Center));
                return true;
            }
            return false;
        }

        private bool IsCoinOrPurse(ref Item item)
        {
            if ((!item.isCoin && item.purse == null) || item.isCoin && (myPlayer.Purse == null || !myPlayer.Purse.equipped))
                return false;
            else
            {
                //Stash p = new RUDD.Stash();
                //if (item.isCoin)
                //    p = Stash.DoConvert(item.value * (uint)item.stack);
                //  TODO: add purses to each other
                //else p += item.purse.Content;
                if (myPlayer.Purse == null || !myPlayer.Purse.active || !myPlayer.Purse.equipped)
                    return false;
                //if (myPlayer.Purse.Content == new RUDD.Stash())
                //    myPlayer.Purse.Content = p;
                //else myPlayer.Purse.Content += p;
                return true;
            }
        }
        private void ProcessPickup(ref Item item)
        {                                          
            Item clone = item.DeepClone(true);
            item.owner = whoAmI;
            if (clone.lamp != null)
            {
                clone.lamp.active = false;
                clone.lamp.owner = item.owner;
                clone.lamp.itemLink = clone;
            }
            clone.width = item.width;
            clone.height = item.height;
            clone.owner = Main.myPlayer.whoAmI;
            item.lamp?.Dispose(true);
            item.Dispose();
            inventory.Add(clone);
        }
        public bool PickupItem(ref Item item)
        {
            if (inventory.Count(t => t != null && t.active) >= 24)
                return false;
            if (item.GetStash != null && item.GetStash.content != null)
            {
                Item i = item;
                item.GetStash.content.First(t => t == i).TurnToAir();
            }
            item.loot?.pile.Remove(item);

            if (IsCoinOrPurse(ref item))
            {
                item.TurnToAir();
                return true;
            }
            
            ProcessPickup(ref item);
            item.TurnToAir();
            return true;
        }
        public void OpenInventory(bool flag)
        {
            Main.open = flag;
        }
        public string StatsText(int life, int mana, TimeSpan time)
        {
            return $"[Life] {life}/{lifeMax}, [Mana] {mana}/{statMaxMana}, [Time] " + _time(time);
        }
        private string _time(TimeSpan time)
        {
            string days = time.Days.ToString();
            string hours = time.Hours.ToString();
            string minutes = time.Minutes.ToString();
            string seconds = time.Seconds.ToString();
            if (hours.Length == 1)
                hours = "0" + hours;
            if (minutes.Length == 1)
                minutes = "0" + minutes;
            if (seconds.Length == 1)
                seconds = "0" + seconds;
            return $"{days}:{hours}:{minutes}:{seconds}";
        }

        public bool QuickPickupItem(ref Item item)
        {
            if (KeyDown(Keys.G) && Item.nearby.Count > 0)
            {
                return PickupItem(ref item);
            }
            return false;
        }
        public new Rectangle hitbox() 
        {
            if (myPlayer == null)
                return Rectangle.Empty;
            return new Rectangle((int)position.X, (int)position.Y, width, height);
        }
        public bool KeyUp(Keys key)
        {
            return Main.keyboard.IsKeyUp(key);
        }
        public bool KeyDown(Keys key)
        {
            return Main.keyboard.IsKeyDown(key);
        }
        public bool IsMoving()
        {
            return velocity != Vector2.Zero;
        }
        public bool IsControlMove()
        {
            return KeyDown(Keys.W) || KeyDown(Keys.A) || KeyDown(Keys.S) || KeyDown(Keys.D);
        }
        public ControlType GetControlType()
        {
            if (KeyDown(Keys.W))
                return ControlType.MoveUp;
            if (KeyDown(Keys.D))
                return ControlType.MoveRight;
            if (KeyDown(Keys.S))
                return ControlType.MoveDown;
            if (KeyDown(Keys.A))
                return ControlType.MoveLeft;
            return ControlType.None;
        }
        public MoveDirection GetMoveDirection()
        {
            float none = maxSpeed / 6f;
            float min = maxSpeed / 2f;
            if (GetControlType() == ControlType.None && 
                (velocity.X < none || velocity.X > -none) && 
                (velocity.Y < none || velocity.Y < -none))
            {
                return MoveDirection.None;
            }
            if (GetControlType() == ControlType.MoveUp && 
                velocity.Y < -none && velocity.X <= min && velocity.X >= min)
            {
                return MoveDirection.Left;
            }
            if (GetControlType() == ControlType.MoveRight &&
                velocity.X > none && velocity.Y < min && velocity.Y > -min)
            {
                return MoveDirection.Right;
            }
            if (GetControlType() == ControlType.MoveDown &&
                velocity.Y < -none && velocity.X < min && velocity.X > min)
            {
                return MoveDirection.Down;
            }
            if (GetControlType() == ControlType.MoveLeft && 
                velocity.X < -none && velocity.Y <= min && velocity.Y >= -min)
            {
                return MoveDirection.Left;
            }
            return MoveDirection.None;
        }
        public bool Discovered(Vector2 target)
        {
            return Distance(target) < Sight && SightLine(target, Tile.Size / 5);
        }
    }
    public enum ControlType
    {
        None,
        MoveUp,
        MoveRight,
        MoveDown,
        MoveLeft
    }
    public enum MoveDirection : int
    {
        None    = -1,
        Up      = 0,
        Right   = 1,
        Down    = 2,
        Left    = 3
    }
}
