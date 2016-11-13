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
    public class ArenaGen : Gen
    {
        public bool GenerateGif { get; set; }
        public bool GeneratePng { get; set; }

        private GroundTileType[,] _pattern;
        private int _sizeFactor = 4;

        public ArenaGen(GroundTileType[,] pattern, int sizeFactor, string output,
            bool generatePng = false, bool generateGif = false)
        {
            _pattern = pattern;
            _sizeFactor = sizeFactor;
            this._width = pattern.GetLength(0) * (sizeFactor+1);
            this._height = pattern.GetLength(1) * (sizeFactor+1);
            this.Output = output;
            this.GenerateGif = generateGif;
            this.GeneratePng = generatePng;

            this._random = new Random();
            this._subworlds = new SubWorld[1 + _width / SUB_WIDTH, 1 + _height / SUB_HEIGHT];
        }

        public void Generate()
        {
            for (int i = 0; i <= _width / SUB_WIDTH; i++)
            {
                for (int j = 0; j <= _height / SUB_HEIGHT; j++)
                {
                    _subworlds[i, j] = new SubWorld(SUB_WIDTH, SUB_HEIGHT);
                }
            }

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    SetGround(i, j, GroundTileType.Undefined) ;
                }
            }

            for (int i = 0; i < _pattern.GetLength(0); i++)
            {
                for (int j = 0; j < _pattern.GetLength(1); j++)
                {
                    int x = (i+1) * _sizeFactor;
                    int y = (j+1) * _sizeFactor;
                    int size = _sizeFactor * _sizeFactor;

                    switch (_pattern[i, j])
                    {
                        case GroundTileType.Grass:
                            Meadow(x, y, size);
                            break;
                        case GroundTileType.Ground:
                            Land(x, y, size);
                            break;
                        case GroundTileType.Sea:
                            Sea(x, y, size);
                            break;
                        case GroundTileType.Sand:
                            Beach(x, y, size);
                            break;
                        default:
                            Land(x, y, size);
                            break;
                    }
                }
            }

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
        }

        #region private methods

        private List<Bitmap> FillMap()
        {
            int k = 0;
            int i = 0;
            int j = 0;
            int count = 0;
            List<Bitmap> frames = new List<Bitmap>();
            bool dir = false;
            frames.Add(CreateBitmap());
            while (k < _width * _height)
            {
                if (dir)
                {
                    i++;
                    if (i >= _width)
                    {
                        i = 0;
                        j++;
                        if (j >= _height)
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
                        j = _height - 1;
                        i--;
                        if (i < 0)
                        {
                            i = _width - 1;
                        }
                    }
                }

                if (GetGround(i, j) == GroundTileType.Undefined)
                {
                    switch (GetCloseTile(i, j, 4).Ground)
                    {
                        case GroundTileType.Grass: Meadow(i, j, 10); break;
                        case GroundTileType.Sea: Sea(i, j, 10); break;
                        case GroundTileType.Ground: Land(i, j, 10); break;
                        case GroundTileType.Sand: Beach(i, j, 10); break;
                        case GroundTileType.Undefined:
                            int rd = _random.Next(500);
                            if (rd < 3)
                            {
                                Meadow(i, j, 10);
                            }
                            else if (rd < 6)
                            {
                                Land(i, j, 10);
                            }
                            break;
                        default: break;
                    }

                    k = 0;
                    i = _random.Next(_width);
                    j = _random.Next(_height);
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
                            case AreaTileType.Wall: SetGround(worldX, worldY, GroundTileType.Grass); break;
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
                    if (GetGround(worldX, worldY) == GroundTileType.Undefined)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetGround(worldX, worldY, GroundTileType.Ground); break;
                            case AreaTileType.Wall: SetGround(worldX, worldY, GroundTileType.Ground); break;
                            default: break;
                        }
                    }
                }
            }
        }

        private void Sea(int x, int y, int size)
        {
            AreaGen area = new AreaGen();
            area.MazeGen(size, 3, false, 4, 1, 0.9, 0.7);
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
                            case AreaTileType.Wall: SetGround(worldX, worldY, GroundTileType.Sea); break;
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
                    if (GetGround(worldX, worldY) == GroundTileType.Undefined)
                    {
                        switch (area.GetTileType(i, j))
                        {
                            case AreaTileType.Depth: break;
                            case AreaTileType.Ground: SetGround(worldX, worldY, GroundTileType.Sand); break;
                            case AreaTileType.Wall: SetGround(worldX, worldY, GroundTileType.Sand); break;
                            default: break;
                        }
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
    }
}
