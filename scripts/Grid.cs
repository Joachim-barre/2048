using Godot;
using Godot.Collections;
using System.Linq;

public partial class Grid : Resource{
    [Export]
    private Array<int> GridFlat;

    public int[,] Values {
        get {
            var Res = new  int[,]{
                {0,0,0,0},
                {0,0,0,0},
                {0,0,0,0},
                {0,0,0,0},
            };
            for(int i=0;i!=4;i++)
                for(int j=0;j!=4;j++)
                    Res[i,j]=GridFlat[i*4+j];
            return Res;
        }
        set {
            if(value.GetLength(0)!=4||value.GetLength(1)!=4){
                GridFlat = new Array<int>(){0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            }
            GridFlat = new Array<int>();
            for(int i=0;i!=4;i++)
                for(int j=0;j!=4;j++)
                    GridFlat.Add(value[i,j]);
        }
    }
}
