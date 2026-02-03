using cotf.Assets;
using REWD.D2D;
using REWD.FoundationR;
using SharpDX;
using SharpDX.Direct2D1;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Bitmap = System.Drawing.Bitmap;
using Image = System.Drawing.Image;
using cotf.Base;
using CotF_dev;
using Rectangle = System.Drawing.Rectangle;
using Point = System.Drawing.Point;
using System.Diagnostics;
using System.Drawing;
using System;
using System.IO;

namespace cotf;

internal class Program
{
	static void Main(string[] args)
	{
		try 
		{ 
			new Game();
		}
		catch (Exception e)
		{
			var time = DateTime.Now;
			using (var s = File.CreateText($"{time.Year}_{time.Month}_{time.Day}_{time.Minute}_{time.Second}_{time.Millisecond}.txt"))
			{
				s.WriteLine(e.ToString());
			}
		}
	}
}

public class Game : Direct2D
{
	public static Game Instance;
	static BufferedGraphicsContext context = BufferedGraphicsManager.Current;

	bool init = false;
	private Rectangle _size => new Rectangle(0, 0, _bounds.Width, _bounds.Height);
	private Size _oldBounds;
	private Size _bounds;

	private static Point _position;
	private static Point _oldPosition;
	public static Point Position => _position;
	public static Camera CAMERA = new Camera();

	public static string SavePath => Path.Combine(new[] { Environment.GetEnvironmentVariable("USERPROFILE"), "Documents", "My Games", "CotF" });
	public static string PlayerSavePath => Path.Combine(SavePath, "Players");
	public static string WorldSavePath => Path.Combine(SavePath, "World");

	public static Lamp playerLamp;

	public Game() : base(800, 600)
	{
		Instance = this;
	}

	System.Drawing.Bitmap titleScreen;
	System.Drawing.Bitmap skyBoss;
	int width = 800, height = 600;
	bool showTitle = false;

	public override void LoadResources()
	{
		Asset.Request("Backgrounds\\MapBGMagno", ".png", out titleScreen);
		Asset.LoadFromFile("Content\\Sky_boss", out skyBoss);
		
		Main.cinnabar = Asset<Image>.Request("cinnabar_dagger");
		Main.bg = Asset<Image>.Request("bg");
		Main.texture = Asset<Image>.Request("temp");
		Main.texture90 = Asset<Image>.Request("temp90");
		Main.pixel = Asset<Image>.Request("pixel");
		Main.fow = Asset<Image>.Request("fow");
		Main.fow50 = Asset<Image>.Request("fow50");
		Main.square = Asset<Image>.Request("Ground/background");
		Main.grass = Asset<Image>.Request("small");
		for (int i = 0; i < Main.trapTexture.Length; i++)
		{
			Main.trapTexture[i] = Main.texture90;
		}
		for (int i = 1; i <= 4; i++)
		{
			Main.wallTexture[i - 1] = Asset<Image>.Request($"Walls/wall{i}");
		}
		Main.chainTexture[0] = Asset<Image>.Request("chain");
		Main.Texture.Add(Asset.LoadFromFile("Content/Projectiles/magno_javelin"));
		Main.Texture.Add(Asset.LoadFromFile("Content/Items/Jobs/Scroll_plague_nova"));
		Main.Texture.Add(Asset.LoadFromFile("Content/Items/flask_mercury"));
		Main.Texture.Add(Asset.LoadFromFile("Content/Walls/magno_brickwall"));
	}

	public override void Initialize()
	{
		new Main();
		TagCompound.SetPaths(PlayerSavePath, WorldSavePath);   //  TODO: make relative to player name
		{
			_bounds = new Size(800, 600);
			Settings();
		}
		Main.Instance.Initialize();
	}

	public bool RateLimiter()
	{
		const int FPS = 600; // Target frames per second
        TimeSpan frameDuration = TimeSpan.FromMilliseconds(1000.0 / FPS);

        Stopwatch stopwatch = Stopwatch.StartNew();

        // Your update logic here
        Console.WriteLine("Updating...");

        stopwatch.Stop();
        TimeSpan elapsedTime = stopwatch.Elapsed;
        TimeSpan sleepTime = frameDuration - elapsedTime;

        if (sleepTime > TimeSpan.Zero)
        {
            return false;
        }
		return true;
	}

	public override void Update()
	{
		//Task.WaitAll(Task.Delay(1));
		if (!showTitle && CotF_dev.Keyboard.IsKeyPressed((int)VIRTUALKEY.VK_RETURN))
		{
			showTitle = true;
		}
		//	TODO: player realtime light
		//playerLamp = Main.lamp[0] = new Lamp(150) { position = Main.myPlayer.Center, lampColor = Lamp.TorchLight, staticLamp = true, active = true, name = "PlrLamp" };

		if (CotF_dev.Keyboard.IsKeyPressed((int)VIRTUALKEY.VK_ESCAPE))
		{
			if (Main.KeyPressTimer == 0)
			{
				Main.KeyPressTimer++;
				if (!Main.open)
				{
					Main.myPlayer.Save(true);
					Entity ent = Entity.None;
					ent.SetSuffix(Main.setMapName("_map", Main.FloorNum));
					using (TagCompound tag = new TagCompound(ent, SaveType.Map))
					{
						tag.WorldMap(TagCompound.Manager.Save);
					}
					// TODO EXIT CODE
					Environment.Exit(1);
				}
				else Main.open = false;
			}
		}
		else
		{
			Main.KeyPressTimer = 0;
		}
		if (!init)
		{
			Instance = this;
			init = true;
		}
		UpdateContinued();
	}

