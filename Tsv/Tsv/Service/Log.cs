using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Tsv.ViewModel;

namespace Tsv.Service
{
    public class Log
    {
        private static bool enable;
        private static Logger nullLogger;

        static Log()
        {
            enable = Ioc.Default.GetInstance<Config>().Settings.EnableLog;
        }

        public static Logger GetLogger(Func<string> func) => (!enable ? (nullLogger ?? (nullLogger = LogManager.CreateNullLogger())) : LogManager.GetLogger(func()));

        public static Logger GetLogger(Func<Type> func) => GetLogger(() => func().FullName);
    }
}
