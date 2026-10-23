#pragma once

#include <string>

#include "Enums.h"

namespace ordercalc {

struct Customer {
    std::string id;
    std::string name;
    MembershipTier tier;
};

} // namespace ordercalc
