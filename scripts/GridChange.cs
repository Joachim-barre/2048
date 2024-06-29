using Godot;

namespace GridChange {
    public abstract class GridChange : GodotObject{
        public abstract void Apply(tile[,] tiles);
    }
}
