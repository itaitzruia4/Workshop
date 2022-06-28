export type PurchaseTerm = PurchaseCompositeTerm | PurchaseSimpleTerm;
export interface PurchaseCompositeTerm { tag: "PurchaseCompositeTerm", value: string, lhs: PurchaseTerm, rhs: PurchaseTerm };
export const makePurchaseCompositeTerm = (value: string, lhs: PurchaseTerm, rhs: PurchaseTerm): PurchaseCompositeTerm => ({ tag: "PurchaseCompositeTerm", value: value, lhs: lhs, rhs: rhs });


export type PurchaseSimpleTerm = ProductPurchaseSimpleTerm | CategoryPurchaseSimpleTerm | BagPurchaseSimpleTerm | UserPurchaseSimpleTerm;
// type means purchase by Price/Quantity/Hour/Date (p/q/h/d)
export interface ProductPurchaseSimpleTerm { tag: "ProductPurchaseSimpleTerm", type: string, action: string, value: string, productId: number };
export const makeProductPurchaseSimpleTerm = (type: string, action: string, value: string, productId: number): ProductPurchaseSimpleTerm => ({ tag: "ProductPurchaseSimpleTerm", type: type, action: action, value: value, productId: productId });

export interface CategoryPurchaseSimpleTerm { tag: "CategoryPurchaseSimpleTerm", type: string, action: string, value: string, category: string };
export const makeCategoryPurchaseSimpleTerm = (type: string, action: string, value: string, category: string): CategoryPurchaseSimpleTerm => ({ tag: "CategoryPurchaseSimpleTerm", type: type, action: action, value: value, category: category });

export interface BagPurchaseSimpleTerm { tag: "BagPurchaseSimpleTerm", type: string, action: string, value: string };
export const makeBagPurchaseSimpleTerm = (type: string, action: string, value: string): BagPurchaseSimpleTerm => ({ tag: "BagPurchaseSimpleTerm", type: type, action: action, value: value });

export interface UserPurchaseSimpleTerm { tag: "UserPurchaseSimpleTerm", action: string, age: number };
export const makeUserPurchaseSimpleTerm = (action: string, value: number): UserPurchaseSimpleTerm => ({ tag: "UserPurchaseSimpleTerm", action: action, age: value });