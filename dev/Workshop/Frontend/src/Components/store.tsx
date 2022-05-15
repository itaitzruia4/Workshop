export type Store = {
    id: number,
    title: string;
}

export type Stores = {
    stores: Store[],
}

export const makeStore = (id: number, title: string): Store => ({ id: id, title: title });
export const isStore = (x: any): x is Store => x.tag === "Store";