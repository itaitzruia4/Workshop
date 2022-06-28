import { Product } from "./product";
import { Store } from "./store";

export type Bag = {
    storeId: number,
    products: Product[]
}

export type Cart = {
    shoppingBags: Bag[],
    price: number
}

export const getBagCost = (bag: Bag) => {
    const prices = bag.products.map(p => { return (p.basePrice * p.quantity) });
    const price = prices.reduce((sum, c) => sum + c, 0);
    return price;
}

export const getBagToStore = (cart: Cart, stores: Store[]) : [Bag,Store][] =>{
    return cart.shoppingBags.map(b => [b, stores.find(s => s.storeId === b.storeId)!]);
}