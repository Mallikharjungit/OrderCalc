#include "OrderCalculator.h"

#include "Discounts.h"
#include "Shipping.h"
#include "Tax.h"

namespace ordercalc {

OrderCalculationResult CalculateOrder(const Order& order) {
    OrderCalculationResult result;
    result.lines.reserve(order.lines.size());

    // Pass 1: per-line quantity discount, accumulated into the subtotal.
    double subtotal = 0.0;
    for (const auto& line : order.lines) {
        LineCalculationResult lineResult;
        lineResult.productId = line.product.id;
        lineResult.lineGross = line.product.unitPrice * line.quantity;
        lineResult.lineDiscountRate = QuantityDiscountRate(line.quantity);
        lineResult.lineNetBeforeOrderDiscount =
            lineResult.lineGross * (1.0 - lineResult.lineDiscountRate);
        lineResult.taxRate = TaxRate(line.product.taxCategory);

        subtotal += lineResult.lineNetBeforeOrderDiscount;
        result.lines.push_back(lineResult);
    }
    result.subtotal = subtotal;

    // Order-level and membership discounts stack multiplicatively.
    result.orderDiscountRate = OrderDiscountRate(subtotal);
    result.membershipDiscountRate = MembershipDiscountRate(order.customer.tier);
    const double combinedDiscountFactor =
        (1.0 - result.orderDiscountRate) * (1.0 - result.membershipDiscountRate);

    // Pass 2: apply the combined discount per line, then tax the result.
    double discountedSubtotal = 0.0;
    double totalTax = 0.0;
    for (auto& lineResult : result.lines) {
        lineResult.lineFinal = lineResult.lineNetBeforeOrderDiscount * combinedDiscountFactor;
        lineResult.lineTax = lineResult.lineFinal * lineResult.taxRate;

        discountedSubtotal += lineResult.lineFinal;
        totalTax += lineResult.lineTax;
    }
    result.discountedSubtotal = discountedSubtotal;
    result.totalTax = totalTax;

    result.shippingCost = CalculateShippingCost(discountedSubtotal);
    result.grandTotal = discountedSubtotal + totalTax + result.shippingCost;

    return result;
}

} // namespace ordercalc
