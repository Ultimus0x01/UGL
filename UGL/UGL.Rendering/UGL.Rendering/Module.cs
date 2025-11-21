using UGL.Core.Data.Modules;

namespace UGL.Rendering;

[Module]
internal sealed class Module
{
    private Module()
    {
        Console.WriteLine("Rendering constructor");
    }

    [ModuleMethod]
    private void PreUpdate()
    {
        Console.WriteLine("Rendering pre-update");
    }
    
    [ModuleMethod]
    private void PostUpdate()
    {
        Console.WriteLine("Rendering post-update");
    }

    [ModuleMethod]
    private void Deinitialize()
    {
        Console.WriteLine("Rendering deinitialize");
    }
}