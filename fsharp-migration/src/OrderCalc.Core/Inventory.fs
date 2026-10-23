module OrderCalc.Core.Inventory

open OrderCalc.Domain

type InventoryStatus =
    { ReorderPoint: float             // units
      EconomicOrderQuantity: float    // units to order each replenishment cycle
      NeedsReorder: bool }            // true when currentStock <= ReorderPoint

/// Reorder point: the stock level at which a new purchase order should
/// be placed, sized to cover expected demand during the lead time plus
/// a safety buffer.
///   reorderPoint = averageDailyUsage * leadTimeDays + safetyStockUnits
let calculateReorderPoint (product: Product) : float =
    product.AverageDailyUsage * float product.LeadTimeDays + float product.SafetyStockUnits

/// Economic Order Quantity (EOQ): the order size that minimizes the
/// combined cost of ordering and holding inventory.
///   eoq = sqrt((2 * annualDemand * orderCost) / holdingCostPerUnit)
let calculateEconomicOrderQuantity (product: Product) : float =
    sqrt ((2.0 * product.AnnualDemand * product.OrderCost) / product.HoldingCostPerUnit)

/// Full inventory status for a product at its current stock level.
let calculateInventoryStatus (product: Product) (currentStock: float) : InventoryStatus =
    let reorderPoint = calculateReorderPoint product
    { ReorderPoint = reorderPoint
      EconomicOrderQuantity = calculateEconomicOrderQuantity product
      NeedsReorder = currentStock <= reorderPoint }
