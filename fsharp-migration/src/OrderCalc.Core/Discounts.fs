module OrderCalc.Core.Discounts

/// Quantity break discount applied to a single order line, based on
/// how many units of that line's product are being purchased.
///   qty >= 50 -> 10% off
///   qty >= 10 -> 5% off
///   otherwise -> 0%
let quantityDiscountRate (quantity: int) : float =
    if quantity >= 50 then 0.10
    elif quantity >= 10 then 0.05
    else 0.0

/// Order-level discount applied to the whole order once line-level
/// discounts have already been subtracted, based on order subtotal.
///   subtotal >= 1000 -> 3% off
///   subtotal >= 500  -> 1.5% off
///   otherwise        -> 0%
let orderDiscountRate (subtotal: float) : float =
    if subtotal >= 1000.0 then 0.03
    elif subtotal >= 500.0 then 0.015
    else 0.0

/// Loyalty discount based on customer membership tier. Stacks
/// multiplicatively with the order discount (not additively).
///   Gold         -> 7% off
///   Premium      -> 4% off
///   StandardTier -> 0%
let membershipDiscountRate (tier: OrderCalc.Domain.MembershipTier) : float =
    match tier with
    | OrderCalc.Domain.Gold -> 0.07
    | OrderCalc.Domain.Premium -> 0.04
    | OrderCalc.Domain.StandardTier -> 0.0
