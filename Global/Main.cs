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
using cotf.ID;
using cotf.World;
using cotf.World.Traps;
using cotf.Collections;
using System.Diagnostics;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;
using tUserInterface.Extension;
using Helper = cotf.Base.Helper;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Cryptography;
using System.Runtime.Versioning;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing.Imaging;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using Bitmap = System.Drawing.Bitmap;
using System.Reflection.Metadata;

namespace cotf
{
    internal class Main : Game
    {
        internal static Main Instance;
        #region EVENTS
        internal Main()
        {
            Instance = this;
            UI.Button.ButtonClickEvent += Button_ButtonClickEvent;
            return; //  Events might mess with multithreading
            Game.InitializeEvent += Program_InitializeEvent;
            Game.MainMenuEvent += Program_MainMenuEvent;
            Game.LoadResourcesEvent += Program_LoadResourcesEvent;
            Game.PreDrawEvent += Program_PreDrawEvent;
            Game.DrawEvent += Program_DrawEvent;
            Game.UpdateEvent += Program_UpdateEvent;
            Game.CameraEvent += Program_CameraEvent;
        }

        private void Button_ButtonClickEvent(object sender, UI.ButtonEventArgs e)
        {
            HandleDialog((UI.Button)sender, e.option, e.parent, e.active);
        }

        private void Program_CameraEvent(object sender, CameraArgs e)
        {
            if (camera1 == null || mainMenu)
                return;
            Camera(camera1);
            if (camera1.follow && camera1.isMoving)
            {
                ScreenX = (int)-camera1.position.X + ScreenWidth / 2;
                ScreenY = (int)-camera1.position.Y + ScreenHeight / 2;
            }
            e.graphics.RenderingOrigin = new System.Drawing.Point((int)camera1.position.X, (int)camera1.position.Y);
            e.graphics.TranslateTransform(
                ScreenX,
                ScreenY,
                MatrixOrder.Append);
            camera1.oldPosition = camera1.position;
        }

        private void Program_UpdateEvent(object sender, UpdateArgs e)
        {
        }

        private void Program_DrawEvent(object sender, DrawingArgs e)
        {
            //Game.dispatcher.Invoke(() => 
            //{ 
            //    
            //});
            //MainWindow.Instance.Buffer.Source = game.wpfSurface;
            
        }

        private void Program_PreDrawEvent(object sender, PreDrawArgs e)
        {
            Graphics = e.graphics;
        }

        private void Program_LoadResourcesEvent(object sender, EventArgs e)
        {
            //LoadResources();
        }

        private void Program_MainMenuEvent(object sender, DrawingArgs e)
        {
            MainMenu(e.graphics);
        }

