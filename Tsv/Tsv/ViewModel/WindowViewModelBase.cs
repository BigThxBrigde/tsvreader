using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tsv.Attributes;

namespace Tsv.ViewModel
{
    public abstract class WindowViewModelBase : ViewModel
    {
        protected object host;

        public bool IsModal { get; set; } = true;

        public WindowViewModelBase()
        {
            var a = this.GetType().GetCustomAttribute<ViewAttribute>();
            if (a != null)
            {
                host = Activator.CreateInstance(a.Type);

                if (host is Window win)
                {
                    win.DataContext = this;
                }
            }
        }

        public virtual void Show()
        {
            if (host is Window win)
            {
                if (IsModal)
                {
                    win.ShowDialog();
                }
                else
                {
                    win.Show();
                }
            }
        }

        public virtual void Close()
        {
            if (host is Window win)
            {
                win.Close();
            }
        }

    }
}
