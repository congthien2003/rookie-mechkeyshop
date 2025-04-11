import axios, { AxiosInstance } from "axios";

const API_BASE_URL = "https://fakestoreapi.com";

const api: AxiosInstance = axios.create({
	baseURL: API_BASE_URL,
	headers: {
		"Content-Type": "application/json",
	},
});

export default api;
