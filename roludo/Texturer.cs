using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace roludo
{
    public struct texture 
    {
        public int id;
        public Vector2 size;
    }
    public static class Textures
    {

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }


        public static texture LoadTexture(string spritePath, bool transparentColor, Color alphaChan)
        {
            texture t = new texture();
            string path = spritePath;
            if (String.IsNullOrEmpty(path)) throw new ArgumentException(path);

            System.Drawing.Imaging.PixelFormat pixelForm;
            if (IsLinux) { pixelForm = System.Drawing.Imaging.PixelFormat.Format32bppArgb; }
            else { pixelForm = System.Drawing.Imaging.PixelFormat.Format32bppPArgb; }

            GL.End();
            int id = GL.GenTexture();
            t.id = id;
            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            using (Bitmap BMP = new Bitmap(path))
            {
                if (transparentColor) { BMP.MakeTransparent(alphaChan); }
                t.size = new Vector2(BMP.Width * Globals.Width, BMP.Height * Globals.Height); 
                BitmapData bmpData = BMP.LockBits(new Rectangle(0 , 0, BMP.Width , BMP.Height), ImageLockMode.ReadOnly, pixelForm);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
                BMP.UnlockBits(bmpData);
            }
            GL.Disable(EnableCap.Texture2D);           
            return t;
        }


        public static texture[] LoadTexture(string spritePath, bool transparentColor, Color alphaChan, int stripFrames)
        {
            List<texture> cache = new List<texture>();
            texture t = new texture();
            string path = spritePath;
            if (String.IsNullOrEmpty(path)) throw new ArgumentException(path);

            System.Drawing.Imaging.PixelFormat pixelForm;
            if (IsLinux) { pixelForm = System.Drawing.Imaging.PixelFormat.Format32bppArgb; }
            else { pixelForm = System.Drawing.Imaging.PixelFormat.Format32bppPArgb; }

            for (int animationFrame = 0; animationFrame < stripFrames; animationFrame++)
            {
                GL.End();
                int id = GL.GenTexture();
                t.id = id;
                GL.BindTexture(TextureTarget.Texture2D, id);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                using (Bitmap BMP = new Bitmap(path))
                {
                    if (transparentColor) { BMP.MakeTransparent(alphaChan); }
                    t.size = new Vector2(BMP.Width / stripFrames * Globals.Width, BMP.Height * Globals.Height);
                    BitmapData bmpData = BMP.LockBits(new Rectangle(0 + animationFrame * (BMP.Width / stripFrames), 0, BMP.Width / stripFrames, BMP.Height), ImageLockMode.ReadOnly, pixelForm);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
                    BMP.UnlockBits(bmpData);
                }
                GL.Disable(EnableCap.Texture2D);
                cache.Add(t);
            }

            return cache.ToArray();


        }


    }
}
