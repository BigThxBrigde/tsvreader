using GalaSoft.MvvmLight;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Tsv.Model;
using Tsv.Service;

namespace Tsv.ViewModel
{
    /// <summary>
    /// ViewModel
    /// </summary>
    public abstract class ViewModel : ViewModelBase
    {
        private Logger log;
        public Logger Logger => log ?? (log = Log.GetLogger(() => this.GetType()));

        public virtual void Load() { }
    }

    /// <summary>
    /// Abstract view model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ViewModel<T> : ViewModel where T : DataModel, new()
    {
        private T data;
        public virtual T Data => data;

        public virtual void Load(T data) => this.data = data;
    }
}
