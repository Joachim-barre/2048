using Godot;
using Godot.Collections;

public partial class SaveData : Resource
{
    [Export]
    public Array<int> vals;

    [Export]
    public int Score;
}
