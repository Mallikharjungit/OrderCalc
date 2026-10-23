module OrderCalc.Core.Tax

open OrderCalc.Domain

/// Sales tax rate for a given tax category.
///   Standard -> 8%
///   Reduced  -> 3%
///   Exempt   -> 0%
let taxRate (category: TaxCategory) : float =
    match category with
    | Standard -> 0.08
    | Reduced -> 0.03
    | Exempt -> 0.0
