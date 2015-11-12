using Anjril.PokemonWorld.Generator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anjril.PokemonWorld.Generator.Entities
{
    class SubWorld
    {
        private WorldTileType[,] tiles;

        public SubWorld(int width, int height)
        {
            tiles = new WorldTileType[width, height];
        }

        public WorldTileType get(int x, int y)
        {
            if (x < 0 || y < 0 || x >= WorldGen.subWidth || y >= WorldGen.subHeight)
            {
                return WorldTileType.Undefined;
            }
            return tiles[x, y];
        }

        public void set(int x, int y, WorldTileType type)
        {
            if (!(x < 0 || y < 0 || x >= WorldGen.subWidth || y >= WorldGen.subHeight))
            {
                tiles[x, y] = type;
            }
        }
    }
}
