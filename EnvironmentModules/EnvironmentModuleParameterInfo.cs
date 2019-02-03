namespace EnvironmentModules
{
    public class EnvironmentModuleParameterInfo
    {
        /// <summary>
        /// Get the unique name of the parameter.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Get or set the module name that has defined the value of the parameter. An empty string indicates that the user has changed the parameter.
        /// </summary>
        public string ModuleFullName { get; set; }

        /// <summary>
        /// Get or set the value of the parameter.
        /// </summary>
        public string Value { get; set; }

        public EnvironmentModuleParameterInfo(string name, string moduleFullName, string value)
        {
            Name = name;
            ModuleFullName = moduleFullName;
            Value = value;
        }
    }
}
