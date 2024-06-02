using Godot;

public partial class IconGen : Node
{
    public override void _Ready(){
        GetNode<tile>("MainIcon/Tile").SetValue(2048);
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
