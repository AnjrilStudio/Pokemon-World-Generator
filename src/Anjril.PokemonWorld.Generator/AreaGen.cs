using Anjril.PokemonWorld.Generator.Entities;
using Anjril.PokemonWorld.Generator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anjril.PokemonWorld.Generator
{
    class AreaGen
    {
        private Random random;

        public int Width { get; set; }
        public int Height { get; set; }
        public AreaTile Center { get; set; }

        private AreaTileType[,] _tiles;

        public AreaGen()
        {
            random = new Random();
        }

        public List<AreaTile> MazeGen(int baseSize, double mazeness, bool square, int resolution, double rmBorder, double rmNoBorder, double noise)
        {
            List<AreaTile> passable = new List<AreaTile>();
            List<AreaTile> border = new List<AreaTile>();

            passable.Add(new AreaTile(0, 0));
            border.Add(new AreaTile(1, 0));
            border.Add(new AreaTile(-1, 0));
            border.Add(new AreaTile(0, 1));
            border.Add(new AreaTile(0, -1));

            int k = random.Next(4);
            for (int i = 0; i < baseSize; i++)
            {
                AreaTile tile = GetRandomBorder(border, mazeness, true);
                bool isLast = square && tile.Equals(border.Last());
                border.Remove(tile);
                passable.Add(tile);

                AreaTile newBorderTile;
                if (!isLast) k = random.Next(4);

                //int d = (random.Next(2) * 2) - 1;
                int d = 1;
                k = (k + d) % 4;
                if (k < 0) k += 4;
                for (int j = 0; j < 4; j++)
                {
                    switch (k)
                    {
                        case 0:
                            newBorderTile = new AreaTile(tile.X + 1, tile.Y);
                            if (!passable.Contains(newBorderTile) && !border.Contains(newBorderTile))
                            {
                                border.Add(newBorderTile);
                            }
                            break;
                        case 1:
                            newBorderTile = new AreaTile(tile.X, tile.Y + 1);
                            if (!passable.Contains(newBorderTile) && !border.Contains(newBorderTile))
                            {
                                border.Add(newBorderTile);
                            }
                            break;
                        case 2:
                            newBorderTile = new AreaTile(tile.X - 1, tile.Y);
                            if (!passable.Contains(newBorderTile) && !border.Contains(newBorderTile))
                            {
                                border.Add(newBorderTile);
                            }
                            break;
                        case 3:
                            newBorderTile = new AreaTile(tile.X, tile.Y - 1);
                            if (!passable.Contains(newBorderTile) && !border.Contains(newBorderTile))
                            {
                                border.Add(newBorderTile);
                            }
                            break;
                    }
                    k = (k + d) % 4;
                    if (k < 0) k += 4;
                }
                k = (k - d) % 4;
                if (k < 0) k += 4;
            }

            if (rmBorder > 0) RemoveBorder(passable, border, rmBorder, random);
            if (rmNoBorder > 0) RemoveNoBorder(passable, border, rmNoBorder, random, false);

            _tiles = ListToMatrix(passable, border, resolution, random, noise);
            return border;
        }

        public void StretchedMazeGen(int baseSize, double mazeness, int stretchX, int stretchY, int stretchBase, bool square, int resolution, double rmBorder, double rmNoBorder, double noise)
        {
            List<AreaTile> passable = new List<AreaTile>();
            List<AreaTile> border = new List<AreaTile>();
            List<AreaTile>[] borders = new List<AreaTile>[stretchX * stretchY];

            for (int i = 0; i < stretchX; i++)
            {
                for (int j = 0; j < stretchY; j++)
                {
                    int x = i * stretchBase - (stretchX - 1) * stretchBase / 2;
                    int y = j * stretchBase - (stretchY - 1) * stretchBase / 2;
                    passable.Add(new AreaTile(x, y));
                    border.Add(new AreaTile(x + 1, y));
                    border.Add(new AreaTile(x - 1, y));
                    border.Add(new AreaTile(x, y + 1));
                    border.Add(new AreaTile(x, y - 1));

                    List<AreaTile> border2 = new List<AreaTile>();
                    borders[j * stretchX + i] = border2;
                    border2.Add(new AreaTile(x + 1, y));
                    border2.Add(new AreaTile(x - 1, y));
                    border2.Add(new AreaTile(x, y + 1));
                    border2.Add(new AreaTile(x, y - 1));
                }
            }

            int k = random.Next(4);
            for (int i = 0; i < baseSize; i++)
            {
                List<AreaTile> border2 = borders[i % (stretchX * stretchY)];
                if (border2.Count == 0)
                {
                    continue;
                }
                AreaTile tile = GetRandomBorder(border2, mazeness, true);
                bool isLast = square && tile.Equals(border2.Last());
                border.Remove(tile);
                border2.Remove(tile);
                passable.Add(tile);

                AreaTile newBorderTile;
                if (!isLast) k = random.Next(4);

                //int d = (random.Next(2) * 2) - 1;
                int d = 1;
                k = (k + d) % 4;
                if (k < 0) k += 4;
                for (int j = 0; j < 4; j++)
                {
                    switch (k)
                    {
                        case 0:
                            newBorderTile = new AreaTile(tile.X + 1, tile.Y);
                            if (!passable.Contains(newBorderTile) && !border.Contains(newBorderTile))
                            {
                                border.Add(newBorderTile);
                                border2.Add(newBorderTile);
                            }
                            break;
                        case 1:
                            newBorderTile = new AreaTile(tile.X, tile.Y + 1);
                            if (!passable.Contains(newBorderTile) && !border.Contains(newBorderTile))
                            {
                                border.Add(newBorderTile);
                                border2.Add(newBorderTile);
                            }
                            break;
                        case 2:
                            newBorderTile = new AreaTile(tile.X - 1, tile.Y);
                            if (!passable.Contains(newBorderTile) && !border.Contains(newBorderTile))
                            {
                                border.Add(newBorderTile);
                                border2.Add(newBorderTile);
                            }
                            break;
                        case 3:
                            newBorderTile = new AreaTile(tile.X, tile.Y - 1);
                            if (!passable.Contains(newBorderTile) && !border.Contains(newBorderTile))
                            {
                                border.Add(newBorderTile);
                                border2.Add(newBorderTile);
                            }
                            break;
                    }
                    k = (k + d) % 4;
                    if (k < 0) k += 4;
                }
                k = (k - d) % 4;
                if (k < 0) k += 4;
            }
            if (rmBorder > 0) RemoveBorder(passable, border, rmBorder, random);
            if (rmNoBorder > 0) RemoveNoBorder(passable, border, rmNoBorder, random, false);


            _tiles = ListToMatrix(passable, border, resolution, random, noise);
        }

        public void LineGen(int deltaX, int deltaY, int resolution, int noise, double proba)
        {
            random = new Random();
            List<AreaTile> passable = new List<AreaTile>();
            AreaTile currentTile = new AreaTile(0, 0);
            passable.Add(currentTile);
            List<AreaTile> border = new List<AreaTile>();
            border.Add(new AreaTile(1, 0));
            border.Add(new AreaTile(-1, 0));
            border.Add(new AreaTile(0, 1));
            border.Add(new AreaTile(0, -1));

            AreaTile lastTile = currentTile;
            AreaTile nextTile;
            while (!passable.Contains(new AreaTile(deltaX, deltaY)))
            {
                int dxtmp, dytmp, ddx, ddy, dx, dy, currentX, currentY, X, Y;
                ddx = 0;
                ddy = 0;
                do
                {
                    dxtmp = deltaX - currentTile.X;
                    dytmp = deltaY - currentTile.Y;

                    if (Math.Abs(dxtmp) + Math.Abs(dytmp) > 5) //todo calculer 5
                    {
                        ddx = (int)Math.Round((random.NextDouble() * 2 - 1) * noise * (double)(Math.Abs(dytmp)) * (double)(Math.Abs(deltaX) + 10) / (double)(Math.Abs(deltaY) + 10));
                        ddy = (int)Math.Round((random.NextDouble() * 2 - 1) * noise * (double)(Math.Abs(dxtmp)) * (double)(Math.Abs(deltaY) + 10) / (double)(Math.Abs(deltaX) + 10));
                    }
                    else
                    {
                        ddx = 0;
                        ddy = 0;
                    }

                    dx = dxtmp + ddx;
                    dy = dytmp + ddy;

                    if (Math.Abs(dx) + Math.Abs(dy) == 0)
                    {
                        dx = dxtmp;
                        dy = dytmp;
                    }

                    currentX = currentTile.X;
                    currentY = currentTile.Y;
                    if (dy == 0 || (double)Math.Abs(dx) / (double)Math.Abs(dy) > (double)Math.Abs(deltaX) / (double)Math.Abs(deltaY))
                    {
                        Y = 0;
                        X = Math.Sign(dx);
                    }
                    else
                    {
                        X = 0;
                        Y = Math.Sign(dy);
                    }

                    nextTile = new AreaTile(currentX + X, currentY + Y);
                } while (nextTile.Equals(lastTile));

                border.Remove(nextTile);
                passable.Add(nextTile);
                lastTile = currentTile;
                currentTile = nextTile;

                AreaTile newBorderTile;
                for (int j = 0; j < 4; j++)
                {
                    switch (j)
                    {
                        case 0:
                            newBorderTile = new AreaTile(nextTile.X + 1, nextTile.Y);
                            if (!passable.Contains(newBorderTile) && !border.Contains(newBorderTile)) border.Add(newBorderTile);
                            break;
                        case 1:
                            newBorderTile = new AreaTile(nextTile.X, nextTile.Y + 1);
                            if (!passable.Contains(newBorderTile) && !border.Contains(newBorderTile)) border.Add(newBorderTile);
                            break;
                        case 2:
                            newBorderTile = new AreaTile(nextTile.X - 1, nextTile.Y);
                            if (!passable.Contains(newBorderTile) && !border.Contains(newBorderTile)) border.Add(newBorderTile);
                            break;
                        case 3:
                            newBorderTile = new AreaTile(nextTile.X, nextTile.Y - 1);
                            if (!passable.Contains(newBorderTile) && !border.Contains(newBorderTile)) border.Add(newBorderTile);
                            break;
                    }
                }
            }

            _tiles = ListToMatrix(passable, border, resolution, random, proba);
        }

        public void HillGen(int baseSize, int height, int stretchX, int stretchY, int stretchBase)
        {
            AreaGen area = new AreaGen();
            area.StretchedMazeGen(baseSize, 0.5, stretchX, stretchY, stretchBase, false, 1, 0.10, 1, 0);

            Width = area.Width;
            Height = area.Height;
            _tiles = area._tiles;
            Center = area.Center;
            area.FillCorners();

            for (int k = 1; k <= height; k++)
            {
                int skx = (stretchX - 1) * k * 5 / stretchX;
                int sky = (stretchY - 1) * k * 5 / stretchY;
                AreaGen area2 = new AreaGen();
                area2.StretchedMazeGen((int)(baseSize * Math.Pow(0.6, Math.Pow(k, 1.5))), 0.5, stretchX + skx, stretchY + sky, (int)(stretchBase * 0.9 / (k + 1)), false, 1, 0.15, 1, 0);
                area2.FillCorners();

                for (int i = 0; i < area2.Width; i++)
                {
                    for (int j = 0; j < area2.Height; j++)
                    {
                        if (area2._tiles[i, j] != AreaTileType.Depth)
                        {
                            _tiles[Center.X - area2.Center.X + i, Center.Y - area2.Center.Y + j] = area2._tiles[i, j];
                        }
                    }
                }
            }
        }

        public void HillGen2Simple(int baseSize, int threshold)
        {
            List<AreaTile> sources = new List<AreaTile>();
            sources.Add(new AreaTile(0, 0));
            HillGen2(baseSize, sources, 1, threshold);
        }

        public void HillGen2(int baseSize, List<AreaTile> sources, int mult, int threshold)
        {
            HashSet<AreaTile> passable = new HashSet<AreaTile>();
            HashSet<AreaTile> border = new HashSet<AreaTile>();
            HashSet<AreaTile> ring = new HashSet<AreaTile>();
            //AreaTile center = new AreaTile(0, 0);
            //initCenter(center, passable, border, ring);
            List<HashSet<AreaTile>> borders = new List<HashSet<AreaTile>>();
            foreach (AreaTile source in sources)
            {
                AreaTile source2 = new AreaTile(source.X * mult, source.Y * mult);
                InitCenter(source2, passable, border, ring);
                HashSet<AreaTile> border2 = new HashSet<AreaTile>();
                borders.Add(border2);
                InitCenter(source2, passable, border2, ring);
            }




            for (int i = 0; i < baseSize; i++)
            {
                HashSet<AreaTile> border2 = borders[i % borders.Count];
                if (border2.Count == 0)
                {
                    continue;
                }

                List<AreaTile> validBorders = GetValidBorders(passable, border2);
                if (validBorders.Count == 0)
                {
                    continue;
                }
                AreaTile tile = validBorders[random.Next(validBorders.Count)];
                border2.Remove(tile);
                border.Remove(tile);
                passable.Add(tile);

                Add8adjTiles(random.Next(8), tile, border, passable, ring, border2);
            }

            RemoveNoBorder(passable, border, 1, random, true);

            List<AreaTile> validborderPaths = GetValidBorderPaths(passable, border);
            for (int i = 0; i < 1 + validborderPaths.Count / 50; i++)
            {
                AreaTile exit = validborderPaths[random.Next(validborderPaths.Count)];
                border.Remove(exit);
                passable.Add(exit);
            }

            int subSize = GetValidCenters(passable, border).Count;
            if (subSize > threshold) HillGen3(subSize, passable, border, threshold);

            passable.UnionWith(ring);

            _tiles = ListToMatrix(passable, border, 1, random, 0);
        }

        public void HillGen3(int baseSize, HashSet<AreaTile> passable, HashSet<AreaTile> border, int threshold)
        {
            HashSet<AreaTile> passable2 = new HashSet<AreaTile>();
            HashSet<AreaTile> border2 = new HashSet<AreaTile>();
            HashSet<AreaTile> ring = new HashSet<AreaTile>();
            List<AreaTile> centers = GetValidCenters(passable, border);
            AreaTile center = centers[random.Next(centers.Count)];
            InitCenter(center, passable2, border2, ring);

            for (int i = 0; i < baseSize; i++)
            {
                List<AreaTile> validBorders = GetValidBorders(passable2, border2);
                AreaTile tile = validBorders[random.Next(validBorders.Count)];
                border2.Remove(tile);
                passable2.Add(tile);

                List<AreaTile> added = Add8adjTiles(random.Next(4), tile, border2, passable2, ring, null);

                List<AreaTile> intersect = new List<AreaTile>();
                foreach (AreaTile t in ring)
                {
                    if (border.Contains(t))
                    {
                        intersect.Add(t);
                    }
                }
                if (intersect.Count > 0)
                {
                    border2.RemoveWhere(t => added.Contains(t));
                    passable2.Remove(tile);
                    border2.Add(tile);
                    ring.RemoveWhere(t => intersect.Contains(t));
                }
            }

            RemoveNoBorder(passable2, border2, 1, random, true);
            List<AreaTile> validborderPaths = GetValidBorderPaths(passable2, border2);
            for (int i = 0; i < 1 + validborderPaths.Count / 50; i++)
            {
                border2.Remove(validborderPaths[random.Next(validborderPaths.Count)]);
            }

            border.UnionWith(border2);
            passable.RemoveWhere(t => border2.Contains(t));

            int subSize = GetValidCenters(passable, border).Count;
            if (subSize > threshold) HillGen3(subSize, passable, border, threshold);
        }

        private bool ValidCenter(AreaTile tile, HashSet<AreaTile> border)
        {
            foreach (AreaTile tile2 in border)
            {
                if (Dist(tile, tile2, true) < 3) return false;
            }
            return true;
        }

        private List<AreaTile> GetValidCenters(HashSet<AreaTile> passable, HashSet<AreaTile> border)
        {
            List<AreaTile> res = new List<AreaTile>();
            foreach (AreaTile tile in passable)
            {
                if (ValidCenter(tile, border)) res.Add(tile);
            }
            return res;
        }

        private List<AreaTile> GetValidBorders(HashSet<AreaTile> passable, HashSet<AreaTile> border)
        {
            List<AreaTile> res = new List<AreaTile>();
            bool b;
            foreach (AreaTile tile in border)
            {
                b = false;
                if (passable.Contains(new AreaTile(tile.X + 1, tile.Y))) b = true;
                if (passable.Contains(new AreaTile(tile.X, tile.Y + 1))) b = true;
                if (passable.Contains(new AreaTile(tile.X - 1, tile.Y))) b = true;
                if (passable.Contains(new AreaTile(tile.X, tile.Y - 1))) b = true;

                if (b) res.Add(tile);
            }
            return res;
        }

        private List<AreaTile> GetValidBorderPaths(HashSet<AreaTile> passable, HashSet<AreaTile> border)
        {
            List<AreaTile> res = new List<AreaTile>();
            int k;
            foreach (AreaTile tile in border)
            {
                k = 0;
                if (passable.Contains(new AreaTile(tile.X + 1, tile.Y))) k++;
                if (passable.Contains(new AreaTile(tile.X, tile.Y + 1))) k++;
                if (passable.Contains(new AreaTile(tile.X - 1, tile.Y))) k++;
                if (passable.Contains(new AreaTile(tile.X, tile.Y - 1))) k++;

                if (k == 1)
                {
                    if (border.Contains(new AreaTile(tile.X + 1, tile.Y))) k++;
                    if (border.Contains(new AreaTile(tile.X, tile.Y + 1))) k++;
                    if (border.Contains(new AreaTile(tile.X - 1, tile.Y))) k++;
                    if (border.Contains(new AreaTile(tile.X, tile.Y - 1))) k++;
                    if (k == 3) res.Add(tile);
                }
            }
            return res;
        }

        private List<AreaTile> Add8adjTiles(int k, AreaTile tile, HashSet<AreaTile> adding, HashSet<AreaTile> removing, HashSet<AreaTile> removing2, HashSet<AreaTile> adding2)
        {
            List<AreaTile> added = new List<AreaTile>();
            AreaTile newTile;
            int d = 1;
            k = (k + d) % 8;
            if (k < 0) k += 8;
            for (int j = 0; j < 8; j++)
            {
                switch (k)
                {
                    case 0:
                        newTile = new AreaTile(tile.X + 1, tile.Y);
                        if (!adding.Contains(newTile) && !removing.Contains(newTile))
                        {
                            adding.Add(newTile);
                            if (adding2 != null) adding2.Add(newTile);
                            if (removing2 != null)
                            {
                                removing2.Remove(newTile);
                                added.Add(newTile);
                            }
                        }
                        break;
                    case 1:
                        newTile = new AreaTile(tile.X, tile.Y + 1);
                        if (!adding.Contains(newTile) && !removing.Contains(newTile))
                        {
                            adding.Add(newTile);
                            if (adding2 != null) adding2.Add(newTile);
                            if (removing2 != null)
                            {
                                removing2.Remove(newTile);
                                added.Add(newTile);
                            }
                        }
                        break;
                    case 2:
                        newTile = new AreaTile(tile.X - 1, tile.Y);
                        if (!adding.Contains(newTile) && !removing.Contains(newTile))
                        {
                            adding.Add(newTile);
                            if (adding2 != null) adding2.Add(newTile);
                            if (removing2 != null)
                            {
                                removing2.Remove(newTile);
                                added.Add(newTile);
                            }
                        }
                        break;
                    case 3:
                        newTile = new AreaTile(tile.X, tile.Y - 1);
                        if (!adding.Contains(newTile) && !removing.Contains(newTile))
                        {
                            adding.Add(newTile);
                            if (adding2 != null) adding2.Add(newTile);
                            if (removing2 != null)
                            {
                                removing2.Remove(newTile);
                                added.Add(newTile);
                            }
                        }
                        break;
                    case 4:
                        newTile = new AreaTile(tile.X + 1, tile.Y + 1);
                        if (!adding.Contains(newTile) && !removing.Contains(newTile))
                        {
                            adding.Add(newTile);
                            if (adding2 != null) adding2.Add(newTile);
                            if (removing2 != null)
                            {
                                removing2.Remove(newTile);
                                added.Add(newTile);
                            }
                        }
                        break;
                    case 5:
                        newTile = new AreaTile(tile.X + 1, tile.Y - 1);
                        if (!adding.Contains(newTile) && !removing.Contains(newTile))
                        {
                            adding.Add(newTile);
                            if (adding2 != null) adding2.Add(newTile);
                            if (removing2 != null)
                            {
                                removing2.Remove(newTile);
                                added.Add(newTile);
                            }
                        }
                        break;
                    case 6:
                        newTile = new AreaTile(tile.X - 1, tile.Y + 1);
                        if (!adding.Contains(newTile) && !removing.Contains(newTile))
                        {
                            adding.Add(newTile);
                            if (adding2 != null) adding2.Add(newTile);
                            if (removing2 != null)
                            {
                                removing2.Remove(newTile);
                                added.Add(newTile);
                            }
                        }
                        break;
                    case 7:
                        newTile = new AreaTile(tile.X - 1, tile.Y - 1);
                        if (!adding.Contains(newTile) && !removing.Contains(newTile))
                        {
                            adding.Add(newTile);
                            if (adding2 != null) adding2.Add(newTile);
                            if (removing2 != null)
                            {
                                removing2.Remove(newTile);
                                added.Add(newTile);
                            }
                        }
                        break;
                }
                k = (k + d) % 8;
                if (k < 0) k += 8;
            }
            k = (k - d) % 8;
            if (k < 0) k += 8;


            HashSet<AreaTile> removing1 = new HashSet<AreaTile>(removing);
            removing1.UnionWith(adding);
            foreach (AreaTile t in added)
            {
                Add8adjTiles(0, t, removing2, removing1, null, null);
            }

            return added;
        }

        private AreaTile GetRandomBorder(List<AreaTile> border, double mazeness, bool exp)
        {
            int k;

            if (exp)
            {
                k = (int)(Math.Pow(random.NextDouble(), mazeness) * border.Count);
            }
            else
            {
                k = random.Next((int)mazeness);
                if (k != 0) k = 0;
                else k = random.Next(border.Count - 1);
                //idée : choisir ceux plus près du centre
                //et qui ont qu'un seul passable en voisin (mode étoile)
                //sauf si deux passables opposés ?
            }

            return border[border.Count - k - 1];
        }

        private AreaTileType[,] ListToMatrix(List<AreaTile> pass, List<AreaTile> bord, int resolution, Random random, double proba)
        {
            int minX = 0;
            int maxX = 0;
            int minY = 0;
            int maxY = 0;

            foreach (AreaTile tile in bord)
            {
                if (tile.X < minX) minX = tile.X;
                if (tile.Y < minY) minY = tile.Y;
                if (tile.X > maxX) maxX = tile.X;
                if (tile.Y > maxY) maxY = tile.Y;
            }

            foreach (AreaTile tile in pass)
            {
                if (tile.X < minX) minX = tile.X;
                if (tile.Y < minY) minY = tile.Y;
                if (tile.X > maxX) maxX = tile.X;
                if (tile.Y > maxY) maxY = tile.Y;
            }
            minX *= resolution;
            minY *= resolution;
            maxX *= resolution;
            maxY *= resolution;

            Width = maxX - minX + resolution;
            Height = maxY - minY + resolution;
            Center = new AreaTile(-minX, -minY);


            AreaTileType[,] res = new AreaTileType[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    res[i, j] = AreaTileType.Depth;
                }
            }

            foreach (AreaTile tile in pass)
            {
                for (int i = 0; i < resolution; i++)
                {
                    for (int j = 0; j < resolution; j++)
                    {
                        res[tile.X * resolution + i - minX, tile.Y * resolution + j - minY] = AreaTileType.Ground;
                    }
                }
            }

            foreach (AreaTile tile in bord)
            {
                for (int i = 0; i < resolution; i++)
                {
                    for (int j = 0; j < resolution; j++)
                    {
                        res[tile.X * resolution + i - minX, tile.Y * resolution + j - minY] = AreaTileType.Wall;
                    }
                }

                if (resolution > 1)
                {

                    //x--
                    //---
                    //---


                    if (!bord.Contains(new AreaTile(tile.X - 1, tile.Y - 1)) &&
                        ((pass.Contains(new AreaTile(tile.X - 1, tile.Y)) && random.NextDouble() < proba) ||
                        (pass.Contains(new AreaTile(tile.X, tile.Y - 1)) && random.NextDouble() < proba)))
                    {
                        res[tile.X * resolution - minX, tile.Y * resolution - minY] = AreaTileType.Ground;
                    }

                    //-x-
                    //---
                    //---

                    if (pass.Contains(new AreaTile(tile.X, tile.Y - 1)))
                    {
                        for (int k = 1; k < resolution - 1; k++)
                        {
                            if (random.NextDouble() < proba)
                            {
                                res[tile.X * resolution + k - minX, tile.Y * resolution - minY] = AreaTileType.Ground;
                            }
                        }
                    }

                    //--x
                    //---
                    //---

                    if (!bord.Contains(new AreaTile(tile.X + 1, tile.Y - 1)) &&
                        ((pass.Contains(new AreaTile(tile.X + 1, tile.Y)) && random.NextDouble() < proba) ||
                        (pass.Contains(new AreaTile(tile.X, tile.Y - 1)) && random.NextDouble() < proba)))
                    {
                        res[(tile.X + 1) * resolution - 1 - minX, tile.Y * resolution - minY] = AreaTileType.Ground;
                    }

                    //---
                    //--x
                    //---

                    if (pass.Contains(new AreaTile(tile.X + 1, tile.Y)))
                    {
                        for (int k = 1; k < resolution - 1; k++)
                        {
                            if (random.NextDouble() < proba)
                            {
                                res[(tile.X + 1) * resolution - 1 - minX, tile.Y * resolution + k - minY] = AreaTileType.Ground;
                            }
                        }
                    }

                    //---
                    //---
                    //--x

                    if (!bord.Contains(new AreaTile(tile.X + 1, tile.Y + 1)) &&
                        ((pass.Contains(new AreaTile(tile.X + 1, tile.Y)) && random.NextDouble() < proba) ||
                        (pass.Contains(new AreaTile(tile.X, tile.Y + 1)) && random.NextDouble() < proba)))
                    {
                        res[(tile.X + 1) * resolution - 1 - minX, (tile.Y + 1) * resolution - 1 - minY] = AreaTileType.Ground;
                    }

                    //-x-
                    //---
                    //---

                    if (pass.Contains(new AreaTile(tile.X, tile.Y + 1)))
                    {
                        for (int k = 1; k < resolution - 1; k++)
                        {
                            if (random.NextDouble() < proba)
                            {
                                res[tile.X * resolution + k - minX, (tile.Y + 1) * resolution - 1 - minY] = AreaTileType.Ground;
                            }
                        }
                    }

                    //---
                    //---
                    //x--

                    if (!bord.Contains(new AreaTile(tile.X - 1, tile.Y + 1)) &&
                        ((pass.Contains(new AreaTile(tile.X - 1, tile.Y)) && random.NextDouble() < proba) ||
                        (pass.Contains(new AreaTile(tile.X, tile.Y + 1)) && random.NextDouble() < proba)))
                    {
                        res[tile.X * resolution - minX, (tile.Y + 1) * resolution - 1 - minY] = AreaTileType.Ground;
                    }

                    //---
                    //x--
                    //---

                    if (pass.Contains(new AreaTile(tile.X - 1, tile.Y)))
                    {
                        for (int k = 1; k < resolution - 1; k++)
                        {
                            if (random.NextDouble() < proba)
                            {
                                res[tile.X * resolution - minX, tile.Y * resolution + k - minY] = AreaTileType.Ground;
                            }
                        }
                    }


                }
            }

            return res;
        }

        private AreaTileType[,] ListToMatrix(HashSet<AreaTile> pass, HashSet<AreaTile> bord, int resolution, Random random, double proba)
        {
            int minX = 0;
            int maxX = 0;
            int minY = 0;
            int maxY = 0;

            foreach (AreaTile tile in bord)
            {
                if (tile.X < minX) minX = tile.X;
                if (tile.Y < minY) minY = tile.Y;
                if (tile.X > maxX) maxX = tile.X;
                if (tile.Y > maxY) maxY = tile.Y;
            }

            foreach (AreaTile tile in pass)
            {
                if (tile.X < minX) minX = tile.X;
                if (tile.Y < minY) minY = tile.Y;
                if (tile.X > maxX) maxX = tile.X;
                if (tile.Y > maxY) maxY = tile.Y;
            }
            minX *= resolution;
            minY *= resolution;
            maxX *= resolution;
            maxY *= resolution;

            Width = maxX - minX + resolution;
            Height = maxY - minY + resolution;
            Center = new AreaTile(-minX, -minY);


            AreaTileType[,] res = new AreaTileType[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    res[i, j] = AreaTileType.Depth;
                }
            }

            foreach (AreaTile tile in pass)
            {
                for (int i = 0; i < resolution; i++)
                {
                    for (int j = 0; j < resolution; j++)
                    {
                        res[tile.X * resolution + i - minX, tile.Y * resolution + j - minY] = AreaTileType.Ground;
                    }
                }
            }

            foreach (AreaTile tile in bord)
            {
                for (int i = 0; i < resolution; i++)
                {
                    for (int j = 0; j < resolution; j++)
                    {
                        res[tile.X * resolution + i - minX, tile.Y * resolution + j - minY] = AreaTileType.Wall;
                    }
                }

                if (resolution > 1)
                {

                    //x--
                    //---
                    //---


                    if (!bord.Contains(new AreaTile(tile.X - 1, tile.Y - 1)) &&
                        ((pass.Contains(new AreaTile(tile.X - 1, tile.Y)) && random.NextDouble() < proba) ||
                        (pass.Contains(new AreaTile(tile.X, tile.Y - 1)) && random.NextDouble() < proba)))
                    {
                        res[tile.X * resolution - minX, tile.Y * resolution - minY] = AreaTileType.Ground;
                    }

                    //-x-
                    //---
                    //---

                    if (pass.Contains(new AreaTile(tile.X, tile.Y - 1)))
                    {
                        for (int k = 1; k < resolution - 1; k++)
                        {
                            if (random.NextDouble() < proba)
                            {
                                res[tile.X * resolution + k - minX, tile.Y * resolution - minY] = AreaTileType.Ground;
                            }
                        }
                    }

                    //--x
                    //---
                    //---

                    if (!bord.Contains(new AreaTile(tile.X + 1, tile.Y - 1)) &&
                        ((pass.Contains(new AreaTile(tile.X + 1, tile.Y)) && random.NextDouble() < proba) ||
                        (pass.Contains(new AreaTile(tile.X, tile.Y - 1)) && random.NextDouble() < proba)))
                    {
                        res[(tile.X + 1) * resolution - 1 - minX, tile.Y * resolution - minY] = AreaTileType.Ground;
                    }

                    //---
                    //--x
                    //---

                    if (pass.Contains(new AreaTile(tile.X + 1, tile.Y)))
                    {
                        for (int k = 1; k < resolution - 1; k++)
                        {
                            if (random.NextDouble() < proba)
                            {
                                res[(tile.X + 1) * resolution - 1 - minX, tile.Y * resolution + k - minY] = AreaTileType.Ground;
                            }
                        }
                    }

                    //---
                    //---
                    //--x

                    if (!bord.Contains(new AreaTile(tile.X + 1, tile.Y + 1)) &&
                        ((pass.Contains(new AreaTile(tile.X + 1, tile.Y)) && random.NextDouble() < proba) ||
                        (pass.Contains(new AreaTile(tile.X, tile.Y + 1)) && random.NextDouble() < proba)))
                    {
                        res[(tile.X + 1) * resolution - 1 - minX, (tile.Y + 1) * resolution - 1 - minY] = AreaTileType.Ground;
                    }

                    //-x-
                    //---
                    //---

                    if (pass.Contains(new AreaTile(tile.X, tile.Y + 1)))
                    {
                        for (int k = 1; k < resolution - 1; k++)
                        {
                            if (random.NextDouble() < proba)
                            {
                                res[tile.X * resolution + k - minX, (tile.Y + 1) * resolution - 1 - minY] = AreaTileType.Ground;
                            }
                        }
                    }

                    //---
                    //---
                    //x--

                    if (!bord.Contains(new AreaTile(tile.X - 1, tile.Y + 1)) &&
                        ((pass.Contains(new AreaTile(tile.X - 1, tile.Y)) && random.NextDouble() < proba) ||
                        (pass.Contains(new AreaTile(tile.X, tile.Y + 1)) && random.NextDouble() < proba)))
                    {
                        res[tile.X * resolution - minX, (tile.Y + 1) * resolution - 1 - minY] = AreaTileType.Ground;
                    }

                    //---
                    //x--
                    //---

                    if (pass.Contains(new AreaTile(tile.X - 1, tile.Y)))
                    {
                        for (int k = 1; k < resolution - 1; k++)
                        {
                            if (random.NextDouble() < proba)
                            {
                                res[tile.X * resolution - minX, tile.Y * resolution + k - minY] = AreaTileType.Ground;
                            }
                        }
                    }


                }
            }

            return res;
        }

        private int GetAdjVoidTiles(AreaTile tile, List<AreaTile> passable, List<AreaTile> border, bool square)
        {
            int res = 0;

            AreaTile adjTile = new AreaTile(tile.X + 1, tile.Y);
            if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
            adjTile = new AreaTile(tile.X, tile.Y + 1);
            if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
            adjTile = new AreaTile(tile.X - 1, tile.Y);
            if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
            adjTile = new AreaTile(tile.X, tile.Y - 1);
            if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;

            if (square)
            {
                adjTile = new AreaTile(tile.X + 1, tile.Y + 1);
                if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
                adjTile = new AreaTile(tile.X + 1, tile.Y - 1);
                if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
                adjTile = new AreaTile(tile.X - 1, tile.Y + 1);
                if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
                adjTile = new AreaTile(tile.X - 1, tile.Y - 1);
                if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
            }



            return res;
        }

        private int GetAdjVoidTiles(AreaTile tile, HashSet<AreaTile> passable, HashSet<AreaTile> border, bool square)
        {
            int res = 0;

            AreaTile adjTile = new AreaTile(tile.X + 1, tile.Y);
            if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
            adjTile = new AreaTile(tile.X, tile.Y + 1);
            if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
            adjTile = new AreaTile(tile.X - 1, tile.Y);
            if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
            adjTile = new AreaTile(tile.X, tile.Y - 1);
            if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;

            if (square)
            {
                adjTile = new AreaTile(tile.X + 1, tile.Y + 1);
                if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
                adjTile = new AreaTile(tile.X + 1, tile.Y - 1);
                if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
                adjTile = new AreaTile(tile.X - 1, tile.Y + 1);
                if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
                adjTile = new AreaTile(tile.X - 1, tile.Y - 1);
                if (!passable.Contains(adjTile) && !border.Contains(adjTile)) res++;
            }



            return res;
        }

        private void RemoveBorder(List<AreaTile> passable, List<AreaTile> border, double proba, Random random)
        {
            List<AreaTile> rm = new List<AreaTile>();

            foreach (AreaTile tile in border)
            {
                if (GetAdjVoidTiles(tile, passable, border, false) > 0)
                {
                    if (random.NextDouble() < proba)
                    {
                        rm.Add(tile);
                    }
                }
            }
            border.RemoveAll(t => rm.Contains(t));
            passable.AddRange(rm);
        }

        private void RemoveNoBorder(List<AreaTile> passable, List<AreaTile> border, double proba, Random random, bool square)
        {
            List<AreaTile> rm = new List<AreaTile>();

            foreach (AreaTile tile in border)
            {
                if (GetAdjVoidTiles(tile, passable, border, square) < 1)
                {
                    if (random.NextDouble() < proba)
                    {
                        rm.Add(tile);
                    }
                }
            }
            border.RemoveAll(t => rm.Contains(t));
            passable.AddRange(rm);
        }

        private void RemoveNoBorder(HashSet<AreaTile> passable, HashSet<AreaTile> border, double proba, Random random, bool square)
        {
            List<AreaTile> rm = new List<AreaTile>();

            foreach (AreaTile tile in border)
            {
                if (GetAdjVoidTiles(tile, passable, border, square) < 1)
                {
                    if (random.NextDouble() < proba)
                    {
                        rm.Add(tile);
                    }
                }
            }
            border.RemoveWhere(t => rm.Contains(t));
            passable.UnionWith(rm);
        }

        private void FillCorners()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (_tiles[i, j] == AreaTileType.Depth)
                    {
                        int k = 0;
                        if (i != Width - 1 && _tiles[i + 1, j] == AreaTileType.Wall) k++;
                        if (j != Height - 1 && _tiles[i, j + 1] == AreaTileType.Wall) k++;
                        if (i != 0 && _tiles[i - 1, j] == AreaTileType.Wall) k++;
                        if (j != 0 && _tiles[i, j - 1] == AreaTileType.Wall) k++;

                        if (k >= 2) _tiles[i, j] = AreaTileType.Undefined;
                    }
                }
            }

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (_tiles[i, j] == AreaTileType.Undefined)
                    {
                        _tiles[i, j] = AreaTileType.Wall;
                    }
                }
            }


        }

        public AreaTileType GetTileType(int x, int y)
        {
            return _tiles[x, y];
        }

        private int Dist(AreaTile t1, AreaTile t2, bool square)
        {
            if (!square) return Math.Abs(t1.X - t2.X) + Math.Abs(t1.Y - t2.Y);
            else return Math.Max(Math.Abs(t1.X - t2.X), Math.Abs(t1.Y - t2.Y));
        }

        private void InitCenter(AreaTile center, HashSet<AreaTile> passable, HashSet<AreaTile> border, HashSet<AreaTile> ring)
        {
            passable.Add(new AreaTile(center.X, center.Y));
            border.Add(new AreaTile(center.X + 1, center.Y));
            border.Add(new AreaTile(center.X - 1, center.Y));
            border.Add(new AreaTile(center.X, center.Y + 1));
            border.Add(new AreaTile(center.X, center.Y - 1));
            border.Add(new AreaTile(center.X + 1, center.Y + 1));
            border.Add(new AreaTile(center.X - 1, center.Y - 1));
            border.Add(new AreaTile(center.X - 1, center.Y + 1));
            border.Add(new AreaTile(center.X + 1, center.Y - 1));
            ring.Add(new AreaTile(center.X + 2, center.Y + 2));
            ring.Add(new AreaTile(center.X + 2, center.Y + 1));
            ring.Add(new AreaTile(center.X + 2, center.Y));
            ring.Add(new AreaTile(center.X + 2, center.Y - 1));
            ring.Add(new AreaTile(center.X + 2, center.Y - 2));
            ring.Add(new AreaTile(center.X + 1, center.Y + 2));
            ring.Add(new AreaTile(center.X, center.Y + 2));
            ring.Add(new AreaTile(center.X - 1, center.Y + 2));
            ring.Add(new AreaTile(center.X - 2, center.Y + 2));
            ring.Add(new AreaTile(center.X - 2, center.Y + 1));
            ring.Add(new AreaTile(center.X - 2, center.Y));
            ring.Add(new AreaTile(center.X - 2, center.Y - 1));
            ring.Add(new AreaTile(center.X - 2, center.Y - 2));
            ring.Add(new AreaTile(center.X - 1, center.Y - 2));
            ring.Add(new AreaTile(center.X, center.Y - 2));
            ring.Add(new AreaTile(center.X + 1, center.Y - 2));
        }
    }
}
