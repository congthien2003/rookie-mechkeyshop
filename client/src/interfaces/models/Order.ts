export interface Order {
	id: string;
	userId: string;
	name: string;
	email: string;
	status: number;
	orderDate: Date;
	totalAmount: number;
	address: string;
	phone: string;
	orderItems: OrderItems[];
}

export interface OrderItems {
	id: string;
	orderId: string;
	productid: string;
	productName: string;
	imageUrl: string;
	quantity: number;
	price: number;
	option: {
		name: string;
		value: string;
	};
}

export interface EditOrder {
	id: string;
	status: number;
	lastUpdatedAt: Date;
	address: string;
	phone: string;
}
