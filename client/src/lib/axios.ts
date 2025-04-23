import axios, { AxiosInstance } from "axios";

const API_BASE_URL = "https://localhost:7125/api/v1/";

const api: AxiosInstance = axios.create({
	baseURL: API_BASE_URL,
	headers: {
		"Content-Type": "application/json",
	},
});

export default api;
