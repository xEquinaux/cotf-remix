using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cotf.Assets
{
	public static class Pipeline
	{
		public static Texture2D BitmapToTex2D(this Bitmap image, GraphicsDevice device)
		{
			Texture2D tex = new Texture2D(device, image.Width, image.Height);
			var bits = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
			tex.SetData(bits.Scan0);
			image.UnlockBits(bits);
			return tex;
		}
		public static Texture2D BitmapToTex2D(this Image texture, GraphicsDevice device)
		{
			Bitmap image = (Bitmap)texture;
			Texture2D tex = new Texture2D(device, image.Width, image.Height);
			var bits = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
			tex.SetData(bits.Scan0);
			image.UnlockBits(bits);
			return tex;
		}
	}
}
