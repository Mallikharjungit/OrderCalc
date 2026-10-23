#include "Tax.h"

namespace ordercalc {

double TaxRate(TaxCategory category) {
    switch (category) {
        case TaxCategory::Standard:
            return 0.08;
        case TaxCategory::Reduced:
            return 0.03;
        case TaxCategory::Exempt:
        default:
            return 0.0;
    }
}

} // namespace ordercalc
