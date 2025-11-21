using UGL.Core.Data;
using UGL.Core.Modules;

namespace UGL.Core;

public sealed class Core
{
    private static Core _instance;

    public static void Run(Application application)
    {
        if (_instance == null)
        {
            _instance = new Core(application);
            _instance.Initialize();
            _instance._application?.Initialize();
            _instance.GameLoop();
            _instance._application?.Deinitialize();
            _instance.Deinitialize();
        }
    }
    
    public static void Stop() => _instance?._isRunning = false;
    
    private readonly Application _application;
    private bool _isRunning;

    private GraphicsModule _graphicsModule;
    
    private Core(Application application)
    {
        _application = application;
        _isRunning = true;
    }

    private void Initialize()
    {
        _graphicsModule = new GraphicsModule();
    }

    private void GameLoop()
    {
        int i = 0;
        while (_isRunning)
        {
            _graphicsModule.PreUpdate();
            _graphicsModule.PostUpdate();
            i++;
            if(i == 5)
                Stop();
        }
    }

    private void Deinitialize()
    {
        _graphicsModule.Deinitialize();
    }
}