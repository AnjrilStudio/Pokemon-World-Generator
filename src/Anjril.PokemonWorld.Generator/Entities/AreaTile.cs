using Anjril.PokemonWorld.Generator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anjril.PokemonWorld.Generator.Entities
{
    class AreaTile : IEquatable<AreaTile>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public AreaTileType Type { get; set; }

        public AreaTile(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(AreaTile other)
        {
            if (other == null) return false;
            return this.Dist(other) == 0;
        }
        
        public override int GetHashCode()
        {
            return X * 13457 + Y * 18947;
        }

        public bool NextTo(AreaTile other)
        {
            return this.Dist(other) == 1;
        }

        public int Dist(AreaTile other)
        {
            return Math.Abs(this.X - other.X) + Math.Abs(this.Y - other.Y);
        }
    }
}
