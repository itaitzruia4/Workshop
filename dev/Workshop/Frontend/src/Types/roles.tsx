import { MarketNotification } from "./Notification";

export type token = userToken | memberToken | StoreToken;
export interface userToken { tag: "userToken", userId: number };
export const makeUserToken = (userId: number): userToken => ({ tag: "userToken", userId: userId});

export interface memberToken { tag: "memberToken", userId: number, notifications: MarketNotification[], membername: string }
export const makeMemberToken = (userId: number, membername: string, notifications: MarketNotification[]): memberToken => ({ tag: "memberToken", userId: userId, membername: membername, notifications: notifications });
export const isMemberToken = (x: any): x is memberToken => x.tag === "memberToken";

export interface StoreToken extends memberToken {storeId: number}

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

export enum Actions {
    AddProduct = 0,
    RemoveProduct = 1,
    ChangeProductName = 2,
    ChangeProductPrice = 3,
    ChangeProductQuantity = 4,
    ChangeProductDescription = 5,
    NominateStoreOwner = 6,
    NominateStoreManager = 7,
    GetWorkersInformation = 8,
    OpenStore = 9,
    CloseStore = 10,
    AddPermissionToStoreManager = 11,
    RemovePermissionFromStoreManager = 12,
    GetStoreOrdersList = 13,
    ViewClosedStore = 14,
    AddDiscount = 15,
    GetMarketStatistics = 16,
    CancelMember = 17,
    GetMembersOnlineStats = 18,
    AddPurchaseTerm = 19,
    ViewStorePurchaseHistory = 20,
    ChangeProductCategory = 21,
}

export interface StorePermission {userId: number, membername: string, storeId: number, permissions: Actions[] }

export const permissionsById = (id: number, storePermissions: StorePermission[]): Actions[] => {
    const actions = [...Array(Math.ceil(Object.keys(Actions).length / 2))].map((_, i) => i as Actions);
    if (storePermissions.length > 0 && storePermissions[0].storeId === -1) { return actions }
    const storePermission = storePermissions.filter(sp => sp.storeId === id)
    return storePermission.length > 0 ? storePermission[0].permissions : [];
}
export const isManager = (id: number, storePermissions: StorePermission[]) => {
    return storePermissions.filter(sp => sp.storeId === id).length > 0
}

export const actionNames = (): string[] => {
    const actions = Object.keys(Actions).filter((item) => {
        return isNaN(Number(item));
    });
    return actions
}

export const hasPermission = (permission: Actions, storePermission: Actions[]): boolean => {
    return storePermission.includes(permission)
}

