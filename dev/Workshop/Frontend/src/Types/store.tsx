import {Product } from "./product";

export type Store = {
    products: Product[]
    name: string
    storeId: number
    open: boolean
}

export type Stores = {
    stores: Store[],
}

export const isStore = (x: any): x is Store => x.tag === "Store";
export const makeStore = (products: Product[], name: string, storeId: number, open: boolean): Store =>
    ({ products: products, name: name, storeId: storeId, open: open });

export const storeById = (id: number, stores: Store[]) => {
    return stores.filter(s => s.storeId === id)[0];
}
