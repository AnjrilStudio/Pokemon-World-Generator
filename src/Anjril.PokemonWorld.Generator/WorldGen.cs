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

        private SubWorld[,] _subworlds;
        private Random _random;

        public WorldGen(int width, int height, string output)
        {
            this.Width = width;
            this.Height = height;
            this.Output = output;

            this._random = new Random();
            this._subworlds = new SubWorld[1 + Width / subWidth, 1 + Height / subHeight];

            var di = new DirectoryInfo(this.Output);

            if (di.Exists)
                di.Delete(true);

            di.Create();
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
                    SetTile(i, j, WorldTileType.Undefined) ;
                }
            }

            //todo faire les bords avec des montagnes et de la deepsea

            Sea(200, 50, 2, 1, 10, 500);
            Forest(100, 300, 250);
            Town(225, 200, 40);
            List<AreaTile> sources = new List<AreaTile>();
            sources.Add(new AreaTile(0, 0));
            sources.Add(new AreaTile(1, 0));
            sources.Add(new AreaTile(2, 0));
            sources.Add(new AreaTile(3, 0));
            sources.Add(new AreaTile(4, 0));
            sources.Add(new AreaTile(4, -1));
            sources.Add(new AreaTile(4, -2));
            sources.Add(new AreaTile(4, -3));
            sources.Add(new AreaTile(4, -4));
            Mountain2(320, 320, 6000, sources);
            
            //saveBitmap(createBitmap(), 1);

            List<Bitmap> frames = FillMap();
            SaveBitmap(CreateBitmap(), 1);
            SaveJSON();
            //saveGif(frames);

            //saveBitmap(createBitmap(), 2);

            /*sea(500, 850, 10, 1, 15, 3000);
            coast(550, 650, 10, 1, 10, 2000);
            coast(450, 750, 1, 10, 10, 2000);
            forest(200, 300, 500);
            createBitmap(1);

            town(200, 500, 70);
            town(200, 700, 50);
            town(200, 200, 100);
            town(200, 850, 50);

            town(600, 200, 100);
            town(600, 450, 150);
            town(600, 600, 100);
            town(600, 750, 80);
            town(450, 400, 150);
            town(750, 400, 70);

            createBitmap(2);
            
            mountain(600, 100, 8, 1, 20, 5000, 4);
            mountain(100, 300, 1, 8, 20, 5000, 4);
            
            createBitmap(2);

            fillMap();

            createBitmap(4);*/
        }

        #region private methods

        private WorldTileType GetTile(int x, int y)
        {
            int X = x / subWidth;
            int Y = y / subHeight;
            return _subworlds[X, Y].get(x - X * subWidth, y - Y * subHeight);
        }

        private WorldTileType GetCloseTile(int x, int y, int dist)
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

        private void SetTile(int x, int y, WorldTileType type)
        {
            int X = x / subWidth;
            int Y = y / subHeight;
            _subworlds[X, Y].set(x - X * subWidth, y - Y * subHeight, type);
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

                if (GetTile(i, j) == WorldTileType.Undefined)
                {
                    switch (GetCloseTile(i, j, 4))
                    {
                        case WorldTileType.Tree: SetTile(i, j, WorldTileType.Grass); break;
                        case WorldTileType.Grass: Meadow(i, j, 20); break;
                        case WorldTileType.Sea: Sea2(i, j, 25); break;
                        case WorldTileType.SeaRock: SetTile(i, j, WorldTileType.SeaRock); break;
                        case WorldTileType.Lake: Lake(i, j, 10); break;
                        case WorldTileType.Street: SetTile(i, j, WorldTileType.Ground); break;
                        case WorldTileType.Building: SetTile(i, j, WorldTileType.Ground); break;
                        case WorldTileType.Ground: Land(i, j, 10); break;
                        case WorldTileType.Mountain: SetTile(i, j, WorldTileType.Rock); break;
                        case WorldTileType.Rock: SetTile(i, j, WorldTileType.Rock); break;
                        case WorldTileType.Undefined:
                            int rd = _random.Next(500);
                            if (rd < 3)
                            {
                                Meadow(i, j, 15);
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
                        case AreaTileType.Ground: SetTile(x - area.Center.X + i, y - area.Center.Y + j, WorldTileType.Grass); break;
                        case AreaTileType.Wall: SetTile(x - area.Center.X + i, y - area.Center.Y + j, WorldTileType.Tree); break;
                        default: break;
                    }
                }
            }
        }

        private void Meadow(int x, int y, int size)
        {
            AreaGen area = new AreaGen();
            area.MazeGen(size, 5, false, 1, 1, 0.5, 1);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    int worldX = x - area.Center.X + i;
                    int worldY = y - area.Center.Y + j;
                    if (GetTile(worldX, worldY) == WorldTileType.Undefined){
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetTile(worldX, worldY, WorldTileType.Grass); break;
                            case AreaTileType.Wall: SetTile(worldX, worldY, WorldTileType.Tree); break;
                            default: break;
                        }
                    }
                }
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
                    if (GetTile(worldX, worldY) == WorldTileType.Undefined)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetTile(worldX, worldY, WorldTileType.Ground); break;
                            case AreaTileType.Wall: SetTile(worldX, worldY, WorldTileType.Rock); break;
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
                    if (GetTile(worldX, worldY) == WorldTileType.Undefined)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetTile(worldX, worldY, WorldTileType.Sea); break;
                            case AreaTileType.Wall: SetTile(worldX, worldY, WorldTileType.Ground); break;
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
                    if (GetTile(worldX, worldY) == WorldTileType.Undefined || GetTile(worldX, worldY) == WorldTileType.Grass || GetTile(worldX, worldY) == WorldTileType.Ground)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground:
                                if (GetTile(worldX, worldY) == WorldTileType.Undefined)
                                {
                                    SetTile(worldX, worldY, WorldTileType.Sea);
                                }
                                else
                                {
                                    Beach(worldX, worldY, 10);
                                }
                                break;
                            case AreaTileType.Wall: SetTile(worldX, worldY, WorldTileType.Ground); break;
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
                    if (GetTile(worldX, worldY) == WorldTileType.Undefined)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetTile(worldX, worldY, WorldTileType.Lake);break;
                            case AreaTileType.Wall: SetTile(worldX, worldY, WorldTileType.Ground); break;
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
                    if (GetTile(worldX, worldY) == WorldTileType.Undefined || GetTile(worldX, worldY) == WorldTileType.Sea)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetTile(worldX, worldY, WorldTileType.Sand); break;
                            case AreaTileType.Wall: SetTile(worldX, worldY, WorldTileType.Ground); break;
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
                        case AreaTileType.Ground: SetTile(x - area.Center.X + i, y - area.Center.Y + j, WorldTileType.Sea); break;
                        case AreaTileType.Wall: SetTile(x - area.Center.X + i, y - area.Center.Y + j, WorldTileType.SeaRock); break;
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
                        case AreaTileType.Ground: SetTile(x - area.Center.X + i, y - area.Center.Y + j, WorldTileType.Ground); break;
                        case AreaTileType.Wall: SetTile(x - area.Center.X + i, y - area.Center.Y + j, WorldTileType.Mountain); break;
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
                        case AreaTileType.Ground: SetTile(x - area.Center.X + i, y - area.Center.Y + j, WorldTileType.Ground); break;
                        case AreaTileType.Wall: SetTile(x - area.Center.X + i, y - area.Center.Y + j, WorldTileType.Mountain); break;
                        default: break;
                    }
                }
            }

            //todo si rencontre avec une autre montagne enlever tout le rayon exterieur ?
        }

        private void Town(int x, int y, int size)
        {
            AreaGen area = new AreaGen();
            area.MazeGen(size, 10, false, 4, 1, 0, 0);
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    switch (area.GetTileType(i, j))
                    {
                        case AreaTileType.Depth: break;
                        case AreaTileType.Ground: SetTile(x - area.Center.X + i, y - area.Center.Y + j, WorldTileType.Street); break;
                        case AreaTileType.Wall: SetTile(x - area.Center.X + i, y - area.Center.Y + j, WorldTileType.Building); break;
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
                    switch (GetTile(i, j))
                    {
                        case WorldTileType.Tree: color = Color.DarkGreen; break;
                        case WorldTileType.Grass: color = Color.Lime; break;
                        case WorldTileType.Sea: color = Color.Blue; break;
                        case WorldTileType.SeaRock: color = Color.DarkSlateBlue; break;
                        case WorldTileType.Lake: color = Color.Blue; break;
                        case WorldTileType.Street: color = Color.LightGray; break;
                        case WorldTileType.Building: color = Color.Gray; break;
                        case WorldTileType.Ground: color = Color.Goldenrod; break;
                        case WorldTileType.Mountain: color = Color.Brown; break;
                        case WorldTileType.Rock: color = Color.Brown; break;
                        case WorldTileType.Sand: color = Color.LightGoldenrodYellow; break;
                        case WorldTileType.Undefined: color = Color.Black; break;
                        default: color = Color.Gray; break;
                    }

                    Bmp.SetPixel(i, j, color);
                }
            }

            return Bmp;
        }

        private void SaveBitmap(Bitmap bmp, int id)
        {
            var path = Path.Combine(Output, String.Format("map{0}.png", id));

            bmp.Save(path);
        }

        private void SaveGif(List<Bitmap> frames)
        {
            var path = Path.Combine(Output, "map.gif");

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
                    s += ((i==0 && j==0)?"":",") + (int)(GetTile(j, i)+1);
                }
            }
            s += "]";

            var path = Path.Combine(Output, "map.json");

            System.IO.StreamWriter file = new System.IO.StreamWriter(path);
            file.WriteLine(s);

            file.Close();

        }

        #endregion
    }
}
