using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsv.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewAttribute : Attribute
    {
        public ViewAttribute(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; private set; }
    }
}
