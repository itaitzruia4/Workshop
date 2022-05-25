import { Products, Product } from "../Components/product";

export type Store = {
    products: []
    name: string
    storeId: number
    open: boolean
}

export type Stores = {
    stores: Store[],
}

export const isStore = (x: any): x is Store => x.tag === "Store";