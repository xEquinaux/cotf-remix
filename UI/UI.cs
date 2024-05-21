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
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Rectangle = System.Drawing.Rectangle;

namespace cotf
{
    public enum ButtonOption
    {
        None,
        Ok, 
        Yes, 
        No, 
        Cancel, 
        Equip,
        Drop,
        Pickup,
        Unequip
    }
    public enum ButtonStyle
    {
        None,
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel,
        EquipDropCancel,
        PickupCancel,
        UnequipCancel
    }
    public class UI
    {
        public class ButtonEventArgs : EventArgs
        {
            public bool active;
            public ButtonOption option;
            public Textbox parent;
        }
        public class Button : Entity
        {
            #region variables
            public readonly ButtonOption option;
            public readonly Textbox parent;
            protected readonly string text;
            internal const int Width = 40;
            internal const int Height = 15;
            internal int index = 0;
            public override int X => (int)position.X;
            public override int Y => (int)position.Y;
            private int offX => -Main.myPlayer.width;
            private int offY => -Main.myPlayer.height;
            private Font font = System.Drawing.SystemFonts.DialogFont;
            private new Rectangle box => new Rectangle(parent.padded.Right - (Button.Width + 10) * (index + 1), parent.padded.Bottom + Button.Height, Width, Height);
            private Rectangle padded => new Rectangle(box.X - margin.Left, box.Y - margin.Top, box.Width + margin.Right * 2, box.Height + margin.Bottom * 2);
            private Margin margin = new Margin(2);
            private StringFormat format;
            private Pen outline = Pens.SlateBlue;
            private Vector2 mouse => Main.MouseWorld;
            private bool clicked() => Main.mouseLeft && box.Contains((int)Main.MouseWorld.X + offX, (int)Main.MouseWorld.Y + offY);
            private bool hover() => padded.Contains((int)Main.MouseWorld.X + offX, (int)Main.MouseWorld.Y + offY);
            #endregion
            public static event EventHandler<ButtonEventArgs> ButtonClickEvent;
            public Button(Textbox parent, ButtonOption option, bool active = false)
            {
                this.text = Enum.GetName(typeof(ButtonOption), option);
                this.option = option;
                this.parent = parent;
                this.active = active;
                format = StringFormat.GenericTypographic;
                format.Alignment = StringAlignment.Center;
                format.Trimming = StringTrimming.Word;
            }
            public void DrawButton(Graphics graphics)
            {
                Brush brush = new SolidBrush(Color.DarkSlateGray);
                if (ticks-- > 0)
                {
                    brush = new SolidBrush(Color.SteelBlue);
                }
                graphics.FillRectangle(brush, padded);
                graphics.DrawRectangle(outline, padded);
                graphics.DrawString(text, font, Brushes.LightBlue, box, format);
            }
            public new void Update()
            {
                if (hover())
                {
                    outline = new Pen(Color.Blue);
                    outline.Width = 2;
                }
                else outline = Pens.SlateBlue;
                if (clicked())
                {
                    ticks = 3;
                    ButtonClickEvent?.Invoke(this, new ButtonEventArgs() 
                    { 
                        active = this.active,
                        option = this.option,
                        parent = this.parent
                    });
                }
            }
            public void Close()
            {
                parent?.Close();
            }
        }
        public class Textbox : Entity
        {
            public Textbox(Item item, Image heading, string name, string text, Vector2 position, int width, ButtonStyle style, bool active, int whoAmI)
            {
                this.item = item;
                this.heading = heading;
                this.name = name;
                this.text = text;
                this.position = position;
                this.width = width;
                this.style = style;
                this.active = active;
                this.whoAmI = whoAmI;
                this.height = 16;
                this.color = item.toolTip.textColor;
                this.ButtonInit();
            }
            public Textbox(Image heading, string name, string text, Vector2 position, int width, ButtonStyle style, bool active, int whoAmI)
            {
                this.heading = heading;
                this.name = name;
                this.text = text;
                this.position = position;
                this.width = width;
                this.style = style;
                this.active = active;
                this.whoAmI = whoAmI;
                this.height = 16;
                this.color = Color.Black;
                this.ButtonInit();
            }
            public Textbox(Image heading, string text, Vector2 position, int width, ButtonStyle style, bool active, int whoAmI)
            {
                this.heading = heading;
                this.text = text;
                this.position = position;
                this.width = width;
                this.style = style;
                this.active = active;
                this.whoAmI = whoAmI;
                this.height = 16;
                this.color = Color.Black;
                this.ButtonInit();
            }
            public Textbox(string text, Vector2 position, Rectangle box, int width, ButtonStyle style, bool active, int whoAmI)
            {
                this.text = text;
                this.position = position;
                this.width = width;
                this.style = style;
                this.active = active;
                this.whoAmI = whoAmI;
                this.rect = box;
                this.height = 16;
                this.color = Color.Black;
                this.ButtonInit();
            }
            readonly ButtonStyle style = ButtonStyle.None;
            private Image heading;
            public Item item;
            private Button[] option = new Button[3];
            Rectangle Center => new Rectangle(Main.WorldZero.X + Main.ScreenWidth / 3, Main.WorldZero.Y + Main.ScreenHeight / 2, width, height);
            private Rectangle rect;
            internal int i, j;
            private new Rectangle box => new Rectangle(Main.WorldZero.X + Main.ScreenWidth / 3 - rect.X, Main.WorldZero.Y + Main.ScreenHeight / 2 + rect.Y, width, height);
            internal Rectangle padded => Padded(i, j);
            internal Rectangle Padded(int i, int j) => new Rectangle(box.X - margin.Left + i, box.Y - margin.Top + j, box.Width + margin.Right * 2, box.Height + margin.Bottom * 2);
            public override int X => (int)position.X;
            public override int Y => (int)position.Y;
            private Margin margin = new Margin(10);
            private Font font = System.Drawing.SystemFonts.MessageBoxFont;
            private StringFormat format;
            private const int headerTextPadding = 4;
            private int headerFontHeight = 14;
            private int headerImageWidth = 64;
            bool init = false;
            public void Init(Graphics graphics)
            {
                if (!init)
                { 
                    format = StringFormat.GenericDefault;
                    format.Trimming = StringTrimming.None;
                    format.Alignment = StringAlignment.Near;
                    string output = "";
                    for (int i = 0; i < text.Length; i++)
                    { 
                        bool newLine = false;
                        var size = graphics.MeasureString(output += text[i], font);
                        if ((int)size.Width >= width - margin.Right * 2 || (newLine = text[i] == '\n'))
                        {
                            height += (int)size.Height / 2 + 1;
                            if (!newLine)
                            { 
                                output = "";
                                output += "\n";
                            }
                        }
                    }
                    init = true;
                }
            }
            private void ButtonInit()
            {
                switch (style)
                {
                    case ButtonStyle.None:
                        break;
                    case ButtonStyle.Ok:
                        option[0] = new Button(this, ButtonOption.Ok, true);
                        goto default;
                    case ButtonStyle.OkCancel:
                        option[1] = new Button(this, ButtonOption.Ok, true);
                        option[0] = new Button(this, ButtonOption.Cancel, true);
                        goto default;
                    case ButtonStyle.YesNo:
                        option[1] = new Button(this, ButtonOption.Yes, true);
                        option[0] = new Button(this, ButtonOption.No, true);
                        goto default;
                    case ButtonStyle.YesNoCancel:
                        option[2] = new Button(this, ButtonOption.Yes, true);
                        option[1] = new Button(this, ButtonOption.No, true);
                        option[0] = new Button(this, ButtonOption.Cancel, true);
                        goto default;
                    case ButtonStyle.EquipDropCancel:
                        option[2] = new Button(this, ButtonOption.Equip, true);
                        option[1] = new Button(this, ButtonOption.Drop, true);
                        option[0] = new Button(this, ButtonOption.Cancel, true);
                        goto default;
                    case ButtonStyle.PickupCancel:
                        option[1] = new Button(this, ButtonOption.Pickup, true);
                        option[0] = new Button(this, ButtonOption.Cancel, true);
                        goto default;
                    case ButtonStyle.UnequipCancel:
                        option[1] = new Button(this, ButtonOption.Unequip, true);
                        option[0] = new Button(this, ButtonOption.Cancel, true);
                        goto default;
                    default:
                        break;
                }
            }
            public new void Update()
            {
                for (int i = 0; i < option.Length; i++)
                {
                    if (option[i] != null && option[i].active)
                    {
                        option[i].index = i;
                        option[i].Update();
                    }
                }
            }
            public void Draw(Graphics graphics)
            {
                this.Init(graphics);
                if (active)
                { 
                    int offX = Main.ScreenX - Main.myPlayer.width;
                    int offY = Main.ScreenY - Main.myPlayer.height;
                    Rectangle _padded = new Rectangle(padded.X + offX, padded.Y + offY, padded.Width, padded.Height);
                    if (heading != null)
                    {
                        float ratio = heading.Width / heading.Height;
                        var header = new Rectangle(padded.Left, padded.Top - headerImageWidth - 4, headerImageWidth, (int)(headerImageWidth * ratio));
                        //  TODO get item color and draw
                        graphics.DrawImage(heading, header);
                        //Drawing.DrawScale(heading, new Vecto, header.Width, header.Height, graphics, Drawing.SetColor(Color.White));
                        graphics.DrawString(name, font, new SolidBrush(color), new PointF(header.Right + headerTextPadding, header.Bottom - headerFontHeight));
                    }
                    Rectangle _box = new Rectangle(box.X + offX, box.Y + offY, box.Width, box.Height);
                    Rectangle result = padded;
                    Rectangle result2 = box;
                    if (text.StartsWith("[Life]"))
                    { 
                        result = _padded;
                        result2 = _box;
                    }
                    graphics.FillRectangle(Brushes.DarkGray, result);
                    graphics.DrawRectangle(Pens.White, result);
                    graphics.DrawString(text, font, Brushes.Black, result2, format);
                    
                    for (int i = 0; i < option.Length; i++)
                    {
                        if (option[i] != null)
                        {
                            option[i].DrawButton(graphics);
                        }
                    }
                }
            }
            public void Close()
            {
                active = false;
                Main.textbox[whoAmI] = null;
                for (int i = 0; i < option.Length; i++)
                {
                    if (option[i] != null)
                    {
                        option[i].active = false;
                        option[i] = null;
                    }
                }
            }
        }
        public class Container
        {
            private const int Size = 64;
            private const int Padding = 24;
            internal static void DrawItems(IList<Item> list, Scroll bar, Graphics graphics)
            {
                const int offset = 6;
                int offX = 0;
                int offY = 0;
                int c = (int)bar.parent.X + offset + offX, r = (int)bar.parent.Y + offset + offY;
                foreach (Item i in list)
                {
                    if (i == null) continue;
                    int n = r - (int)(bar.value * bar.parent.Height);
                    i.DrawInventory(c, n, n < bar.parent.Height + bar.parent.Y - Padding && n > bar.parent.Y, graphics);
                    c += Size;
                    if (c > bar.parent.Width + bar.parent.X - Padding)
                    {
                        r += Size;
                        c = (int)bar.parent.X + offset;
                    }
                }
            }
        }
        public class Scroll : Entity
        {
            public float value;
            private float x => parent.Right - Width;
            private float y => parent.Top + parent.Height * value;
            public override int X => (int)x;
            public override int Y => (int)y;
            public Rectangle parent;
            public new Rectangle hitbox => new Rectangle(X, Y, Width, Height);
            public const int Width = 12;
            public const int Height = 32;
            private bool 
                clicked,
                flag;
            internal static void KbInteract(Scroll bar)
            {
                if (bar.parent.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y))
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        if (bar.value * (bar.parent.Height - Height) < bar.parent.Height - Height)
                        {
                            bar.value += 0.04f;
                        }
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        if (bar.value > 0f)
                        {
                            bar.value -= 0.04f;
                        }
                        else bar.value = 0f;
                    }
                }
            }
            internal static void MouseInteract(Scroll bar)
            {
                int offX = Main.myPlayer.width;
                int offY = Main.myPlayer.height;
                if (Main.mouseLeft && bar.hitbox.Contains((int)Main.MouseWorld.X - offX, (int)Main.MouseWorld.Y - offY))
                    bar.clicked = true;
                bar.flag = Main.LeftMouse();
                if (!Main.LeftMouse())
                    bar.clicked = false;
                if (bar.clicked && bar.flag)
                { 
                    Vector2 mouse = new Vector2(Main.MouseWorld.X, Main.MouseWorld.Y - bar.parent.Top - Height / 2 - offY);
                    bar.value = Math.Max(0f, Math.Min(mouse.Y / bar.parent.Height, 1f));
                }
            }
        }
    }
    public class Thumbnail : Entity
    {
        public Thumbnail[] icon;
        public Vector2 anchor;
        private int ticksMax = 12;
        private int ticks2 = 3;
        private bool flag;
        public new Rectangle hitbox => PlaceUI();
        public new Rectangle box => new Rectangle((int)anchor.X, (int)anchor.Y, width, height);
        public Image image;
        public Vector2 mouseWorld => Main.MouseWorld;
        public Vector2 mouseScreen => Main.MouseScreen;
        public static bool showDateTime = false;
        private const int ActiveY = 150;
        protected static Thumbnail selectedIndex;
        public Thumbnail()
        {
            scale = 0.5f;
            width = 48;
            height = 48;
            image = Asset<Image>.Request("temp");
        }
        public void Click()
        {
            switch (type)
            {
                case Type.None:
                    break;
                case Type.Generic:
                    showDateTime = !showDateTime;
                    break;
            }
            selectedIndex = this;
        }
        public void Update(int length = 10)
        {
            for (int i = 0; i < icon?.Length; i++)
            {
                var item = icon[i];
                if (item == null)
                    continue;
                int index = item.whoAmI - length / 2;
                if (mouseScreen.Y <= ActiveY)
                    item.flag = true;
                bool proximity = mouseScreen.Y <= ActiveY;
                if (item.flag)
                { 
                    if (proximity && item.ticks < ticksMax)
                    { 
                        item.anchor.X += index;
                        item.ticks++;
                    }
                    if (!proximity && item.ticks > 0)
                    { 
                        item.anchor.X -= index;
                        item.ticks--;
                    } 
                    if (item.ticks == 0)
                        item.flag = false;
                }
                int offset = 20;
                int offsetHalf = 10;
                Rectangle box = new Rectangle(item.box.X - Main.ScreenX - offsetHalf, item.box.Y - Main.ScreenY - offsetHalf, item.box.Width + offset, item.box.Height + offset);
                if (Main.mouseLeft && box.Contains((int)mouseWorld.X, (int)mouseWorld.Y))
                {
                    item.Click();
                    item.color = Color.Firebrick;
                }
                if (item.color != default && item.ticks2-- < 0)
                {
                    item.color = default;
                    item.ticks2 = 3;
                }
            }
        }
        public void Init(int length, int uiWidth)
        {
            icon = new Thumbnail[length];
            for (int i = 0; i < length; i++)
            {
                int half = icon.Length / 2;
                int index = i - half;
                icon[i] = new Thumbnail();
                icon[i].anchor = new Vector2(index * (half + icon[i].width + 4) + uiWidth / 2 + icon[i].width / 2, 60);
                icon[i].whoAmI = i;
                icon[i].type = 1;
            }
        }
        public void DrawUI(Graphics graphics)
        {
            int offX = Main.myPlayer.width * 2;
            int offY = Main.myPlayer.height;
            for (int i = 0; i < icon.Length; i++)
            {
                Rectangle h = new Rectangle(icon[i].hitbox.X + Main.ScreenX - offX, icon[i].hitbox.Y + Main.ScreenY - offY, icon[i].hitbox.Width, icon[i].hitbox.Height);
                graphics.DrawImage(image, h);
                if (icon[i].color != default || selectedIndex == icon[i])
                {
                    Pen pen = new Pen(Brushes.Firebrick);
                    pen.Width = 2;
                    
                    graphics.DrawRectangle(pen, h);
                }
            }
        }
        private Rectangle PlaceUI()
        {
            float scale = Math.Abs(Math.Max(Math.Min((float)Main.Distance(mouseScreen, anchor) / 100, 1f), 0f) - 1f);
            int w = width / 2;
            int h = height / 2;
            var v2 = anchor;
            int x = (int)(v2.X - (width / 2 * scale)) + width / 2 + Main.WorldZero.X;
            int y = (int)(v2.Y + 50f * scale) + Main.WorldZero.Y;
            return new Rectangle(x - w - (int)(w * scale), y - h - (int)(h * scale), (int)(width * (scale + 1f)), (int)(height * (scale + 1f)));
        }
        public sealed class Type
        {
            public const short
                None = 0,
                Generic = 1;
        }
    }
}
