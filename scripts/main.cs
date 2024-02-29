
using Godot;
using Godot.Collections;

public partial class main : Node{
    [Export]
    public PackedScene TileScene { get; set; }

    [Export]
    public Vector2 TileMapPos { get; set; }

    [Export]
    public Vector2 TileMapSize { get; set; }

    [Export]
    public Vector2 TileSize { get; set; }

    [Export]
    public Vector2 TileSpacing { get; set; }

    [Signal]
    public delegate void GameOverEventHandler();

    public bool IsGameOver;

    private tile[,] tiles;
    private int[,] pre_value;

    public enum Dir
    {
        None,
        Right,
        Left,
        Up,
        Down,
    }
    
    // transform r and c to get the first array index to acces the array in a specified direction
    private int get_idx2(Dir dir,int r, int c)
    {
        switch(dir)
        {   case Dir.Right:
            case Dir.Left:
                return r;
            case Dir.Down:
                return 3-c;
            default:
                return c;
        }
    }

    // transform r and c to get the first array index to acces the array in a specified direction
    private int get_idx1(Dir dir, int r, int c)
    {
        switch(dir)
        {
            case Dir.Right:
                return 3-c;
            case Dir.Left:
                return c;
            default:
                return r;
        }
    }

    // turn window position to grid position
    public Vector2 WinToGrid(Vector2 _win)
    {
        Vector2 _grid = _win;
        _grid -= TileMapPos;
        _grid /= TileSize + TileSpacing; 
        return _grid;
    }

    // turn grid position to window position
    public Vector2 GridToWin(Vector2 _grid)
    {
        Vector2 _win = _grid;
        _win *= TileSize + TileSpacing; 
        _win += TileMapPos;
        return _win;
    }

    public void WaitForAnimation(){
        while(GetTree().HasGroup("animating_tiles")){}
    }

    // create a tile a the specified grid position
    public tile CreateTile(Vector2 _pos)
    {
        var Tile = TileScene.Instantiate<tile>();
        Tile._pos = GridToWin(_pos);
        Tile.scale = 0.5F;
        Tile.UpdatePos();
        AddChild(Tile);
        tiles[Mathf.FloorToInt(_pos.X), Mathf.FloorToInt(_pos.Y)] = Tile;
        return Tile;
    }

