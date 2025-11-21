using System.Reflection;
using UGL.Core.Data.Modules;
using UGL.Util.Extensions;

namespace UGL.Core.Modules;

internal sealed class ModuleLoader
{
    private const string MODULE_ERROR_NO_MODULE_DEFINED =
        "Module: {0} at path {1} has no modules defined";

    private const string MODULE_ERROR_MULTIPLE_MODULES_DEFINED =
        "Module: {0} at path {1} defines multiple modules, Only one module allowed";

    private const string MODULE_ERROR_MODULE_IS_PUBLIC =
        "Module: {0} at path {1} cannot be public";

    private const string MODULE_ERROR_MODULE_IS_NOT_SEALED =
        "Module: {0} at path {1} must be sealed";

    private const string MODULE_ERROR_MODULE_CAN_ONLY_HAVE_ONE_CTOR =
        "Module: {0} at path {1} has have multiple CTORS, only one CTOR allowed";

    private const string MODULE_ERROR_MODULE_CTOR_INVALID_PARAMETERS =
        "Module: {0} at path {1} has invalid parameters {2}";

    private const string MODULE_ERROR_FAILED_TO_INITIALIZE_MODULE =
        "Module: {0} at path {1} can't be initialized";

    private const string MODULE_ERROR_METHOD_CANT_HAVE_PARAMETERS =
        "Module: {0} at path {1} module method {2} can't have any parameters";

    private const string MODULE_ERROR_METHOD_NOT_FOUND =
        "Module: {0} at path {1} does not contain a method named {2}";

    private const string MODULE_EXT = ".dll";

    private readonly static IReadOnlySet<Type> CTOR_PARAM_TYPES = new HashSet<Type>()
    {
        typeof(string)
    };

    public Action this[string name]
    {
        get
        {
            if (_methods.ContainsKey(name))
                return _methods[name];
            throw GetException<ArgumentException>(
                MODULE_ERROR_METHOD_NOT_FOUND,
                _name,
                _assemblyPath,
                name
            );
        }
    }

    private readonly string _name;
    private readonly string _assemblyPath;
    private readonly Assembly _assembly;
    private readonly Type _type;
    private readonly object _moduleObject;
    private readonly Dictionary<string, Action> _methods;

    /// <summary>
    /// Finds the dll to load by name
    /// </summary>
    /// <param name="name">the module name</param>
    internal ModuleLoader(string name)
    {
        _name = name;
        _assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), _name + MODULE_EXT);
        _assembly = Assembly.LoadFile(_assemblyPath);
        _type = FindModule();
        _moduleObject = CreateModule();
        _methods = _type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .ToDictionary(mi => mi.Name, mi => _moduleObject.CreateAction(mi));
        ;
    }

    private Type FindModule()
    {
        var types = _assembly.GetTypes();
        var modulesWithAttribute = _assembly.GetTypes()
            .Where(t => t.HasAttribute<ModuleAttribute>());
        try
        {
            return modulesWithAttribute.Single();
        }
        catch (Exception ex)
        {
            if (modulesWithAttribute.Any())
                throw GetException<ApplicationException>(
                    MODULE_ERROR_MULTIPLE_MODULES_DEFINED,
                    _name,
                    _assemblyPath);
            else
                throw GetException<ApplicationException>(
                    MODULE_ERROR_NO_MODULE_DEFINED,
                    _name,
                    _assemblyPath
                );
        }
    }

    private object CreateModule()
    {
        var ctors = _type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        if (_type.IsPublic)
            throw GetException<ApplicationException>(
                MODULE_ERROR_MODULE_IS_PUBLIC,
                _name,
                _assemblyPath);
        else if (!_type.IsSealed)
            throw GetException<ApplicationException>(
                MODULE_ERROR_MODULE_IS_NOT_SEALED,
                _name,
                _assemblyPath);
        else if (ctors.Length != 1)
            throw GetException<ApplicationException>(
                MODULE_ERROR_MODULE_CAN_ONLY_HAVE_ONE_CTOR,
                _name,
                _assemblyPath);

        var selectedCtor = ctors.First();
        var ctorParams = selectedCtor.GetParameters();

        if (!ctorParams.All(param => CTOR_PARAM_TYPES.Contains(param.ParameterType)))
        {
            var invalidParametersNames = ctorParams
                .Where(param => !CTOR_PARAM_TYPES.Contains(param.ParameterType))
                .Select(param => param.ParameterType.Name);
            throw GetException<ApplicationException>(
                MODULE_ERROR_MODULE_CTOR_INVALID_PARAMETERS,
                _name,
                _assemblyPath,
                string.Join(", ",
                    invalidParametersNames
                )
            );
        }

        //TODO add params
        var paramObjects = new object[] { };
        object moduleObject;
        if (ctorParams.Any())
            moduleObject = selectedCtor?.Invoke(_type, paramObjects);
        else
            moduleObject = selectedCtor?.Invoke(null);

        if (moduleObject == null)
            throw GetException<ArgumentNullException>(
                MODULE_ERROR_FAILED_TO_INITIALIZE_MODULE,
                _name,
                _assemblyPath
            );

        return moduleObject;
    }

    private Exception GetException<T>(string errorMessage, params string[] fields) where T : Exception
    {
        var message = string.Format(errorMessage, fields);
        return Activator.CreateInstance(typeof(T), message) as T;
    }
}