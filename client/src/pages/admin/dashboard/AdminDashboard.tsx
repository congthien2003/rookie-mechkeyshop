import { useEffect, useState } from "react";
import { DashboardData } from "@/interfaces/models/DashboardData";
import { useLoadingStore } from "@/store/store";
import { dashboardService } from "@/services/apiDashboard";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
	ArrowUp,
	Users,
	Package,
	ShoppingCart,
	DollarSign,
	CheckCircle,
	Clock,
} from "lucide-react";

function AdminDashboard() {
	const [dashboardData, setDashboardData] = useState<DashboardData>();
	const showLoading = useLoadingStore((state) => state.showLoading);
	const hideLoading = useLoadingStore((state) => state.hideLoading);

	const fetchDashboardData = async () => {
		showLoading();
		try {
			const response = await dashboardService.getDashboardData();

			setDashboardData(response.data);
		} catch (error) {
			console.error(error);
		} finally {
			hideLoading();
		}
	};

	useEffect(() => {
		fetchDashboardData();
	}, []);

	const formatCurrency = (amount: number) => {
		return new Intl.NumberFormat("en-US", {
			style: "currency",
			currency: "USD",
		}).format(amount);
	};

	const formatNumber = (num: number) => {
		return new Intl.NumberFormat("en-US").format(num);
	};

	return (
		<div className="space-y-6 p-6">
			<div className="flex items-center justify-between">
				<h1 className="text-3xl font-bold">Dashboard Overview</h1>
				<div className="text-sm text-gray-500">
					Last updated: {new Date().toLocaleString()}
				</div>
			</div>

			{/* Main Stats Cards */}
			<div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
				<Card>
					<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
						<CardTitle className="text-sm font-medium">
							Total Revenue
						</CardTitle>
						<DollarSign className="h-4 w-4 text-muted-foreground" />
					</CardHeader>
					<CardContent>
						<div className="text-2xl font-bold">
							{formatCurrency(dashboardData?.totalRevenue || 0)}
						</div>
						<p className="text-xs text-muted-foreground">
							+20.1% from last month
						</p>
					</CardContent>
				</Card>

				<Card>
					<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
						<CardTitle className="text-sm font-medium">
							Total Orders
						</CardTitle>
						<ShoppingCart className="h-4 w-4 text-muted-foreground" />
					</CardHeader>
					<CardContent>
						<div className="text-2xl font-bold">
							{formatNumber(dashboardData?.totalOrderCount || 0)}
						</div>
						<p className="text-xs text-muted-foreground">
							+180.1% from last month
						</p>
					</CardContent>
				</Card>

				<Card>
					<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
						<CardTitle className="text-sm font-medium">
							Total Products
						</CardTitle>
						<Package className="h-4 w-4 text-muted-foreground" />
					</CardHeader>
					<CardContent>
						<div className="text-2xl font-bold">
							{formatNumber(
								dashboardData?.totalProductAvailable || 0
							)}
						</div>
						<p className="text-xs text-muted-foreground">
							+12% from last month
						</p>
					</CardContent>
				</Card>

				<Card>
					<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
						<CardTitle className="text-sm font-medium">
							Active Users
						</CardTitle>
						<Users className="h-4 w-4 text-muted-foreground" />
					</CardHeader>
					<CardContent>
						<div className="text-2xl font-bold">
							{formatNumber(
								dashboardData?.totalUserAvailable || 0
							)}
						</div>
						<p className="text-xs text-muted-foreground">
							+201 since last month
						</p>
					</CardContent>
				</Card>
			</div>

			{/* Order Status Cards */}
			<div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
				<Card>
					<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
						<CardTitle className="text-sm font-medium">
							Completed Orders
						</CardTitle>
						<CheckCircle className="h-4 w-4 text-green-500" />
					</CardHeader>
					<CardContent>
						<div className="text-2xl font-bold">
							{formatNumber(
								dashboardData?.totalOrderCompleted || 0
							)}
						</div>
						<div className="flex items-center text-xs text-green-500">
							<ArrowUp className="mr-1 h-3 w-3" />
							<span>+12% from last month</span>
						</div>
					</CardContent>
				</Card>

				<Card>
					<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
						<CardTitle className="text-sm font-medium">
							Pending Orders
						</CardTitle>
						<Clock className="h-4 w-4 text-yellow-500" />
					</CardHeader>
					<CardContent>
						<div className="text-2xl font-bold">
							{formatNumber(
								dashboardData?.totalOrderPending || 0
							)}
						</div>
						<div className="flex items-center text-xs text-yellow-500">
							<ArrowUp className="mr-1 h-3 w-3" />
							<span>+5% from last month</span>
						</div>
					</CardContent>
				</Card>

				<Card>
					<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
						<CardTitle className="text-sm font-medium">
							Total Sales
						</CardTitle>
						<ShoppingCart className="h-4 w-4 text-blue-500" />
					</CardHeader>
					<CardContent>
						<div className="text-2xl font-bold">
							{formatNumber(dashboardData?.totalSellCount || 0)}
						</div>
						<div className="flex items-center text-xs text-blue-500">
							<ArrowUp className="mr-1 h-3 w-3" />
							<span>+8% from last month</span>
						</div>
					</CardContent>
				</Card>
			</div>

			{/* Charts Section */}
			{/* <div className="grid gap-4 md:grid-cols-2">
				<Card className="col-span-2">
					<CardHeader>
						<CardTitle>Revenue Overview</CardTitle>
					</CardHeader>
					<CardContent>
						<div className="h-[300px] flex items-center justify-center text-gray-500">
							Revenue Chart Placeholder
						</div>
					</CardContent>
				</Card>

				<Card>
					<CardHeader>
						<CardTitle>Order Status Distribution</CardTitle>
					</CardHeader>
					<CardContent>
						<div className="h-[300px] flex items-center justify-center text-gray-500">
							Pie Chart Placeholder
						</div>
					</CardContent>
				</Card>

				<Card>
					<CardHeader>
						<CardTitle>Top Selling Products</CardTitle>
					</CardHeader>
					<CardContent>
						<div className="h-[300px] flex items-center justify-center text-gray-500">
							Bar Chart Placeholder
						</div>
					</CardContent>
				</Card>
			</div> */}
		</div>
	);
}

export default AdminDashboard;
