using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace MetaTracInterface.Helpers
{
    static class XmlRpcUtil
    {
        public static T GetValue<T>(this XmlRpcStruct xmlRpcStruct, string key)
        {
            Type type = typeof(T);
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType == null)
            {
                return (T)Convert.ChangeType(xmlRpcStruct[key], typeof(T), CultureInfo.InvariantCulture);
            }
            else
            {
                if (string.IsNullOrWhiteSpace((string)xmlRpcStruct[key]))
                {
                    return default(T);
                }
                else
                {
                    return (T)Convert.ChangeType(xmlRpcStruct[key], underlyingType, CultureInfo.InvariantCulture);
                }
            }
        }

        public static string GetXMLRPCString(this object value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            else
            {
                if (value is IFormattable)
                {
                    return ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture);
                }
                else
                {
                    return value.ToString();
                }

                
            }
        }
    }
}