        private void Program_InitializeEvent(object sender, InitializeArgs e)
        {
            //surface = e.form;
            Initialize();
        }
        #endregion
        //private System.Windows.Forms.Form surface;
        public static KeyboardState keyboard;
        public static MouseState mouse;
        internal static Image
            texture,
            texture90,
            bg,
            pixel,
            fow,
            fow50,
            square,
            grass,
            ground,
            wall,
            bg0;
        public static Image[] trapTexture = new Image[TrapID.Sets.Total];
        public static Image[] chainTexture = new Image[1];
        public static Image[] wallTexture = new Image[4];
        //public static KeyStates EscState = KeyStates.None;
        public static Camera camera1 = new Camera();
        public static Vector2 MouseWorld;
        public static Vector2 MouseScreen;
        public static Vector2 ScreenCenter => new Vector2(ScreenWidth / 2, ScreenHeight / 2);
        public static Vector2 InventoryCenter => new Vector2(640 / 2, 480 / 2);
        public static System.Drawing.Point Zero => new System.Drawing.Point(-ScreenX - (int)(camera1.position.X - ScreenWidth / 2), -ScreenY - (int)(camera1.position.Y - ScreenHeight / 2));
        public static System.Drawing.Point WorldZero => new System.Drawing.Point(-ScreenX - (int)(camera1.position.X - ScreenWidth / 2) + -ScreenX, -ScreenY - (int)(camera1.position.Y - ScreenHeight / 2) + -ScreenY);
        internal static Thumbnail thumbnail;
        internal static Worldgen worldgen;
        internal static bool
            mouseLeft,
            mouseRight,
            pressO;
        internal static Player[] player = new Player[256];
        internal static Player myPlayer;
        internal static UI.Scroll[] scroll = new UI.Scroll[2];
        internal static UI.Textbox[] textbox = new UI.Textbox[20];
        internal static Item[] item = new Item[256];
        internal static Tile[,] tile = new Tile[,] { };
        internal static Background[,] background = new Background[,] { };
        //public static Lighting[,] fog = new Lighting[,] { };
        internal static Lightmap[,] lightmap = new Lightmap[,] { };
        internal static Lamp[] lamp = new Lamp[10];
        internal static Projectile[] projectile = new Projectile[256];
        internal static Npc[] npc = new Npc[128];
        internal static Door[] door = new Door[51];
        internal static Scenery[] scenery = new Scenery[256];
        internal static Trap[] trap = new Trap[101];
        //  Fix
        internal static Dictionary<int, Room> room = new Dictionary<int, Room>();
        internal static Loot[] loot = new Loot[101];
        internal static Stash[] stash = new Stash[101];
        internal static WorldObject[] wldObject = new WorldObject[20];
        internal static Staircase[] staircase = new Staircase[6];
        internal static List<Floor> floor = new List<Floor>();
        //  Fog
        internal static IList<Legacy.Fog> effect = new List<Legacy.Fog>();
        //public static LitEffect[,] effect = new LitEffect[,] { };
        internal static Graphics Graphics { get; set; }
        internal static Color Mask => Color.FromArgb(0, 255, 0);
        public static Vector2 ScreenPosition => new Vector2(ScreenX, ScreenY);
        int ticks, ticks2, ticks3;
        internal static bool open = false;
        private bool init = false;
        public static rand rand = new rand();
        public static float TimeScale => timeScale();

