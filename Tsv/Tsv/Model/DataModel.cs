using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tsv.Attributes;
using Tsv.Service;

namespace Tsv.Model
{
    public abstract class DataModel { }

    public class TsvData : DataModel
    {
        public List<TsvData> Children { get; set; }
        public bool HasChildren => this.Children.Count > 0;

        public TsvData Parent { get; set; }
        public bool HasParent => this.Parent != null;

        public TsvData()
        {
            this.Children = new List<TsvData>();
            this.Id = TsvReader.Id++;
        }

        public int? Id { get; set; }

        [TsvColumnIndex(0)]
        public int? Level { get; set; }

        [TsvColumnIndex(1)]
        public string Item { get; set; }

        [TsvColumnIndex(2)]
        public string Description { get; set; }

        [TsvColumnIndex(3)]
        public int? Revision { get; set; }

        [TsvColumnIndex(4)]
        public string Type { get; set; }

        [TsvColumnIndex(5)]
        public string Status { get; set; }

        [TsvColumnIndex(6)]
        public string EnginerringItem { get; set; }

        [TsvColumnIndex(7)]
        public int? ItemSeq { get; set; }

        [TsvColumnIndex(8)]
        public int? OpSeq { get; set; }

        [TsvColumnIndex(9)]
        public string Alternate { get; set; }

        [TsvColumnIndex(10)]
        public string EngineeringBill { get; set; }

        [TsvColumnIndex(11)]
        public string Comments { get; set; }

        [TsvColumnIndex(12)]
        public string Uom { get; set; }

        [TsvColumnIndex(13)]
        public string Basis { get; set; }

        [TsvColumnIndex(14)]
        public decimal? Quantity { get; set; }

        [TsvColumnIndex(15)]
        public decimal? Planning { get; set; }

        [TsvColumnIndex(16)]
        public decimal? Yield { get; set; }

        [TsvColumnIndex(17)]
        public decimal? ExtendedQuantity1 { get; set; }

        [TsvColumnIndex(18)]
        public string EffectivityControl { get; set; }

        [TsvColumnIndex(19)]
        public string From { get; set; }

        [TsvColumnIndex(20)]
        public string To { get; set; }

        [TsvColumnIndex(21)]
        public string FromDate { get; set; }

        [TsvColumnIndex(22)]
        public string ToDate { get; set; }

        [TsvColumnIndex(23)]
        public string Implemented { get; set; }

        [TsvColumnIndex(24)]
        public string Eco { get; set; }

        [TsvColumnIndex(25)]
        public string SupplyType { get; set; }

        [TsvColumnIndex(26)]
        public string Subinventory { get; set; }

        [TsvColumnIndex(27)]
        public string Locator { get; set; }

        [TsvColumnIndex(28)]
        public string Costed { get; set; }

        [TsvColumnIndex(29)]
        public decimal? UnitCost { get; set; }

        [TsvColumnIndex(30)]
        public decimal? ExtendedQuantity { get; set; }

        [TsvColumnIndex(31)]
        public decimal? ExtendedCost { get; set; }

        [TsvColumnIndex(32)]
        public int? OperationSeq { get; set; }

        [TsvColumnIndex(33)]
        public int? Manufacturing { get; set; }

        [TsvColumnIndex(34)]
        public string Offset { get; set; }

        [TsvColumnIndex(35)]
        public int? CumulativeManufacuturing { get; set; }

        [TsvColumnIndex(36)]
        public int? CumulativeTotal { get; set; }

        [TsvColumnIndex(37)]
        public string Optional { get; set; }

        [TsvColumnIndex(38)]
        public string MutuallyExclusive { get; set; }

        [TsvColumnIndex(39)]
        public string ATP { get; set; }

        [TsvColumnIndex(40)]
        public decimal? MinQty { get; set; }

        [TsvColumnIndex(41)]
        public decimal? MaxQty { get; set; }

        [TsvColumnIndex(42)]
        public string SalesOrderBasis { get; set; }

        [TsvColumnIndex(43)]
        public string Shippable { get; set; }

        [TsvColumnIndex(44)]
        public string IncludeOnShipDocs { get; set; }

        [TsvColumnIndex(45)]
        public string RequiredToShip { get; set; }

        [TsvColumnIndex(46)]
        public string RequiredForRevenue { get; set; }
        public string Line { get; set; }


        private static void BuildContent(StringBuilder sb, TsvData data)
        {
            sb.AppendLine(data.Line);
            if (data.HasChildren)
            {
                foreach (var item in data.Children)
                {
                    BuildContent(sb, item);
                }
            }
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            BuildContent(sb, this);
            return sb.ToString();
        }

    }
}
