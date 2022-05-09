export interface Store {
    tag: 'Store',
    id: number,
    title: string;
}

export const makeStore = (id: number, title: string): Store => ({ tag: "Store", id: id, title: title });
export const isStore = (x: any): x is Store => x.tag === "Store";