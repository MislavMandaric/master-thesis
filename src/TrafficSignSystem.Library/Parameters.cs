using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    /// <summary>
    /// Parameters class. It holds all the needed parameters as Key - Value pairs.
    /// </summary>
    public class Parameters : Dictionary<object, object>
    {
        /// <summary>
        /// Method for retriving staticly typed parameter by name.
        /// </summary>
        /// <typeparam name="T">Type of the parameter</typeparam>
        /// <param name="key">Parameter key</param>
        /// <param name="value">Out: Parameter value</param>
        /// <returns>True if key exists, False otherwise</returns>
        public bool TryGetValueByType<T>(ParametersEnum key, out T value)
        {
            object obj;
            if (this.TryGetValue(key, out obj))
            {
                try
                {
                    value = (T)obj;
                    return true;
                }
                catch(Exception)
                {
                    value = default(T);
                    return false;
                }
            }
            else
            {
                value = default(T);
                return false;
            }
        }
    }
}
