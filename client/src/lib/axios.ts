import axios, { AxiosInstance } from "axios";

const API_BASE_URL =
	import.meta.env.VITE_API_URL || "https://localhost:5000/api/v1";

console.log(API_BASE_URL);

const api: AxiosInstance = axios.create({
	baseURL: API_BASE_URL,
	headers: {
		"Content-Type": "application/json",
	},
});
console.log(api);

export default api;
