using Godot;

namespace GridChanges {
    public abstract partial class GridChange : GodotObject{
        public abstract void Apply(Tile[,] _tiles);
    }
}
