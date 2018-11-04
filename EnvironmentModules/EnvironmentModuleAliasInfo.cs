namespace EnvironmentModules
{
    public class EnvironmentModuleAliasInfo
    {
        public string Name { get; protected set; }

        public string ModuleFullName { get; protected set; }

        public string Definition { get; protected set; }

        public string Description { get; protected set; }

        public EnvironmentModuleAliasInfo(string name, string moduleFullName, string definition, string description)
        {
            Name = name;
            ModuleFullName = moduleFullName;
            Definition = definition;
            Description = description;
        }
    }
}
