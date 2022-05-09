import React, { Component } from 'react';

export type Discount = CompositeDiscount | ConcreteDiscount;
export type CompositeDiscount = AndDiscount | OrDiscount | XorDiscount;
export interface AndDiscount { tag: "AndDiscount", lhs: Discount, rhs: Discount };
export const makeAndDiscount = (lhs: Discount, rhs: Discount): AndDiscount => ({ tag: "AndDiscount", lhs: lhs, rhs: rhs });
export const isAndDiscount = (x: any): x is AndDiscount => x.tag === "AndDiscount";

export interface OrDiscount { tag: "OrDiscount", lhs: Discount, rhs: Discount };
export const makeOrDiscount = (lhs: Discount, rhs: Discount): OrDiscount => ({ tag: "OrDiscount", lhs: lhs, rhs: rhs });
export const isOrDiscount = (x: any): x is OrDiscount => x.tag === "OrDiscount";

export interface XorDiscount { tag: "XorDiscount", lhs: Discount, rhs: Discount, discountTerm: DiscountTerm };
export const makeXorDiscount = (lhs: Discount, rhs: Discount, discountTerm: DiscountTerm): XorDiscount => ({ tag: "XorDiscount", lhs: lhs, rhs: rhs, discountTerm: discountTerm });
export const isXorDiscount = (x: any): x is XorDiscount => x.tag === "XorDiscount";


export type ConcreteDiscount = SimpleDiscount | ConditionalDiscount;
export interface SimpleDiscount { tag: "SimpleDiscount", priceAction: PriceAction };
export const makeSimpleDiscount = (priceAction: PriceAction): SimpleDiscount => ({ tag: "SimpleDiscount", priceAction: priceAction });
export const isSimpleDiscount = (x: any): x is SimpleDiscount => x.tag === "SimpleDiscount";

export interface ConditionalDiscount { tag: "ConditionalDiscount", priceAction: PriceAction, discountTerm: DiscountTerm };
export const makeConditionalDiscount = (priceAction: PriceAction, discountTerm: DiscountTerm): ConditionalDiscount => ({ tag: "ConditionalDiscount", priceAction: priceAction, discountTerm: discountTerm });
export const isConditionalDiscount = (x: any): x is ConditionalDiscount => x.tag === "ConditionalDiscount";


export type PriceAction = PriceActionComposite | PriceActionSimple;
export interface PriceActionComposite { tag: "PriceActionComposite", value: "sum" | "max", lhs: PriceAction, rhs: PriceAction };
export const makePriceActionComposite = (value: "sum" | "max", lhs: PriceAction, rhs: PriceAction): PriceActionComposite => ({ tag: "PriceActionComposite", value: value, lhs: lhs, rhs: rhs});
export const isPriceActionComposite = (x: any): x is PriceActionComposite => x.tag === "PriceActionComposite";


export type PriceActionSimple = ProductPriceActionSimple | CategoryPriceActionSimple;
export interface ProductPriceActionSimple { tag: "ProductPriceActionSimple", percentage: number, productId: number }
export const makePriceActionSimple = (percentage: number, productId: number): ProductPriceActionSimple => ({ tag: "ProductPriceActionSimple", percentage: percentage, productId: productId });

export interface CategoryPriceActionSimple { tag: "CategoryPriceActionSimple", percentage: number, category: string };
export const makeCategoryPriceActionSimple = (percentage: number, category: string): CategoryPriceActionSimple => ({ tag: "CategoryPriceActionSimple", percentage: percentage, category: category });


export type DiscountTerm = DiscountCompositeTerm | DiscountSimpleTerm;
export interface DiscountCompositeTerm { tag: "DiscountCompositeTerm", value: "and" | "or" | "xor", lhs: DiscountTerm, rhs: DiscountTerm };
export const makeDiscountCompositeTerm = (value: "and" | "or" | "xor", lhs: DiscountTerm, rhs: DiscountTerm): DiscountCompositeTerm => ({ tag: "DiscountCompositeTerm", value: value, lhs: lhs, rhs: rhs });


export type DiscountSimpleTerm = ProductDiscountSimpleTerm | CategoryDiscountSimpleTerm | BagDiscountSimpleTerm;
export interface ProductDiscountSimpleTerm { tag: "ProductDiscountSimpleTerm", action: "<" | ">" | "=" | ">=" | "<=", value: number, productId: number };
export const makeProductDiscountSimpleTerm = (action: "<" | ">" | "=" | ">=" | "<=", value: number, productId: number): ProductDiscountSimpleTerm => ({ tag: "ProductDiscountSimpleTerm", action: action, value: value, productId: productId });

export interface CategoryDiscountSimpleTerm { tag: "CategoryDiscountSimpleTerm", action: "<" | ">" | "=" | ">=" | "<=", value: number, category: string };
export const makeCategoryDiscountSimpleTerm = (action: "<" | ">" | "=" | ">=" | "<=", value: number, category: string): CategoryDiscountSimpleTerm => ({ tag: "CategoryDiscountSimpleTerm", action: action, value: value, category: category });

export interface BagDiscountSimpleTerm { tag: "BagDiscountSimpleTerm", action: "<" | ">" | "=" | ">=" | "<=", value: number };
export const makeBagDiscountSimpleTerm = (action: "<" | ">" | "=" | ">=" | "<=", value: number): BagDiscountSimpleTerm => ({ tag: "BagDiscountSimpleTerm", action: action, value: value });
