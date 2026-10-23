module OrderCalc.Core.OrderCalculator

open OrderCalc.Domain

/// Per-line detail of how a line's amount was derived.
type LineCalculationResult =
    { ProductId: string
      LineGross: float                  // unitPrice * quantity
      LineDiscountRate: float           // quantity-tier discount
      LineNetBeforeOrderDiscount: float // lineGross * (1 - lineDiscountRate)
      LineFinal: float                  // after order + membership discount
      TaxRate: float
      LineTax: float }                  // lineFinal * taxRate

/// Full breakdown of an order's calculation, preserving every
/// intermediate amount used to reach the grand total.
type OrderCalculationResult =
    { Lines: LineCalculationResult list
      Subtotal: float                // sum of lineNetBeforeOrderDiscount
      OrderDiscountRate: float
      MembershipDiscountRate: float
      DiscountedSubtotal: float       // sum of lineFinal
      TotalTax: float
      ShippingCost: float
      GrandTotal: float }

/// Calculates the full price breakdown for an order:
///  1. Apply a per-line quantity discount to each line.
///  2. Sum the discounted lines into a subtotal.
///  3. Apply an order-level discount (by subtotal) and a membership
///     discount (by customer tier), stacked multiplicatively.
///  4. Tax each line's final discounted amount at its product's tax rate.
///  5. Add shipping (free above a subtotal threshold, otherwise flat rate).
let calculateOrder (order: Order) : OrderCalculationResult =
    // Pass 1: per-line quantity discount, accumulated into the subtotal.
    let partialLines =
        order.Lines
        |> List.map (fun line ->
            let lineGross = line.Product.UnitPrice * float line.Quantity
            let lineDiscountRate = Discounts.quantityDiscountRate line.Quantity
            {| ProductId = line.Product.Id
               LineGross = lineGross
               LineDiscountRate = lineDiscountRate
               LineNetBeforeOrderDiscount = lineGross * (1.0 - lineDiscountRate)
               TaxRate = Tax.taxRate line.Product.TaxCategory |})

    let subtotal = partialLines |> List.sumBy (fun l -> l.LineNetBeforeOrderDiscount)

    // Order-level and membership discounts stack multiplicatively.
    let orderDiscRate = Discounts.orderDiscountRate subtotal
    let membershipDiscRate = Discounts.membershipDiscountRate order.Customer.Tier
    let combinedDiscountFactor = (1.0 - orderDiscRate) * (1.0 - membershipDiscRate)

    // Pass 2: apply the combined discount per line, then tax the result.
    let lines =
        partialLines
        |> List.map (fun l ->
            let lineFinal = l.LineNetBeforeOrderDiscount * combinedDiscountFactor
            { ProductId = l.ProductId
              LineGross = l.LineGross
              LineDiscountRate = l.LineDiscountRate
              LineNetBeforeOrderDiscount = l.LineNetBeforeOrderDiscount
              LineFinal = lineFinal
              TaxRate = l.TaxRate
              LineTax = lineFinal * l.TaxRate })

    let discountedSubtotal = lines |> List.sumBy (fun l -> l.LineFinal)
    let totalTax = lines |> List.sumBy (fun l -> l.LineTax)
    let shippingCost = Shipping.calculateShippingCost discountedSubtotal

    { Lines = lines
      Subtotal = subtotal
      OrderDiscountRate = orderDiscRate
      MembershipDiscountRate = membershipDiscRate
      DiscountedSubtotal = discountedSubtotal
      TotalTax = totalTax
      ShippingCost = shippingCost
      GrandTotal = discountedSubtotal + totalTax + shippingCost }
