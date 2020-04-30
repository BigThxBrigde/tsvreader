using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Tsv.Attributes;

namespace Tsv.Service
{
    public class Reflector
    {

        public static readonly Func<PropertyInfo, bool> DefaultFilter = p => p.CustomAttributes.Any(a => a.AttributeType == typeof(AutoSetValueAttribute));

        private static Dictionary<Type, Dictionary<string, PropertyInfo>> PropertyMapper { get; } = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        private static Logger _Log = Log.GetLogger(() => typeof(Reflector));


        public static void Bind<TSource, TDest>(TSource source, TDest dest) where TSource : new() where TDest : new()
        {
            Bind(source, dest, DefaultFilter);
        }

        public static void Bind<TSource, TDest>(TSource source, TDest dest, Func<PropertyInfo, bool> filter) where TSource : new() where TDest : new()
        {
            try
            {
                var src = GetProperties(typeof(TSource));
                var dst = GetProperties(typeof(TDest), filter);

                foreach (var item in dst)
                {
                    if (src.ContainsKey(item.Key))
                    {
                        object v = src[item.Key].GetValue(source, null);
                        if (v != null)
                        {
                            item.Value.SetValue(dest, ChangeType(item.Value.PropertyType, v), null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _Log.Error("Error when set value: {0}", ex.StackTrace);

            }
        }

        public static readonly Func<Type, object, object> ChangeType = (type, value) =>
        {
            try
            {
                if (typeof(string) == type)
                {
                    return value;
                }
                else if (type.FullName.Contains("System.Nullable"))
                {

                    var v = value != null ? value.ToString() : "";
                    return string.IsNullOrEmpty(v) ? null : Convert.ChangeType(v, type.GetGenericArguments()[0]);
                }
                else
                {
                    return Convert.ChangeType(value, type);
                }
            }
            catch (Exception ex)
            {
                _Log.Error("Error when change type: {0}", ex.StackTrace);
                return value;
            }

        };

        public static Dictionary<string, PropertyInfo> GetProperties(Type t, Func<PropertyInfo, bool> filter = null)
        {
            Dictionary<string, PropertyInfo> pi = null;
            if (!PropertyMapper.TryGetValue(t, out pi))
            {
                var query = t.GetProperties().AsEnumerable();
                if (filter != null)
                {
                    query = query.Where(filter);
                }
                pi = query.ToDictionary(k => k.Name, v => v);
                PropertyMapper[t] = pi;
            }
            return pi;
        }
    }
}
