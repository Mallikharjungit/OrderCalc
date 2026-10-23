module OrderCalc.Cli.Program

open OrderCalc.Domain
open OrderCalc.Core

let private printOrderResult (result: OrderCalculator.OrderCalculationResult) =
    for line in result.Lines do
        printfn
            "  Line %s: gross=%.2f lineDiscount=%.0f%% netBeforeOrderDiscount=%.2f final=%.2f tax=%.2f"
            line.ProductId
            line.LineGross
            (line.LineDiscountRate * 100.0)
            line.LineNetBeforeOrderDiscount
            line.LineFinal
            line.LineTax
    printfn "  Subtotal (pre order/membership discount): %.2f" result.Subtotal
    printfn "  Order discount: %.0f%%" (result.OrderDiscountRate * 100.0)
    printfn "  Membership discount: %.0f%%" (result.MembershipDiscountRate * 100.0)
    printfn "  Discounted subtotal: %.2f" result.DiscountedSubtotal
    printfn "  Total tax: %.2f" result.TotalTax
    printfn "  Shipping: %.2f" result.ShippingCost
    printfn "  Grand total: %.2f" result.GrandTotal

let private printInventoryStatus (productId: string) (status: Inventory.InventoryStatus) =
    printfn
        "  %s: reorderPoint=%.4f eoq=%.4f needsReorder=%b"
        productId
        status.ReorderPoint
        status.EconomicOrderQuantity
        status.NeedsReorder

[<EntryPoint>]
let main _ =
    let widget =
        { Id = "P1"
          Name = "Widget"
          UnitPrice = 20.00
          TaxCategory = Standard
          AverageDailyUsage = 12.5
          LeadTimeDays = 7
          SafetyStockUnits = 30
          AnnualDemand = 4500.0
          OrderCost = 50.0
          HoldingCostPerUnit = 2.0 }

    let gadget =
        { Id = "P2"
          Name = "Gadget"
          UnitPrice = 5.00
          TaxCategory = Reduced
          AverageDailyUsage = 4.0
          LeadTimeDays = 5
          SafetyStockUnits = 10
          AnnualDemand = 1200.0
          OrderCost = 25.0
          HoldingCostPerUnit = 1.0 }

    let goldCustomer = { Id = "C1"; Name = "Acme Corp"; Tier = Gold }

    let order =
        { Customer = goldCustomer
          Lines = [ { Product = widget; Quantity = 60 }; { Product = gadget; Quantity = 8 } ] }

    printfn "=== Order calculation ==="
    let result = OrderCalculator.calculateOrder order
    printOrderResult result

    printfn ""
    printfn "=== Inventory status ==="
    printInventoryStatus widget.Id (Inventory.calculateInventoryStatus widget 100.0)
    printInventoryStatus gadget.Id (Inventory.calculateInventoryStatus gadget 50.0)

    0
