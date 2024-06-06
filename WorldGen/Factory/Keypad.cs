using ArchaeaMod.Interface.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Button = tUserInterface.ModUI.Button;
using Terraria.ObjectData;
using System.Security.Policy;
using Terraria.GameContent.ItemDropRules;
using Composite = ArchaeaMod.Composite.Composite;
using System.Drawing;
using ArchaeaMod.NPCs;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;
using static tModPorter.ProgressUpdate;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria.Utilities;
using ArchaeaMod.Items.Tiles;
using rail;
using ArchaeaMod.Effects;

namespace ArchaeaMod.Structure
{
    internal class Keypad : ModTile
    {
        public override string Texture => $"ArchaeaMod/Gores/arrow";
        public static Code[] code = new Code[201];
        public bool 
            interact,
            display,
            send,
            init;
        public int
            i,
            j,
            x, 
            y,
            width,
            height,
            num2, 
            num3 = 1;
        public readonly float 
            Range = 16 * 10;
        private string
            complete = "";
        public Portal 
            begin = new Portal(),
            end = new Portal();
        public Code
            _lock;
        public Button[,] 
            input = new Button[3, 3];
        public TextBox[]
            textbox = new TextBox[4];
        private Color
            chainColor = Color.White;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.NotReallySolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(210, 110, 110));
            TileID.Sets.DisableSmartCursor[Type] = true;
            MineResist = 1f;
            MinPick = 45;
        }
        private Vector2 getEntrance(int i, int j)
        {
            return new Vector2(i * 16 + 8, j * 16 - Player.defaultHeight);
        }
        private Code GetLock(int i, int j)
        {
            return code.FirstOrDefault(t => t != null && t.portal.entrance == getEntrance(i, j));
        }
        private Code GetLockByExit(Vector2 exit)
        {
            return code.FirstOrDefault(t => t != null && t.portal.entrance == exit);
        }
        public override bool CanPlace(int i, int j)
        {
            for (int n = 0; n < Vector2.Distance(Main.player[Main.myPlayer].Center, new Vector2(i * 16 + 8, j * 16 + 8)); n += 2)
            {
                Vector2 v = ArchaeaNPC.AngleToSpeed(ArchaeaNPC.AngleTo(Main.player[Main.myPlayer].Center, new Vector2(i * 16 + 8, j * 16 + 8)), n);
                int x = (int)v.X / 16;
                int y = (int)v.Y / 16;
                if (Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].type])
                {
                    return false;
                }
            }
            return true;
        }
        public override void PlaceInWorld(int i, int j, Item item)
        {
            _lock = Code.NewLock(getEntrance(i, j));
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Code l = GetLock(i, j);
            if (l != null)
            { 
                l.active = false;
            }
        }
        public override bool RightClick(int i, int j)
        {
            this.i = i;
            this.j = j;
            if (_lock == null || !_lock.active)
            {
                PlaceInWorld(i, j, default);
            }
            if (Main.player[Main.myPlayer].Distance(getEntrance(i, j)) > Range / 3)
            {
                return false;
            }
            if (!init)
            {
                x = (int)(Main.screenWidth / 2f - 150);
                y = (int)(Main.screenHeight / 2f - 200);
                width = 300 - 20;
                height = 400;
                int num = 0;
                for (int n = 2; n >= 0; n--)
                {
                    for (int m = 0; m < 3; m++)
                    {
                        input[m, n] = new Button((++num).ToString(), new Rectangle(x + 10 * (m + 1) + m * 80, y + 10 * (n + 1) + n * 80, 80, 80), null) { active = true, drawMagicPixel = true };
                    }
                }
                for (int k = 0; k < textbox.Length; k++)
                {
                    textbox[k] = new TextBox(new Rectangle(x + 10 * (k + 1) + k * 60,  y + height - 90, 50, 50)) { active = true };
                }
                init = true;
            }
            Array.ForEach(textbox, t => t.text = "");
            display = Main.player[Main.myPlayer].InInteractionRange(i, j, TileReachCheckSettings.Simple);
            if (display)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen, Main.player[Main.myPlayer].Center);
            }
            return display;
        }
        public override bool PreDraw(int i, int j, SpriteBatch sb)
        {
            Texture2D tex = Fx.BasicArrow();//Mod.Assets.Request<Texture2D>("Gores/arrow").Value;
            Code here, there = default;
            here = GetLock(i, j);
            if (here != default)
            { 
                there = GetLockByExit(here.portal.exit);
            }
            if (here != default && there != default)
            {
                Fx.DrawTileOnScreen(tex, i, j, 16, 16, 0, 0, 0, here.connected ? Color.Green : Color.Red, here.portal.entrance.X < there.portal.entrance.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
                return false;
            }
            if (here != default)
            {
                Fx.DrawTileOnScreen(tex, i, j, 16, 16, 0, 0, 0, here.connected ? Color.Green : Color.Red, SpriteEffects.None);
                return false;
            }
            Fx.DrawTileOnScreen(tex, i, j, 16, 16, 0, 0, 0, Color.White, SpriteEffects.None);
            return false;
        }
        public void DrawKeyPad(SpriteBatch sb)
        {
            interact = Main.player[Main.myPlayer].Center.Distance(getEntrance(i, j)) < Range / 3;
            bool close = (!interact && display) || Main.playerInventory;
            if (close)
            {
                Clear();
                return;
            }
            _lock = GetLock(i, j);
            if (_lock != null && !_lock.connected && display)
            {
                int m       = (int)Main.MouseWorld.X; 
                int n       = (int)Main.MouseWorld.Y;
                Rectangle r = new Rectangle(m, n, 16, 16);
                for (int l = 0; l < code.Length; l++)
                {
                    if (code[l] != null && code[l].portal.Hitbox(false).Intersects(r) && code[l] != _lock)
                    {
                        ArchaeaNPC.DrawChainColored(Mod.Assets.Request<Texture2D>("Gores/chain").Value, sb, _lock.portal.entrance, code[l].portal.entrance, code[l].connected ? Color.Red : Color.White);
                        if (Main.mouseLeft)
                        {
                            if (!code[l].connected)
                            {
                                _lock.portal.exit = code[l].portal.entrance;
                                _lock.connected = true;
                                var c = GetLockByExit(_lock.portal.exit);
                                c.portal.exit = _lock.portal.entrance;
                                c.connected = true;
                            }
                            else
                            {
                                SoundEngine.PlaySound(SoundID.PlayerHit, code[l].portal.exit);
                            }
                            break;
                        }
                        return;
                    }
                }
                ArchaeaNPC.DrawChainColored(Mod.Assets.Request<Texture2D>("Gores/chain").Value, sb, _lock.portal.entrance, Main.MouseWorld, Color.White);
                return;
            }
            if (close)
            {
                Clear();
                SoundEngine.PlaySound(SoundID.MenuClose, Main.player[Main.myPlayer].Center);
            }
            if (display && interact)
            {
                drawKeyPad(sb);
            }
        }
        public void drawKeyPad(SpriteBatch sb)
        {
            if (_lock == null)
            {
                _lock = GetLock(i, j);
                return;
            }
            x = (int)(Main.screenWidth / 2 - 150);
            y = (int)(Main.screenHeight / 2 - 200);
            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(x, y, width, height), Color.PaleVioletRed);
            int num = 0;
            for (int n = 2; n >= 0; n--)
            {
                for (int m = 0; m < 3; m++)
                {
                    num++;
                    string t = num.ToString();
                    Rectangle idk = new Rectangle(input[m, n].box.X, input[m, n].box.Y, input[m, n].box.Width, input[m, n].box.Height);
                    input[m, n].Draw(input[m, n].HoverOver(idk));
                    if (input[m, n].LeftClick(idk))
                    {
                        if (num3 == 0 && input[m, n].reserved == 0)
                        {
                            input[m, n].reserved = 1;
                            if (!_lock.Complete())
                            {
                                if (num2 < 4)
                                { 
                                    complete += t;
                                    textbox[num2].text = _lock.Obsfucated(ref t)[0].ToString();
                                    num2++;
                                }
                                else
                                {
                                    Clear();
                                }
                            }
                        }
                    }
                    else
                    {
                        input[m, n].reserved = 0;
                    }
                }
            }
            num3 = 0;
            for (int m = 0; m < 4; m++)
            {
                sb.Draw(TextureAssets.MagicPixel.Value, textbox[m].box, Color.White * 0.5f);
                textbox[m].DrawText();
            }
            if (_lock != null && complete != string.Empty && _lock.Compare(complete))
            {
                Clear();
                Main.player[Main.myPlayer].Teleport(_lock.portal.exit);
                _lock = GetLockByExit(_lock.portal.exit);
            }
        }
        private void Clear()
        {
            for (int k = 0; k < textbox.Length; k++)
            {
                if (textbox[k] != null)
                { 
                    textbox[k].text = string.Empty;
                }
            }
            _lock?.Clear();
            num2 = 0;
            display = false;
            complete = "";
        }
    }
    internal struct Portal
    {
        public string code;
        public Vector2 entrance, exit;
        public Rectangle Hitbox(bool getExit)
        {
            Vector2 v = entrance;
            if (getExit)
            {
                v = this.exit;
            }
            return new Rectangle((int)v.X, (int)v.Y, 16, 16);
        }
        public static bool operator !=(Portal a, Portal b)
        {
            return a.entrance != b.entrance;
        }
        public static bool operator ==(Portal a, Portal b)
        {
            return a.entrance == b.entrance;
        }
        public override bool Equals(object obj)
        {
            return this == (Portal)obj;
        }
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
    internal class Code
    {
        public bool connected;
        public bool newInput;
        public bool active;
        public int whoAmI;
        public string code = "";
        public string compare = "";
        public Portal portal;
        public static Code NewLock(Vector2 position)
        {
            int num = 200;
            for (int i = 0; i < Keypad.code.Length; i++)
            {
                if (Keypad.code[i] == null || !Keypad.code[i].active)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    return default;
                }
            }
            Keypad.code[num] = new Code();
            Keypad.code[num].whoAmI = num;
            Keypad.code[num].active = true;
            Keypad.code[num].portal = new Portal();
            Keypad.code[num].portal.entrance = position;
            Keypad.code[num].connected = false;
            Keypad.code[num].Initialize();
            return Keypad.code[num];
        }
        private Code GetLockByExit(Vector2 exit)
        {
            return Keypad.code.FirstOrDefault(t => t != null && t.portal.entrance == exit);
        }
        private void Initialize()
        {
            if (string.IsNullOrEmpty(compare) && string.IsNullOrEmpty(code))
            { 
                code = "";
                compare = "";
            }
        }
        public bool Complete()
        {
            if (code.Length >= 4 && compare.Length == 0)
            {
                portal.code = code;
                compare = code;
                return true;
            }
            return false;
        }
        public void Clear()
        {
            code = string.Empty;
        }
        public string Obsfucated(ref string add)
        {
            Initialize();
            code += add;
            add = "";
            int len = code.Length + 1;
            string result = "";
            for (int i = 0; i < len; i++)
            {
                result += "*";
            }
            return result;
        }
        public bool Compare(string input)
        {
            return EndPointVerify(input) && compare == input;
        }
        public bool EndPointVerify(string input)
        {
            return GetLockByExit(portal.exit).compare == input;
        }
        public static bool operator !=(Code a, Code b)
        {
            return a?.portal != b?.portal;
        }
        public static bool operator ==(Code a, Code b)
        {
            return a?.portal == b?.portal;
        }
        public override bool Equals(object obj)
        {
            return this == (Code)obj;
        }
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
