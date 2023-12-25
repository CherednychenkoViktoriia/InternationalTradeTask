var commodityRepository = new TestTasks.InternationalTradeTask.CommodityRepository();
var commodityName = "Raw sugar,beet & cane";
var importTariff = commodityRepository.GetImportTarif(commodityName);
Console.WriteLine(importTariff);
var exportTariff = commodityRepository.GetExportTarif(commodityName);
Console.WriteLine(exportTariff);
