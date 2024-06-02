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
        try{
            Log.info($"saving main icon to : {ProjectSettings.GlobalizePath("user://main_icon.png")}");
            GetNode<SubViewport>("MainIcon").GetTexture().GetImage().SavePng("user://main_icon.png");
            Log.info($"saving background icon to : {ProjectSettings.GlobalizePath("user://background_icon.png")}");
            GetNode<SubViewport>("Background").GetTexture().GetImage().SavePng("user://background_icon.png");
            Log.info($"saving foreground icon to : {ProjectSettings.GlobalizePath("user://foreground_icon.png")}");
            GetNode<SubViewport>("Foregroud").GetTexture().GetImage().SavePng("user://foreground_icon.png");
            Log.info("Image saved quitting");
        }catch(System.Exception e){
            Log.error($"failed saving icons : {e}");
        }finally{
            RenderingServer.FramePostDraw -= SaveViewports;
            GetTree().Quit();
        }
    }
}
