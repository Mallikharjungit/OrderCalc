#include <iomanip>
#include <iostream>

#include "Inventory.h"
#include "Order.h"
#include "OrderCalculator.h"

using namespace ordercalc;

namespace {

void PrintOrderResult(const OrderCalculationResult& result) {
    std::cout << std::fixed << std::setprecision(2);
    for (const auto& line : result.lines) {
        std::cout << "  Line " << line.productId
                  << ": gross=" << line.lineGross
                  << " lineDiscount=" << (line.lineDiscountRate * 100) << "%"
                  << " netBeforeOrderDiscount=" << line.lineNetBeforeOrderDiscount
                  << " final=" << line.lineFinal
                  << " tax=" << line.lineTax << "\n";
    }
    std::cout << "  Subtotal (pre order/membership discount): " << result.subtotal << "\n";
    std::cout << "  Order discount: " << (result.orderDiscountRate * 100) << "%\n";
    std::cout << "  Membership discount: " << (result.membershipDiscountRate * 100) << "%\n";
    std::cout << "  Discounted subtotal: " << result.discountedSubtotal << "\n";
    std::cout << "  Total tax: " << result.totalTax << "\n";
    std::cout << "  Shipping: " << result.shippingCost << "\n";
    std::cout << "  Grand total: " << result.grandTotal << "\n";
}

void PrintInventoryStatus(const std::string& productId, const InventoryStatus& status) {
    std::cout << std::fixed << std::setprecision(4);
    std::cout << "  " << productId
              << ": reorderPoint=" << status.reorderPoint
              << " eoq=" << status.economicOrderQuantity
              << " needsReorder=" << (status.needsReorder ? "true" : "false") << "\n";
}

} // namespace

int main() {
    Product widget{
        /*id=*/"P1", /*name=*/"Widget", /*unitPrice=*/20.00, /*taxCategory=*/TaxCategory::Standard,
        /*averageDailyUsage=*/12.5, /*leadTimeDays=*/7, /*safetyStockUnits=*/30,
        /*annualDemand=*/4500.0, /*orderCost=*/50.0, /*holdingCostPerUnit=*/2.0};

    Product gadget{
        /*id=*/"P2", /*name=*/"Gadget", /*unitPrice=*/5.00, /*taxCategory=*/TaxCategory::Reduced,
        /*averageDailyUsage=*/4.0, /*leadTimeDays=*/5, /*safetyStockUnits=*/10,
        /*annualDemand=*/1200.0, /*orderCost=*/25.0, /*holdingCostPerUnit=*/1.0};

    Customer goldCustomer{/*id=*/"C1", /*name=*/"Acme Corp", /*tier=*/MembershipTier::Gold};

    Order order{goldCustomer, {OrderLine{widget, 60}, OrderLine{gadget, 8}}};

    std::cout << "=== Order calculation ===\n";
    const OrderCalculationResult result = CalculateOrder(order);
    PrintOrderResult(result);

    std::cout << "\n=== Inventory status ===\n";
    PrintInventoryStatus(widget.id, CalculateInventoryStatus(widget, /*currentStock=*/100.0));
    PrintInventoryStatus(gadget.id, CalculateInventoryStatus(gadget, /*currentStock=*/50.0));

    return 0;
}
