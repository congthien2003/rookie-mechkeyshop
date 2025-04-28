import {
	CreateProduct,
	EditProduct,
	Product,
} from "@/interfaces/models/Product.ts";
import api from "../lib/axios.ts";
import { Result } from "@/interfaces/common/Result.ts";

const getList = async (
	page: number = 1,
	pageSize: number = 10,
	searchTerm: string = "",
	categoryId: string = "",
	sortCol: string = "",
	asc: boolean = true
) => {
	try {
		const response = await api.get(
			`/product/list?page=${page}&pageSize=${pageSize}&searchTerm=${searchTerm}&categoryId=${categoryId}&sortCol=${sortCol}&asc=${asc}`
		);
		return response.data;
	} catch (error) {
		console.error(error);
		return [];
	}
};

const create = async (model: CreateProduct) => {
	try {
		console.log("model", model);
		const response = await api.post(`/product`, model);
		return response.data;
	} catch (error) {
		console.error(error);
		throw new Error("Created fail");
	}
};

const edit = async (model: EditProduct) => {
	try {
		const response = await api.put(`/product`, model);
		return response.data as Result<Product>;
	} catch (error) {
		console.error(error);
		throw new Error("Updated fail");
	}
};

const deleteById = async (id: string) => {
	try {
		const response = await api.delete(`/product/${id}`);
		return response.data as Result<Product>;
	} catch (error) {
		console.error(error);
		throw new Error("Updated fail");
	}
};

export const uploadImage = async (
	base64String: string | ArrayBuffer | null
) => {
	try {
		const response = await api.post("/supabase", {
			Base64String: base64String,
		});
		return response.data;
	} catch (error) {
		console.error(error);
		throw error;
	}
};

export const productService = {
	getList,
	create,
	edit,
	deleteById,
};
