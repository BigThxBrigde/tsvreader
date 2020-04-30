using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Tsv.Attributes;
using Tsv.Model;
using Tsv.Service;

namespace Tsv.ViewModel
{
    public class TsvDataViewModel : ViewModel<TsvData>
    {
        private int level;
        private string item;
        private string description;
        private string uom;
        private decimal? quantity;
        private decimal? extendedQuantity;
        private decimal? yield;
        private decimal? unitCost;
        private decimal? extendedCost;
        private decimal? materialCost;
        private decimal? manualCost;

        private ObservableCollection<TsvDataViewModel> children;
        private TsvDataViewModel parent;

        public ObservableCollection<TsvDataViewModel> Children
        {
            get => children;
            set => base.Set(() => Children, ref children, value);
        }

        public TsvDataViewModel Parent
        {
            get => parent;
            set => base.Set(() => Parent, ref parent, value);

        }

        public bool HasChildren => Children != null && Children.Count > 0;
        public bool HasParent => Parent != null;

        [AutoSetValue]
        public int Level
        {
            get => level;
            set => base.Set(() => Level, ref level, value);
        }

        [AutoSetValue]
        public string Item
        {
            get => item;
            set => base.Set(() => Item, ref item, value);
        }

        [AutoSetValue]
        public string Description
        {
            get => description;
            set => base.Set(() => Description, ref description, value);
        }

        [AutoSetValue]
        public decimal? Quantity
        {
            get => quantity;
            set => base.Set(() => Quantity, ref quantity, value);
        }

        [AutoSetValue]
        public decimal? ExtendedQuantity
        {
            get => extendedQuantity;
            set => base.Set(() => ExtendedQuantity, ref extendedQuantity, value);
        }

        [AutoSetValue]
        public decimal? Yield
        {
            get => yield;
            set => base.Set(() => Yield, ref yield, value);
        }

        [AutoSetValue]
        public string Uom
        {
            get => uom;
            set => base.Set(() => Uom, ref uom, value);
        }

        [AutoSetValue]
        public decimal? UnitCost
        {
            get => unitCost;
            set => base.Set(() => UnitCost, ref unitCost, value);
        }

        [AutoSetValue]
        public decimal? ExtendedCost
        {
            get => extendedCost;
            set => base.Set(() => ExtendedCost, ref extendedCost, value);
        }

        [AutoSetValue]
        public decimal? MaterialCost
        {
            get => materialCost;
            set => base.Set(() => MaterialCost, ref materialCost, value);
        }

        [AutoSetValue]
        public decimal? ManualCost
        {
            get => manualCost;
            set => base.Set(() => manualCost, ref manualCost, value);
        }

        public override void Load(TsvData data)
        {
            base.Load(data);
            this.Children = new ObservableCollection<TsvDataViewModel>();
            Reflector.Bind(data, this);
        }


        public void CalcPrice()
        {
            //if (this.HasChildren)
            //{

            //    var lowerLevelItems = GetLowestLevelItems(this);
            //    decimal sum = 0;
            //    foreach (var item in lowerLevelItems)
            //    {
            //        if (item.ExtendedCost.HasValue)
            //        {
            //            sum += item.ExtendedCost.Value;
            //        }
            //    }


            //    this.ManualCost = this.ExtendedCost - sum;
            //    this.MaterialCost = sum;
            //}
            //else
            //{
            //    this.MaterialCost = this.ExtendedCost;
            //}
        }

        public static IEnumerable<TsvDataViewModel> GetLowestLevelItems(TsvDataViewModel vm)
        {
            if (!vm.HasChildren)
            {
                vm.Logger.Debug($"Found no children data:{Environment.NewLine}{vm}");
                yield return vm;
            }
            else
            {
                foreach (var item in vm.Children)
                {
                    foreach (var o in GetLowestLevelItems(item))
                    {
                        yield return o;
                    }
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("---------------------------------------------------------")
            .AppendLine($"{Level} | {Item} | {Description}")
            .AppendLine($"Extened Quantity -> {ExtendedQuantity} | Extend Cost -> {ExtendedCost}")
            .AppendLine($"Material Cost -> {MaterialCost} | Manual Cost -> {ManualCost}")
            .AppendLine("-------------------------------------------------------------");

            return sb.ToString();
        }

    }
}
