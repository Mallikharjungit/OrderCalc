#pragma once

#include <vector>

#include "Customer.h"
#include "OrderLine.h"

namespace ordercalc {

struct Order {
    Customer customer;
    std::vector<OrderLine> lines;
};

} // namespace ordercalc