        public static float Gamma = 1.2f;
        public static int KeyPressTimer;
        public static int
            ScreenWidth,
            ScreenHeight;
        private static int screenX, screenY;
        internal static bool mainMenu = true;
        public static Stopwatch time;
        public static TimeSpan timeSpan;
        public static int
            FloorNumber = 1,
            CurrentFloor = 1;
        public static int netMode = 0;
        private static int floorNum = 1; // Default is set to start at floor 1
        public static int offX => 1;//-Main.myPlayer.width;
        public static int offY => 1;//-Main.myPlayer.height;
        public static int ScreenX
        {
            get { return screenX; }
            set { screenX = value; }
        }
        public static int ScreenY
        {
            get { return screenY; }
            set { screenY = value; }
        }
        public static int
            WorldWidth,
            WorldHeight;
        public static float Progress
        {
            get { return progress; }
            set { progress = value; }
        }
        public static int FloorNum
        {
            get { return floorNum; }
            internal set { floorNum = value; }
        }
        static float progress;
        public static bool rest = false;
        public static float GlobalTime => timeSpan.Seconds * TimeScale;
        public static int GlobalScale(int integer, float scale) => (int)(integer * scale);
        public static Font DefaultFont => System.Drawing.SystemFonts.DefaultFont;
        internal static string setMapName(string name, int num)
        {
            return $"{name}{num}";
        }
        public static bool DoesMapExist(string name, int num)
        {
            return File.Exists(Path.Combine(Game.WorldSavePath, $"null{name}{num}"));
        }
        private static float timeScale()
        {
            float scale = 0f;
            float max = (float)Math.Round(myPlayer.velocity.MaxNormal(), 3);
            // Player resting 
            //  TODO, make TimeScale player-proximity based
            if (!myPlayer.IsMoving() && myPlayer.KeyDown(Keys.R))
            {
                scale = 1.2f;
                if (myPlayer.KeyDown(Keys.Space))
                { 
                    scale = 2f;
                }
                return scale;
            }
            if (myPlayer.IsMoving())
                return Math.Min(1f / (max / Player.maxSpeed), 1f);
            else if (myPlayer.KeyDown(Keys.Space))
            {
                return 1f;
            }
            if (max <= 0.1f)
                return 0f;
            return 0f;
        }
        public void MainMenu(Graphics graphics)
        {
            if (mainMenu)
            {
                graphics.FillRectangle(Brushes.Black, new Rectangle(0, 0, ScreenWidth, ScreenHeight));
                graphics.DrawString("Alpha", System.Drawing.SystemFonts.DefaultFont, System.Drawing.Brushes.Red, 10, 30);
            }
        }
        public new void Initialize()
        {
            time = Stopwatch.StartNew();
            ScreenWidth = 800;
            ScreenHeight = 600;
            //rand = new rand();
            //Mouse.Capture(MainWindow.Instance.Buffer);
            thumbnail = new Thumbnail();
            thumbnail.Init(10, ScreenWidth);
            camera1 = new Camera();
            camera1.position = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
            camera1.follow = true;
            myPlayer = new Player();
            myPlayer.name = "LocalPlayer";
            for (int i = 0; i < scroll.Length; i++)
                scroll[i] = new UI.Scroll();
            worldgen = new Worldgen();
            var box = CenterBox(ScreenWidth, ScreenHeight, 300, 75);
            #region DEBUG
            return;
            Trap.NewTrap(myPlayer.X, myPlayer.Y, 32, 32, ID.TrapID.FlameGeyser);
            textbox[0] =
                new UI.Textbox(
                    texture,
                    "Debug texture",
                    "debugging text drawing in text boxes\n" +
                    "asdasdasdsad\n" +
                    "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\n" +
                    "blah",
                    new Vector2(0, 0),
                    box.Width,
                    ButtonStyle.YesNo,
                    false,
                    0);
            //for (int i = 0; i < 3; i++)
            //  Item.NewItem(myPlayer.position.X, myPlayer.position.Y, 32, 32, ItemID.Purse, 255, CirclePrefect.Objects.Stash.GoldValue);
            Item.NewItem(myPlayer.position.X + Tile.Size, myPlayer.position.Y, 32, 32, ItemID.Torch);
            WorldObject.NewObject(myPlayer.position.X + Tile.Size, myPlayer.position.Y, 42, 42, true);
            #endregion
        }
        public void Camera(Camera camera)
        {
        }
        public void HandleDialog(UI.Button button, ButtonOption option, UI.Textbox parent, bool active)
        {
            if (button == null)
                return;
            switch (option)
            {
                case ButtonOption.None:
                case ButtonOption.Cancel:
                    goto default;
                case ButtonOption.Drop:
                    Item.Drop(ref parent.item, myPlayer.position);
                    myPlayer.inventory.Remove(parent.item);
                    goto default;
                case ButtonOption.Pickup:
                    if (myPlayer.PickupItem(ref parent.item))
                        Item.nearby.Remove(parent.item);
                    goto default;
                case ButtonOption.Equip:
                    if (parent.item.EquipItem(myPlayer))
                        myPlayer.inventory.Remove(parent.item);
                    goto default;
                case ButtonOption.Unequip:
                    if (parent.item.UnequipItem(myPlayer, out Item drop))
                        myPlayer.inventory.Add(drop);
                    goto default;
                case ButtonOption.No:
                    goto default;
                case ButtonOption.Yes:
                    goto default;
                case ButtonOption.Ok:
                    goto default;
                default:
                    parent.Close();
                    break;
            }
        }
        internal static void FloorTransition(StaircaseDirection direction)
        {
            Entity ent = Entity.None;
            ent.SetSuffix(setMapName("_map", FloorNum));
            using (TagCompound tag = new TagCompound(ent, SaveType.Map))
            {
                tag.WorldMap(TagCompound.Manager.Save);
            }
            Map.Unload();
            if (direction == StaircaseDirection.LeadingDown) FloorNum++;
            else if (direction == StaircaseDirection.LeadingUp) FloorNum--;
            if (!Main.DoesMapExist("_map", Main.FloorNum))
            {
                Map.GenerateFloor(new Margin(3000));
            }
            else
            { 
                Entity ent2 = Entity.None;
                ent2.SetSuffix(setMapName("_map", FloorNum));
                using (TagCompound tag = new TagCompound(ent2, SaveType.Map))
                {
                    tag.WorldMap(TagCompound.Manager.Load);
                }
            }
        }
        public void Update()
        {
            if (!init)
            {
                int width = 3000;
                int height = 3000;
                //  Legacy darkness effect
                Legacy.Fog.Create(0, 0, Main.ScreenWidth, Main.ScreenHeight);   //  TODO: recreate when window resized
                //GenerateFloor(new Margin(3000));
                //myPlayer.lamp = lamp[Lamp.NewLamp(myPlayer.Center, myPlayer.lightRange, Lamp.TorchLight, myPlayer, false, 0)];
                lightmap = worldgen.InitLightmap(width, height);
                myPlayer.Init();
                init = true; 
                return;
            }
            //EscState = Keyboard.GetState().IsKeyDown(Keys.Escape);
            timeSpan = TimeSpan.FromMilliseconds(time.ElapsedMilliseconds);
            var point = System.Windows.Forms.Cursor.Position;
            MouseScreen = new Vector2(Math.Max(point.X - (float)Game.Position.X, 0f), Math.Max(point.Y - (float)Game.Position.Y, 0f));//  -7 to X coord, -30 to Y coord due to WPF factor
            MouseWorld = MouseScreen + new Vector2(WorldZero.X, WorldZero.Y);
            pressO = ticks3++ == 1 && myPlayer.KeyDown(Keys.O);
            if (Keyboard.GetState().IsKeyUp(Keys.O))
                ticks3 = 0;
            mouseLeft = ticks++ == 1 && LeftMouse();
            if (!LeftMouse())
                ticks = 0;
            mouseRight = ticks2++ == 1 && RightMouse();
            if (!RightMouse())
                ticks2 = 0;
            camera1.position = myPlayer.position - new Vector2(myPlayer.width / 2, myPlayer.height / 2);
            thumbnail.Update();
            myPlayer.Update();
            myPlayer.collide = false;
            myPlayer.colUp = false;
            myPlayer.colDown = false;
            myPlayer.colRight = false;
            myPlayer.colLeft = false;
            if (Player.itemTextBox != null && Player.itemTextBox.active)
                Player.itemTextBox?.Update();
            UpdateArrays();
        }
        public bool PreDraw(Graphics graphics)
        {
            InitDraw(graphics);
            if (!open) return true;
            int width = 640;
            int height = 480;
            int x = ScreenWidth / 2 - width / 2 - (int)ScreenX;
            int y = ScreenHeight / 2 - height / 2 - (int)ScreenY;
            Rectangle bound = new Rectangle(x, y, width, height);
            Rectangle inventory = new Rectangle(x + 20, y + 20, width / 2 - 20, height - 40);

            graphics.DrawImageUnscaledAndClipped(bg, bound);
            graphics.DrawRectangle(Pens.White, inventory);

            int index = 0;
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
                graphics.DrawRectangle(Pens.GhostWhite, box = new Rectangle(newX, newY, Item.DrawSize, Item.DrawSize));
                if (myPlayer.equipment[i] != null && myPlayer.equipment[i].active && myPlayer.equipment[i].equipped && myPlayer.equipment[i].owner == myPlayer.whoAmI)
                {
                    int w = (int)Helper.RatioConvert(Helper.Ratio(Item.DrawSize, myPlayer.equipment[i].texture.Width), myPlayer.equipment[i].texture.Width);
                    Rectangle slot = new Rectangle(box.X + 1, box.Y + 1, Item.DrawSize - 1, w - 1);
                    Drawing.DrawScale(myPlayer.equipment[i].texture, new Vector2(slot.X, slot.Y), slot.Width, slot.Height, Color.Green, graphics, Drawing.SetColor(myPlayer.equipment[i].defaultColor));
                    //graphics.DrawImage(myPlayer.equipment[i].texture, slot);
                }
            }

