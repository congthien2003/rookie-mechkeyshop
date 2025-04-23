export interface Rating {
	id: number;
	stars: number;
	comment: string;
	ratedAt: Date;
	productId: string;
	userId: string;
	name: string;
}
