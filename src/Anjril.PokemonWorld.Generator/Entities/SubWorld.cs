using Anjril.PokemonWorld.Generator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anjril.PokemonWorld.Generator.Entities
{
    public class SubWorld
    {
        public WorldTile[,] WorldTiles {get; set;}

        public SubWorld(int width, int height)
        {
            WorldTiles = new WorldTile[width, height];
        }
    }
}
