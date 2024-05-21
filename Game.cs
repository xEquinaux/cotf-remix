using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Point = Microsoft.Xna.Framework.Point;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using cotf.Assets;
using cotf.Base;
using cotf.Legacy;
using System.Diagnostics;
using System.Collections.Generic;
using System.Timers;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Timer = System.Timers.Timer;

namespace cotf
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphicsMngr;
        private SpriteBatch _spriteBatch;
        private Viewport _viewport
        {
            get { return GraphicsDevice.Viewport; }
            set { GraphicsDevice.Viewport = value; }
        }
        static BufferedGraphicsContext context = BufferedGraphicsManager.Current;
        
        private int _portX => _viewport.X;
        private int _portY => _viewport.Y;
        private Rectangle _size => new Rectangle(0, 0, _bounds.Width, _bounds.Height);
        private Size _oldBounds;
        private Size _bounds;

        private static Point _position;
        private static Point _oldPosition;
        public static Point Position => _position;
        public static Camera CAMERA = new Camera();

        internal static Texture2D fog; 
        private Texture2D tile;

        public static string SavePath => Path.Combine(new[] { Environment.GetEnvironmentVariable("USERPROFILE"), "Documents", "My Games", "CotF" });
        public static string PlayerSavePath => Path.Combine(SavePath, "Players");
        public static string WorldSavePath => Path.Combine(SavePath, "World");

        public Game()
        {
            _graphicsMngr = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            new Main();
            TagCompound.SetPaths(PlayerSavePath, WorldSavePath);   //  TODO: make relative to player name
            _Initialize();
            { 
                _bounds = new Size(800, 600);
                Settings();
            }
            base.Initialize();
        }

        protected void Resize(object sender, EventArgs e)
        {
            _graphicsMngr.PreferredBackBufferWidth = _bounds.Width;
            _graphicsMngr.PreferredBackBufferHeight = _bounds.Height;
            _graphicsMngr.ApplyChanges();
        }

        protected override bool BeginDraw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            {
                _position = Window.Position;
                if (_oldBounds != _bounds || _oldPosition != _position)
                {
                    ResizeEvent.Invoke(this, new EventArgs());
                }
                _oldBounds = _bounds;
                _oldPosition = _position;
            }

            return base.BeginDraw();
        }

        protected override void EndDraw()
        {
            _spriteBatch.End();
            GraphicsDevice.Present();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void LoadContent()
        {
            Game.fog = Content.Load<Texture2D>("fow");
            this.tile = Content.Load<Texture2D>("temp");
            LoadResources();
            {
                _viewport = new Viewport(_portX, _portY, 800, 600);
                ResizeEvent += Resize;
            }
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (Main.KeyPressTimer == 0)
                {
                    Main.KeyPressTimer++;
                    if (!Main.open)
                    {
                        //for (int i = 0; i < Main.player.Length; i++)
                        //{
                        //    Main.player[i]?.Save(false);
                        //}
                        Exit();
                    }
                }
            }
            else
            {
                Main.KeyPressTimer = 0;
            }
            Update();
            base.Update(gameTime);
        }

        int seconds = 0;
        long average = 0;
        List<long> elapsed = new List<long>(10);
        Stopwatch watch = new Stopwatch();
        Timer timer = new Timer();
        protected override void Draw(GameTime gameTime)
        {
            using (Bitmap bmp = new Bitmap(_bounds.Width, _bounds.Height))
            {
                using (Graphics graphics = Graphics.FromImage(bmp))
                using (BufferedGraphics buffered = context.Allocate(graphics, new System.Drawing.Rectangle(0, 0, _bounds.Width, _bounds.Height)))
                {
                    SetQuality(buffered.Graphics, new System.Drawing.Rectangle(0, 0, _bounds.Width, _bounds.Height));
                    graphics.Clear(System.Drawing.Color.CornflowerBlue);
                    {
                        this.Camera(buffered.Graphics, CAMERA);
                        this.PreDraw(buffered.Graphics);
                        this.Draw(buffered.Graphics);
                        this.TitleScreen(buffered.Graphics);
                    }
                    buffered.Render();
                }
                using (MemoryStream stream = new MemoryStream())
                {
                    bmp.Save(stream, ImageFormat.Bmp);
                    Texture2D surface = Texture2D.FromStream(_graphicsMngr.GraphicsDevice, stream);
                    _spriteBatch.Draw(surface, Vector2.Zero, Color.White);
                    surface.Dispose();
                }
            }
            if (!Main.open && !Main.mainMenu)                                                     
            {
                Main.Instance.PostDraw(_spriteBatch);
            }
            if (!Main.mainMenu && !Main.open)
            { 
                using (Bitmap bmp = new Bitmap(_bounds.Width, _bounds.Height))
                {
                    var transparent = System.Drawing.Color.FromArgb(20, 20, 20);
                    using (Graphics graphics = Graphics.FromImage(bmp))
                    {
                        graphics.Clear(transparent);
                        SetQuality(graphics, new System.Drawing.Rectangle(0, 0, _bounds.Width, _bounds.Height));
                        { 
                            Main.myPlayer.playerData?.Draw(graphics);
                            Main.Instance.DrawOverlays(graphics);
                            //graphics.DrawString(gameTime.ElapsedGameTime.Milliseconds.ToString(), Main.DefaultFont, Brushes.White, PointF.Empty);
                        }
                    }
                    bmp.MakeTransparent(transparent);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bmp.Save(stream, ImageFormat.Png);
                        Texture2D surface = Texture2D.FromStream(_graphicsMngr.GraphicsDevice, stream);
                        _spriteBatch.Draw(surface, Vector2.Zero, Color.White);
                        surface.Dispose();
                    }
                }
            }
            //cotf.World.FogMethods.DrawEffect(fog, _spriteBatch);
            base.Draw(gameTime);
        }

        protected void Settings()
        {
            this.Window.Title = "cotf";
            this.Window.IsBorderless = true;
            this.Window.AllowUserResizing = true;
            this.Window.AllowAltF4 = false;
        }

        #region events
        public static event EventHandler<EventArgs> ResizeEvent;
        public static event EventHandler<InitializeArgs> InitializeEvent;
        public static event EventHandler<EventArgs> LoadResourcesEvent;
        public static event EventHandler<DrawingArgs> MainMenuEvent;
        public static event EventHandler<PreDrawArgs> PreDrawEvent;
        public static event EventHandler<DrawingArgs> DrawEvent;
        public static event EventHandler<UpdateArgs> UpdateEvent;
        public static event EventHandler<CameraArgs> CameraEvent;
        public class DrawingArgs : EventArgs
        {
            public Graphics graphics;
        }
        public class PreDrawArgs : EventArgs
        {
            public Graphics graphics;
        }
        public class UpdateArgs : EventArgs
        {
        }
        public class CameraArgs : EventArgs
        {
            public Graphics graphics;
            public Camera CAMERA;
        }
        public class InitializeArgs : EventArgs
        {
        }
        #endregion
        #region methods
        private void LoadResources()
        {
            Main.bg = Asset<Image>.Request("bg");
            Main.texture = Asset<Image>.Request("temp");
            Main.texture90 = Asset<Image>.Request("temp90");
            Main.pixel = Asset<Image>.Request("pixel");
            Main.fow = Asset<Image>.Request("fow");
            Main.fow50 = Asset<Image>.Request("fow50");
            Main.square = Asset<Image>.Request("background");
            Main.grass = Asset<Image>.Request("small");
            for (int i = 0; i < Main.trapTexture.Length; i++)
            {
                Main.trapTexture[i] = Main.texture90;
            }
            Main.chainTexture[0] = Asset<Image>.Request("chain");
        }
        private void _Initialize()
        {
            Main.Instance.Initialize();
        }
        private void TitleScreen(Graphics graphics)
        {
            Main.Instance.MainMenu(graphics);
        }
        private void PreDraw(Graphics graphics)
        {
            Main.Graphics = graphics;
        }
        private void Draw(Graphics graphics)
        {
            if (!Main.mainMenu && Main.Instance.PreDraw(graphics))
            { 
                Main.Instance.Draw(graphics);
            }
        }
        private void Update()
        {
            Main.keyboard = Keyboard.GetState();
            if (Main.myPlayer.KeyDown(Keys.Escape))
            {
                if (Main.KeyPressTimer == 0)
                {
                    Main.KeyPressTimer++;
                    if (Main.open)
                    {
                        Main.myPlayer.OpenInventory(false);
                    }
                }
            }
            else
            {
                Main.KeyPressTimer = 0;
            }
            if (Main.myPlayer.KeyDown(Keys.Space))
                Main.mainMenu = false;
            if (!Main.mainMenu) Main.Instance.Update();
        }
        private void Camera(Graphics graphics, Camera CAMERA)
        {
            if (Main.camera1 == null || Main.mainMenu)
                return;
            Main.Instance.Camera(Main.camera1);
            if (Main.camera1.follow && Main.camera1.isMoving)
            {                                                                       
                Main.ScreenX = (int)-Main.camera1.position.X + Main.ScreenWidth / 2 - Main.myPlayer.width;
                Main.ScreenY = (int)-Main.camera1.position.Y + Main.ScreenHeight / 2 - Main.myPlayer.height;
            }
            graphics.RenderingOrigin = new System.Drawing.Point((int)Main.camera1.position.X, (int)Main.camera1.position.Y);
            graphics.TranslateTransform(
                Main.ScreenX,
                Main.ScreenY,
                MatrixOrder.Append);
            Main.camera1.oldPosition = Main.camera1.position;
        }
        #endregion
        #region quality settings
        public CompositingQuality compositingQuality = CompositingQuality.AssumeLinear;
        public CompositingMode compositingMode = CompositingMode.SourceOver;
        public InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic;
        public TextRenderingHint textRenderHint = TextRenderingHint.ClearTypeGridFit;
        public GraphicsUnit graphicsUnit = GraphicsUnit.Pixel;
        public SmoothingMode smoothingMode = SmoothingMode.Default;
        private void SetQuality(Graphics graphics, System.Drawing.Rectangle bounds)
        {
            graphics.CompositingQuality = compositingQuality;
            graphics.CompositingMode = compositingMode;
            graphics.InterpolationMode = interpolationMode;
            graphics.TextRenderingHint = textRenderHint;
            //graphics.RenderingOrigin = new Point(bounds.X, bounds.Y);
            //graphics.Clip = new System.Drawing.Region(bounds);
            graphics.PageUnit = graphicsUnit;
            graphics.SmoothingMode = smoothingMode;
        }
        #endregion
    }
    public class Camera
    {
        public Vector2 oldPosition;
        public Vector2 position;
        public Vector2 velocity;
        public bool isMoving => velocity != Vector2.Zero || oldPosition != position;
        public bool follow = false;
        public bool active = false;
    }
}