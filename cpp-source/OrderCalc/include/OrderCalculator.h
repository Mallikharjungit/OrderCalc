#pragma once

#include <string>
#include <vector>

#include "Order.h"

namespace ordercalc {

// Per-line detail of how a line's amount was derived.
struct LineCalculationResult {
    std::string productId;
    double lineGross;                   // unitPrice * quantity
    double lineDiscountRate;             // quantity-tier discount
    double lineNetBeforeOrderDiscount;   // lineGross * (1 - lineDiscountRate)
    double lineFinal;                    // after order + membership discount
    double taxRate;
    double lineTax;                      // lineFinal * taxRate
};

// Full breakdown of an order's calculation, preserving every
// intermediate amount used to reach the grand total.
struct OrderCalculationResult {
    std::vector<LineCalculationResult> lines;
    double subtotal;               // sum of lineNetBeforeOrderDiscount
    double orderDiscountRate;
    double membershipDiscountRate;
    double discountedSubtotal;     // sum of lineFinal
    double totalTax;
    double shippingCost;
    double grandTotal;
};

// Calculates the full price breakdown for an order:
//  1. Apply a per-line quantity discount to each line.
//  2. Sum the discounted lines into a subtotal.
//  3. Apply an order-level discount (by subtotal) and a membership
//     discount (by customer tier), stacked multiplicatively.
//  4. Tax each line's final discounted amount at its product's tax rate.
//  5. Add shipping (free above a subtotal threshold, otherwise flat rate).
OrderCalculationResult CalculateOrder(const Order& order);

} // namespace ordercalc
