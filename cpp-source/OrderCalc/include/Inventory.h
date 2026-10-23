#pragma once

#include "Product.h"

namespace ordercalc {

struct InventoryStatus {
    double reorderPoint;           // units
    double economicOrderQuantity;  // units to order each replenishment cycle
    bool needsReorder;             // true when currentStock <= reorderPoint
};

// Reorder point: the stock level at which a new purchase order should
// be placed, sized to cover expected demand during the lead time plus
// a safety buffer.
//   reorderPoint = averageDailyUsage * leadTimeDays + safetyStockUnits
double CalculateReorderPoint(const Product& product);

// Economic Order Quantity (EOQ): the order size that minimizes the
// combined cost of ordering and holding inventory.
//   eoq = sqrt((2 * annualDemand * orderCost) / holdingCostPerUnit)
double CalculateEconomicOrderQuantity(const Product& product);

// Full inventory status for a product at its current stock level.
InventoryStatus CalculateInventoryStatus(const Product& product, double currentStock);

} // namespace ordercalc
