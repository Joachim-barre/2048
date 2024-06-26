using Godot;
using Godot.Collections;
using GridChanges;
using System.Linq;

public partial class GridWithChanges : GodotObject{
    public Grid grid;
    public Array<GridChange> changes;

    public GridWithChanges(Grid _grid){
        grid = _grid;
    }

    public GridWithChanges Bind(System.Func<Grid, GridWithChanges> func){
        var res = func(grid);
        res.changes = new  Array<GridChange>(changes.Concat(res.changes).ToArray());
        return res;
    }

    public Grid Unwrap(){
        return grid;
    }
}
