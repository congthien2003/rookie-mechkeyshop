import ApplicationUser from "@/interfaces/models/ApplicationUser";
import api from "@/lib/axios";

const getList = async (
	page: number = 1,
	pageSize: number,
	searchTerm: string = ""
) => {
	try {
		const response = await api.get(
			`/users/list?page=${page}&pageSize=${pageSize}&searchTerm=${searchTerm}`
		);
		return response.data;
	} catch (error) {
		console.error(error);
		return [];
	}
};

const getById = async (id: string) => {
	try {
		const response = await api.get(`/users/${id}`);
		return response.data;
	} catch (error) {
		console.error(error);
		return null;
	}
};

const updateById = async (id: string, user: ApplicationUser) => {
	try {
		const response = await api.put(`/users/${id}`, user);
		return response.data;
	} catch (error) {
		console.error(error);
		return null;
	}
};

const deleteById = async (id: string) => {
	try {
		const response = await api.delete(`/users/${id}`);
		return response.data;
	} catch (error) {
		console.error(error);
		return null;
	}
};

export const userService = {
	getList,
	getById,
	updateById,
	deleteById,
};
