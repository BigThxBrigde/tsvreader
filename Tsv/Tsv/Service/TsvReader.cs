using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tsv.Attributes;
using Tsv.Model;

namespace Tsv.Service
{
    public class TsvReader
    {
        public static int Id = 0;

        static Logger _Log = Log.GetLogger(() => typeof(TsvReader));
        private static int _RootLevel = 1;
        private static Dictionary<int, PropertyInfo> _PropertyMapper;
        private static readonly string _Separator = "\t";

        private static Func<string, Dictionary<int, PropertyInfo>, TsvData> GetData = (line, properties) =>
        {
            _Log.Debug("Read line: {0}", line);
            var values = line.Split(new[] { _Separator }, 47, StringSplitOptions.None);
            var data = new TsvData();
            data.Line = line;

            for (int i = 0; i < values.Length; i++)
            {
                var v = values[i];
                var p = properties[i];
                _Log.Debug("Set value [{0}] on property [{1}]", v, p.Name);
                p.SetValue(data, Reflector.ChangeType(p.PropertyType, v), null);
            }

            //foreach (var e in properties)
            //{
            //    var v = values[e.Key];
            //    var p = e.Value;
            //    _Log.Debug("Set value [{0}] on property [{1}]", v, p.Name);
            //    p.SetValue(data, Reflector.ChangeType(p.PropertyType, v), null);
            //}
            return data;
        };

        static TsvData FindParent(int level, List<TsvData> data)
        {
            TsvData parent = data.LastOrDefault(o => o.Level + 1 == level);
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent(level, data.Last().Children);
            }
        }


        static TsvReader()
        {
            _PropertyMapper = typeof(TsvData).GetProperties().Where(p =>
             {
                 return p.CustomAttributes.Any(a => a.AttributeType == typeof(TsvColumnIndexAttribute));
             })
             .ToDictionary(p =>
             {
                 var a = p.GetCustomAttribute<TsvColumnIndexAttribute>();
                 return a.Index;
             }, p => p);
        }


        private string path;

        public TsvReader(string path)
        {
            this.path = path;
        }


        public List<TsvData> Parse(Action<string, string> callback = null)
        {
            if (!File.Exists(path))
            {
                callback?.Invoke("Error", string.Format("{0} not exists", path));
                return null;
            }

            try
            {
                var data = new List<TsvData>();
                var lines = File.ReadAllLines(path);

                for (int i = 1; i < lines.Length; i++)
                {
                    var item = GetData(lines[i], _PropertyMapper);

                    if (item.Level.Value == _RootLevel)
                    {
                        data.Add(item);
                        continue;
                    }
                    else
                    {
                        var parent = FindParent(item.Level.Value, data);
                        item.Parent = parent;
                        parent.Children.Add(item);

                    }

                }
                return data;
            }
            catch (Exception ex)
            {
                _Log.Error("Error reading file {0}", ex.StackTrace);
                callback?.Invoke("Error", "Error parsing tsv file");
            }

            return null;
        }
    }
}
