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
