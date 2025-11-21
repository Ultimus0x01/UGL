namespace UGL.Core.Modules;

internal class GraphicsModule
{
    internal readonly Action PreUpdate;
    internal readonly Action PostUpdate;
    internal readonly Action Deinitialize;
    
    internal GraphicsModule()
    {
        var mdl = new ModuleLoader("UGL.Rendering");
        PreUpdate = mdl["PreUpdate"];
        PostUpdate = mdl["PostUpdate"];
        Deinitialize = mdl["Deinitialize"];
    }
}