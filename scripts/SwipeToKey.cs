using Godot;

public partial class SwipeToKey : Node2D
{
    public float SwipeMin = 0.5f;
    public Vector2? SwipeStart = null;

    public override void _UnhandledInput(InputEvent @event){
            if(@event is InputEventMouseButton eventMouseButton){
                if(eventMouseButton.IsActionPressed("click")){
                    SwipeStart = eventMouseButton.Position;
                }else if(eventMouseButton.IsActionReleased("click")){
                    if(SwipeStart == null){
                        return;
                    }
                    Log.dbg($"Swipe detected from : {SwipeStart} to : {eventMouseButton.Position}");
                    Vector2 Swipe = (Vector2)SwipeStart - eventMouseButton.Position;
                    var SwipeMinScaled = GetViewportRect().Size * SwipeMin;
                    if(Swipe.Abs().X >= SwipeMinScaled.X && !(Swipe.Abs().Y >= SwipeMinScaled.Y)){
                        if(Swipe.X >= 0){
                            Input.ActionPress("move_right");
                        }else
                            Input.ActionPress("move_left");
                    }if(Swipe.Abs().Y >= SwipeMinScaled.Y){
                        if(Swipe.Y >= 0){
                            Input.ActionPress("move_down");
                        }else
                            Input.ActionPress("move_up");
                    }
                    SwipeStart = null;
                }
            }
    }
}
