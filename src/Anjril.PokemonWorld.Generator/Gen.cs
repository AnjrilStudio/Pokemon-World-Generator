using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGif;
using System.Drawing;
using Anjril.PokemonWorld.Generator.Enums;
using Anjril.PokemonWorld.Generator.Entities;
using System.IO;

namespace Anjril.PokemonWorld.Generator
{
    public abstract class Gen
    {
        public const int subWidth = 100;
        public const int subHeight = 100;

        public int Width { get; set; }
        public int Height { get; set; }

        protected SubWorld[,] _subworlds;
        protected Random _random;

        public string Output { get; set; }


        protected WorldTile GetTile(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                int X = x / subWidth;
                int Y = y / subHeight;
                return _subworlds[X, Y].WorldTiles[x - X * subWidth, y - Y * subHeight];
            }

            return new WorldTile(GroundTileType.Undefined);
        }

        protected GroundTileType GetGround(int x, int y)
        {
            return GetTile(x, y).Ground;
        }

        protected ObjectTileType GetObject(int x, int y)
        {
            return GetTile(x, y).Object;
        }

        protected WorldTile GetCloseTile(int x, int y, int dist)
        {
            int dx = -1 + _random.Next(2) * 2;
            int dy = -1 + _random.Next(2) * 2;

            int i = x;
            int j = y;

            for (int d = 0; d < dist; d++)
            {
                switch (_random.Next(3) - 1)
                {
                    case -1: i += dx; break;
                    case 0: break;
                    case 1: j += dy; break;
                }
            }

            return GetTile(i, j);
        }

        protected void SetTile(int x, int y, GroundTileType type, ObjectTileType obj)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                int X = x / subWidth;
                int Y = y / subHeight;
                _subworlds[X, Y].WorldTiles[x - X * subWidth, y - Y * subHeight] = new WorldTile(type, obj);
            }
        }

        protected void SetGround(int x, int y, GroundTileType type)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                int X = x / subWidth;
                int Y = y / subHeight;
                _subworlds[X, Y].WorldTiles[x - X * subWidth, y - Y * subHeight] = new WorldTile(type);
            }
        }

        protected void SetObject(int x, int y, ObjectTileType obj)
        {
            var ground = GetTile(x, y).Object = obj;
        }
        
        protected bool hasAdjTileOfType(int x, int y, GroundTileType type)
        {

            if (GetGround(x + 1, y) == type) return true;
            if (GetGround(x - 1, y) == type) return true;
            if (GetGround(x, y + 1) == type) return true;
            if (GetGround(x, y - 1) == type) return true;

            return false;
        }

        #region persistence

        public GroundTileType[,] GetGroundMatrix()
        {
            var matrix = new GroundTileType[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    matrix[i, j] = GetGround(i, j);
                }
            }

            return matrix;
        }

        protected Bitmap CreateBitmap()
        {
            Bitmap Bmp = new Bitmap(Width, Height);
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Color color;
                    switch (GetGround(i, j))
                    {
                        case GroundTileType.Grass: color = Color.Lime; break;
                        case GroundTileType.Sea: color = Color.Blue; break;
                        case GroundTileType.Lake: color = Color.Blue; break;
                        case GroundTileType.Street: color = Color.LightGray; break;
                        case GroundTileType.Building: color = Color.Gray; break;
                        case GroundTileType.Ground: color = Color.Goldenrod; break;
                        case GroundTileType.Mountain: color = Color.Brown; break;
                        case GroundTileType.Sand: color = Color.LightGoldenrodYellow; break;
                        case GroundTileType.Undefined: color = Color.Black; break;
                        default: color = Color.Gray; break;
                    }

                    switch (GetObject(i, j))
                    {
                        case ObjectTileType.Tree: color = Color.DarkGreen; break;
                        case ObjectTileType.Rock: color = Color.Brown; break;
                        case ObjectTileType.HighGrass: color = Color.LimeGreen; break;
                        case ObjectTileType.Bush: color = Color.Green; break;
                    }

                    Bmp.SetPixel(i, j, color);
                }
            }

            return Bmp;
        }

        protected void SaveBitmap(Bitmap bmp, int id)
        {
            var path = Path.Combine(Output, String.Format("map{0}_{1}.png", id, DateTime.Now.Ticks));

            bmp.Save(path);
        }

        protected void SaveGif(List<Bitmap> frames)
        {
            var path = Path.Combine(Output, String.Format("map_{0}.gif", DateTime.Now.Ticks));

            AnimatedGifEncoder e = new AnimatedGifEncoder();
            e.Start(path);
            e.SetDelay(100);
            //-1:no repeat,0:always repeat
            e.SetRepeat(0);
            foreach (Bitmap bmp in frames)
            {
                e.AddFrame(bmp);
            }
            e.Finish();
        }

        protected void SaveJSON()
        {
            String s = "[";
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    s += ((i == 0 && j == 0) ? "" : ",") + (int)(GetGround(j, i) + 1) + "." + (int)(GetObject(j, i) + 1);
                }
            }
            s += "]";

            var path = Path.Combine(Output, String.Format("map_{0}.json", DateTime.Now.Ticks));

            System.IO.StreamWriter file = new System.IO.StreamWriter(path);
            file.WriteLine(s);

            file.Close();

        }

        #endregion
    }
}
