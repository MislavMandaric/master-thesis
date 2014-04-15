using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public class Parameters : Dictionary<string, object>
    {
        public bool TryGetValueByType<T>(string key, out T value)
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
