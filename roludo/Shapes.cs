using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace roludo
{

    public class square
    {
        public List<Vector3> puntos;
        public Vector2 center;
        public Vector2 size;
        public Vector2 velocity;
        public Color color = Color.White;
        public int hp = 100;
        public float z = 0;

        public square()
        { }

        public square(Vector2 position_0, Vector2 size_0, Color? color_0 = null, Vector2? velocity_0 = null)
        {
            if (color_0 == null) { color_0 = Color.White; }
            if (velocity_0 == null) { velocity_0 = new Vector2 {X = 0, Y = 0 }; }
            center = position_0;
            velocity = (Vector2)velocity_0;
            color = (Color)color_0;
            size = size_0;
            updateVertices();
        }

        public void updateVertices()
        {
            float x = this.center.X;
            float y = this.center.Y;
            float w = this.size.X / 2;
            float h = this.size.Y / 2;
            this.puntos = new List<Vector3> 
			{

				new Vector3 {X = x - w, Y = y - h, Z = this.z},
				new Vector3 {X = x - w, Y = y + h, Z = this.z},
				new Vector3 {X = x + w, Y = y + h, Z = this.z},
				new Vector3 {X = x + w, Y = y - h, Z = this.z}
			};
        }

        public virtual void update(double time)
        {
			
            this.center.X += this.velocity.X * (float)time; // 60.0f;
            this.center.Y += this.velocity.Y * (float)time; // 60.0f;
            updateVertices();
        }

        //X left Y Top Z Right W Bot
        public virtual Vector4 getSides()
        {
            Vector4 v = new Vector4();
            v.X= this.puntos[0].X;
            v.Y = this.puntos[1].Y;
            v.Z = this.puntos[2].X;
            v.W = this.puntos[3].Y;
            return v;

        }

        public virtual bool isInside(Vector2 p)
        {   
            Vector4 v = getSides();
            if(p.X >= v.X && p.X <= v.Z && p.Y >= v.W && p.Y <= v.Y)
            {
                return true;
            }
            return false;
        }

        public virtual Vector4 collision(Vector4 p)
        {
            // tells you what part of self is being touched by p
            Vector4 c = new Vector4 { X = 0, Y = 0, Z = 0, W = 0 };
            Vector4 v = getSides();
            // check left
            // p's right is to the right of v's left AND
            // p's right is to the left of v's right AND
            // p's top is above the bot and its bot i below the top
            if (p.Z >= v.X && p.Z <= v.Z && p.Y > v.W && p.W < v.Y)
            {
                c.X = 1;
            }
            // check right
            if (p.X >= v.X && p.X <= v.Z && p.Y > v.W && p.W < v.Y)
            {
                c.Z = 1;
            }
            // check top
            if (p.W >= v.W && p.W <= v.Y && p.X < v.Z && p.Z > v.X)
            {
                c.Y = 1;
            }
            // check bot
            if (p.Y >= v.W && p.Y <= v.Y && p.X < v.Z && p.Z > v.X)
            {
                c.W = 1;
            }
            if (c.X == c.Z)
            {
                c.X = 0;
                c.Z = 0;
            }
            if (c.Y == c.W)
            {
                c.Y = 0;
                c.W = 0;
            }

            return c;


        }

        public void RenderToScreen()
        {
            //GL.Color4(Color.White);
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(color);
            foreach (Vector3 v in puntos)
            {
                GL.Vertex3(v);
            }
            GL.End();

        }
    }

    public class sprite : square
    {

        public texture texture;
        public texture[] textures;
        public int animationFrame;
        public double frameTime;
        public double frameDuration;

        public sprite(Vector2 position_0, Vector2 velocity_0, texture texture_0, Vector2? size_0 = null, float zLevel = 0)
        {
            center = position_0;
            velocity = velocity_0;
            texture = texture_0;
            z = zLevel;
            frameDuration = 0;// since we won't have animation -- it is not animated
            if (size_0 == null)
            {
                //if size_0 is not overriden, use texture size
                size = texture.size;
            }

            updateVertices();
        }

        public sprite(Vector2 position_0, Vector2 velocity_0, texture[] textures_0, double frameDuration_0, Vector2? size_0 = null, float zLevel = 0)
        {
            center = position_0;
            velocity = velocity_0;
            textures = textures_0;
            animationFrame = 0;
            texture = textures[animationFrame];
            z = zLevel;
            frameDuration = frameDuration_0;// since we won't have animation -- it is not animated
            if (size_0 == null)
            {
                //if size_0 is not overriden, use texture size
                size = texture.size;
            }

            updateVertices();
        }

        public override void update(double time)
        {
            this.center.X += this.velocity.X * (float)time; // 60.0f;
            this.center.Y += this.velocity.Y * (float)time; // 60.0f;

            if (frameDuration > 0) // then it is animated
            {
                frameTime += (float)time;
                if (frameTime >= frameDuration)
                {
                    frameTime = 0;
                    animationFrame += 1;
                    if (animationFrame > textures.Length - 1) { animationFrame = 0; }
                    texture = textures[animationFrame];
                }
            }

            updateVertices();

        }

        new public void RenderToScreen()
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texture.id);
            GL.Color4(Color.White);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1);
            GL.Vertex3(this.puntos[0]);
            GL.TexCoord2(0, 0);
            GL.Vertex3(this.puntos[1]);
            GL.TexCoord2(1, 0);
            GL.Vertex3(this.puntos[2]);
            GL.TexCoord2(1, 1);
            GL.Vertex3(this.puntos[3]);
            GL.End();
            GL.Disable(EnableCap.Texture2D);
            GL.Color4(Color.White);
        }


    }


	public class arc
	{
		public List<Vector3> puntos;
		public Vector2 player;
		public Vector2 center;
		public Vector2 direction;

		public Vector2 leftEnd;
		public Vector2 rightEnd;
		public double angleDirection;

		public float width;

		public Vector2 velocity;
		public Color color = Color.White;
		public int hp = 100;
		public float z = 0;
		public float arcLength = (float)(Math.PI/4.0);

		public arc()
		{ }

		public arc(Vector2 position_0, Vector2 direction_0, float size_0, Color? color_0 = null)
		{
			if (color_0 == null) { color_0 = Color.White; }
			center = position_0;
			direction = direction_0;
			color = (Color)color_0;
			width = size_0;
			puntos = new List<Vector3>();

			updateVertices();
		}

		public void updateVertices()
		{
			angleDirection = Math.Atan2 ((double)direction.Y, (double)direction.X);
			leftEnd = center + new Vector2 (direction.X * (float)Math.Cos(angleDirection + 0.5f* arcLength), direction.Y * (float)Math.Sin(angleDirection + 0.5f* arcLength));
			rightEnd = center + new Vector2 (direction.X * (float)Math.Cos(angleDirection - 0.5f* arcLength), direction.Y * (float)Math.Sin(angleDirection - 0.5f* arcLength));

			if ( puntos.Count == 0) 
			{
				// do the bezier thing with the given angle, etc

				puntos = new List<Vector3> 
				{

					new Vector3 {X = leftEnd.X - direction.X , Y = leftEnd.Y - direction.Y, Z = this.z},
					new Vector3 {X = leftEnd.X + direction.X , Y = leftEnd.Y + direction.Y, Z = this.z},
					new Vector3 {X = rightEnd.X + direction.X , Y = rightEnd.Y + direction.Y, Z = this.z},
					new Vector3 {X = rightEnd.X - direction.X , Y = rightEnd.Y - direction.Y, Z = this.z},
				};

			} 
			else 
			{
				// maybe cheaper to transform the points?
				puntos = new List<Vector3> 
				{

					new Vector3 {X = leftEnd.X - direction.X , Y = leftEnd.Y - direction.Y, Z = this.z},
					new Vector3 {X = leftEnd.X + direction.X , Y = leftEnd.Y + direction.Y, Z = this.z},
					new Vector3 {X = rightEnd.X + direction.X , Y = rightEnd.Y + direction.Y, Z = this.z},
					new Vector3 {X = rightEnd.X - direction.X , Y = rightEnd.Y - direction.Y, Z = this.z},
				};
			}


		}

		public virtual void update(double time, Vector2 player_0, Vector2 direction_0)
		{	
			direction = direction_0;
			player = player_0;

			direction -= - player;
			direction /= direction.Length;

			center = player + direction*12f;

			updateVertices();
		}

		//X left Y Top Z Right W Bot
		public virtual Vector4 getSides()
		{
			Vector4 v = new Vector4();
			v.X= this.puntos[0].X;
			v.Y = this.puntos[1].Y;
			v.Z = this.puntos[2].X;
			v.W = this.puntos[3].Y;
			return v;

		}

		public virtual bool isInside(Vector2 p)
		{   
			Vector4 v = getSides();
			if(p.X >= v.X && p.X <= v.Z && p.Y >= v.W && p.Y <= v.Y)
			{
				return true;
			}
			return false;
		}

		public virtual Vector4 collision(Vector4 p)
		{
			// tells you what part of self is being touched by p
			Vector4 c = new Vector4 { X = 0, Y = 0, Z = 0, W = 0 };
			Vector4 v = getSides();
			// check left
			// p's right is to the right of v's left AND
			// p's right is to the left of v's right AND
			// p's top is above the bot and its bot i below the top
			if (p.Z >= v.X && p.Z <= v.Z && p.Y > v.W && p.W < v.Y)
			{
				c.X = 1;
			}
			// check right
			if (p.X >= v.X && p.X <= v.Z && p.Y > v.W && p.W < v.Y)
			{
				c.Z = 1;
			}
			// check top
			if (p.W >= v.W && p.W <= v.Y && p.X < v.Z && p.Z > v.X)
			{
				c.Y = 1;
			}
			// check bot
			if (p.Y >= v.W && p.Y <= v.Y && p.X < v.Z && p.Z > v.X)
			{
				c.W = 1;
			}
			if (c.X == c.Z)
			{
				c.X = 0;
				c.Z = 0;
			}
			if (c.Y == c.W)
			{
				c.Y = 0;
				c.W = 0;
			}

			return c;


		}

		public void RenderToScreen()
		{
			//GL.Color4(Color.White);
			GL.Begin(PrimitiveType.Quads);
			GL.Color4(color);
			foreach (Vector3 v in puntos)
			{
				GL.Vertex3(v);
			}
			GL.End();

		}


	}

}
