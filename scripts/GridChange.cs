using Godot;

namespace GridChanges {
    public abstract partial class GridChange : GodotObject{
        public abstract void Apply(Tile[,] _tiles);
    }

    public abstract partial class Push : GridChange{
        public Vector2 Tile1;
        public Vector2 Tile2;

        public override void Apply(Tile[,] _tiles){

        }
    }

    public abstract partial class Merge : GridChange{
        public Vector2 Tile1;
        public Vector2 Tile2;

        public override void Apply(Tile[,] _tiles){

        }
    }

    public abstract partial class GeneratedTile : GridChange{
        public Vector2 Tile;

        public override void Apply(Tile[,] _tiles){

        }
    }
}
