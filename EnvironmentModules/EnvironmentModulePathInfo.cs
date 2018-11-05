using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentModules
{
    public enum EnvironmentModulePathType { UNKNOWN, APPEND, PREPEND, SET }

    public class EnvironmentModulePathInfo
    {
        public string ModuleFullName { get; protected set; }

        public EnvironmentModulePathType PathType { get; set; }

        public string Variable { get; set; }

        public List<string> Values { get; set; }

        public EnvironmentModulePathInfo(string moduleFullName, EnvironmentModulePathType pathType, string variable, List<string> values = null)
        {
            ModuleFullName = moduleFullName;
            PathType = pathType;
            Variable = variable;

            if (values == null)
                values = new List<string>();

            Values = values;
        }

        public override bool Equals(object obj)
        {
            EnvironmentModulePathInfo other = obj as EnvironmentModulePathInfo;

            if (other == null)
                return false;

            return other.Variable == Variable && other.PathType == PathType;
        }

        public override int GetHashCode()
        {
            return PathType.GetHashCode() ^ Variable.GetHashCode();
        }
    }
}
