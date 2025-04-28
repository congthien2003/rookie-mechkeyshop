import api from "@/lib/axios";

const getList = async (
	page: number = 1,
	pageSize: number = 10,
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

export const userService = {
	getList,
};
