#pragma once

namespace ordercalc {

// Tax category applied to a product's sale price.
enum class TaxCategory {
    Standard, // 8%
    Reduced,  // 3%
    Exempt    // 0%
};

// Customer loyalty tier; grants a stacked discount on top of order discounts.
enum class MembershipTier {
    Standard, // 0%
    Premium,  // 4%
    Gold      // 7%
};

} // namespace ordercalc
