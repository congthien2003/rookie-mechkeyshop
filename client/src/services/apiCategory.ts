import { Result } from "@/interfaces/common/Result";
import { ResultPagination } from "@/interfaces/common/ResultPagination";
import { Category } from "@/interfaces/models/Category";
import api from "@/lib/axios";

const getList = async (
	page: number = 1,
	pageSize: number = 10,
	searchTerm: string = "",
	asc: boolean = true
) => {
	try {
		const response = await api.get(
			`/category/list?page=${page}&pageSize=${pageSize}&searchTerm=${searchTerm}&asc=${asc}`
		);
		return response.data as ResultPagination<Category>;
	} catch (error) {
		console.error(error);
		throw new Error("Fetch Failed");
	}
};

const getById = async function (id: string) {
	try {
		const response = await api.get(`/category/${id}`);
		return response.data as Result<Category>;
	} catch (error) {
		console.error(error);
		return [];
	}
};

const update = async function (data: Category) {
	try {
		const response = await api.put(`/category/${data.id}`, data);
		return response.data as Result<Category>;
	} catch (error) {
		console.log(error);
		return;
	}
};

const create = async function (data: Category) {
	try {
		const response = await api.post(`/category`, data);
		return response.data as Result<Category>;
	} catch (error) {
		console.log(error);
		return;
	}
};

const deleteById = async function (id: string) {
	try {
		const response = await api.delete(`/category/${id}`);
		return response.data as Result;
	} catch (error) {
		console.error(error);
		return null;
	}
};

export const categoryService = {
	getList,
	getById,
	create,
	update,
	deleteById,
};
