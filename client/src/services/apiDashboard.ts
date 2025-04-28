import api from "@/lib/axios";
import { DashboardData } from "@/interfaces/models/DashboardData";

const getDashboardData = async () => {
	try {
		const response = await api.get("/dashboard");
		return response.data as DashboardData;
	} catch (error) {
		console.error(error);
		return {
			totalRevenue: 0,
			totalSellCount: 0,
			totalOrderCount: 0,
			totalOrderCompleted: 0,
			totalOrderPending: 0,
			totalProductAvalible: 0,
			totalUserAvalible: 0,
		} as DashboardData;
	}
};

export const dashboardService = {
	getDashboardData,
};
