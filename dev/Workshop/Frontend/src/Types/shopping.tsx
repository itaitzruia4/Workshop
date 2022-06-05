import { Product } from "./product";
import { Store } from "./store";

export type Bag = {
    storeId: number,
    products: Product[]
}

export type Cart = {
    shoppingBags: Bag[]
}