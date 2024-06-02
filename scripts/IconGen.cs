using Godot;

public partial class IconGen : Node
{
    public override void _Ready(){
        tile t = GetNode<tile>("MainIcon/Tile");
        t.SetValue(2048);
        t.SetProcess(false);
        t.Scale = new Vector2(2.4f,2.4f);
        RenderingServer.FramePostDraw += SaveViewports;
    }

    public void SaveViewports(){
        Log.info($"saving main icon to : {ProjectSettings.GlobalizePath("user://main_icon.png")}");
        GetNode<SubViewport>("MainIcon").GetTexture().GetImage().SavePng("user://main_icon.png");
        RenderingServer.FramePostDraw -= SaveViewports;
        Log.info("Image saved quitting");
        GetTree().Quit();
    }
}
