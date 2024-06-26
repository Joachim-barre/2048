
using Godot;
using Godot.Collections;

public partial class Tile : Node2D
{
    public class StateChage{
        public Vector2 new_pos = new Vector2(0,0);
        public StateChageCode stateCode = StateChageCode.None;
        public Tile fusedWith = null;
        public StateChage()
        {

        }
        public StateChage(Vector2 _pos, StateChageCode _code = StateChageCode.None)
        {
            new_pos = _pos;
            stateCode = _code;
        }
    }


    public float scale;
    private int value;
    private int display_value;
    public StateChage state_target;
    public Vector2 _pos;

    private static readonly Dictionary<int, Color> colors;

    public enum StateChageCode
    {
        None,
        Delete,
        Upgrade,
    }

    static Tile()
    {
        colors = new Dictionary<int, Color>()
        {
            {2     ,Color.Color8(230, 230, 230)},
            {4     ,Color.Color8(236, 203, 138)},
            {8     ,Color.Color8(255, 166, 71 )},
            {16    ,Color.Color8(255, 101, 0  )},
            {32    ,Color.Color8(255, 68 , 68 )},
            {64    ,Color.Color8(255, 0  , 0  )},
            {128   ,Color.Color8(244, 255, 0 )},
            {256   ,Color.Color8(255, 229, 42 )},
            {512   ,Color.Color8(255, 217, 0 )},
            {1024  ,Color.Color8(192, 255, 0  )},
            {2048  ,Color.Color8(65 , 255, 0  )},
            {4096  ,Color.Color8(0  , 255, 133)},
            {8192  ,Color.Color8(0  , 175, 255)},
            {16384 ,Color.Color8(0  , 14 , 255)},
            {32768 ,Color.Color8(84 , 0  , 153)},
            {65536 ,Color.Color8(156, 0  , 171)},
            {131072,Color.Color8(255, 0  , 236)},
        };
    }

    public override string ToString()
    {
        return "Pos : " + _pos.ToString() + " value : " + value.ToString();
    }

    public void UpdateTexture(){
        var label = GetNode<Label>("Label");
        var style = new StyleBoxFlat();
        style.BgColor = colors[value];
        style.CornerRadiusBottomLeft = 30;
        style.CornerRadiusBottomRight = 30;
        style.CornerRadiusTopLeft = 30;
        style.CornerRadiusTopRight = 30;
        ((Control)label).AddThemeStyleboxOverride("normal", style);
        label.Text = display_value.ToString();
    }

    public override void _Ready(){
        value = 2;
        scale = 0.5F;
        UpdateTexture();
    } 

    public void SetValue(int new_value){
        value = new_value;
        display_value = value;
        UpdateTexture();
    }

    public int GetValue(){
        return value;
    }

    // Update position to match the scale
    public void UpdatePos()
    {
        Position = new Vector2(_pos.X + 40 * (1-scale), _pos.Y + 40 * (1-scale));
    }

    // set the target state of a tile 
    public void Update(StateChage state)
    {
        AddToGroup("animating_tiles");
        if(state.new_pos != _pos || state.stateCode != StateChageCode.None)
        {
            Log.dbg("tile_chance : ");
            Log.dbg("\t" + _pos.ToString());
            Log.dbg("\t" + state.new_pos.ToString());
            Log.dbg("\t" + state.stateCode.ToString());
            Log.dbg("\t" + (state.fusedWith == null ? "null" : state.fusedWith.ToString()));
        }if(state.stateCode == StateChageCode.Upgrade){
            value *= 2;
        }if(state_target != null){
            state_target.new_pos = state.new_pos;
            if(state_target.fusedWith != null && state_target.stateCode == StateChageCode.Upgrade)
            {
                state_target.fusedWith.state_target.new_pos = state.new_pos;
            }
            if(state_target.fusedWith == null){
                state_target.fusedWith = state.fusedWith;
            }
            switch(state.stateCode)
            {
                case StateChageCode.Delete:
                    state_target.stateCode = StateChageCode.Delete;
                    break;
                case StateChageCode.Upgrade:
                    if(state_target.stateCode == StateChageCode.Delete)
                        break;
                    state_target.stateCode = state.stateCode;
                    break;
                default:
                        break;
            }
        }else
            state_target = state;
    }

    public override void _Process(double delta)
    {
        Scale = new Vector2(scale, scale); 
        if(scale < 1)
        {
            scale += 2F * (float)delta;
            if(scale >= 1){
                scale = 1.1F;
            }
        }if(scale > 1){
            scale -= 1F * (float)delta;
            scale = scale <= 1 ? 1 : scale;
        }
        if(state_target == null)
        {
            UpdatePos();
            return;
        }if(_pos == state_target.new_pos)
        {
            RemoveFromGroup("animating_tiles");
            switch(state_target.stateCode){
                case StateChageCode.Delete:
                    state_target.fusedWith.scale = 1.3F;
                    QueueFree();
                    break;
                case StateChageCode.Upgrade:
                    SetValue(value);
                    break;
                default:
                    break;
            }
            state_target = null;
        }else if(_pos.DistanceTo(state_target.new_pos) <= 10)
        {
            _pos = state_target.new_pos;
        }else
        {
            _pos += (state_target.new_pos - _pos) * 7 * (float)delta;
        }
        UpdatePos();
    }
}
