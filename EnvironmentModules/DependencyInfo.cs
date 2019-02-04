namespace EnvironmentModules
{
    public class DependencyInfo
    {
        public string ModuleFullName { get; protected set; }

        public bool IsOptional { get; protected set; }

        public DependencyInfo(string moduleFullName, bool isOptional = false)
        {
            ModuleFullName = moduleFullName;
            IsOptional = isOptional;
        }
    }
}
