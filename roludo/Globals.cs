using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace roludo
{
    public static class Globals
    {
        public static float Width;
        public static float Height;
        public static bool ExitFlag = false;
        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
        public static Random rnd = new Random();
        public static float Ratio
        {
            get
            {
                return Width / Height;
            }
        }

    }

    public static class Fijis
    {

        public static Random rnd = new Random();
        public static double spawnChance = 0.2;
        public static Vector2 RandomVector(double magnitude)
        {
            float X = (float)rnd.NextDouble() - 0.5f;
            float Y = (float)rnd.NextDouble() - 0.5f;
            Vector2 V = new Vector2 { X = X, Y = Y };
            V.NormalizeFast();
            V *= (float)magnitude;
            return V;

        }
        public static List<Vector2> fijiLocs = new List<Vector2>
        {
            new Vector2 { X = 20, Y = 96 }, new Vector2 { X = 20, Y = 93 }, new Vector2 { X = 20, Y = 90 }, new Vector2 { X = 20, Y = 87 }, new Vector2 { X = 20, Y = 84 },
            new Vector2 { X = 20, Y = 81 }, new Vector2 { X = 20, Y = 78 }, new Vector2 { X = 20, Y = 75 }, new Vector2 { X = 20, Y = 72 }, 
            new Vector2 { X = 26, Y = 96 }, new Vector2 { X = 32, Y = 96 }, new Vector2 { X = 26, Y = 87 }, 

            new Vector2 { X = 35, Y = 72 }, new Vector2 { X = 35, Y = 75 }, new Vector2 { X = 35, Y = 78 }, new Vector2 { X = 35, Y = 84 }, 

            new Vector2 { X = 44, Y = 72 }, new Vector2 { X = 44, Y = 75 }, new Vector2 { X = 44, Y = 78 }, new Vector2 { X = 44, Y = 84 }, 
            new Vector2 { X = 44, Y = 69 }, new Vector2 { X = 44, Y = 66 }, new Vector2 { X = 41, Y = 63 }, new Vector2 { X = 38, Y = 66 }, 

            new Vector2 { X = 53, Y = 72 }, new Vector2 { X = 53, Y = 75 }, new Vector2 { X = 53, Y = 78 }, new Vector2 { X = 53, Y = 84 }, 
                        
            new Vector2 { X = 61, Y = 72 }, new Vector2 { X = 61, Y = 75 }, new Vector2 { X = 61, Y = 78 }, new Vector2 { X = 61, Y = 81 }, new Vector2 { X = 61, Y = 84 }, 
            new Vector2 { X = 61, Y = 87 }, new Vector2 { X = 61, Y = 90 }, new Vector2 { X = 61, Y = 93 }, new Vector2 { X = 61, Y = 96 }, new Vector2 { X = 61, Y = 66 }, 

            new Vector2 { X = 71, Y = 72 }, new Vector2 { X = 71, Y = 75 }, new Vector2 { X = 71, Y = 78 }, new Vector2 { X = 71, Y = 81 }, new Vector2 { X = 71, Y = 84 }, 
            new Vector2 { X = 71, Y = 87 }, new Vector2 { X = 71, Y = 90 }, new Vector2 { X = 71, Y = 93 }, new Vector2 { X = 71, Y = 96 }, new Vector2 { X = 71, Y = 66 }, 
                        
            new Vector2 { X = 80, Y = 72 }, new Vector2 { X = 80, Y = 75 }, new Vector2 { X = 80, Y = 78 }, new Vector2 { X = 80, Y = 81 }, new Vector2 { X = 80, Y = 84 }, 
            new Vector2 { X = 80, Y = 87 }, new Vector2 { X = 80, Y = 90 }, new Vector2 { X = 80, Y = 93 }, new Vector2 { X = 80, Y = 96 }, new Vector2 { X = 80, Y = 66 }, 

        };
    }

    public static class Rukos
    {
        public static Random rnd = new Random();
        public static double shotCooldown = 0.50;
        public static double shotCooldownCounter = 0;
        public static double elapsedTime = 0;
        public static double waveCounter = 0;
        public static double enemyCooldown = 2;
        public static double enemyCooldownCounter = 0;
        public static double increaseCooldown = 12;
        public static double increaseCooldownCounter = 12;
        public static Vector2 enemySize = new Vector2 { X = 2, Y = 2 };
        public static float enemySpeed = 50f;


    }
}
