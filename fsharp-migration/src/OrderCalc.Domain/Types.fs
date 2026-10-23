namespace OrderCalc.Domain

/// Tax category applied to a product's sale price.
type TaxCategory =
    | Standard // 8%
    | Reduced  // 3%
    | Exempt   // 0%

/// Customer loyalty tier; grants a stacked discount on top of order discounts.
type MembershipTier =
    | StandardTier // 0%
    | Premium      // 4%
    | Gold         // 7%

/// A sellable item plus the inventory-planning parameters used to
/// decide when and how much of it to reorder.
type Product =
    { Id: string
      Name: string
      UnitPrice: float
      TaxCategory: TaxCategory

      // Inventory planning parameters.
      AverageDailyUsage: float // units/day
      LeadTimeDays: int        // days between placing and receiving an order
      SafetyStockUnits: int    // buffer stock kept against demand variability
      AnnualDemand: float      // units/year, used for Economic Order Quantity
      OrderCost: float         // fixed cost ($) of placing one purchase order
      HoldingCostPerUnit: float } // annual holding cost ($) per unit in stock

type Customer =
    { Id: string
      Name: string
      Tier: MembershipTier }

type OrderLine = { Product: Product; Quantity: int }

type Order = { Customer: Customer; Lines: OrderLine list }
