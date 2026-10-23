# OrderCalc — F# Migration

F# port of the reference C++ order/inventory calculator at
[`cpp-source/OrderCalc`](../cpp-source/OrderCalc). Business rules, calculation
order, and numeric results are preserved exactly (see
`tests/OrderCalc.Tests/OrderCalculatorTests.fs` for hand-derived parity checks).

## Project layout

| C++ (original)                         | F# (migrated)                                        |
|-----------------------------------------|-------------------------------------------------------|
| `include/Enums.h`, `Product.h`, `Customer.h`, `OrderLine.h`, `Order.h` | `src/OrderCalc.Domain/Types.fs` |
| `include/Discounts.h` + `src/Discounts.cpp` | `src/OrderCalc.Core/Discounts.fs` |
| `include/Tax.h` + `src/Tax.cpp`         | `src/OrderCalc.Core/Tax.fs`                            |
| `include/Shipping.h` + `src/Shipping.cpp` | `src/OrderCalc.Core/Shipping.fs`                     |
| `include/Inventory.h` + `src/Inventory.cpp` | `src/OrderCalc.Core/Inventory.fs`                  |
| `include/OrderCalculator.h` + `src/OrderCalculator.cpp` | `src/OrderCalc.Core/OrderCalculator.fs` |
| `src/main.cpp`                          | `src/OrderCalc.Cli/Program.fs`                         |
| *(none)*                                | `tests/OrderCalc.Tests/*` (new xUnit coverage)         |

## Build, run, test

```powershell
$env:PATH = "$env:USERPROFILE\.dotnet;$env:PATH"   # if dotnet isn't already on PATH
dotnet build
dotnet test
dotnet run --project src/OrderCalc.Cli
```

## Business rules preserved

1. **Line discount** — quantity ≥ 50 → 10% off; ≥ 10 → 5% off; otherwise 0%.
2. **Order discount** — subtotal ≥ 1000 → 3% off; ≥ 500 → 1.5% off; otherwise 0%.
3. **Membership discount** — Gold 7%, Premium 4%, Standard 0%; stacks
   *multiplicatively* with the order discount (not additively).
4. **Tax** — Standard 8%, Reduced 3%, Exempt 0%, applied per line after all
   discounts.
5. **Shipping** — free once the discounted subtotal reaches $200, otherwise a
   flat $15.
6. **Inventory** — reorder point = `avgDailyUsage * leadTimeDays + safetyStock`;
   EOQ = `sqrt(2 * annualDemand * orderCost / holdingCostPerUnit)`.
