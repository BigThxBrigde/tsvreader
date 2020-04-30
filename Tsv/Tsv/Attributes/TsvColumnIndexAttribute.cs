using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsv.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TsvColumnIndexAttribute
        : Attribute
    {
        public TsvColumnIndexAttribute(int index)
        {
            this.Index = index;
        }

        public int Index { get; private set; }
    }
}
