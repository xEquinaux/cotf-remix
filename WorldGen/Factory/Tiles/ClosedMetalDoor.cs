using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ObjectData;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace ArchaeaMod.Structure.Tiles
{
    public class ClosedMetalDoor : ModTile
    {
        public override string Texture => "ArchaeaMod/WorldGen/Factory/Tiles/ClosedMetalDoor";
        public int tileTime = 300;
        public override void SetStaticDefaults()
        {
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileSolid[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;
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

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.75f;
            g = 0.5f;
            b = 0.25f;
        }
        public override void HitWire(int i, int j)
        {
            Wiring.TripWire(i, j - 3, 1, 1);
            int type = ArchaeaWorld.factoryMetalDoorOpening;
            ModContent.GetInstance<ClosedMetalDoor>().tileTime = 600;
            Main.tile[i, j].type = (ushort)type;
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
