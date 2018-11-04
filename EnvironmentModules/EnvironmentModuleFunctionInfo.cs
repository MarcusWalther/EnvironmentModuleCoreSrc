namespace EnvironmentModules
{
    using System.Management.Automation;

    public class EnvironmentModuleFunctionInfo
    {
        public string Name { get; protected set; }

        public string ModuleFullName { get; protected set; }

        public ScriptBlock Definition { get; protected set; }

        public EnvironmentModuleFunctionInfo(string name, string moduleFullName, ScriptBlock definition)
        {
            Name = name;
            ModuleFullName = moduleFullName;
            Definition = definition;
        }
    }
}
