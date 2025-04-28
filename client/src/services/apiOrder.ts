import api from "@/lib/axios";
import { EditOrder, Order, OrderItems } from "@/interfaces/models/Order";
import { ResultPagination } from "@/interfaces/common/ResultPagination";

const getList = async (
	page: number = 1,
	pageSize: number = 10,
	searchTerm: string = "",
	startDate: string = "",
	endDate: string = "",
	sortCol: string = "",
	asc: boolean = true
) => {
	try {
		const params = new URLSearchParams({
			page: page.toString(),
			pageSize: pageSize.toString(),
			searchTerm,
			startDate,
			endDate,
			sortCol,
			asc: asc.toString(),
		});

		const response = await api.get(`/orders/list?${params.toString()}`);
		return response.data;
	} catch (error) {
		console.error(error);
		return {
			isSuccess: false,
			message: "Failed to fetch orders",
			data: {
				items: [],
				totalItems: 0,
				page: 1,
				pageSize: 10,
				totalPages: 0,
			},
		} as ResultPagination<Order>;
	}
};

const getOrderItems = async (orderId: string) => {
	try {
		const response = await api.get(`/orders/${orderId}/items`);
		return response.data;
	} catch (error) {
		console.error(error);
		return [] as OrderItems[];
	}
};

const updateOrder = async (order: Order) => {
	try {
		const data: EditOrder = {
			id: order.id,
			status: order.status,
			lastUpdatedAt: new Date(new Date().getTime() + 7 * 60 * 60 * 1000),
			address: order.address,
			phone: order.phone,
		};
		const response = await api.put(`/orders`, data);
		return response.data;
	} catch (error) {
		console.error(error);
		return {
			isSuccess: false,
			message: "Failed to update order",
		};
	}
};

const deleteOrder = async (id: string) => {
	try {
		const response = await api.delete(`/orders/${id}`);
		return response.data;
	} catch (error) {
		console.error(error);
		return {
			isSuccess: false,
			message: "Failed to delete order",
		};
	}
};

export const orderService = {
	getList,
	getOrderItems,
	updateOrder,
	deleteOrder,
};
