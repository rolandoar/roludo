using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace roludo
{
    public static class Scenographer
    {
        public static scene activeScene = new scene();
        public static List<scene> scenes = new List<scene>();

        public static void populatScenes()
        {
            //activeScene.onLoad = ()
            //activeScene = new ruko();
            //activeScene = new fiji();
            activeScene = new banda();
        }

    }

    public class scene
    {
        public virtual void onLoad()
        {

        }
        public virtual void onRender()
        {

        }
        public virtual void onUpdate(double time)
        {

        }
        public virtual void mouseMgmgt()
        {

        }
        public virtual void keyboardMgmt()
        {

        }
        public virtual void unLoad()
        {

        }


    }

    public class fiji: scene
    {
        List<square> bricks;
        List<square> balls;
        square player;

        public override void  onLoad()
        {
            // initialize all objects
            bricks = new List<square>();
            balls = new List<square>();
            player = new square(new Vector2 { X = 50, Y = 5 }, new Vector2 {X = 18, Y = 2 });

            balls.Add(new square(new Vector2 { X = 50, Y = 7 }, new Vector2 {X = 1, Y = 1 }, velocity_0 :new Vector2 { X = 80f * (float) Globals.rnd.NextDouble() - 40, Y = 30 }));

            // make line of bricks
            for (float i = 4; i <= 97; i += 6)
            {
                for (float j = 45; j <= 55; j += 3)
                {
                    bricks.Add(new square(new Vector2{X = i, Y = j}, new Vector2{X = 5f, Y = 2f}));
                }
            }

            //make fiji bricks
            foreach (Vector2 position in Fijis.fijiLocs)
            {
                bricks.Add(new square(position, new Vector2 { X = 5f, Y = 2f }));
            }
        }

        public override void onRender()
        {
            player.RenderToScreen();
            foreach (square b in bricks)
            {
                b.RenderToScreen();
            }
            foreach (square b in balls)
            {
                b.RenderToScreen();
            }

        }

        public override void onUpdate(double time)
        {
            KeyboardState Kboard = OpenTK.Input.Keyboard.GetState();
            if (Kboard[Key.Escape]) { Globals.ExitFlag = true; }
            if (player.getSides().X > 0 && (Kboard[Key.A] || Kboard[Key.Left]))
            {
                player.velocity.X = -50f;
            }
            else if (player.getSides().Z < 100 && (Kboard[Key.D] || Kboard[Key.Right]))
            {
                player.velocity.X = 50f;
            }
            else
            {
                player.velocity.X = 0;
            }
            collisionManagement();

            player.update(time);
            foreach (square b in bricks)
            {
                b.update(time);
            }
            foreach (square b in balls)
            {
                b.update(time);
            }
            // check brick death
            for (int k = bricks.Count - 1; k >= 0; k--)
            {
                if (bricks[k].hp == 0)
                {
                    if (Globals.rnd.NextDouble() <Fijis.spawnChance)
                    {
                        Vector2 velocity = Fijis.RandomVector(40);
                        velocity.Y = -40f;
                        velocity.NormalizeFast();
                        velocity *= 40f;
                        balls.Add(new square(bricks[k].center, new Vector2 { X = 1, Y = 1 }, velocity_0: velocity));

                    }
                    bricks.RemoveAt(k);
                }
            }            
            // check ball death
            for (int k = balls.Count - 1; k >= 0; k--)
            {
                if (balls[k].hp == 0)
                {
                    balls.RemoveAt(k);
                }
            }
            if (balls.Count == 0) { Globals.ExitFlag = true; }
            if (bricks.Count == 0) 
            {
                Scenographer.activeScene = new ruko();
                Scenographer.activeScene.onLoad();
            }

        }

        public void collisionManagement()
        {
            for (int i = balls.Count - 1; i >= 0; i--)
            {
                // check side collision
                Vector4 b = balls[i].getSides();
                if (b.X <= 0 || b.Z >= 100 || player.collision(b).X == 1 || player.collision(b).Z == 1) 
                {
                    balls[i].velocity.X *= -1;
                    if (b.X < 0) { balls[i].center.X = 0 + balls[i].size.X/2; }
                    if (b.Z > 100) { balls[i].center.X = 100 - balls[i].size.X / 2; }
                    if (player.collision(b).X == 1) { balls[i].center.X = player.getSides().X - balls[i].size.X / 2; }
                    if (player.collision(b).Z == 1) { balls[i].center.X = player.getSides().Z + balls[i].size.X / 2; }
                }

                // check top or paddle collision
                if (b.Y >= 100 || player.collision(b).Y == 1) 
                {
                    if (b.Y > 100) 
                    { 
                        balls[i].velocity.Y *= -1;
                        balls[i].center.Y = 100 - balls[i].size.Y / 2; 
                    }
                    if (player.collision(b).Y == 1)
                    {
                        balls[i].velocity.Y *= -1;
                        if (Math.Sign(balls[i].velocity.X * player.velocity.X) > 0)
                        {
                            balls[i].velocity.X *= 1.5f;
                        }
                        if (Math.Sign(balls[i].velocity.X * player.velocity.X) < 0)
                        {
                            balls[i].velocity.X *= 0.5f;
                        }
                        balls[i].center.Y = player.getSides().Y + balls[i].size.Y / 2; 
                    }
                }

                //check bottom collision
                if (b.Y <= 0) { balls[i].hp = 0; }

                // check brick collision
                for (int k = bricks.Count - 1; k >= 0; k--)
                {
                    Vector4 brickCollision = bricks[k].collision(b);
                    if (brickCollision.X == 1 || brickCollision.Z == 1)
                    {
                        bricks[k].hp = 0;
                        balls[i].velocity.X *= -1;
                    }
                    if (brickCollision.Y == 1 || brickCollision.W == 1)
                    {
                        bricks[k].hp = 0;
                        balls[i].velocity.Y *= -1;
                    }
                }

            }
        }
    }

    public class ruko : scene
    {
        List<square> enemies;
        List<square> bullets;
        square player;

        public override void onLoad()
        {
            enemies = new List<square>();
            bullets = new List<square>();
            player = new square(new Vector2 { X = 50, Y = 5 }, new Vector2 { X = 6, Y = 4 }, color_0: Color.CadetBlue);

        }

        public override void onRender()
        {
            player.RenderToScreen();
            foreach (square b in enemies)
            {
                b.RenderToScreen();
            }
            foreach (square b in bullets)
            {
                b.RenderToScreen();
            }

        }

        public override void onUpdate(double time)
        {
            // collision Management
            collisionManagement();
            //cooldown update
            Rukos.shotCooldownCounter -= time;

            //
            KeyboardState Kboard = OpenTK.Input.Keyboard.GetState();
            if (Kboard[Key.Escape]) { Globals.ExitFlag = true; }

            if (player.getSides().X > 0 && (Kboard[Key.A] || Kboard[Key.Left]))
            {
                player.velocity.X = -60f;
            }
            else if (player.getSides().Z < 100 && (Kboard[Key.D] || Kboard[Key.Right]))
            {
                player.velocity.X = 60f;
            }
            else
            {
                player.velocity.X = 0;
            }

            if (player.getSides().W > 0 && (Kboard[Key.S] || Kboard[Key.Down]))
            {
                player.velocity.Y = -60f;
            }
            else if (player.getSides().Y < 100 && (Kboard[Key.W] || Kboard[Key.Up]))
            {
                player.velocity.Y = 60f;
            }
            else
            {
                player.velocity.Y = 0;
            }

            if (Kboard[Key.Space] && Rukos.shotCooldownCounter <= 0)
            {
                Rukos.shotCooldownCounter = Rukos.shotCooldown;
                if (Rukos.waveCounter < 3)
                {
                    square bullet = new square(position_0: new Vector2 { X = player.center.X, Y = player.getSides().Y }, size_0: new Vector2 { X = 0.25f, Y = 1.5f }, color_0: Color.OrangeRed, velocity_0: new Vector2 { X = 0, Y = 50f });
                    bullets.Add(bullet);
                }
                else if (Rukos.waveCounter < 6)
                {
                    square bullet01 = new square(position_0: new Vector2 { X = player.center.X - 1f, Y = player.getSides().Y }, size_0: new Vector2 { X = 0.5f, Y = 1f }, color_0: Color.OrangeRed, velocity_0: new Vector2 { X = 0, Y = 50f });
                    square bullet02 = new square(position_0: new Vector2 { X = player.center.X + 1f, Y = player.getSides().Y }, size_0: new Vector2 { X = 0.5f, Y = 1f }, color_0: Color.OrangeRed, velocity_0: new Vector2 { X = 0, Y = 50f });
                    bullets.Add(bullet01);
                    bullets.Add(bullet02);
                }
                else
                {
                    square bullet01 = new square(position_0: new Vector2 { X = player.center.X - 1.5f, Y = player.getSides().Y }, size_0: new Vector2 { X = 0.75f, Y = 0.85f }, color_0: Color.OrangeRed, velocity_0: new Vector2 { X = 0, Y = 50f });
                    square bullet02 = new square(position_0: new Vector2 { X = player.center.X, Y = player.getSides().Y }, size_0: new Vector2 { X = 0.75f, Y = 0.85f }, color_0: Color.OrangeRed, velocity_0: new Vector2 { X = 0, Y = 50f });
                    square bullet03 = new square(position_0: new Vector2 { X = player.center.X + 1.5f, Y = player.getSides().Y }, size_0: new Vector2 { X = 0.75f, Y = 0.85f }, color_0: Color.OrangeRed, velocity_0: new Vector2 { X = 0, Y = 50f });
                    bullets.Add(bullet01);
                    bullets.Add(bullet02);
                    bullets.Add(bullet03);
                }
            }

            player.update(time);
            foreach (square b in bullets)
            {
                b.update(time);
            }
            foreach (square b in enemies)
            {
                b.update(time);
            }

            // collision Management
            collisionManagement();
            enemyGenerator(time);

            // check enemy death
            for (int k = enemies.Count - 1; k >= 0; k--)
            {
                if (enemies[k].hp <= 0)
                {
                    enemies.RemoveAt(k);
                }
            }

            // check bullet death
            for (int k = bullets.Count - 1; k >= 0; k--)
            {
                if (bullets[k].hp <= 0)
                {
                    bullets.RemoveAt(k);
                }
            }

            if (player.hp <= 0)
            {
                Globals.ExitFlag = true;
            }

        }
        
        public  void collisionManagement()
        {
            // check if bullets hit enemies
            for (int i = 0; i < bullets.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++ )
                {
                    Vector4 collision = enemies[j].collision(bullets[i].getSides());
                    if (collision.Length > 0 || enemies[j].isInside(bullets[i].center))
                    {
                        enemies[j].hp -= bullets[i].hp;
                        bullets[i].hp = 0;
                    }
                }
                if (bullets[i].center.Y > 100)
                {
                    bullets[i].hp = 0;
                }

            }

            //check if player collides with enemies
            for (int j = 0; j < enemies.Count; j ++)
            {
                Vector4 collision = enemies[j].collision(player.getSides());
                if (collision.Length > 0)
                {
                    player.hp = 0;
                }
            }
        }

        public void enemyGenerator(double time)
        {
            // manage wave increase
            Rukos.increaseCooldownCounter -= time;
            if (Rukos.increaseCooldownCounter < 0)
            {
                Rukos.waveCounter++;
                Rukos.increaseCooldownCounter = Rukos.increaseCooldown;
                Rukos.enemySpeed += 10f;
                Rukos.shotCooldown *= 0.75f;
                Rukos.enemyCooldown *= 0.75f;
                Rukos.enemySize = new Vector2 { X = Rukos.enemySize.X + 2, Y = Rukos.enemySize.Y + 1 };
            }

            Rukos.enemyCooldownCounter -= time;
            if (Rukos.enemyCooldownCounter < 0)
            {
                Rukos.enemyCooldownCounter = Rukos.enemyCooldown;
                float Xposition = 10 + (float)Rukos.rnd.NextDouble() * 80;
                square enemy = new square(position_0: new Vector2 { X = Xposition, Y = 110f }, size_0: Rukos.enemySize, color_0: Color.Violet, velocity_0: new Vector2 { X = 0, Y = -Rukos.enemySpeed });
                enemies.Add(enemy);
            }


        }
    }

    public class banda : scene
    {
        List<square> enemies;
        List<square> bullets;
        square player;
		square ground;
		arc crossHairs;

        public override void onLoad()
        {
            enemies = new List<square>();
			ground = new square (new Vector2 {X = 50, Y = 4}, new Vector2 {X = 100, Y = 8}, color_0: Color.BurlyWood);
            bullets = new List<square>();
            player = new square(new Vector2 { X = 50, Y = 14 }, new Vector2 { X = 3, Y = 12 }, color_0: Color.CadetBlue);
			crossHairs = new arc(new Vector2 { X = 50, Y = 50 }, new Vector2 { X= 1f, Y= 0f}, 2f, color_0:  Color.MistyRose);

        }

        public override void onRender()
        {
			ground.RenderToScreen ();
            player.RenderToScreen();
			crossHairs.RenderToScreen ();
            foreach (square b in enemies)
            {
                b.RenderToScreen();
            }
            foreach (square b in bullets)
            {
                b.RenderToScreen();
            }

        }

        public override void onUpdate(double time)
        {
            // collision Management
            collisionManagement();
            //cooldown update
            Rukos.shotCooldownCounter -= time;

			//MouseState Mus = OpenTK.Input.Mouse.GetState ();

			//crossHairs.center = new Vector2() {X = Mus.X * Globals.Width - 50, Y = Mus.Y * Globals.Height - 50};

			Vector2 mus = game.getMouse ();
			crossHairs.center = new Vector2() {X = mus.X * Globals.Width, Y = mus.Y * Globals.Height  };
			if (crossHairs.center.X < 0) {crossHairs.center.X = 0;}
			if (crossHairs.center.X > 100) {crossHairs.center.X = 100;}
			if (crossHairs.center.Y < 0) {crossHairs.center.Y = 0;}
			if (crossHairs.center.Y > 100) {crossHairs.center.Y = 100;}

			crossHairs.update (time, player.center, crossHairs.center);
            //
            KeyboardState Kboard = OpenTK.Input.Keyboard.GetState();
			if (Kboard [Key.B]) 
			{
				Console.WriteLine ("Mus X = " + mus.X.ToString());
				Console.WriteLine ("Mus Y = " + mus.Y.ToString());
				Console.WriteLine ("Ratio = " + Globals.Ratio.ToString());
				Console.WriteLine ("Height= " + Globals.Height.ToString());
				Console.WriteLine ("Width = " + Globals.Width.ToString());
				Console.WriteLine ("MusX' = " + (mus.X * Globals.Ratio).ToString());
				Console.WriteLine ("MusY' = " + (mus.Y * Globals.Ratio).ToString());

			}
			if (Kboard [Key.C]) 
			{
				Console.WriteLine ("C ArcLength = " + (180 * crossHairs.arcLength / Math.PI).ToString());
				Console.WriteLine ("C ArcDirection = " + (180 * crossHairs.angleDirection / Math.PI).ToString());
				Console.WriteLine ("C leftEnd = " + crossHairs.leftEnd.ToString());
				Console.WriteLine ("C rightEnd = " + crossHairs.leftEnd.ToString());

			}

            if (Kboard[Key.Escape]) { Globals.ExitFlag = true; }

            if (player.getSides().X > 0 && (Kboard[Key.A] || Kboard[Key.Left]))
            {
                player.velocity.X = -60f;
            }
            else if (player.getSides().Z < 100 && (Kboard[Key.D] || Kboard[Key.Right]))
            {
                player.velocity.X = 60f;
            }
            else
            {
                player.velocity.X = 0;
            }

			if (player.getSides ().W <= ground.getSides ().Y && (Kboard [Key.W] || Kboard [Key.Up])) {
				player.velocity.Y = 120f;
			} 
			else if (player.getSides ().W > ground.getSides ().Y) {
				player.velocity.Y *= 0.95f;
				player.velocity.Y -= 12.0f;
			} 
			else 
			{
				player.velocity.Y = 0f;
			}

            if (Kboard[Key.Space] && Rukos.shotCooldownCounter <= 0)
            {
                Rukos.shotCooldownCounter = Rukos.shotCooldown;
                if (Rukos.waveCounter < 3)
                {
                    square bullet = new square(position_0: new Vector2 { X = player.center.X, Y = player.getSides().Y }, size_0: new Vector2 { X = 0.25f, Y = 1.5f }, color_0: Color.OrangeRed, velocity_0: new Vector2 { X = 0, Y = 50f });
                    bullets.Add(bullet);
                }
                else if (Rukos.waveCounter < 6)
                {
                    square bullet01 = new square(position_0: new Vector2 { X = player.center.X - 1f, Y = player.getSides().Y }, size_0: new Vector2 { X = 0.5f, Y = 1f }, color_0: Color.OrangeRed, velocity_0: new Vector2 { X = 0, Y = 50f });
                    square bullet02 = new square(position_0: new Vector2 { X = player.center.X + 1f, Y = player.getSides().Y }, size_0: new Vector2 { X = 0.5f, Y = 1f }, color_0: Color.OrangeRed, velocity_0: new Vector2 { X = 0, Y = 50f });
                    bullets.Add(bullet01);
                    bullets.Add(bullet02);
                }
                else
                {
                    square bullet01 = new square(position_0: new Vector2 { X = player.center.X - 1.5f, Y = player.getSides().Y }, size_0: new Vector2 { X = 0.75f, Y = 0.85f }, color_0: Color.OrangeRed, velocity_0: new Vector2 { X = 0, Y = 50f });
                    square bullet02 = new square(position_0: new Vector2 { X = player.center.X, Y = player.getSides().Y }, size_0: new Vector2 { X = 0.75f, Y = 0.85f }, color_0: Color.OrangeRed, velocity_0: new Vector2 { X = 0, Y = 50f });
                    square bullet03 = new square(position_0: new Vector2 { X = player.center.X + 1.5f, Y = player.getSides().Y }, size_0: new Vector2 { X = 0.75f, Y = 0.85f }, color_0: Color.OrangeRed, velocity_0: new Vector2 { X = 0, Y = 50f });
                    bullets.Add(bullet01);
                    bullets.Add(bullet02);
                    bullets.Add(bullet03);
                }
            }

            player.update(time);
            foreach (square b in bullets)
            {
                b.update(time);
            }
            foreach (square b in enemies)
            {
                b.update(time);
            }

            // collision Management
            collisionManagement();
            enemyGenerator(time);

            // check enemy death
            for (int k = enemies.Count - 1; k >= 0; k--)
            {
                if (enemies[k].hp <= 0)
                {
                    enemies.RemoveAt(k);
                }
            }

            // check bullet death
            for (int k = bullets.Count - 1; k >= 0; k--)
            {
                if (bullets[k].hp <= 0)
                {
                    bullets.RemoveAt(k);
                }
            }

            if (player.hp <= 0)
            {
                Globals.ExitFlag = true;
            }

        }

        public void collisionManagement()
        {
            // check if bullets hit enemies
            for (int i = 0; i < bullets.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    Vector4 collision = enemies[j].collision(bullets[i].getSides());
                    if (collision.Length > 0 || enemies[j].isInside(bullets[i].center))
                    {
                        enemies[j].hp -= bullets[i].hp;
                        bullets[i].hp = 0;
                    }
                }
                if (bullets[i].center.Y > 100)
                {
                    bullets[i].hp = 0;
                }

            }

            //check if player collides with enemies
            for (int j = 0; j < enemies.Count; j++)
            {
                Vector4 collision = enemies[j].collision(player.getSides());
                if (collision.Length > 0)
                {
                    player.hp = 0;
                }
            }
        }

        public void enemyGenerator(double time)
        {
            // manage wave increase
            Rukos.increaseCooldownCounter -= time;
            if (Rukos.increaseCooldownCounter < 0)
            {
                Rukos.waveCounter++;
                Rukos.increaseCooldownCounter = Rukos.increaseCooldown;
                Rukos.enemySpeed += 10f;
                Rukos.shotCooldown *= 0.75f;
                Rukos.enemyCooldown *= 0.75f;
                Rukos.enemySize = new Vector2 { X = Rukos.enemySize.X + 2, Y = Rukos.enemySize.Y + 1 };
            }

            Rukos.enemyCooldownCounter -= time;
            if (Rukos.enemyCooldownCounter < 0)
            {
                Rukos.enemyCooldownCounter = Rukos.enemyCooldown;
                float Xposition = 10 + (float)Rukos.rnd.NextDouble() * 80;
                square enemy = new square(position_0: new Vector2 { X = Xposition, Y = 110f }, size_0: Rukos.enemySize, color_0: Color.Violet, velocity_0: new Vector2 { X = 0, Y = -Rukos.enemySpeed });
                enemies.Add(enemy);
            }


        }
    }
}
