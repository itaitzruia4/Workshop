export interface Product { tag: 'Product', id: number, name: string, basePrice: number, description: string, quantity: number };
export const makeProduct = (id: number, name: string, basePrice: number, description: string, quantity: number): Product =>
    ({ tag: "Product", id: id, name: name, basePrice: basePrice, description: description, quantity: quantity });