import api from "@/lib/axios";
import { DashboardData } from "@/interfaces/models/DashboardData";
import { Result } from "@/interfaces/common/Result";

const getDashboardData = async () => {
	try {
		const response = await api.get<Result<DashboardData>>("/dashboard");
		return response.data;
	} catch (error) {
		console.error(error);
		return {
			isSuccess: false,
			message: "Failed to fetch dashboard data",
			data: {
				totalRevenue: 0,
				totalSellCount: 0,
				totalOrderCount: 0,
				totalOrderCompleted: 0,
				totalOrderPending: 0,
				totalProductAvailable: 0,
				totalUserAvailable: 0,
			} as DashboardData,
		} as Result<DashboardData>;
	}
};

export const dashboardService = {
	getDashboardData,
};
