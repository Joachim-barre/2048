using Godot;

public partial class Hud : Control
{
    [Signal]
    public delegate void ResetEventHandler ();

    [Signal]
    public delegate void UndoEventHandler ();

    private uint GameOverOpacity = 0;
    private bool GameOverShown = false;

    public void UpdateScore(int Score, int HighScore)
    {
        GetNode<Label>("TileTop/ScoreLabel").Text = $"score:\n{Score}";
        GetNode<Label>("TileTop/HighscoreLabel").Text = $"highscore:\n{HighScore}";
    }

    public void Update(bool _game_over = false)
    {
        var panel = GetNode<Panel>("TileTop/UndoButton"); 
        var style = new StyleBoxFlat();
        style.BgColor = new Color("#7a6a4e"); 
        style.CornerRadiusBottomLeft = 30;
        style.CornerRadiusBottomRight = 30;
        style.CornerRadiusTopLeft = 30;
        style.CornerRadiusTopRight = 30;
        ((Control)panel).AddThemeStyleboxOverride("panel", style);
        panel = GetNode<Panel>("TileTop/ResetButton");
        if(_game_over)
        {
            style = new StyleBoxFlat();
            style.BgColor = new Color("#e7ce57");
            style.CornerRadiusBottomLeft = 30;
            style.CornerRadiusBottomRight = 30;
            style.CornerRadiusTopLeft = 30;
            style.CornerRadiusTopRight = 30;
            GameOverShown = true;
        }else
            GameOverShown = false;
        ((Control)panel).AddThemeStyleboxOverride("panel", style);
    }

    public override void _Ready()
    {
        Update();
    }

    public void OnResetButtonPressed(){
        Update();
        EmitSignal(SignalName.Reset);
    }

    public void OnUndoButtonPressed(){
        Update();
        EmitSignal(SignalName.Undo);
    }

    public override void _Process(double delta)
    {
        if(GameOverShown)
        {
            GameOverOpacity = GameOverOpacity > 30 ? GameOverOpacity : GameOverOpacity + 1;
        }else
            GameOverOpacity = 0;
        GetNode<Label>("TileTop/TilesBg/GameOverLabel").Modulate = new Color(1,1,1,GameOverOpacity / 30F);
    }

    public Vector2 GetTileMapPos(){
        return GetNode<Control>("TileTop/TilesBg/GridContainer").GlobalPosition;
    }

    public Vector2 GetTileMapSize(){
        return GetNode<Control>("TileTop/TilesBg/GridContainer").Size;
    }

    public Vector2 GetTileSize(){
        return GetNode<Control>("TileTop/TilesBg/GridContainer/TileVoid1").Size;
    }

    public Vector2 GetTileSpacing(){
        return new Vector2(20,20);
    }
}
