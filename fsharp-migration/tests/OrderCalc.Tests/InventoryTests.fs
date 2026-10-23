module OrderCalc.Tests.InventoryTests

open Xunit
open OrderCalc.Domain
open OrderCalc.Core.Inventory

let private widget =
    { Id = "P1"
      Name = "Widget"
      UnitPrice = 20.00
      TaxCategory = Standard
      AverageDailyUsage = 12.5
      LeadTimeDays = 7
      SafetyStockUnits = 30
      AnnualDemand = 4500.0
      OrderCost = 50.0
      HoldingCostPerUnit = 2.0 }

let private gadget =
    { Id = "P2"
      Name = "Gadget"
      UnitPrice = 5.00
      TaxCategory = Reduced
      AverageDailyUsage = 4.0
      LeadTimeDays = 5
      SafetyStockUnits = 10
      AnnualDemand = 1200.0
      OrderCost = 25.0
      HoldingCostPerUnit = 1.0 }

[<Fact>]
let ``calculateReorderPoint sums lead-time demand and safety stock`` () =
    Assert.Equal(117.5, calculateReorderPoint widget, 6)
    Assert.Equal(30.0, calculateReorderPoint gadget, 6)

[<Fact>]
let ``calculateEconomicOrderQuantity matches the EOQ formula`` () =
    let expectedWidgetEoq = sqrt ((2.0 * 4500.0 * 50.0) / 2.0)
    let expectedGadgetEoq = sqrt ((2.0 * 1200.0 * 25.0) / 1.0)
    Assert.Equal(expectedWidgetEoq, calculateEconomicOrderQuantity widget, 6)
    Assert.Equal(expectedGadgetEoq, calculateEconomicOrderQuantity gadget, 6)

[<Fact>]
let ``calculateInventoryStatus flags reorder need based on current stock`` () =
    let widgetStatus = calculateInventoryStatus widget 100.0
    Assert.True(widgetStatus.NeedsReorder)

    let gadgetStatus = calculateInventoryStatus gadget 50.0
    Assert.False(gadgetStatus.NeedsReorder)
