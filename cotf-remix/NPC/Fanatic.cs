using cotf.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace cotf
{
    public class Fanatic : Npc
    {
        public override void SetDefaults()
        {
            name = "Fanatic";
            width = 34;
            height = 42;
            lifeMax = 80;
            defense = 10;
            damage = 0;
            speed = 0f;
            hostile = true;
            iFramesMax = 45;
            knockBack = 1f;
            frameCount = 3;
            frameHeight = 58;
        }
        private int direction = 1;
        public int timer;
        public int target = 0;
        public int maxAttacks
        {
            get { return 4; }
        }
        public int DustType;
        public Vector2 move;
        public Player npcTarget
        {
            get { return Main.player[target]; }
        }
        private bool init;
        private float compensate
        {
            get { return (float)(npcTarget.velocity.Y * (0.017d * 2.5d)); }
        }
        private bool fade;
        public void PreAI()
        {
            scale = 0.2f;
        }
        public override void AI()
        {
            base.AI();

            if (!active) return;

            int attackTime = 180 + 90 * maxAttacks;
            if (timer++ > 60 + attackTime)
                timer = 0;
            
            if (!init)
            {
                //DustType = 6;
                //var dusts = ArchaeaNPC.DustSpread(NPC.Center, 1, 1, DustType, 10);
                //foreach (Dust d in dusts)
                //    d.noGravity = true;
                PreAI();
                init = true;
            }
            if (!fade)
            {
                if (alpha > 0)
                    alpha -= 255 / 60;
            }
            else
            {
                if (alpha < 255)
                    alpha += 255 / 50;
                else
                {
                    timer = attackTime + 50;
                    move = Helper.FindAny(this, npcTarget, true);
                    if (move != Vector2.Zero)
                    {
                        //SyncNPC(move.X, move.Y);
                        //var dusts = ArchaeaNPC.DustSpread(NPC.Center - new Vector2(NPC.width / 4, NPC.height / 4), NPC.width / 2, NPC.height / 2, DustType, 10, 2.4f);
                        //foreach (Dust d in dusts)
                        //    d.noGravity = true;
                        fade = false;
                        timer = 0;
                    }
                }
            }
            if (timer > 180 && timer <= attackTime)
            {
                OrbGrow();
                if (timer >= 180 + 60 && timer % 90 == 0)
                    Attack();
            }
            if (timer >= attackTime)
                fade = true;
            else fade = false;
        }
        
        private float weight;
        public void Attack()
        {
            int proj = Projectile.NewProjectile(Center + new Vector2(width * 0.35f * direction, -4f), Helper.AngleToSpeed(Helper.AngleTo(this.Center, npcTarget.Center) + compensate, 4f), Helper.AngleTo(this.Center, npcTarget.Center), ProjectileID.Fireball, this);
            Main.projectile[proj].timeLeft = 300;
            Main.projectile[proj].friendly = false;
            //Main.projectile[proj].tileCollide = false;
            scale = 0.2f;
        }
        public void OrbGrow()
        {
            direction = position.X < npcTarget.position.X ? 1 : -1;
            //NPC.spriteDirection = NPC.direction;
            scale += 0.03f;
            //energy = Dust.NewDustDirect(NPC.Center + new Vector2(NPC.width * 0.35f * NPC.direction, -4f), 3, 3, DustType, 0f, -0.2f, 0, default(Color), scale);
            //energy.noGravity = true;
        }
        //public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        //{
        //    return alpha < 20;
        //}

        //public void SyncNPC(float x, float y)
        //{
        //    if (Main.netMode != 0)
        //        NPC.netUpdate = true;
        //    else
        //    {
        //        NPC.position = new Vector2(x, y);
        //    }
        //}
        //public override void SendExtraAI(BinaryWriter writer)
        //{
        //    writer.WriteVector2(move);
        //}
        //public override void ReceiveExtraAI(BinaryReader reader)
        //{
        //    NPC.position = reader.ReadVector2();
        //}

        public override void FrameAnimate(ref int frameTicks, int interval, int startFrame)
        {
            int attackPhase = 90 * maxAttacks;
            if (timer < 180 || timer >= 180 + attackPhase)
                frame = 0;
            if (timer > 180 && timer < 180 + attackPhase && timer % 30 == 0)
                frame++;
            if (frame == frameCount)
                frame = startFrame;
        }

        //public override void ModifyNPCLoot(NPCLoot npcLoot)
        //{
        //    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Merged.Items.Materials.magno_core>(), 10));
        //    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Merged.Items.magno_book>(), 3));
        //    npcLoot.Add(ItemDropRule.ByCondition(new Items.ArchaeaModeDrop(), ModContent.ItemType<Merged.Items.Armors.ancient_shockhelmet>(), 13));
        //    npcLoot.Add(ItemDropRule.ByCondition(new Items.ArchaeaModeDrop(), ModContent.ItemType<Merged.Items.Armors.ancient_shockplate>(), 13));
        //    npcLoot.Add(ItemDropRule.ByCondition(new Items.ArchaeaModeDrop(), ModContent.ItemType<Merged.Items.Armors.ancient_shockgreaves>(), 13));
        //}
        //public override float SpawnChance(NPCSpawnInfo spawnInfo)
        //{
        //    bool MagnoBiome = spawnInfo.Player.GetModPlayer<ArchaeaPlayer>().MagnoBiome;
        //    return MagnoBiome ? 0.2f : 0f;
        //}
    }
}