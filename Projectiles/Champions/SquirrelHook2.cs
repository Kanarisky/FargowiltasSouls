using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class SquirrelHook2 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_13";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squirrel Hook");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.aiStyle = -1;
            projectile.timeLeft = 420;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().ImmuneToGuttedHeart = true;
        }

        public override bool CanDamage()
        {
            return projectile.ai[1] == 1;
        }

        public override void AI()
        {
            if (!(projectile.ai[0] > -1 && projectile.ai[0] < Main.maxNPCs && Main.npc[(int)projectile.ai[0]].active
                && Main.npc[(int)projectile.ai[0]].type == ModContent.NPCType<NPCs.Champions.TimberChampionHead>()
                && (Main.npc[(int)projectile.ai[0]].ai[0] == 7 || Main.npc[(int)projectile.ai[0]].ai[0] == 8)))
            {
                projectile.Kill();
                return;
            }

            NPC npc = Main.npc[(int)projectile.ai[0]];

            if (npc.ai[0] == 8) //deal damage
            {
                projectile.ai[1] = 1;
                projectile.localAI[0] = npc.Center.X;
                projectile.localAI[1] = npc.Center.Y;

                const int increment = 100; //dust
                int distance = (int)projectile.Distance(npc.Center);
                Vector2 direction = projectile.DirectionTo(npc.Center);
                for (int i = 0; i < distance; i += increment)
                {
                    float offset = i + Main.rand.NextFloat(-increment, increment);
                    if (offset < 0)
                        offset = 0;
                    if (offset > distance)
                        offset = distance;
                    int d = Dust.NewDust(projectile.Center + direction * offset,
                        projectile.width, projectile.height, 92, 0f, 0f, 0, default(Color), 1.5f);
                    Main.dust[d].noGravity = true;
                }
            }

            if (projectile.Distance(npc.Center) > 1500 + npc.Distance(Main.player[npc.target].Center))
                projectile.velocity = Vector2.Zero;

            if (!projectile.tileCollide && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                projectile.tileCollide = true;

            projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projectile.ai[1] == 1)
            {
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                    new Vector2(projectile.localAI[0], projectile.localAI[1]), projectile.Center);
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity = Vector2.Zero;
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[0] > -1 && projectile.ai[0] < Main.maxNPCs && Main.npc[(int)projectile.ai[0]].active
                && Main.npc[(int)projectile.ai[0]].type == ModContent.NPCType<NPCs.Champions.TimberChampionHead>())
            {
                Texture2D texture = Main.chainTexture;
                Vector2 position = projectile.Center;
                Vector2 mountedCenter = Main.npc[(int)projectile.ai[0]].Center;
                Rectangle? sourceRectangle = new Rectangle?();
                Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                float num1 = texture.Height;
                Vector2 vector24 = mountedCenter - position;
                float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
                bool flag = true;
                if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                    flag = false;
                if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                    flag = false;
                while (flag)
                    if (vector24.Length() < num1 + 1.0)
                    {
                        flag = false;
                    }
                    else
                    {
                        Vector2 vector21 = vector24;
                        vector21.Normalize();
                        position += vector21 * num1;
                        vector24 = mountedCenter - position;
                        Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                        color2 = projectile.GetAlpha(color2);
                        Main.spriteBatch.Draw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0.0f);
                    }
            }

            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = SpriteEffects.None;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}