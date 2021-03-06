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

export type SupplyAdress = {
    name: string,
    address: string,
    city: string,
    country: string,
    zip: string
}

export const getAddressString = (address: SupplyAdress): string => {
    return address.country + " - " + address.city + " - " + address.address
}

export type Order = {
    id: number,
    buyerName: string,
    address: SupplyAdress,
    storeId: number,
    products: Product[],
    date: string,
    price: number
}
export const getOrders = (orders: { id: number, orders: Order[] }[], id: number): Order[] => {
    return orders.length > 0 ? orders.find(o => o.id === id)!.orders : [];
}
export const isStore = (x: any): x is Store => x.tag === "Store";
export const makeStore = (products: Product[], name: string, storeId: number, open: boolean): Store =>
    ({ products: products, name: name, storeId: storeId, open: open });

export const storeById = (id: number, stores: Store[]) => {
    return stores.filter(s => s.storeId === id)[0];
}
