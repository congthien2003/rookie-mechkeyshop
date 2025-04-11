import api from "../lib/axios.ts";

export const getList = async () => {
	try {
		const response = await api.get(`/products/1`);
		return response.data;
	} catch (error) {
		console.error(error);
		throw error;
	}
};
