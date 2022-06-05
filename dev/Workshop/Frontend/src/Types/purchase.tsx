export type PurchaseTerm = PurchaseCompositeTerm | PurchaseSimpleTerm;
export interface PurchaseCompositeTerm { tag: "PurchaseCompositeTerm", value: "and" | "or", lhs: PurchaseTerm, rhs: PurchaseTerm };
export const makePurchaseCompositeTerm = (value: "and" | "or", lhs: PurchaseTerm, rhs: PurchaseTerm): PurchaseCompositeTerm => ({ tag: "PurchaseCompositeTerm", value: value, lhs: lhs, rhs: rhs });


export type PurchaseSimpleTerm = ProductPurchaseSimpleTerm | CategoryPurchaseSimpleTerm | BagPurchaseSimpleTerm | UserPurchaseSimpleTerm;
// type means purchase by Price/Quantity/Hour/Date
export interface ProductPurchaseSimpleTerm { tag: "ProductPurchaseSimpleTerm", type: "p" | "q" | "h" | "d", action: "<" | ">" | "=" | "!=" | ">=" | "<=", value: string, productId: number };
export const makeProductPurchaseSimpleTerm = (type: "p" | "q" | "h" | "d", action: "<" | ">" | "=" | "!=" | ">=" | "<=", value: string, productId: number): ProductPurchaseSimpleTerm => ({ tag: "ProductPurchaseSimpleTerm", type: type, action: action, value: value, productId: productId });

export interface CategoryPurchaseSimpleTerm { tag: "CategoryPurchaseSimpleTerm", type: "p" | "q" | "h" | "d", action: "<" | ">" | "=" | "!=" | ">=" | "<=", value: string, category: string };
export const makeCategoryPurchaseSimpleTerm = (type: "p" | "q" | "h" | "d", action: "<" | ">" | "=" | "!=" | ">=" | "<=", value: string, category: string): CategoryPurchaseSimpleTerm => ({ tag: "CategoryPurchaseSimpleTerm", type: type, action: action, value: value, category: category });

export interface BagPurchaseSimpleTerm { tag: "BagPurchaseSimpleTerm", type: "p" | "q" | "h" | "d", action: "<" | ">" | "=" | "!=" | ">=" | "<=", value: string };
export const makeBagPurchaseSimpleTerm = (type: "p" | "q" | "h" | "d", action: "<" | ">" | "=" | "!=" | ">=" | "<=", value: string): BagPurchaseSimpleTerm => ({ tag: "BagPurchaseSimpleTerm", type: type, action: action, value: value });

export interface UserPurchaseSimpleTerm { tag: "UserPurchaseSimpleTerm", action: ">" | ">=" | "!=", age: number };
export const makeUserPurchaseSimpleTerm = (action: ">" | ">=" | "!=", value: number): UserPurchaseSimpleTerm => ({ tag: "UserPurchaseSimpleTerm", action: action, age: value });
