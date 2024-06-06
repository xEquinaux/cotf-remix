using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ArchaeaMod.Structure.Tiles
{
    public class MetalDoorOpening : ModTile
    {
        public override string Texture => "ArchaeaMod/WorldGen/Factory/Tiles/MetalDoorOpening";
        public override void SetStaticDefaults()
        {
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            Main.tileFrame[Type] = 4;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileNoSunLight[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
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

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
        }
        public override bool PreDraw(int i, int j, SpriteBatch SB)
        {
            if (ModContent.GetInstance<ClosedMetalDoor>().tileTime-- <= 576)
            { 
                Main.tile[i, j].type = ArchaeaWorld.factoryMetalDoorOpen;
            }
            return false;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 5)
            {
                frameCounter = 0;
                frame++;
                if (frame > 4)
                {
                    frame = 0;
                }
            }
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