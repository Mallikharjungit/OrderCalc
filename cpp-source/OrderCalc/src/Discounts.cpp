#include "Discounts.h"

namespace ordercalc {

double QuantityDiscountRate(int quantity) {
    if (quantity >= 50) {
        return 0.10;
    }
    if (quantity >= 10) {
        return 0.05;
    }
    return 0.0;
}

double OrderDiscountRate(double subtotal) {
    if (subtotal >= 1000.0) {
        return 0.03;
    }
    if (subtotal >= 500.0) {
        return 0.015;
    }
    return 0.0;
}

double MembershipDiscountRate(MembershipTier tier) {
    switch (tier) {
        case MembershipTier::Gold:
            return 0.07;
        case MembershipTier::Premium:
            return 0.04;
        case MembershipTier::Standard:
        default:
            return 0.0;
    }
}

} // namespace ordercalc
