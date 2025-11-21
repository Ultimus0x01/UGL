using System.Runtime.InteropServices.Marshalling;

namespace UGL.Core.Data;

public abstract class Application
{
    protected Application()
    {
        
    }

    internal void Initialize()
    {
        OnInitialization();
    }
    
    /// <summary>
    /// client implementation of initialization
    /// </summary>
    protected abstract void OnInitialization();

    internal void Deinitialize()
    {
        OnDeinitialize();
    }
    
    /// <summary>
    /// client implementation of deinitialization
    /// </summary>
    protected abstract void OnDeinitialize();
}