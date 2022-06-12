import { Product } from "./product";
import { Store } from "./store";

export type Bag = {
    storeId: number,
    products: Product[]
}

export type Cart = {
    shoppingBags: Bag[]
}

export const getBagCost = (bag: Bag) => {
    const prices = bag.products.map(p => p.basePrice);
    const price = prices.reduce((sum, c) => sum + c, 0);
    return price;
}