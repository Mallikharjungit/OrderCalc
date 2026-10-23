#pragma once

#include "Enums.h"

namespace ordercalc {

// Sales tax rate for a given tax category.
//   Standard -> 8%
//   Reduced  -> 3%
//   Exempt   -> 0%
double TaxRate(TaxCategory category);

} // namespace ordercalc
