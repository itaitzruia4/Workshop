export type Role = MarketManager | StoreRole;
export interface MarketManager {tag: 'MarketManager' };

export type StoreRole = StoreOwner | StoreManager | StoreFounder;
export interface StoreOwner { tag: 'StoreOwner', storeId: number };
export interface StoreManager { tag: 'StoreManager', storeId: number };
export interface StoreFounder { tag: 'StoreFounder', storeId: number };