using Godot;
using Godot.Collections;

public partial class SaveData : Resource
{
    [Export]
    public Array<int> vals;

    [Export]
    public int Score;

    [Export]
    public int HighScore;

    public int[,] GetVals(){
        if(vals.Count != 16){
            Log.error("Failed to get values from save data");
            return null;
        }
        var Res = new  int[,]{
            {0,0,0,0},
            {0,0,0,0},
            {0,0,0,0},
            {0,0,0,0},
        };
        for(int i=0;i!=4;i++)
            for(int j=0;j!=4;j++)
                Res[i,j]=vals[i*4+j];
        return Res;
    }

    public void LoadVals(int[,] _vals){
        if(_vals.GetLength(0)!=4||_vals.GetLength(1)!=4){
            Log.error("Failed to save values");
            return;
        }
        vals = new Array<int>();
        for(int i=0;i!=4;i++)
            for(int j=0;j!=4;j++)
                vals.Add(_vals[i,j]);
    }

    public void Write(FileAccess file) {
        file.Store32(2048); // magic number
        file.Store32((uint)Score);
        file.Store32((uint)HighScore);
        file.StoreVar(vals);
    }

    public static SaveData Read(FileAccess file) {
        if (file.Get32() != 2048) {
            Log.error("invalid save file format (bad magic number)");
            return null;
        }
        var res = new SaveData();
        res.Score = (int)file.Get32();
        res.HighScore = (int)file.Get32();
        var vals = file.GetVar(false);
        if (vals.VariantType != Variant.Type.Array) {
            Log.error("invalid save file format (invalid type for the array :"+vals.VariantType.ToString()+")");
            return null;
        }
        res.vals = vals.AsGodotArray<int>();
        return res;
    }
}
