using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Core.Utils;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ArchaeaMod.Structure.Tiles
{
    public class MetalDoorOpen : ModTile
    {
        bool init;
        public override string Texture => "ArchaeaMod/WorldGen/Factory/Tiles/MetalDoorOpen";
        public override void SetStaticDefaults()
        {
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileNoSunLight[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorValidTiles = new int[] 
            { 
                ArchaeaWorld.factoryBrick, 
                ArchaeaWorld.factoryMetalDoor,
                ArchaeaWorld.factoryMetalDoorOpen,
                ArchaeaWorld.factoryMetalDoorOpening
            };
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(200, 150, 100));
            MineResist = 1f;
            MinPick = 1000;
        }
        public override void PostDraw(int i, int j, SpriteBatch SB)
        {
            if (!init)
            {
                //SoundEngine.PlaySound(new SoundStyle("Sounds/Custom/DoorOpening"), new Vector2(i * 16, j * 16));
                init = true;
            }
            if (ModContent.GetInstance<ClosedMetalDoor>().tileTime <= 0)
            {
                //SoundEngine.PlaySound(new SoundStyle("Sounds/Custom/DoorClosing"), new Vector2(i * 16, j * 16));
                Main.tile[i, j].type = ArchaeaWorld.factoryMetalDoor;
            }
            else ModContent.GetInstance<ClosedMetalDoor>().tileTime--;
        }

        public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
        {
            return false;
        }
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return false;
        }
        public override bool CanDrop(int i, int j)
        {
            return false;
        }
    }
}