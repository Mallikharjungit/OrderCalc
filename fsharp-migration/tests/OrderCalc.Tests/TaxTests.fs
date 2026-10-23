module OrderCalc.Tests.TaxTests

open Xunit
open OrderCalc.Domain
open OrderCalc.Core.Tax

[<Fact>]
let ``taxRate maps each category`` () =
    Assert.Equal(0.08, taxRate Standard, 6)
    Assert.Equal(0.03, taxRate Reduced, 6)
    Assert.Equal(0.0, taxRate Exempt, 6)
