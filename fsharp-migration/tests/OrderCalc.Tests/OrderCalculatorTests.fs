module OrderCalc.Tests.OrderCalculatorTests

open Xunit
open OrderCalc.Domain
open OrderCalc.Core.OrderCalculator

let private widget =
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

let private gadget =
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

let private goldCustomer = { Id = "C1"; Name = "Acme Corp"; Tier = Gold }

/// Scenario: a Gold-tier customer buying 60 widgets (triggers the 10% line
/// discount and, with an order subtotal over 1000, the 3% order discount
/// plus the 7% Gold membership discount) and 8 gadgets (no line discount).
/// Expected values are hand-derived from the business rules:
///   Line1: gross=1200.00, netBeforeOrderDiscount=1080.00
///   Line2: gross=40.00,   netBeforeOrderDiscount=40.00
///   Subtotal=1120.00, orderDiscount=3%, membershipDiscount=7%
///   combinedFactor=0.97*0.93=0.9021
///   Line1Final=974.268, Line1Tax=77.94144
///   Line2Final=36.084,  Line2Tax=1.08252
///   DiscountedSubtotal=1010.352, TotalTax=79.02396, Shipping=0
///   GrandTotal=1089.37596
[<Fact>]
let ``calculateOrder applies line, order and membership discounts then tax and shipping`` () =
    let order =
        { Customer = goldCustomer
          Lines = [ { Product = widget; Quantity = 60 }; { Product = gadget; Quantity = 8 } ] }

    let result = calculateOrder order

    Assert.Equal(2, result.Lines.Length)

    let line1 = result.Lines.[0]
    Assert.Equal(1200.00, line1.LineGross, 6)
    Assert.Equal(0.10, line1.LineDiscountRate, 6)
    Assert.Equal(1080.00, line1.LineNetBeforeOrderDiscount, 6)
    Assert.Equal(974.268, line1.LineFinal, 6)
    Assert.Equal(77.94144, line1.LineTax, 6)

    let line2 = result.Lines.[1]
    Assert.Equal(40.00, line2.LineGross, 6)
    Assert.Equal(0.0, line2.LineDiscountRate, 6)
    Assert.Equal(40.00, line2.LineNetBeforeOrderDiscount, 6)
    Assert.Equal(36.084, line2.LineFinal, 6)
    Assert.Equal(1.08252, line2.LineTax, 6)

    Assert.Equal(1120.00, result.Subtotal, 6)
    Assert.Equal(0.03, result.OrderDiscountRate, 6)
    Assert.Equal(0.07, result.MembershipDiscountRate, 6)
    Assert.Equal(1010.352, result.DiscountedSubtotal, 6)
    Assert.Equal(79.02396, result.TotalTax, 6)
    Assert.Equal(0.0, result.ShippingCost, 6)
    Assert.Equal(1089.37596, result.GrandTotal, 6)

[<Fact>]
let ``calculateOrder charges flat shipping when discounted subtotal is below the free threshold`` () =
    let standardCustomer = { Id = "C2"; Name = "Small Buyer"; Tier = StandardTier }
    let order = { Customer = standardCustomer; Lines = [ { Product = gadget; Quantity = 2 } ] }

    let result = calculateOrder order

    // gross=10.00, no discounts apply, subtotal=10.00 < 200 -> flat shipping
    Assert.Equal(10.00, result.DiscountedSubtotal, 6)
    Assert.Equal(15.0, result.ShippingCost, 6)

[<Fact>]
let ``calculateOrder on an empty order charges flat shipping and nothing else`` () =
    let standardCustomer = { Id = "C3"; Name = "No Items"; Tier = StandardTier }
    let order = { Customer = standardCustomer; Lines = [] }

    let result = calculateOrder order

    Assert.Empty(result.Lines)
    Assert.Equal(0.0, result.Subtotal, 6)
    Assert.Equal(0.0, result.DiscountedSubtotal, 6)
    Assert.Equal(0.0, result.TotalTax, 6)
    Assert.Equal(15.0, result.ShippingCost, 6)
    Assert.Equal(15.0, result.GrandTotal, 6)

[<Fact>]
let ``calculateOrder treats a zero-quantity line as contributing nothing`` () =
    let standardCustomer = { Id = "C4"; Name = "Mixed Order"; Tier = StandardTier }

    let order =
        { Customer = standardCustomer
          Lines = [ { Product = gadget; Quantity = 0 }; { Product = gadget; Quantity = 5 } ] }

    let result = calculateOrder order

    let zeroQtyLine = result.Lines.[0]
    Assert.Equal(0.0, zeroQtyLine.LineGross, 6)
    Assert.Equal(0.0, zeroQtyLine.LineFinal, 6)
    Assert.Equal(0.0, zeroQtyLine.LineTax, 6)

    Assert.Equal(25.00, result.DiscountedSubtotal, 6)
    Assert.Equal(0.75, result.TotalTax, 6)
    Assert.Equal(15.0, result.ShippingCost, 6)
    Assert.Equal(40.75, result.GrandTotal, 6)

[<Fact>]
let ``calculateOrder applies zero tax for an Exempt-category product`` () =
    let exemptProduct = { gadget with Id = "P3"; TaxCategory = Exempt; UnitPrice = 100.00 }
    let standardCustomer = { Id = "C5"; Name = "Exempt Buyer"; Tier = StandardTier }
    let order = { Customer = standardCustomer; Lines = [ { Product = exemptProduct; Quantity = 3 } ] }

    let result = calculateOrder order

    Assert.Equal(0.0, result.Lines.[0].LineTax, 6)
    Assert.Equal(300.00, result.DiscountedSubtotal, 6)
    Assert.Equal(0.0, result.TotalTax, 6)
    Assert.Equal(0.0, result.ShippingCost, 6) // discounted subtotal >= 200
    Assert.Equal(300.00, result.GrandTotal, 6)

[<Fact>]
let ``calculateOrder applies the quantity discount independently per line`` () =
    let premiumCustomer = { Id = "C6"; Name = "Repeat Buyer"; Tier = Premium }

    let order =
        { Customer = premiumCustomer
          Lines = [ { Product = gadget; Quantity = 10 }; { Product = gadget; Quantity = 10 } ] }

    let result = calculateOrder order

    // Each line independently crosses the 10-unit / 5% threshold.
    for line in result.Lines do
        Assert.Equal(0.05, line.LineDiscountRate, 6)
        Assert.Equal(45.6, line.LineFinal, 6)

    Assert.Equal(95.00, result.Subtotal, 6)
    Assert.Equal(0.0, result.OrderDiscountRate, 6) // subtotal below 500
    Assert.Equal(0.04, result.MembershipDiscountRate, 6)
    Assert.Equal(91.2, result.DiscountedSubtotal, 6)
    Assert.Equal(2.736, result.TotalTax, 6)
    Assert.Equal(15.0, result.ShippingCost, 6)
    Assert.Equal(108.936, result.GrandTotal, 6)
