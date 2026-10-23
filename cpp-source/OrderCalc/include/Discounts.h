#pragma once

#include "Enums.h"

namespace ordercalc {

// Quantity break discount applied to a single order line, based on
// how many units of that line's product are being purchased.
//   qty >= 50 -> 10% off
//   qty >= 10 -> 5% off
//   otherwise -> 0%
double QuantityDiscountRate(int quantity);

// Order-level discount applied to the whole order once line-level
// discounts have already been subtracted, based on order subtotal.
//   subtotal >= 1000 -> 3% off
//   subtotal >= 500  -> 1.5% off
//   otherwise        -> 0%
double OrderDiscountRate(double subtotal);

// Loyalty discount based on customer membership tier. Stacks
// multiplicatively with the order discount (not additively).
//   Gold     -> 7% off
//   Premium  -> 4% off
//   Standard -> 0%
double MembershipDiscountRate(MembershipTier tier);

} // namespace ordercalc
