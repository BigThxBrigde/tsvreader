using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Tsv.Service
{


    public class Evaluator
    {
        public static List<string> Namespaces { get; private set; } = new List<string>();
        public static List<string> Assemblies { get; private set; } = new List<string>();

        public static void AddAllReferences()
        {
            var path = Path.GetDirectoryName(typeof(Evaluator).Assembly.Location);
            foreach (var item in Directory.GetFiles(path))
            {
                var ext = Path.GetExtension(item).ToLower();
                if (ext == ".dll" || ext == ".exe")
                {
                    Assemblies.Add(item);
                }
            }
            Assemblies = Assemblies.Distinct().ToList();
        }

        public static void UsingAllNamespaces()
        {
            Namespaces.AddRange(typeof(Evaluator).Assembly.GetTypes().Select(t => t.Namespace).Where(ns => !string.IsNullOrEmpty(ns)).Distinct());
        }

        public static void UsingNamespaces(params string[] namespaces)
        {
            Evaluator.Namespaces.AddRange(namespaces);
            Evaluator.Namespaces = namespaces.Distinct().ToList();
        }

        public static void AddReferences(params string[] assemblies)
        {
            Evaluator.Assemblies.AddRange(assemblies);
            Evaluator.Assemblies = assemblies.Distinct().ToList();
        }

        public Evaluator(EvaluatorItem[] items)
        {
            ConstructEvaluator(items);
        }

        public Evaluator(Type returnType, string expression, string name)
        {
            EvaluatorItem[] items = { new EvaluatorItem(returnType, expression, name) };
            ConstructEvaluator(items);
        }

        public Evaluator(EvaluatorItem item)
        {
            EvaluatorItem[] items = { item };
            ConstructEvaluator(items);
        }

        private void ConstructEvaluator(EvaluatorItem[] items)
        {
            var comp = new CSharpCodeProvider();

            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("system.core.dll");
            cp.ReferencedAssemblies.Add("system.data.dll");
            cp.ReferencedAssemblies.Add("system.xml.dll");
            cp.ReferencedAssemblies.Add("system.xml.linq.dll");

            foreach (var item in Assemblies)
            {
                cp.ReferencedAssemblies.Add(item);
            }

            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;

            StringBuilder code = new StringBuilder();

            code.AppendLine("using System;");
            code.AppendLine("using System.Data;");
            code.AppendLine("using System.Xml;");
            code.AppendLine("using System.Collections.Generic;");
            code.AppendLine("using System.Linq;");
            code.AppendLine("using System.Text;");
            code.AppendLine("using System.Threading.Tasks;");

            foreach (var item in Namespaces)
            {
                code.AppendLine($"using {item};");
            }

            code.AppendLine("namespace AutoEval {");

            code.AppendLine("\tpublic class _Evaluator {");
            foreach (EvaluatorItem item in items)
            {
                code.AppendLine($"\t\tpublic {item.ReturnType.Name} {item.Name}()");

                code.AppendLine("\t\t{ ");
                code.AppendLine($"\t\t\treturn ({item.Expression});");
                code.AppendLine("\t\t}");
            }
            code.AppendLine("\t}");
            code.AppendLine("}");


            CompilerResults cr = comp.CompileAssemblyFromSource(cp, code.ToString());

            if (cr.Errors.HasErrors)
            {
                StringBuilder error = new StringBuilder();
                error.Append("Compile expreesion error: ");
                foreach (CompilerError err in cr.Errors)
                {
                    error.AppendFormat("{0}\n", err.ErrorText);
                }
                throw new Exception("Compile expression error: " + error.ToString());
            }
            Assembly a = cr.CompiledAssembly;
            _Compiled = a.CreateInstance("AutoEval._Evaluator");
        }




        public int EvaluateInt(string name)
        {
            return (int)Evaluate(name);
        }

        public string EvaluateString(string name)
        {
            return (string)Evaluate(name);
        }

        public bool EvaluateBool(string name)
        {
            return (bool)Evaluate(name);
        }

        public object Evaluate(string name)
        {
            MethodInfo mi = _Compiled.GetType().GetMethod(name);
            return mi.Invoke(_Compiled, null);
        }


        static public int EvaluateToInteger(string code)
        {
            Evaluator eval = new Evaluator(typeof(int), code, staticMethodName);
            return (int)eval.Evaluate(staticMethodName);
        }

        static public string EvaluateToString(string code)
        {
            Evaluator eval = new Evaluator(typeof(string), code, staticMethodName);
            return (string)eval.Evaluate(staticMethodName);
        }

        static public bool EvaluateToBool(string code)
        {
            Evaluator eval = new Evaluator(typeof(bool), code, staticMethodName);
            return (bool)eval.Evaluate(staticMethodName);
        }

        static public object EvaluateToObject(string code)
        {
            Evaluator eval = new Evaluator(typeof(object), code, staticMethodName);
            return eval.Evaluate(staticMethodName);
        }

        static public T Evaluate<T>(string code)
        {
            Evaluator eval = new Evaluator(typeof(object), code, staticMethodName);
            return (T)eval.Evaluate(staticMethodName);
        }

        private const string staticMethodName = "__foo";

        object _Compiled = null;

    }

    public class EvaluatorItem
    {

        public Type ReturnType;

        public string Expression;

        public string Name;

        public EvaluatorItem(Type returnType, string expression, string name)
        {
            ReturnType = returnType;
            Expression = expression;
            Name = name;
        }
    }
}

