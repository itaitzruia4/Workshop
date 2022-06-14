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
    AddProduct,
    RemoveProduct,
    ChangeProductName,
    ChangeProductPrice,
    ChangeProductQuantity,
    ChangeProductDescription,
    NominateStoreOwner,
    NominateStoreManager,
    GetWorkersInformation,
    OpenStore,
    CloseStore,
    AddPermissionToStoreManager,
    RemovePermissionFromStoreManager,
    GetStoreOrdersList,
    ViewClosedStore,
    AddDiscount,
    GetMarketStatistics,
    CancelMember,
    GetMembersOnlineStats,
    AddPurchaseTerm
}

