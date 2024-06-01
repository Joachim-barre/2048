using Godot;

public partial class SwipeToKey : Node2D
{
    public float SwipeMin = 0.3f;
    public Vector2? SwipeStart = null;

    public override void _Process(double delta){
            if(Input.IsActionJustPressed("swipe")){
                SwipeStart = GetViewport().GetMousePosition();
            }else if(Input.IsActionJustReleased("swipe")){
                if(SwipeStart == null){
                    return;
                }
                Log.dbg($"Swipe detected from : {SwipeStart} to : {GetViewport().GetMousePosition()}");
                Vector2 Swipe = (Vector2)SwipeStart - GetViewport().GetMousePosition();
                var SwipeMinScaled = GetViewportRect().Size * SwipeMin;
                if(Swipe.Abs().X >= SwipeMinScaled.X && !(Swipe.Abs().Y >= SwipeMinScaled.Y)){
                    if(Swipe.X <= 0){
                        Input.ActionPress("move_right");
                    }else
                        Input.ActionPress("move_left");
                }if(Swipe.Abs().Y >= SwipeMinScaled.Y){
                    if(Swipe.Y <= 0){
                        Input.ActionPress("move_down");
                    }else
                        Input.ActionPress("move_up");
                }
                SwipeStart = null;
            }
    }
}
