#pragma once

namespace ordercalc {

// Orders are shipped free once the post-discount subtotal reaches
// this amount; otherwise a flat shipping fee is charged.
constexpr double FreeShippingThreshold = 200.0;
constexpr double FlatShippingRate = 15.0;

// Shipping cost for an order given its subtotal after all discounts
// (but before tax).
double CalculateShippingCost(double discountedSubtotal);

} // namespace ordercalc
