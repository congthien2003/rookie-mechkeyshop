import { Rating } from "./Rating";
import { Variant } from "./Variant";

export interface Product {
	id: string;
	name: string;
	description: string;
	price: number;
	imageUrl: string;
	categoryId: string;
	categoryName: string;
	rating: Rating[];
	totalRating: number;
	sellCount: number;
	variants: Variant[];
	isDeleted: boolean;
}

export interface CreateProduct {
	name: string;
	description: string;
	price: number;
	categoryId: string;
	variants: Variant[];
	base64string: string;
}
