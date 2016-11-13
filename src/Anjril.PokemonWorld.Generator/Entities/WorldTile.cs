using Anjril.PokemonWorld.Generator.Enums;

namespace Anjril.PokemonWorld.Generator.Entities
{
    public class WorldTile
    {
        public GroundTileType Ground { get; private set; }
        public ObjectTileType Object { get; set; }
        
        public WorldTile(GroundTileType ground)
        {
            Ground = ground;
            Object = ObjectTileType.None;
        }

        public WorldTile(GroundTileType ground, ObjectTileType obj)
        {
            Ground = ground;
            Object = obj;
        }
    }
}