    // generate a tile at a random free postion on the grid
    public tile GenerateTile(){
        var nullIndices = new Array<Vector2>();
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                if (tiles[i, j] == null)
                {
                    nullIndices.Add(new Vector2(i, j));
                }
            }
        }
        int idx = (int)Mathf.Abs(GD.Randi() % nullIndices.Count);
        Vector2 pos = nullIndices[idx];
        Log.dbg("generated tile : ");
        Log.dbg("\t" + pos.ToString());
        var Tile = CreateTile(pos);
        Tile.SetValue(((int)Mathf.Abs(GD.Randi() % 10))<=7 ? 2 : 4);
        return Tile;
    }

    // get the grid as an array of values
    public int[,] GetValues()
    {
        int[,] vals = new int[4,4]{
            {0,0,0,0},
            {0,0,0,0},
            {0,0,0,0},
            {0,0,0,0},
        };
        for(int i=0;i != 4;i++)
        {
            for(int j=0;j != 4;j++)
                vals[i,j] = (tiles[i,j]==null)?0: tiles[i,j].GetValue();
        }
        return vals;
    }

    public void OnReset()
    {
        Log.dbg("<<<  RESET  >>>");
        pre_value = GetValues();
        tiles = new tile[4,4]{
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
        };
        GetTree().CallGroup("tiles", Node.MethodName.QueueFree);
        GenerateTile();
        GenerateTile();
        IsGameOver = false;
    }

    public void OnUndo()
    {
        GetTree().CallGroup("tiles", Node.MethodName.QueueFree);
        Log.dbg("<<<  UNDO  >>>");
        tiles = new tile[4,4]{
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
        };
        IsGameOver = false;
        for(int i = 0; i != 4;i++)
        {
            for(int j = 0;j != 4;j++)
            {
                if(pre_value[i,j] != 0)
                {
                    tile t = CreateTile(new Vector2(i,j));
                    t.SetValue(pre_value[i,j]);
                }
            }
        }
    }

    public override void _Ready()
    {
        GD.Randomize();
        tiles = new tile[4,4]{
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
        };
        /*pre_value = new int[,]{
            {0    ,16    ,256    ,4096 },
            {2    ,32    ,512    ,8192 },
            {4    ,64    ,1024   ,16384},
            {8    ,128   ,2048   ,32768},
        };*/
        OnReset();
        //OnUndo();
    }

    // push and merge
    // TODO : fix it
    public void Merge(Dir dir)
    {
        var new_pos = new tile.StateChage[4,4]{
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
        };
        for(int i=0;i != 4;i++)
        {
            for(int j=0;j != 4;j++)
                new_pos[i,j] = (tiles[i,j]==null)?null: new tile.StateChage( GridToWin(new Vector2(i,j)));
        }
        int[,] vals = GetValues(); 
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                // Log.dbg("---------------------------------------");
                int idx1 = get_idx1(dir, i, j);
                int idx2 = get_idx2(dir, i, j);
                int val1 = vals[idx1, idx2];
                //Log.dbg("(pos(r,c), pos(r,c).T, val) : " + new Vector2(i, j).ToString() + " " + new Vector2(idx1, idx2).ToString() + " " + val1.ToString());
                
                if (val1 == 0)
                    continue;

                int k = j + 1; 
                int idx3 = get_idx1(dir, i, k);
                int idx4 = get_idx2(dir, i, k);
                int val2 = vals[idx3, idx4];

                if (val2 == 0)
                    continue;
                else if (val1 == val2)
                {
                    Log.dbg("merge :");
                    Log.dbg("\tdir : " + dir.ToString());
                    Log.dbg("\t(val1, val2) : " + new Vector2(val1, val2).ToString());
                    Log.dbg("\ttile 1 : " + tiles[idx1, idx2].ToString());
                    Log.dbg("\ttile 2 : " + tiles[idx3, idx4].ToString());
                    vals[idx1, idx2] = val1 + val2;
                    vals[idx3, idx4] = 0;
                    new_pos[idx3, idx4].stateCode = tile.StateChageCode.Delete;
                    new_pos[idx1, idx2].stateCode = tile.StateChageCode.Upgrade;
                    new_pos[idx3, idx4].new_pos = new_pos[idx1, idx2].new_pos;
                    new_pos[idx3,idx4].fusedWith = tiles[idx1, idx2];
                    new_pos[idx1,idx2].fusedWith = tiles[idx3, idx4]; 
                }
            }
        }
        tile[,] next_tiles = new tile[4,4]{
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
        };

        for(int i = 0;i != 4;i++)
        {
            for(int j = 0;j != 4;j++)
            {   
                tiles[i,j]?.Update(new_pos[i,j]);
                if(vals[i,j] != 0)
                {                            
                    if(new_pos[i,j].stateCode != tile.StateChageCode.Delete)
                    {
                        Vector2 _pos = WinToGrid(new_pos[i,j].new_pos);
                        next_tiles[(int)_pos.X, (int)_pos.Y]=tiles[i,j];
                    }
                }
            }
        }
        tiles = next_tiles;
    }

    // push tiles
    // TODO : fix this thing
    public void Push(Dir dir){
        var new_pos = new tile.StateChage[4,4]{
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
        };
        for(int i=0;i != 4;i++)
        {
            for(int j=0;j != 4;j++)
                new_pos[i,j] = (tiles[i,j]==null)?null: new tile.StateChage( GridToWin(new Vector2(i,j)));
        }
        int[,] vals = GetValues(); 
        for (int i = 0; i < 4; i++)
        {
            for (int j = 3; j >= 0; j--) 
            {
                // Log.dbg("---------------------------------------");
                int idx1 = get_idx1(dir, i, j);
                int idx2 = get_idx2(dir, i, j);
                int val1 = vals[idx1, idx2];
                //Log.dbg("(pos(r,c), pos(r,c).T, val) : " + new Vector2(i, j).ToString() + " " + new Vector2(idx1, idx2).ToString() + " " + val1.ToString());
                
                if (val1 == 0)
                    continue;

                for (int k = j - 1; k >= 0; k--)
                {
                    int idx3 = get_idx1(dir, i, k);
                    int idx4 = get_idx2(dir, i, k);
                    int val2 = vals[idx3, idx4];
                    if(val2 != 0)
                        break;
                    if(k == 0)
                    {
                        Log.dbg("push (border):");
                        Log.dbg("\tdir : " + dir.ToString());
                        Log.dbg("\t(val1, val2) : " + new Vector2(val1, val2).ToString());
                        Log.dbg("\ttile : " + tiles[idx1, idx2].ToString());
                        Log.dbg("\tpos : " + new Vector2(idx3, idx4));
                        vals[idx1, idx2] = 0;
                        vals[idx3, idx4] = val1;

                        new_pos[idx1, idx2].new_pos = GridToWin(new Vector2(idx3, idx4));
                        continue;
                    }


                    int idx5 = get_idx1(dir, i, k-1);
                    int idx6 = get_idx2(dir, i, k-1);
                    int val3 = vals[idx5, idx6];
                                        
                    if (val3 != 0)
                    {
                        Log.dbg("push :");
                        Log.dbg("\tdir : " + dir.ToString());
                        Log.dbg("\t(val1, val2, val3) : " + new Vector3(val1, val2, val3).ToString());
                        Log.dbg("\ttile : " + tiles[idx1, idx2].ToString());
                        Log.dbg("\tpos : " + new Vector2(idx3, idx4));
                        vals[idx1, idx2] = 0; 
                        vals[idx3, idx4] = val1;

                        new_pos[idx1, idx2].new_pos = GridToWin(new Vector2(idx3, idx4));
                    }
                }
            }
        }
        tile[,] next_tiles = new tile[4,4]{
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
            {null,null,null,null},
        };

        for(int i = 0;i != 4;i++)
        {
            for(int j = 0;j != 4;j++)
            {   
                tiles[i,j]?.Update(new_pos[i,j]);
                if(tiles[i,j] != null)
                {                            
                    if(tiles[i,j].state_target?.stateCode != tile.StateChageCode.Delete)
                    {
                        Vector2 _pos = WinToGrid(new_pos[i,j].new_pos);
                        next_tiles[(int)_pos.X, (int)_pos.Y]=tiles[i,j];
                    }
                }
            }
        }
        tiles = next_tiles;

    }

    // do the same as the merge function but doesn't update tile and check for game over
    // TODO : stop repetition and fix this
    public bool DryMerge(Dir dir)
    {
        int[,] vals = GetValues(); 
        for (int i = 0; i < 4; i++)
        {
            for (int j = 2; j >= 0; j--)
            {
                // Log.dbg("---------------------------------------");
                int idx1 = get_idx1(dir, i, j);
                int idx2 = get_idx2(dir, i, j);
                int val1 = vals[idx1, idx2];
                //Log.dbg("(pos(r,c), pos(r,c).T, val) : " + new Vector2(i, j).ToString() + " " + new Vector2(idx1, idx2).ToString() + " " + val1.ToString());
                
                if (val1 == 0)
                    continue;

                for (int k = j + 1; k < 4; k++) 
                {
                    int idx3 = get_idx1(dir, i, k);
                    int idx4 = get_idx2(dir, i, k);
                    int val2 = vals[idx3, idx4];

                    if (val2 == 0)
                        continue;
                    else if (val1 == val2)
                    {
                        /*
                        Log.dbg("dry merge :");
                        Log.dbg("\tdir : " + dir.ToString());
                        Log.dbg("\t(val1, val2) : " + new Vector2(val1, val2).ToString());
                        Log.dbg("\ttile 1 : " + tiles[idx1, idx2].ToString());
                        Log.dbg("\ttile 2 : " + tiles[idx3, idx4].ToString());
                        */
                        vals[idx1, idx2] = val1 + val2;
                        vals[idx3, idx4] = 0;
                        break; 
                    }else
                        break;
                }
            }
        }

        for (int i = 0; i < vals.GetLength(0); i++)
        {
            for (int j = 0; j < vals.GetLength(1); j++)
            {
                if (vals[i,j] == 0)
                {
                    return true;
                }
            }
        }


        return false;        
    }

    public override void _Process(double delta)
    {
        if(!(IsGameOver || GetTree().HasGroup("animating_tiles")))
        {
            
            Dir dir = Dir.None;

            //check for key
            if (Input.IsActionPressed("move_right"))
            {
                dir = Dir.Right;
                Input.ActionRelease( "move_right" );
            }

            if (Input.IsActionPressed("move_left"))
            {
                dir = Dir.Left;
                Input.ActionRelease("move_left");
            }

            if (Input.IsActionPressed("move_down"))
            {
                dir = Dir.Down;
                Input.ActionRelease("move_down");
            }

            if (Input.IsActionPressed("move_up"))
            {
                dir = Dir.Up;
                Input.ActionRelease("move_up");
            }
            
            if(dir != Dir.None && DryMerge(dir))
            {
                // merges
                /*
                for(int i = 0; i != 4;i++)
                {
                    for(int j = 3;j != 0;j--)
                    {
                        GD.Print("---------------------------------------");
                        int idx1 = get_idx1(dir, i);
                        int idx2 = get_idx2(dir, j);
                        int val1 = vals[idx1, idx2];
                        GD.Print( "(pos1, pos2, pos2.T, val) : " + (new Vector4(i, j,idx2,val1)).ToString());
                        if(val1 == 0)
                            continue;
                        for(int k = j - 1;k != -1;k--)
                        {
                            int idx3 = get_idx2(dir, k);
                            int val2 = vals[idx1, idx3];
                            if(val2 == 0)
                                continue;
                            else if(val1 == val2)
                            {
                                GD.Print( "real(y, x1, x2) : " + (new Vector3(i,j,k)).ToString());
                                GD.Print( "dir : " + dir.ToString());
                                GD.Print( "transpose(y, x1, x2) : " + (new Vector3(idx1,idx2,idx3)).ToString()); 
                                GD.Print( "(val1, val2)" + (new Vector2(val1, val2)).ToString());
                                GD.Print( "tile 1 : " + tiles[idx1,idx2].ToString());
                                GD.Print( "tile 2 : " + tiles[idx1,idx3].ToString());
                                vals[idx1, idx2] = val2;
                                vals[idx1, idx3] = 0;
                                new_pos[idx1,idx3].stateCode = tile.StateChageCode.Delete;
                                new_pos[idx1,idx2].stateCode = tile.StateChageCode.Upgrade;
                                new_pos[idx1,idx3].new_pos = new_pos[idx1,idx2].new_pos;
                            }
                        }
                    }
                }*/

                pre_value = GetValues();
                Push(dir);
                Merge(dir);
                Push(dir);
                GenerateTile();
                // checkj for game over
                // TODO : fix this too
                if(!(DryMerge(Dir.Right) || DryMerge(Dir.Left) || DryMerge(Dir.Up)|| DryMerge(Dir.Down)))
                {
                    IsGameOver = false;
                    EmitSignal(SignalName.GameOver);
                }
            }
        }
    }
}