            var floor = scroll[0].parent = new Rectangle(x + width / 2 + 20, y + 20, width / 2 - 40, height / 2 - 20);
            var storage = scroll[1].parent = new Rectangle(x + width / 2 + 20, y + height / 2 + 20, width / 2 - 40, height / 2 - 40);

            graphics.FillRectangles(Brushes.Black, new Rectangle[]
            {
                floor,
                storage
            });
            graphics.FillRectangles(Brushes.GhostWhite, new RectangleF[]
            {
                new RectangleF(floor.Right - UI.Scroll.Width, floor.Top, UI.Scroll.Width, floor.Height),
                new RectangleF(storage.Right - UI.Scroll.Width, storage.Top, UI.Scroll.Width, storage.Height)
            });
            graphics.FillRectangles(Brushes.Gray, new RectangleF[]
            {
                new RectangleF(scroll[0].X, scroll[0].Y, UI.Scroll.Width, UI.Scroll.Height),
                new RectangleF(scroll[1].X, scroll[1].Y, UI.Scroll.Width, UI.Scroll.Height)
            });
            UI.Container.DrawItems(Item.nearby, scroll[0], graphics);
            UI.Container.DrawItems(myPlayer.inventory, scroll[1], graphics);
            if (Player.itemTextBox != null && Player.itemTextBox.active)
            {
                Player.itemTextBox.Draw(graphics);
            }
            return false;
        }
        public void Draw(Graphics graphics)
        {
            for (int i = 0; i < background.GetLength(0); i++)
            {
                for (int j = 0; j < background.GetLength(1); j++)
                {
                    background[i, j]?.Draw(graphics);
                }
            }
            Worldgen.UpdateLampMaps();  //  Lighting priority (LightSurface/Brush?)
            DrawBackground(graphics);
            for (int i = 0; i < trap.Length; i++)
            {
                if (trap[i] != null && trap[i].active)
                {
                    trap[i].Draw(graphics);
                }
            }
            for (int i = 0; i < loot.Length; i++)
            {
                if (loot[i] != null)
                {
                    loot[i].Draw(graphics);
                }
            }
            for (int i = 0; i < stash.Length; i++)
            {
                if (stash[i] != null)
                {
                    stash[i].Draw(graphics);
                }
            }
            for (int i = 0; i < npc.Length; i++)
            {
                if (npc[i] != null)
                {
                    npc[i].Draw(graphics);
                }
            }
            //  DEBUG: drawing player entity
            //graphics.FillRectangle(Brushes.White, myPlayer.hitbox());
            myPlayer.Draw(graphics);
            for (int i = 0; i < wldObject.Length; i++)
            {
                if (wldObject[i] != null)
                {
                    wldObject[i].Draw(graphics);
                }
            }
            for (int i = 0; i < projectile.Length; i++)
            {
                if (projectile[i] != null)
                {
                    projectile[i].Draw(graphics);
                }
            }
            foreach (Room room in EntityHelper.RoomWhile(t => t != null))
            {
                room.Draw(graphics);
            }
            return;
            //  DEBUG: UI interaction check flag
            if (Thumbnail.showDateTime)
            {
                graphics.DrawString(DateTime.Now.ToString(), System.Drawing.SystemFonts.DialogFont, Brushes.White, 0f, 0f);
            }
        }
        public void PostDraw(SpriteBatch sb)
        {
            using (Bitmap bmp = new Bitmap(Main.ScreenWidth, Main.ScreenHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Rectangle box = new Rectangle(0, 0, Main.ScreenWidth, Main.ScreenHeight);
                    g.FillRectangle(new SolidBrush(Color.Black), box);
                    for (int i = 0; i < Main.lightmap.GetLength(0); i++)
                    {
                        for (int j = 0; j < Main.lightmap.GetLength(1); j++)
                        {
                            Lightmap map = Main.lightmap[i, j];
                            if (map == null)
                                return;
                            if (map.active && box.IntersectsWith(map.Hitbox))
                            {
                                if (map.DefaultColor != Color.FromArgb(255, 20, 20, 20))
                                {
                                    g.FillRectangle(new SolidBrush(Main.Mask), map.Hitbox);
                                }
                            }
                        }
                    }
                    for (int i = 0; i < Main.lamp.Length; i++)
                    {
                        if (Main.lamp[i] != null && Main.lamp[i].active)
                        { 
                            g.FillEllipse(new SolidBrush(Main.Mask), new Rectangle((int)(Main.lamp[i].Center.X + Main.ScreenPosition.X) - (int)Main.lamp[i].range / 2, (int)(Main.lamp[i].Center.Y + Main.ScreenPosition.Y) - (int)Main.lamp[i].range / 2, (int)Main.lamp[i].range, (int)Main.lamp[i].range));
                        }
                    }
                    float range = Main.myPlayer.lightRange;
                    if (Main.myPlayer.lamp == null)
                    {
                        range /= 2f;
                    }
                    g.FillEllipse(new SolidBrush(Main.Mask), new Rectangle(Main.ScreenWidth / 2 - (int)range, Main.ScreenHeight / 2 - (int)range, (int)range * 2, (int)range * 2));
                    // TODO: apply a textured brush
                    bmp.MakeTransparent(Main.Mask);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bmp.Save(stream, ImageFormat.Png);
                        Texture2D surface = Texture2D.FromStream(sb.GraphicsDevice, stream);
                        sb.Draw(surface, Vector2.Zero, Microsoft.Xna.Framework.Color.White);
                        surface.Dispose();
                    }
                }
            }
        }

        internal static double Distance(Vector2 start, Vector2 end)
        {
            return Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
        }
        private void UpdateArrays()
        {
            for (int i = 0; i < trap.Length; i++)
            {
                if (trap[i] != null && trap[i].active)
                {
                    trap[i].Update();
                }
            }
            for (int i = 0; i < wldObject.Length; i++)
            {
                if (wldObject[i] != null)
                {
                    wldObject[i].Update();
                    if (wldObject[i].physics)
                    {
                        wldObject[i].PhysicsMove(myPlayer);
                    }
                }
            }
            for (int i = 0; i < loot.Length; i++)
            {
                if (loot[i] != null)
                {
                    loot[i].Update();
                }
            }
            for (int i = 0; i < stash.Length; i++)
            {
                if (stash[i] != null)
                {
                    stash[i].Update();
                }
            }
            for (int i = 0; i < scroll.Length; i++)
            {
                if (scroll[i] != null)
                {
                    UI.Scroll.KbInteract(scroll[i]);
                    UI.Scroll.MouseInteract(scroll[i]);
                }
            }
            for (int i = 0; i < textbox.Length; i++)
            {
                textbox[i]?.Update();
            }
            for (int i = 0; i < tile.GetLength(0); i++)
            {
                for (int j = 0; j < tile.GetLength(1); j++)
                {
                    if (tile[i, j] != null && tile[i, j].Active && tile[i, j].floorNumber >= CurrentFloor - 1 && tile[i, j].floorNumber <= CurrentFloor + 1)
                    {
                        tile[i, j].Update();
                        tile[i, j].Collision(myPlayer);
                    }
                }
            }
            //for (int i = 0; i < lightmap.GetLength(0); i++)
            //{
            //    for (int j = 0; j < lightmap.GetLength(1); j++)
            //    {
            //        if (lightmap[i, j] != null && lightmap[i, j].active)
            //        {
            //            lightmap[i, j].Update();
            //        }
            //    }
            //}
            for (int i = 0; i < effect.Count; i++)
            {
                effect[i]?.Update();
            }
            for (int i = 0; i < door.Length; i++)
            {
                door[i]?.Update();
            }
            for (int i = 0; i < item.Length; i++)
            {
                item[i]?.Update(myPlayer);
            }
            for (int i = 0; i < lamp.Length; i++)
            {
                if (lamp[i] != null)
                {
                    lamp[i].Update();
                }
            }
            for (int i = 0; i < staircase.Length; i++)
            {
                if (staircase[i] != null)
                {
                    staircase[i].Update();
                }
            }
            for (int i = 0; i < background.GetLength(0); i++)
            {
                for (int j = 0; j < background.GetLength(1); j++)
                {
                    background[i, j]?.Update();
                }
            }
            for (int i = 0; i < npc.Length; i++)
            {
                if (npc[i] != null)
                {
                    if (!npc[i].PreUpdate())
                        continue;
                    npc[i].AI();
                    if (npc[i] == null)
                        continue;
                    npc[i].collide = false;
                    npc[i].colUp = false;
                    npc[i].colDown = false;
                    npc[i].colRight = false;
                    npc[i].colLeft = false;
                }
            }
            for (int i = 0; i < projectile.Length; i++)
            {
                if (projectile[i] != null)
                {
                    projectile[i].AI();
                }
            }
            for (int i = 0; i < CombatText.text.Count; i++)
            {
                if (CombatText.text[i].active)
                {
                    CombatText.text[i].Update();
                }
            }
            for (int i = 0; i < scenery.Length; i++)
            {
                if (scenery[i] != null)
                {
                    scenery[i].Update();
                }
            }
            
            //  Fog Update
            //for (int i = 0; i < fog.GetLength(0); i++)
            //{
            //    for (int j = 0; j < fog.GetLength(1); j++)
            //    {
            //        if (fog[i, j] != null)
            //        {
            //            fog[i, j].Update();
            //        }
            //    }
            //}
            if (pressO)
                open = !open;
            if (!Main.open)
            {
                Item.nearby.Clear();
                for (int i = 0; i < Main.item.Length; i++)
                {
                    if (Main.item[i] != null && Main.item[i].owner == 255 && Main.item[i].type > ItemID.None && Main.item[i].active && item[i].InProximity(myPlayer, Stash.LootRange))
                    {
                        Item.nearby.Add(Main.item[i]);
                    }
                }
                if (Item.stash != null)
                {
                    if (Item.stash.InProximity(myPlayer, Stash.LootRange))
                    {
                        //  Refresh contents to negate automatic refilling
                        Item.stash.item.Clear();
                        Item.stash.content.ToList().ForEach(t => { Item.stash.item.Add(t); });
                        Item.stash.item.RemoveAll(t => t == null || !t.active || string.IsNullOrWhiteSpace(t.name));
                        foreach (Item i in Item.stash.item)
                            Item.nearby.Add(i);
                    }
                    else Item.stash.open = false;
                }
            }
            Item.nearby.RemoveAll(t => t == null || !t.active || string.IsNullOrWhiteSpace(t.name));
        }
        private void DrawBackground(Graphics graphics)
        {
            for (int i = 0; i < scenery.Length; i++)
            {
                if (scenery[i] != null)
                {
                    scenery[i].Draw(graphics);
                }
            }
            for (int i = 0; i < staircase.Length; i++)
            {
                if (staircase[i] != null)
                {
                    staircase[i].Draw(graphics);
                }
            }
            for (int i = 0; i < tile.GetLength(0); i++)
            {
                for (int j = 0; j < tile.GetLength(1); j++)
                {
                    tile[i, j].Draw(graphics);
                }
            }
            //for (int i = 0; i < lightmap.GetLength(0); i++)
            //{
            //    for (int j = 0; j < lightmap.GetLength(1); j++)
            //    {
            //        if (lightmap[i, j] != null && lightmap[i, j].active)
            //        {
            //            lightmap[i, j].Draw(myPlayer);
            //        }
            //    }
            //}
            for (int i = 0; i < door.Length; i++)
            {
                door[i]?.Draw(graphics);
            }
            for (int i = 0; i < item.Length; i++)
            {
                if (item[i] != null && item[i].active && item[i].type > ItemID.None && !myPlayer.inventory.Contains(item[i]))
                {
                    item[i].Draw(graphics);
                }
            }
            for (int i = 0; i < lamp.Length; i++)
            {
                if (lamp[i] != null)
                {
                    lamp[i].PreDraw(graphics);
                }
            }
        }
        internal void DrawOverlays(Graphics graphics)
        {
            for (int i = 0; i < CombatText.text.Count; i++)
            {
                if (CombatText.text[i].active)
                {
                    CombatText.text[i].Draw(graphics);
                }
            }
            for (int i = 0; i < textbox.Length; i++)
            {
                if (textbox[i] != null && textbox[i].active)
                {
                    textbox[i].Draw(graphics);
                }
            }
            thumbnail.DrawUI(graphics);
        }
        bool flag = false;
        private void InitDraw(Graphics graphics)
        {
            if (init && !flag)
            {
                LightPass.PreProcessing();
                //  DEBUG: textbox
                //textbox[0].Init(graphics);
                flag = true;
            }
        }
        [Obsolete("Does not generate lighting effects correctly.")]
        public static void GenerateFloor(int width = 3000, int height = 3000, Player player = null, bool randSpawn = false)
        {
            lightmap = worldgen.InitLightmap(width, height);
            Main.tile = worldgen.CastleGen(Tile.Size, width, height, width / 250, 300f, 600f);
            Room.ConstructAllRooms();
            player?.FindRandomTile(randSpawn);
        }
        public static bool LeftMouse()
        {
            return Input.IsLeftPressed();
        }
        public static bool RightMouse()
        {
            return Input.IsRightPressed();
        }
        private Rectangle CenterBox(int screenWidth, int screenHeight, int centerWidth, int centerHeight)
        {
            return new Rectangle(screenWidth / 2 - centerWidth / 2, screenHeight / 2 - centerHeight / 2, centerWidth, centerHeight);
        }
    }
}
