#pragma once

#include <string>

#include "Enums.h"

namespace ordercalc {

// A sellable item plus the inventory-planning parameters used to
// decide when and how much of it to reorder.
struct Product {
    std::string id;
    std::string name;
    double unitPrice;
    TaxCategory taxCategory;

    // Inventory planning parameters.
    double averageDailyUsage;   // units/day
    int leadTimeDays;           // days between placing and receiving an order
    int safetyStockUnits;       // buffer stock kept against demand variability
    double annualDemand;        // units/year, used for Economic Order Quantity
    double orderCost;           // fixed cost ($) of placing one purchase order
    double holdingCostPerUnit;  // annual holding cost ($) per unit in stock
};

} // namespace ordercalc
