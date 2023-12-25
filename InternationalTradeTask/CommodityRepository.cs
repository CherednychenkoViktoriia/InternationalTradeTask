using System;
using TestTasks.InternationalTradeTask.Models;

namespace TestTasks.InternationalTradeTask
{
    internal class CommodityRepository
    {
        public double GetExportTarif(string commodityName)
        {
            bool isExportTarif = true;
            return GetTarif(commodityName, isExportTarif);
        }

        public double GetImportTarif(string commodityName)
        {
            bool isExportTarif = false;
            return GetTarif(commodityName, isExportTarif);
        }

        private double GetTarif(string commodityName, bool isExportTarif)
        {
            foreach (var rootGroup in _allCommodityGroups)
            {
                var (group, tarif) = GetTarifFromSubGroups(rootGroup, commodityName, isExportTarif, null);
                if (group is not null)
                {
                    if (tarif is not null)
                    {
                        return tarif.Value;
                    }
                    else
                    {
                        return GetTarifFromRootGroup(isExportTarif, rootGroup);
                    }
                }
            }

            throw new ArgumentException("Commodity not found in the repository.");
        }

        private double GetTarifFromRootGroup(bool isExportTarif, ICommodityGroup rootGroup)
        {
            if (isExportTarif)
            {
                //Not checking for null, because in the FullySpecifiedCommodityGroup constructor, exportTarif cannot be null
                return rootGroup.ExportTarif.Value;
            }
            else
            {
                //Not checking for null, because in the FullySpecifiedCommodityGroup constructor, importTarif cannot be null
                return rootGroup.ImportTarif.Value;
            }
        }

        private (ICommodityGroup?, double? tarif) GetTarifFromSubGroups(ICommodityGroup group, string commodityName,
            bool isExportTarif, double? tarif)
        {
            if (group.Name == commodityName)
            {
                tarif = AssignTarifIfNotNull(group, isExportTarif, tarif);
                return (group, tarif);
            }

            if (group.SubGroups is not null)
            {
                tarif = AssignTarif(group, isExportTarif);

                foreach (var subGroup in group.SubGroups)
                {
                    var (foundGroup, foundTarif) = GetTarifFromSubGroups(subGroup, commodityName, isExportTarif, tarif);
                    if (foundGroup is not null)
                    {
                        return (foundGroup, foundTarif);
                    }
                }
            }

            return (null, null);
        }

        private double? AssignTarifIfNotNull(ICommodityGroup group, bool isExportTarif, double? tarif)
        {
            if (isExportTarif && group.ExportTarif is not null)
            {
                return group.ExportTarif;
            }
            else if (group.ImportTarif is not null)
            {
                return group.ImportTarif;
            }

            return tarif;
        }

        private double? AssignTarif(ICommodityGroup group, bool isExportTarif)
        {
            if (isExportTarif)
            {
                return group.ExportTarif;
            }
            else
            {
                return group.ImportTarif;
            }
        }

        private FullySpecifiedCommodityGroup[] _allCommodityGroups = new FullySpecifiedCommodityGroup[]
        {
            new FullySpecifiedCommodityGroup("06", "Sugar, sugar preparations and honey", 0.05, 0)
            {
                SubGroups = new CommodityGroup[]
                {
                    new CommodityGroup("061", "Sugar and honey")
                    {
                        SubGroups = new CommodityGroup[]
                        {
                            new CommodityGroup("0611", "Raw sugar,beet & cane"),
                            new CommodityGroup("0612", "Refined sugar & other prod.of refining,no syrup"),
                            new CommodityGroup("0615", "Molasses", 0, 0),
                            new CommodityGroup("0616", "Natural honey", 0, 0),
                            new CommodityGroup("0619", "Sugars & syrups nes incl.art.honey & caramel"),
                            new CommodityGroup("0666", "Fake sugar", 0.1, 0.1)
                        }
                    },
                    new CommodityGroup("062", "Sugar confy, sugar preps. Ex chocolate confy", 0, 0)
                }
            },
            new FullySpecifiedCommodityGroup("282", "Iron and steel scrap", 0, 0.1)
            {
                SubGroups = new CommodityGroup[]
                {
                    new CommodityGroup("28201", "Iron/steel scrap not sorted or graded"),
                    new CommodityGroup("28202", "Iron/steel scrap sorted or graded/cast iron"),
                    new CommodityGroup("28203", "Iron/steel scrap sort.or graded/tinned iron"),
                    new CommodityGroup("28204", "Rest of 282.0")
                }
            }
        };
    }
}