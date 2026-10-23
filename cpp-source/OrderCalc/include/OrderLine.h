#pragma once

#include "Product.h"

namespace ordercalc {

struct OrderLine {
    Product product;
    int quantity;
};

} // namespace ordercalc