	public override void Draw(DeviceContext rt)
	{
		using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height))
		{
			using (Graphics graphics = Graphics.FromImage(bmp))
			using (BufferedGraphics buffered = context.Allocate(graphics, new System.Drawing.Rectangle(0, 0, width, height)))
			{
				SetQuality(buffered.Graphics, new System.Drawing.Rectangle(0, 0, width, height));
				graphics.Clear(System.Drawing.Color.CornflowerBlue);
				{
					this.Camera(buffered.Graphics, CAMERA);
					this.PreDraw(buffered.Graphics);
					this.Draw(buffered.Graphics);
					this.TitleScreen(buffered.Graphics);
				}
				buffered.Render();
			}
			var surface = ConvertBitmap(bmp, deviceContext);
			rt.DrawBitmap(surface, 1f, BitmapInterpolationMode.NearestNeighbor);
			surface.Dispose();
		}
		if (!Main.open && !Main.mainMenu)
		{
			Main.Instance.PostDraw();
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
				var surface = ConvertBitmap(bmp, deviceContext);
				rt.DrawBitmap(surface, 1f, BitmapInterpolationMode.NearestNeighbor);
				surface.Dispose();
			}
		}
		//cotf.World.FogMethods.DrawEffect(fog, _spriteBatch);
	}

	private new SharpDX.Direct2D1.Bitmap ConvertBitmap(System.Drawing.Bitmap bitmap, SharpDX.Direct2D1.DeviceContext deviceContext)
	{
		var bitmapProperties = new SharpDX.Direct2D1.BitmapProperties(
			new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied));

		var bitmapData = bitmap.LockBits(
			new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
			System.Drawing.Imaging.ImageLockMode.ReadOnly,
			System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

		using (var dataStream = new SharpDX.DataStream(bitmapData.Scan0, bitmapData.Stride * bitmap.Height, true, false))
		{
			var sharpDxBitmap = new SharpDX.Direct2D1.Bitmap(deviceContext, new Size2(bitmap.Width, bitmap.Height), dataStream, bitmapData.Stride, bitmapProperties);
			bitmap.UnlockBits(bitmapData);
			return sharpDxBitmap;
		}
	}

	void ResizeImageToWidth(int targetWidth, ref int srcWidth, ref int srcHeight)
	{
		float aspectRatio = (float)srcHeight / srcWidth;
		int calculatedHeight = (int)(targetWidth * aspectRatio);

		srcWidth = targetWidth;
		srcHeight = calculatedHeight;
	}
	private void TitleScreen(Graphics graphics)
	{
		if (!showTitle)
		{
			int w = titleScreen.Width;
			int h = titleScreen.Height;
			ResizeImageToWidth(800, ref w, ref h);
			int centerHeight = (800 / 2 - h / 2) / 2;
			graphics.DrawString("Archaea Mod", new Font("Helvetica", 16f), Brushes.Purple, new Point(0, centerHeight / 2 - 8));
			graphics.DrawString("Press Enter", new Font("Helvetica", 12f), Brushes.Gray, new Point(0, 600 - centerHeight / 2 - 6));
			graphics.DrawImage(titleScreen, new Rectangle(0, centerHeight, w, h));
		}
		else Main.mainMenu = false;
	}

	protected void Settings()
	{
		//this.Window.Title = "cotf";
		//this.Window.IsBorderless = true;
		//this.Window.AllowUserResizing = true;
		//this.Window.AllowAltF4 = false;
	}

	#region methods
	/* private void TitleScreen(Graphics graphics)
	{
		Main.Instance.MainMenu(graphics);
	}  */
	private void PreDraw(Graphics graphics)
	{
		Main.Graphics = graphics;
	}
	private void Draw(Graphics graphics)
	{
		if (!Main.mainMenu && Main.Instance.PreDraw(graphics))
		{
			// TODO Realtime player light
			LightPass.PreProcessing(
				Main.tile,
				Main.background, 
				new Lamp[] 
				{
					Main.lamp[0]
				}
			);
			Main.Instance.Draw(graphics);
		}
	}
	private void UpdateContinued()
	{
		if (Main.myPlayer.KeyDown(VIRTUALKEY.VK_ESCAPE))
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
		if (Main.myPlayer.KeyDown(VIRTUALKEY.VK_SPACE))
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
	public CompositingQuality compositingQuality = CompositingQuality.HighQuality;
	public CompositingMode compositingMode = CompositingMode.SourceOver;
	public System.Drawing.Drawing2D.InterpolationMode interpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
	public TextRenderingHint textRenderHint = TextRenderingHint.ClearTypeGridFit;
	public GraphicsUnit graphicsUnit = GraphicsUnit.Pixel;
	public SmoothingMode smoothingMode = SmoothingMode.AntiAlias;
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