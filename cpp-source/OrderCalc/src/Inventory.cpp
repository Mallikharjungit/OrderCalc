#include "Inventory.h"

#include <cmath>

namespace ordercalc {

double CalculateReorderPoint(const Product& product) {
    return product.averageDailyUsage * product.leadTimeDays + product.safetyStockUnits;
}

double CalculateEconomicOrderQuantity(const Product& product) {
    return std::sqrt((2.0 * product.annualDemand * product.orderCost) / product.holdingCostPerUnit);
}

InventoryStatus CalculateInventoryStatus(const Product& product, double currentStock) {
    const double reorderPoint = CalculateReorderPoint(product);
    InventoryStatus status;
    status.reorderPoint = reorderPoint;
    status.economicOrderQuantity = CalculateEconomicOrderQuantity(product);
    status.needsReorder = currentStock <= reorderPoint;
    return status;
}

} // namespace ordercalc
