import { Login } from "../interfaces/models/Login";
import axios from "../lib/axios";
const API_URL = import.meta.env.VITE_API_URL || "http://localhost:3000/api";

interface User {
	id: string;
	email: string;
	// Add other user properties as needed
}

export const login = async (credentials: Login): Promise<User> => {
	const response = await axios.post(`auth/login`, credentials, {
		withCredentials: true, // This is important for cookies
	});
	return response.data.data;
};

export const logout = async () => {
	await axios.post(
		`auth/logout`,
		{},
		{
			withCredentials: true,
		}
	);
};

export const checkAuth = async (): Promise<User> => {
	const response = await axios.get(`auth/check`, {
		withCredentials: true,
	});
	return response.data.user;
};
