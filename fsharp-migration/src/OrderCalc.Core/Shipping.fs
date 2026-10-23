module OrderCalc.Core.Shipping

/// Orders are shipped free once the post-discount subtotal reaches
/// this amount; otherwise a flat shipping fee is charged.
[<Literal>]
let FreeShippingThreshold = 200.0

[<Literal>]
let FlatShippingRate = 15.0

/// Shipping cost for an order given its subtotal after all discounts
/// (but before tax).
let calculateShippingCost (discountedSubtotal: float) : float =
    if discountedSubtotal >= FreeShippingThreshold then 0.0
    else FlatShippingRate
