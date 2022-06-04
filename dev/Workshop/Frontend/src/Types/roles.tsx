export interface userToken { userId: number, notifications: string[] };
export interface memberToken extends userToken { userId: number, membername: string }
export interface StoreToken extends memberToken {storeId: number}
export type token = userToken | memberToken | StoreToken

export type Role = MarketManager | StoreRole;
export interface MarketManager {tag: 'MarketManager' };

export type StoreRole = StoreOwner | StoreManager | StoreFounder;

export interface StoreOwner { tag: 'StoreOwner', storeId: number };
export const makeStoreOwner = (storeId: number): StoreOwner => ({ tag: "StoreOwner", storeId: storeId });
export const isEmptyDiscount = (x: any): x is StoreOwner => x.tag === "StoreOwner";

export interface StoreManager { tag: 'StoreManager', storeId: number };
export const makeStoreManager = (storeId: number): StoreManager => ({ tag: "StoreManager", storeId: storeId });
export const isStoreManager = (x: any): x is StoreManager => x.tag === "StoreManager";

export interface StoreFounder { tag: 'StoreFounder', storeId: number };
export const makeStoreFounder = (storeId: number): StoreFounder => ({ tag: "StoreFounder", storeId: storeId });
export const isStoreFounder = (x: any): x is StoreFounder => x.tag === "StoreFounder";

