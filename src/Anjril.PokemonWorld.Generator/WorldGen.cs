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
    public class WorldGen
    {
        public const int subWidth = 100;
        public const int subHeight = 100;

        public int Width { get; set; }
        public int Height { get; set; }
        public string Output { get; set; }
        public bool GenerateGif { get; set; }
        public bool GeneratePng { get; set; }

        private SubWorld[,] _subworlds;
        private Random _random;

        public WorldGen(int width, int height, string output,
            bool generatePng = false, bool generateGif = false)
        {
            this.Width = width;
            this.Height = height;
            this.Output = output;
            this.GenerateGif = generateGif;
            this.GeneratePng = generatePng;

            this._random = new Random();
            this._subworlds = new SubWorld[1 + Width / subWidth, 1 + Height / subHeight];

            /*var di = new DirectoryInfo(this.Output);

            if (di.Exists)
                di.Delete(true);

            di.Create();*/
        }

        public void GenerateMap()
        {
            for (int i = 0; i <= Width / subWidth; i++)
            {
                for (int j = 0; j <= Height / subHeight; j++)
                {
                    _subworlds[i, j] = new SubWorld(subWidth, subHeight);
                }
            }

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    SetGround(i, j, GroundTileType.Undefined) ;
                }
            }

            //todo faire les bords avec des montagnes et de la deepsea

            Sea(200, 80, 2, 1, 10, 500);
            Forest(150, 300, 250);
            Road(225, 200, 325, 150);
            Town(225, 200, 80);
            Town(325, 150, 50);

            /*List<AreaTile> sources = new List<AreaTile>();
            sources.Add(new AreaTile(0, 0));
            sources.Add(new AreaTile(1, 0));
            sources.Add(new AreaTile(2, 0));
            sources.Add(new AreaTile(3, 0));
            sources.Add(new AreaTile(4, 0));
            sources.Add(new AreaTile(4, -1));
            sources.Add(new AreaTile(4, -2));
            sources.Add(new AreaTile(4, -3));
            sources.Add(new AreaTile(4, -4));
            Mountain2(320, 320, 6000, sources);*/

            //saveBitmap(createBitmap(), 1);


            List<Bitmap> frames = new List<Bitmap>();
            frames = FillMap();

            if (this.GeneratePng)
            {
                SaveBitmap(CreateBitmap(), 1);
            }

            if (this.GenerateGif)
            {
                SaveGif(frames);
            }

            SaveJSON();

            //SaveBitmap(createBitmap(), 2);

            /*Sea(500, 850, 10, 1, 15, 3000);
            Coast(550, 650, 10, 1, 10, 2000);
            Coast(450, 750, 1, 10, 10, 2000);
            Forest(200, 300, 500);
            CreateBitmap(1);

            Town(200, 500, 70);
            Town(200, 700, 50);
            Town(200, 850, 50);

            Town(600, 200, 100);
            Town(600, 450, 150);
            Town(600, 600, 100);
            Town(600, 750, 80);
            Town(450, 400, 150);
            Town(750, 400, 70);

            CreateBitmap(2);
            
            Mountain(600, 100, 8, 1, 20, 5000, 4);
            Mountain(100, 300, 1, 8, 20, 5000, 4);
            
            CreateBitmap(2);

            FillMap();

            CreateBitmap(4);*/
        }

        #region private methods

        private WorldTile GetTile(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                int X = x / subWidth;
                int Y = y / subHeight;
                return _subworlds[X, Y].WorldTiles[x - X * subWidth, y - Y * subHeight];
            }

            return new WorldTile(GroundTileType.Undefined);
        }

        private GroundTileType GetGround(int x, int y)
        {
            return GetTile(x, y).Ground;
        }

        private ObjectTileType GetObject(int x, int y)
        {
            return GetTile(x, y).Object;
        }

        private WorldTile GetCloseTile(int x, int y, int dist)
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

        private void SetTile(int x, int y, GroundTileType type, ObjectTileType obj)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                int X = x / subWidth;
                int Y = y / subHeight;
                _subworlds[X, Y].WorldTiles[x - X * subWidth, y - Y * subHeight] = new WorldTile(type, obj);
            }
        }

        private void SetGround(int x, int y, GroundTileType type)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                int X = x / subWidth;
                int Y = y / subHeight;
                _subworlds[X, Y].WorldTiles[x - X * subWidth, y - Y * subHeight] = new WorldTile(type);
            }
        }

        private void SetObject(int x, int y, ObjectTileType obj)
        {
            var ground = GetTile(x, y).Object = obj;
        }

        private List<Bitmap> FillMap()
        {
            int k = 0;
            int i = 0;
            int j = 0;
            int count = 0;
            List<Bitmap> frames = new List<Bitmap>();
            bool dir = false;
            frames.Add(CreateBitmap());
            while (k < Width * Height)
            {
                if (dir)
                {
                    i++;
                    if (i >= Width)
                    {
                        i = 0;
                        j++;
                        if (j >= Height)
                        {
                            j = 0;
                        }
                    }

                }
                else
                {
                    j--;
                    if (j < 0)
                    {
                        j = Height - 1;
                        i--;
                        if (i < 0)
                        {
                            i = Width - 1;
                        }
                    }
                }

                if (GetGround(i, j) == GroundTileType.Undefined)
                {
                    switch (GetCloseTile(i, j, 4).Ground)
                    {
                        //case GroundTileType.Tree: SetGround(i, j, GroundTileType.Grass); break;
                        case GroundTileType.Grass: MeadowOrGrassLand(i, j, 15, 10, 0.7); break;
                        case GroundTileType.Sea: Sea2(i, j, 25); break;
                        //case GroundTileType.SeaRock: SetTile(i, j, GroundTileType.SeaRock); break;
                        case GroundTileType.Lake: Lake(i, j, 8); break;
                        case GroundTileType.Street: SetGround(i, j, GroundTileType.Ground); break;
                        case GroundTileType.Building: SetGround(i, j, GroundTileType.Ground); break;
                        case GroundTileType.Ground: Land(i, j, 10); break;
                        //case GroundTileType.Mountain: SetTile(i, j, GroundTileType.Rock); break;
                        //case GroundTileType.Rock: SetTile(i, j, GroundTileType.Rock); break;
                        case GroundTileType.Undefined:
                            int rd = _random.Next(500);
                            if (rd < 3)
                            {
                                MeadowOrGrassLand(i, j, 15, 10, 0.7);
                            }
                            else if (rd < 6)
                            {
                                Land(i, j, 15);
                            }
                            else if (rd < 7)
                            {
                                Lake(i, j, 15);
                            }
                            break;
                        default: break;
                    }

                    k = 0;
                    i = _random.Next(Width);
                    j = _random.Next(Height);
                    dir = !dir;
                    count++;
                    if (count % 100 == 0) frames.Add(CreateBitmap());
                }
                else
                {
                    k++;
                }
            }
            frames.Add(CreateBitmap());
            return frames;
        }

        private bool hasAdjTileOfType(int x, int y, GroundTileType type)
        {

            if (GetGround(x + 1, y) == type) return true;
            if (GetGround(x - 1, y) == type) return true;
            if (GetGround(x, y + 1) == type) return true;
            if (GetGround(x, y - 1) == type) return true;

            return false;
        }



        #endregion

        #region biomes

        private void Forest(int x, int y, int size)
        {
            AreaGen area = new AreaGen();
            area.MazeGen(size, 10, false, 3, 0.1, 0, 0.4);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    switch (area.GetTileType(i, j))
                    {
                        case AreaTileType.Depth: break;
                        case AreaTileType.Ground: SetGround(x - area.Center.X + i, y - area.Center.Y + j, GroundTileType.Grass); break;
                        case AreaTileType.Wall: SetTile(x - area.Center.X + i, y - area.Center.Y + j, GroundTileType.Grass, ObjectTileType.Tree); break;
                        default: break;
                    }
                }
            }
        }

        private void Meadow(int x, int y, int size)
        {
            AreaGen area = new AreaGen();
            area.MazeGen(size, 5, false, 1, 1, 0.3, 1);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    int worldX = x - area.Center.X + i;
                    int worldY = y - area.Center.Y + j;
                    if (GetGround(worldX, worldY) == GroundTileType.Undefined){
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetGround(worldX, worldY, GroundTileType.Grass); break;
                            case AreaTileType.Wall: SetTile(worldX, worldY, GroundTileType.Grass, ObjectTileType.Bush); break;
                            default: break;
                        }

                        if (hasAdjTileOfType(worldX, worldY, GroundTileType.Ground))
                        {
                            SetObject(worldX, worldY, ObjectTileType.Tree);
                        }
                    }
                }
            }
        }

        private void GrassLand(int x, int y, int size)
        {
            AreaGen area = new AreaGen();
            area.MazeGen(size, 5, false, 1, 1, 1, 1);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    int worldX = x - area.Center.X + i;
                    int worldY = y - area.Center.Y + j;
                    if (GetGround(worldX, worldY) == GroundTileType.Undefined)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetTile(worldX, worldY, GroundTileType.Grass, ObjectTileType.HighGrass); break;
                            default: break;
                        }

                        if (hasAdjTileOfType(worldX, worldY, GroundTileType.Ground))
                        {
                            SetObject(worldX, worldY, ObjectTileType.Tree);
                        }
                    }
                }
            }
        }

        private void MeadowOrGrassLand(int x, int y, int size1, int size2, double meadowChance)
        {
            if (_random.NextDouble() < meadowChance)
            {
                Meadow(x, y, size1);
            } else
            {
                GrassLand(x, y, size2);
            }
        }

        private void Land(int x, int y, int size)
        {
            AreaGen area = new AreaGen();
            area.MazeGen(size, 5, false, 1, 1, 0.5, 1);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    int worldX = x - area.Center.X + i;
                    int worldY = y - area.Center.Y + j;
                    if (GetGround(worldX, worldY) == GroundTileType.Undefined)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetGround(worldX, worldY, GroundTileType.Ground); break;
                            case AreaTileType.Wall: SetTile(worldX, worldY, GroundTileType.Ground, ObjectTileType.Rock); break;
                            default: break;
                        }
                    }
                }
            }
        }

        private void Sea(int x, int y, int stretchX, int stretchY, int stretchBase, int size)
        {
            AreaGen area = new AreaGen();
            area.StretchedMazeGen(size, 3, stretchX, stretchY, stretchBase, false, 4, 1, 0.9, 0.7);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    int worldX = x - area.Center.X + i;
                    int worldY = y - area.Center.Y + j;
                    if (GetGround(worldX, worldY) == GroundTileType.Undefined)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetGround(worldX, worldY, GroundTileType.Sea); break;
                            case AreaTileType.Wall: SetGround(worldX, worldY, GroundTileType.Ground); break;
                            default: break;
                        }
                    }
                }
            }
        }

        private void Sea2(int x, int y, int size)
        {
            AreaGen area = new AreaGen();
            area.MazeGen(size, 5, false, 1, 1, 1, 0);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    int worldX = x - area.Center.X + i;
                    int worldY = y - area.Center.Y + j;
                    if (GetGround(worldX, worldY) == GroundTileType.Undefined || GetGround(worldX, worldY) == GroundTileType.Grass || GetGround(worldX, worldY) == GroundTileType.Ground)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground:
                                if (GetGround(worldX, worldY) == GroundTileType.Undefined)
                                {
                                    SetGround(worldX, worldY, GroundTileType.Sea);
                                }
                                else
                                {
                                    Beach(worldX, worldY, 10);
                                }
                                break;
                            case AreaTileType.Wall: SetGround(worldX, worldY, GroundTileType.Ground); break;
                            default: break;
                        }
                    }
                }
            }
        }

        private void Lake(int x, int y, int size)
        {
            AreaGen area = new AreaGen();
            area.MazeGen(size, 5, false, 1, 1, 1, 0);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    int worldX = x - area.Center.X + i;
                    int worldY = y - area.Center.Y + j;
                    if (GetGround(worldX, worldY) == GroundTileType.Undefined)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetGround(worldX, worldY, GroundTileType.Lake);break;
                            case AreaTileType.Wall: SetTile(worldX, worldY, GroundTileType.Lake, ObjectTileType.Rock); break;
                            default: break;
                        }
                    }
                }
            }
        }

        private void Beach(int x, int y, int size)
        {
            AreaGen area = new AreaGen();
            area.MazeGen(size, 5, false, 1, 1, 1, 0);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    int worldX = x - area.Center.X + i;
                    int worldY = y - area.Center.Y + j;
                    if (GetGround(worldX, worldY) == GroundTileType.Undefined || GetGround(worldX, worldY) == GroundTileType.Sea)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetGround(worldX, worldY, GroundTileType.Sand); break;
                            case AreaTileType.Wall: SetGround(worldX, worldY, GroundTileType.Ground); break;
                            default: break;
                        }
                    }
                }
            }
        }

        private void Coast(int x, int y, int stretchX, int stretchY, int stretchBase, int size)
        {
            AreaGen area = new AreaGen();
            area.StretchedMazeGen(size, 3, stretchX, stretchY, stretchBase, false, 2, 1, 0, 0.7);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    switch (area.GetTileType(i, j))
                    {
                        case AreaTileType.Depth: break;
                        case AreaTileType.Ground: SetGround(x - area.Center.X + i, y - area.Center.Y + j, GroundTileType.Sea); break;
                        case AreaTileType.Wall: SetTile(x - area.Center.X + i, y - area.Center.Y + j, GroundTileType.Sea, ObjectTileType.Rock); break;
                        default: break;
                    }
                }
            }
        }

        private void Mountain(int x, int y, int size)
        {
            AreaGen area = new AreaGen();
            List<AreaTile> sources = new List<AreaTile>();
            sources.Add(new AreaTile(0, 0));
            area.HillGen2(size, sources, 1, 100);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    switch (area.GetTileType(i, j))
                    {
                        case AreaTileType.Depth: break;
                        case AreaTileType.Ground: SetGround(x - area.Center.X + i, y - area.Center.Y + j, GroundTileType.Ground); break;
                        case AreaTileType.Wall: SetGround(x - area.Center.X + i, y - area.Center.Y + j, GroundTileType.Mountain); break;
                        default: break;
                    }
                }
            }

            //todo si rencontre avec une autre montagne enlever tout le rayon exterieur ?
        }

        private void Mountain2(int x, int y, int size, List<AreaTile> sources)
        {
            AreaGen area = new AreaGen();
            area.HillGen2(size, sources, 15, 100);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    switch (area.GetTileType(i, j))
                    {
                        case AreaTileType.Depth: break;
                        case AreaTileType.Ground: SetGround(x - area.Center.X + i, y - area.Center.Y + j, GroundTileType.Ground); break;
                        case AreaTileType.Wall: SetGround(x - area.Center.X + i, y - area.Center.Y + j, GroundTileType.Mountain); break;
                        default: break;
                    }
                }
            }

            //todo si rencontre avec une autre montagne enlever tout le rayon exterieur ?
        }

        private void Town(int x, int y, int size)
        {
            AreaGen area = new AreaGen();
            area.MazeGen(size, 8, false, 3, 1, 0.1, 0);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    switch (area.GetTileType(i, j))
                    {
                        case AreaTileType.Depth: break;
                        case AreaTileType.Ground: SetGround(x - area.Center.X + i, y - area.Center.Y + j, GroundTileType.Street); break;
                        case AreaTileType.Wall: SetGround(x - area.Center.X + i, y - area.Center.Y + j, GroundTileType.Building); break;
                        default: break;
                    }
                }
            }
        }

        private void Road(int x, int y, int x2, int y2)
        {
            AreaGen area = new AreaGen();
            area.LineGen(x2 - x, y2 - y, 1, 1, 1, 0, 5);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    switch (area.GetTileType(i, j))
                    {
                        case AreaTileType.Depth: break;
                        case AreaTileType.Ground: SetGround(x - area.Center.X + i, y - area.Center.Y + j, GroundTileType.Street); break;
                        case AreaTileType.Wall: break;
                        default: break;
                    }
                }
            }
        }

        #endregion

        #region persistence

        private Bitmap CreateBitmap()
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

        private void SaveBitmap(Bitmap bmp, int id)
        {
            var path = Path.Combine(Output, String.Format("map{0}_{1}.png", id, DateTime.Now.Ticks));

            bmp.Save(path);
        }

        private void SaveGif(List<Bitmap> frames)
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

        private void SaveJSON()
        {
            String s = "[";
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    s += ((i==0 && j==0)?"":",") + (int)(GetGround(j, i)+1) + "." + (int)(GetObject(j, i) + 1);
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
