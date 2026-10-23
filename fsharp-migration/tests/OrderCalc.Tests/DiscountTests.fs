module OrderCalc.Tests.DiscountTests

open Xunit
open OrderCalc.Domain
open OrderCalc.Core.Discounts

[<Theory>]
[<InlineData(0, 0.0)>]
[<InlineData(5, 0.0)>]
[<InlineData(9, 0.0)>]
[<InlineData(10, 0.05)>]
[<InlineData(49, 0.05)>]
[<InlineData(50, 0.10)>]
[<InlineData(200, 0.10)>]
let ``quantityDiscountRate applies the correct tier`` (quantity: int) (expected: float) =
    Assert.Equal(expected, quantityDiscountRate quantity, 6)

[<Theory>]
[<InlineData(0.0, 0.0)>]
[<InlineData(499.99, 0.0)>]
[<InlineData(500.0, 0.015)>]
[<InlineData(999.99, 0.015)>]
[<InlineData(1000.0, 0.03)>]
[<InlineData(5000.0, 0.03)>]
let ``orderDiscountRate applies the correct tier`` (subtotal: float) (expected: float) =
    Assert.Equal(expected, orderDiscountRate subtotal, 6)

[<Fact>]
let ``membershipDiscountRate maps each tier`` () =
    Assert.Equal(0.0, membershipDiscountRate StandardTier, 6)
    Assert.Equal(0.04, membershipDiscountRate Premium, 6)
    Assert.Equal(0.07, membershipDiscountRate Gold, 6)
