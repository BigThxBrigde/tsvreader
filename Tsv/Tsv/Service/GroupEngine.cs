using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tsv.ViewModel;

namespace Tsv.Service
{
    public class GroupEngine : ViewModel.ViewModel
    {
        private static GroupEngine _instance = null;

        public static GroupEngine Instance => _instance ?? (_instance ?? new GroupEngine());

        Dictionary<string, Func<TsvDataViewModel, bool>> Expressions = new Dictionary<string, Func<TsvDataViewModel, bool>>();
        private GroupEngine()
        {
            // retrieve all
            Evaluator.AddAllReferences();
            Evaluator.UsingAllNamespaces();
        }

        public List<TsvDataViewModel> Parse(List<TsvDataViewModel> sourceData, Action<string, string> callback = null)
        {
            var result = new List<TsvDataViewModel>();

            Logger.Debug("Enter GroupEngine to parse");
            try
            {
                sourceData = sourceData.SelectMany(o => TsvDataViewModel.GetLowestLevelItems(o)).ToList();

                var groups = Ioc.Default.GetInstance<Config>().Settings.Groups;

                foreach (var group in groups)
                {
                    var func = Evaluator.Evaluate<Func<TsvDataViewModel, bool>>(group.Expression);
                    Expressions[group.Name] = func;

                    var list = sourceData.Where(func).ToList();
                    if (list.Count > 0)
                    {
                        var vm = new TsvDataViewModel
                        {
                            Item = group.Name,
                            Level = 0,
                            Description = group.Description
                        };

                        vm.Children = new ObservableCollection<TsvDataViewModel>(list);
                        result.Add(vm);
                    }
                }

                if (result.Count > 0)
                {
                    CalcPrice(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Group engine parse error:{ex.StackTrace}");
                callback?.Invoke("Error", "Group engine parse data failed");
            }
            return result;
        }

        void CalcPrice(List<TsvDataViewModel> list)
        {
            
            foreach (var group in list)
            {
                var faceParents = new List<TsvDataViewModel>();
                var parents = new List<TsvDataViewModel>();
                foreach (var vm in group.Children)
                {
                    var parent = GetParent(1, vm);
                    if (parent != null && !faceParents.Contains(parent))
                    {
                        faceParents.Add(parent);
                        var actualParents = FindActualParents(parent, group.Item);
                        parents.AddRange(actualParents);
                    }
                }
                if (parents.Count > 0)
                {
                    var extendedCost = parents.Sum(p => p.ExtendedCost);
                    var materialCost = group.Children.Sum(p => p.ExtendedCost);
                    group.MaterialCost = materialCost;
                    group.ManualCost = extendedCost - materialCost;
                }

            }
        }

        private List<TsvDataViewModel> FindActualParents(TsvDataViewModel parent, string name)
        {
            var lowestChildren = TsvDataViewModel.GetLowestLevelItems(parent);
            if (lowestChildren.All(Expressions[name]))
            {
                return new List<TsvDataViewModel> { parent };
            }
            else
            {
                var children = lowestChildren.Where(Expressions[name]);
                var result = new List<TsvDataViewModel>();
                foreach (var item in children)
                {
                    var newParent = GetParent(parent.Level + 1, item);
                    if (newParent != null && !result.Contains(newParent))
                    {
                        result.Add(newParent);
                    }
                }
                return result;

            }



        }

        // get specified level parent
        TsvDataViewModel GetParent(int level, TsvDataViewModel vm)
        {
            TsvDataViewModel node = vm;

            if (node.Level == level && !node.HasChildren)
            {
                return node;
            }

            while (node.HasParent && node.Level > level)
            {
                node = node.Parent;
                if (node.Level == level)
                {
                    return node;
                }
            }
            return null;
        }

    }

}
