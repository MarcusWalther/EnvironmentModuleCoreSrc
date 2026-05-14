namespace EnvironmentModuleCore.Test.Dummy
{
    internal class DummyDependencyClass
    {
        public string ModuleFullName { get; set; }

        public bool IsOptional { get; set; }

        public DummyDependencyClass()
        {
            
        }

        public DummyDependencyClass(string moduleFullName, bool isOptional = false)
        {
            ModuleFullName = moduleFullName;
            IsOptional = isOptional;
        }
    }
}
