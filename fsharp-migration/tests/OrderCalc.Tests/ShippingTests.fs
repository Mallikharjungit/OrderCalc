module OrderCalc.Tests.ShippingTests

open Xunit
open OrderCalc.Core.Shipping

[<Theory>]
[<InlineData(0.0, 15.0)>]
[<InlineData(199.99, 15.0)>]
[<InlineData(200.0, 0.0)>]
[<InlineData(500.0, 0.0)>]
let ``calculateShippingCost is free above the threshold`` (subtotal: float) (expected: float) =
    Assert.Equal(expected, calculateShippingCost subtotal, 6)
