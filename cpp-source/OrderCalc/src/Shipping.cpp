#include "Shipping.h"

namespace ordercalc {

double CalculateShippingCost(double discountedSubtotal) {
    if (discountedSubtotal >= FreeShippingThreshold) {
        return 0.0;
    }
    return FlatShippingRate;
}

} // namespace ordercalc
