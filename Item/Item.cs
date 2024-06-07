using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf.Base;
using cotf.World;
using cotf.Collections;
using cotf.Assets;
using ToolTip = cotf.Base.ToolTip;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace cotf
{
    public class Item : Entity
    {
        #region variables
        public bool channel;
        public bool autoReuse;
        public bool isCoin = false;
        public bool identified = false;
        public short
            useTime,
            useSpeed;
        public const short MaxTicks = 60;
        public byte 
            useStyle,
            holdStyle;
        public bool inUse;
        internal float 
            usingAngle, 
            useAngle;
        protected float 
            startAngle,
            endAngle,
            angle,
            mouseAngle;
        internal bool flag = false;
        internal bool equipped = false;
        internal bool justDropped = false;
        internal Vector2 oldPosition;
        public const int DrawSize = 48;
        public const int PileDistance = 150;
        public int equipType;
        public int stack;
        public uint value;
        public override int X => (int)position.X;
        public override int Y => (int)position.Y;
        protected new Color color;
        private Vector2 inventoryPosition;
        public new Rectangle hitbox;
        public Rectangle hurtbox;
        public override Vector2 Center => new Vector2(position.X + width / 2, position.Y + height / 2);
        public new Image texture = Main.texture90;
        public static List<Item> nearby = new List<Item>();
        public virtual ToolTip ToolTip => new ToolTip();
        protected virtual Color rarity => Color.White;
        public Loot loot;
        public Loot worldItem;    
        public static Stash stash;
        public Stash GetStash => Main.stash.FirstOrDefault(t => t != null && t.active && t.content.Contains(this));
        public bool enchanted, cursed;
        public ToolTip toolTip { get; protected set; }
        public bool
            inPile = false,
            inStash = false;
        public Prefix prefix;
        public Suffix suffix;
        public float weight;
        public override string ToString()
        {
            return $"Name:{name}, X:{X}, Y:{Y}, Active:{active}, Index:{whoAmI}";
        }
        #endregion
        public Item()
        {
            SetToolTip();
            color = defaultColor;
        }
        public virtual ToolTip SetToolTip()
            => toolTip = new ToolTip(name, text, color);
        public virtual void SetDefaults()
        {
            texture = Asset<Image>.Request(TextureName);
            preTexture = (Bitmap)texture;
        }
        protected virtual void Init()
        {
            ticks = MaxTicks;
        }
        public static void RollStatus(Item item)
        {
            int rand = Main.rand.Next(4);
            switch (rand)
            {
                default:
                case 0:     // rolls nothing
                    break;
                case 1:
                    item.Cursed(true);
                    break;
                case 2:
                    item.Enchanted(true);
                    break;
               case 3:
                    item.Enchanted(true);
                    break;
            }
        }
        public bool Enchanted(bool flag)
        {
            if (flag)
            {
                if (Main.rand.NextBool(2))
                {
                    suffix = new Suffix();
                    suffix.Apply(suffix.RollTrait(true));
                    borderColor = Color.LightSkyBlue;
                    cursed = false;
                }
                if (Main.rand.NextBool(2))
                {
                    prefix = new Prefix();
                    prefix.Apply(prefix.RollTrait(true));
                    borderColor = Color.LightSkyBlue;
                    cursed = false;
                }
            }
            return enchanted = flag;
        }
        public bool Cursed(bool flag)
        {
            if (flag)
            {
                if (Main.rand.NextBool(2))
                { 
                    suffix = new Suffix();
                    suffix.Apply(suffix.RollTrait(false));
                    borderColor = Color.Purple;
                    enchanted = false;
                }
                if (Main.rand.NextBool(2))
                {
                    prefix = new Prefix();
                    prefix.Apply(prefix.RollTrait(false));
                    borderColor = Color.Purple;
                    enchanted = false;
                }
            }
            return cursed = flag;
        }
        public virtual void OnEquip(Player player)
        {
            if (!this.identified)
            {
                RollStatus(this);
                IdentifyItem(this);
            }
        }
        public static void IdentifyItem(Item item)
        {
            item.identified = true;
        }
        public bool EquipItem(Player myPlayer)
        {
            if (myPlayer.equipment[equipType] == null || !myPlayer.equipment[equipType].active || !myPlayer.equipment[equipType].equipped)
            {
                OnEquip(myPlayer);
                myPlayer.equipment[equipType] = this;
                myPlayer.equipment[equipType].equipped = true;
                myPlayer.equipment[equipType].active = true;
                myPlayer.equipment[equipType].owner = myPlayer.whoAmI;
                switch (type)
                {
                    case ItemID.Torch:
                        myPlayer.EquipTorch(this);
                        break;
                    case ItemID.Purse:
                        break;
                    default:
                        break;
                }
                switch (equipType)
                {
                    case EquipType.MainHand:
                        break;
                    case EquipType.OffHand:
                        break;
                    default:
                        break;
                }
                return true;
            }
            return false;
        }
        public bool UnequipItem(Player myPlayer, out Item drop)
        {
            drop = myPlayer.equipment[equipType];
            if (myPlayer.equipment[equipType] == null || !myPlayer.equipment[equipType].active || !myPlayer.equipment[equipType].equipped)
                return false; 
            myPlayer.equipment[equipType].equipped = false;
            myPlayer.equipment[equipType].owner = myPlayer.whoAmI;
            switch (type)
            {
                case ItemID.Torch:
                    myPlayer.UnequipTorch(this);
                    break;
                case ItemID.Purse:
                    //drop = ((Purse)myPlayer.equipment[equipType]).DeepClone(true);
                    //((Purse)myPlayer.equipment[equipType]).Content = new RUDD.Stash();
                    break;
                default:
                    break;
            }
            return true;
        }
        public static void DropTorch(Player player, Item item)
        {
            if (item.type == ItemID.Torch)
            {
                if (item.lamp == null)
                {
                    //item.lamp = Main.lamp[Lamp.NewLamp(item.X, item.Y, 150f)];
                }
                item.lamp.active = true;
                item.lamp.owner = 255;
                item.lamp.range = 150f;
                item.lamp.lampColor = Lamp.TorchLight;
                item.lamp.position = player.Center;
                if (!Main.lamp.Contains(item.lamp))
                {
                    Lamp.AddLamp(item.lamp);
                }
            }
        }
        public virtual bool UseItem(Player myPlayer)
        {
            if (owner == 255)
                return false;
            if (Main.mouseLeft && !inUse)
            {
                mouseAngle = AngleTo(Main.myPlayer.Center, Main.MouseWorld);
                startAngle = mouseAngle - Helper.ToRadian(45f);
                endAngle = EndAngle(startAngle);
                ticks = 0;
                inUse = true;
            }
            if (inUse)
            {
                switch (useStyle)
                { 
                    case UseStyle.Swing:
                        if (startAngle < endAngle)
                        {
                            int xOffset = 12;
                            var plr = myPlayer.position;
                            var v2 = AngleToSpeed(startAngle += 0.2f, 30f);
                            position = plr + v2 + new Vector2(-myPlayer.width / 2 + xOffset, 0);
                            hurtbox = new Rectangle((int)position.X, (int)position.Y, width, height);
                        }
                        else
                        { 
                            useAngle = 0f;
                            usingAngle = 0f;
                            inUse = false;
                        }
                        break;
                    case UseStyle.Stab:
                        if (ticks++ < 15)
                        { 
                            var plr = myPlayer.position;
                            var v2 = AngleToSpeed(mouseAngle, ticks * 5f);
                            position = plr + v2 + new Vector2(-myPlayer.width / 2 + 9, 0);
                            hurtbox = new Rectangle((int)position.X, (int)position.Y, width, height);
                        }
                        else
                        {
                            ticks = 0;
                            inUse = false;
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
        public virtual void HoldItem(Player myPlayer)
        {
            if (owner == 255)
                return;
            switch (holdStyle)
            {
                case HoldStyle.None:
                    goto default;
                case HoldStyle.HoldOut:
                    inUse = true;
                    var plr = myPlayer.position;
                    var v2 = AngleToSpeed(angle = AngleTo(myPlayer.Center, Main.MouseWorld), 30f);
                    position = plr + v2 + new Vector2(-myPlayer.width / 1.5f + myPlayer.width / 2 + 2, -myPlayer.height / 3f + myPlayer.height / 2 - 6);
                    box = new Rectangle((int)position.X, (int)position.Y, width, height);
                    break;
                default:
                    break;
            }
        }
        public virtual void Update(Player myPlayer)
        {
            bool update = false;
            foreach (Npc n in Main.npc)
            {
                if (n == null || owner == 255 || !n.InProximity(this, 200f))
                    continue;
                if (n.NpcItemHit(this))
                    n.NpcHurt(damage, knockBack, Helper.AngleTo(n.Center, Center));
            }
            myPlayer.ItemHit(this);
            if (update = !Main.open && !equipped && owner == 255)
            {
                if (flag)
                {
                    position = oldPosition;
                    flag = false;
                }
                if (!discovered)
                    discovered = Discovered(myPlayer);
                hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
            }
            else 
            { 
                int width = 640;
                int height = 480;
                int index = 0;
                int x = Main.ScreenWidth / 2 - width / 2 - (int)Main.ScreenX;
                int y = Main.ScreenHeight / 2 - height / 2 - (int)Main.ScreenY;
                Rectangle bound = new Rectangle(x, y, width, height);
                Rectangle inventory = new Rectangle(x + 20, y + 20, width / 2 - 20, height - 40);
                for (int i = 0; i < myPlayer.equipment.Length; i++)
                {
                    Rectangle box;
                    int X = x + 25;
                    int Y = y + 25 + (Item.DrawSize + 4) * i;
                    int newX = X;
                    int newY = Y;
                    if (Y > y + height - 80)
                    { 
                        newX = inventory.Right - Item.DrawSize - 4;
                        index = -8;
                        newY = y + 25 + (Item.DrawSize + 4) * (i + index);
                    }
                    box = new Rectangle(newX, newY, Item.DrawSize, Item.DrawSize);
                    if (myPlayer.equipment[i] != null && myPlayer.equipment[i].active && myPlayer.equipment[i].equipped && myPlayer.equipment[i].owner == myPlayer.whoAmI)
                    {
                        int w = (int)Helper.RatioConvert(Helper.Ratio(Item.DrawSize, myPlayer.equipment[i].texture.Width), myPlayer.equipment[i].texture.Width);
                        Rectangle slot = new Rectangle(box.X + 1, box.Y + 1, Item.DrawSize - 1, w - 1);
                        myPlayer.equipment[i].hitbox = slot;
                    }
                }
            }
            WorldUpdate(true);
        }
        public virtual void WorldUpdate(bool update)
        {
            if (!update)
                return;

            var pile = Main.item.Where(t => t != null && !t.equipped && !t.inStash && t.owner == 255 && t.Distance(Center) < PileDistance);
            if (pile.Count() > 3)
            {
                if (loot == null)
                {
                    int avgX = hitbox.X;
                    int avgY = hitbox.Y;
                    loot = Loot.NewPile(avgX, avgY, pile.ToArray());
                }
                inPile = true;
                return;
            }
            else if (loot != null)
            {
                loot.Dispose();
                inPile = false;
            }
            return;
            if (!inPile && worldItem == null)
            {
                worldItem = Loot.NewLoot(this);
            }
            if (worldItem != null && worldItem.item != null && (worldItem.item.owner != 255 || !worldItem.item.equipped || !worldItem.item.active))
                worldItem.Dispose();
        }
        public virtual void Draw(Graphics graphics)
        {
            if ((active && owner == 255) || inUse)
            {
                if (equipped)
                {
                    if (holdStyle == HoldStyle.HoldOut)
                        Drawing.DrawRotate(texture, box, new Rectangle(0, 0, width, height), Helper.ToDegrees(angle) + 135f, new PointF(width / 2, height / 2), defaultColor, Color.Black, RotateType.GraphicsTransform, graphics);
                    else if (useStyle == UseStyle.Stab)
                        Drawing.DrawRotate(texture, hurtbox, new Rectangle(0, 0, width, height), Helper.ToDegrees(AngleTo(Main.myPlayer.Center, Center)) + 135f, new PointF(width / 2, height / 2), defaultColor, Color.Black, RotateType.MatrixTransform, graphics);
                    else if (inUse) 
                        Drawing.DrawRotate(texture, hurtbox, new Rectangle(0, 0, width, height), Helper.ToDegrees(AngleTo(Main.myPlayer.Center, Center)) + 135f, new PointF(width / 2, height / 2), defaultColor, Color.Black, RotateType.MatrixTransform, graphics);
                }
                else if (discovered && !inPile && !inStash)
                {
                    if (preTexture != null)
                    { 
                        Drawing.LightmapHandling(preTexture, this, gamma, graphics);
                    }
                    else
                    {
                        Drawing.LightmapHandling(texture, this, gamma, graphics);
                    }
                }
            }
        }
        
        //  Use designating procedural stash contents
        public static Item[] FillStash(int x, int y, int length)
        {
            Item[] item = new Item[Math.Min(30, length)];
            for (int i = 0; i < item.Length; i++)
            {
                int index = NewItem(x, y, 32, 32, (short)(Main.rand.Next(12) + 1));
                item[i] = Main.item[index];
                item[i].owner = 255;
            }
            return item;
        }
        public bool IsTorch()
        {
            return type == ItemID.Torch;
        }
        public static int NewItem(float x, float y, int width, int height, short type, byte owner = 255, uint value = 0, int stack = 1)
        {
            int num = Main.item.Length - 1;
            for (int i = 0; i < Main.item.Length; i++)
            {
                if (Main.item[i] == null || !Main.item[i].active)
                {
                    num = i;
                    break;
                }
            }
            switch (type)
            { 
                case ItemID.None:
                    goto default;
                case ItemID.Broadsword:
                    Main.item[num] = new Broadsword();
                    break;
                case ItemID.Torch:
                    Main.item[num] = new Torch();
                    Main.item[num].lamp = Main.lamp[Lamp.NewLamp((int)x, (int)y, 200f, false, owner)];
                    Main.item[num].lamp.itemLink = Main.item[num];
                    break;
                case ItemID.Spear:
                    Main.item[num] = new Spear();
                    break;
                case ItemID.Purse:
                    if (value == 0)
                    {
                        //value = (uint)Main.rand.Next(100, RUDD.Stash.GoldCoin);
                    }
                    Main.item[num] = new Purse(value);
                    Main.item[num].value = value;
                    break;
                case ItemID.IronCoin:
                    Main.item[num] = new IronCoin();
                    Main.item[num].stack = Main.rand.Next(100);
                    break;
                case ItemID.CopperCoin:
                    Main.item[num] = new CopperCoin();
                    Main.item[num].stack = Main.rand.Next(100);
                    break;
                case ItemID.SilverCoin:
                    Main.item[num] = new SilverCoin();
                    Main.item[num].stack = Main.rand.Next(100);
                    break;
                case ItemID.GoldCoin:
                    Main.item[num] = new GoldCoin();
                    Main.item[num].stack = Main.rand.Next(100);
                    break;
                case ItemID.PlatinumCoin:
                    Main.item[num] = new PlatinumCoin();
                    Main.item[num].stack = Main.rand.Next(100);
                    break;
                case ItemID.Scroll:
                    Main.item[num] = new Scroll();
                    break;
                case ItemID.Wand:
                    Main.item[num] = new Wand();
                    break;
                case ItemID.Potion:
                    Main.item[num] = new Potion();
                    break;
                default:
                    Main.item[num] = new Item();
                    break;
            }
            Main.item[num].TextureName = $"item{0}";
            Main.item[num].SetDefaults();
            Main.item[num].active = true;
            Main.item[num].position = new Vector2(x, y);
            Main.item[num].width = width;
            Main.item[num].height = height;
            Main.item[num].type = type;
            Main.item[num].whoAmI = num;
            Main.item[num].owner = owner;
            Main.item[num].Init();
            return num;
        }
        
        public static int Drop(ref Item item, Vector2 position)
        {
            //  Either modify drop to clone or use ref
            //int i = Item.NewItem(position.X, position.Y, item.width, item.height, item.type, 255, item.value, item.stack);
            //Item drop = Main.item[i];
            item.justDropped = true;
            item.discovered = true;
            item.position = position;
            item.oldPosition = position;
            item.owner = 255;

            DropTorch(Main.myPlayer, item);
            nearby.Add(item);
            return item.whoAmI;
        }
        internal void DrawInventory(int x, int y, bool onScreen, Graphics graphics)
        {
            //  Draw Inventory Items
            if (!active)
                return;
            inventoryPosition = new Vector2(x, y);
            if (onScreen)
            {
                if (texture != null)
                {
                    //  TODO: sort out relative dimensions
                    float ratio = (float)width / height;
                    hitbox = new Rectangle(x + DrawSize / 2 - width / 2, y, width, (int)(ratio * height));
                    Drawing.DrawTexture(texture, hitbox, hitbox.Width, hitbox.Height, graphics, Drawing.SetColor(defaultColor));
                }
            }
        }
        private float EndAngle(float start)
        {
            return start + Helper.ToRadian(90f);
        }
        internal void TurnToAir()
        {
            Main.item[whoAmI] = this;
            if (Main.item[whoAmI] != null)
            { 
                Main.item[whoAmI].active = false;
                if (Main.item[whoAmI].lamp != null)
                    Main.item[whoAmI].lamp.Dispose();
                if (Main.item[whoAmI].loot != null)
                    Main.item[whoAmI].loot.Dispose();
                if (Main.item[whoAmI].worldItem != null)
                    Main.item[whoAmI].worldItem.Dispose();
                Main.item[whoAmI] = null;
            }
        }
        public Item DeepClone(bool active)
        {
            int i = 255;
            Item item = default;
            if (purse != null && purse.active)
            {
                i = Item.NewItem(X, Y, width, height, type, (byte)owner, 0/*purse.Content.TotalCopper()*/);
                item = Main.item[i];
                item.purse = purse;
                item.purse.active = active;
            }
            else
            {
                i = Item.NewItem(X, Y, width, height, type, (byte)owner);
                item = Main.item[i];
                item.lamp?.Dispose();
            }
            if (lamp != null)
            {
                int index = Lamp.NewLamp(lamp.X, lamp.Y, lamp.range, lamp.staticLamp, lamp.owner);
                item.lamp = Main.lamp[index];
                item.lamp.active = active;
            }
            item.active = active;
            item.text = text;
            item.SetDefaults();
            item.SetToolTip();
            item.Init();
            return item;
        }
        public void Dispose(bool check = false)
        {
            if (check && Main.item[whoAmI].owner < 255)
                return;
            Main.item[whoAmI].active = false;
            Main.item[whoAmI].position = Vector2.Zero;
            Main.item[whoAmI].lamp?.Dispose();
            Main.item[whoAmI] = null;
        }
    }
    public sealed class HoldStyle
    {
        public const byte
            None = 0,
            HoldOut = 1;
    }
    public sealed class UseStyle
    {
        public const byte
            None = 0,
            Swing = 1,
            Stab = 2,
            Slash = 3;
    }
    public sealed class ItemID
    {
        public sealed class Sets
        {
            public static bool[] Hurts
            {
                get
                {
                    bool[] result = new bool[Total];
                    for (int i = 0; i < Total; i++)
                    {
                        switch (i)
                        {
                            case 1:
                            case 3:
                            case 13:
                            case 14:
                            case 15:
                            case 16:
                            case 17:
                            case 18:
                                result[i] = true;
                                break;
                        }
                    }
                    return result;
                }
            }
            public static bool[] isSword
            {
                get
                {
                    bool[] result = new bool[Total];
                    for (int i = 0; i < Total; i++)
                    {
                        switch (i)
                        {
                            case 1:
                            case 16:
                            case 17:
                            case 18:
                                result[i] = true;
                                break;
                        }
                    }
                    return result;
                }
            }
            public static bool[] Utility
            {
                get
                {
                    bool[] result = new bool[Total];
                    for (int i = 0; i < Total; i++)
                    {
                        switch (i)
                        {
                            case 2:
                            case 4:
                            case 12:
                            case 19:
                                result[i] = true;
                                break;
                        }
                    }
                    return result;
                }
            }
            public static bool[] Ability
            {
                get
                {
                    bool[] result = new bool[Total];
                    for (int i = 0; i < Total; i++)
                    {
                        switch (i)
                        {
                            case 10:
                            case 11:
                            case 12:
                                result[i] = true;
                                break;
                        }
                    }
                    return result;
                }
            }
            public static bool[] isCoin
            {
                get
                {
                    bool[] result = new bool[Total];
                    for (int i = 0; i < Total; i++)
                    {
                        switch (i)
                        {
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                                result[i] = true;
                                break;
                        }
                    }
                    return result;
                }
            }
            public static bool[] Armor
            {
                get
                {
                    bool[] result = new bool[Total];
                    for (int i = 0; i < Total; i++)
                    {
                        switch (i)
                        {
                            case 19:
                            case 20:
                            case 21:
                            case 22:
                            case 23:
                            case 24:
                            case 25:
                            case 26:
                            case 27:
                            //   Extra? |
                            //          v
                            case 28:
                            case 29:
                            case 30:
                            case 31:
                            case 32:
                                result[i] = true;
                                break;
                        }
                    }
                    return result;
                }
            }
        }

        public const int
            None = 0,
            Broadsword = 1,  
            Torch = 2,
            Spear = 3,
            Purse = 4,
            IronCoin = 5,
            CopperCoin = 6,
            SilverCoin = 7,
            GoldCoin = 8,
            PlatinumCoin = 9,
            Scroll = 10,
            Wand = 11,
            Potion = 12,
            Bow = 13,        // fast
            CrossBow = -13,  // slow
            Mace = 14,       // swing motion effect
            Club = 15,       // forward effect
            Dagger = 16,     // quick motion effect
            Longsword = 17,  // bigger swing
            Shortsword = 18, // fast
                             // | Armor
            Belt = 19,       // v
            BodyArmor = 20,  // Mail
            Gauntlets = 21,  // Gloves
            Cape = 22,
            Helm = 23,       // Coif
            Boots = 24,
            Ring1 = 25,
            Ring2 = 26,
            Necklace = 27, 
            Shirt = -28,      //  | Extra? 
            Coif = -29,       //  v
            Cap = -30,
            Leggings = -31,
            Wrists = -32;
        public const byte 
            Total = 33;
    }
    public sealed class EquipType
    {
        public const byte
            MainHand = 0,
            OffHand = 1,
            Helm = 2,
            Torso = 3,
            Greaves = 4,
            Boots = 5,
            Cloak = 6,
            Pack = 7,
            Purse = 8,
            Necklace = 9,
            Ring1 = 10,
            Ring2 = 11,
            Gauntlets = 12,
            Belt = 13,
            Bracers = 14,
            Wrists = 15;
    }
}
