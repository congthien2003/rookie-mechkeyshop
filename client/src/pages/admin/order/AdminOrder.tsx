import { useEffect, useState } from "react";
import { ColumnDef } from "@tanstack/react-table";
import { Badge } from "@/components/ui/badge";
import PaginationComponent from "@/components/app-pagination";
import CustomTable from "@/components/custom-table";
import { Button } from "@/components/ui/button";
import FormOrder from "./form-order";
import { Input } from "@/components/ui/input";
import { Search } from "lucide-react";
import { Order } from "@/interfaces/models/Order";
import { ResultPagination } from "@/interfaces/common/ResultPagination";
import { useLoadingStore } from "@/store/store";
import { orderService } from "@/services/apiOrder";
import { Label } from "@/components/ui/label";
import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
} from "@/components/ui/select";

export const columns: ColumnDef<Order>[] = [
	// {
	// 	accessorKey: "id",
	// 	header: "Order ID",
	// },
	{
		accessorKey: "name",
		header: "Customer Name",
	},
	{
		accessorKey: "email",
		header: "Email",
	},
	{
		accessorKey: "phone",
		header: "Phones",
	},
	{
		accessorKey: "totalAmount",
		header: "Total Amount",
		cell: ({ row }) => {
			return `$${row.original.totalAmount}`;
		},
	},
	{
		accessorKey: "status",
		header: "Status",
		cell: ({ row }) => {
			const status = row.original.status;
			switch (status) {
				case 0:
					return (
						<Badge variant="outline" className="bg-yellow-300">
							Pending
						</Badge>
					);
				case 1:
					return <Badge variant="default">Accepted</Badge>;
				case 2:
					return <Badge variant="destructive">Cancelled</Badge>;
				case 3:
					return (
						<Badge variant="secondary" className="bg-green-400">
							Completed
						</Badge>
					);
				default:
					return <Badge variant="outline">Unknown</Badge>;
			}
		},
	},
	{
		accessorKey: "orderDate",
		header: "Order Date",
		cell: ({ row }) => {
			return new Date(row.original.orderDate).toLocaleString();
		},
	},
	{
		accessorKey: "address",
		header: "Shipping Address",
		cell: ({ row }) => {
			return <>{row.original.address.substring(0, 20)}...</>;
		},
	},
];

function AdminOrder() {
	const [currentPage, setCurrentPage] = useState(1);
	const [pageSize] = useState(10);
	const [totalPages, setTotalPages] = useState(0);
	const [isOpenForm, setIsOpenForm] = useState(false);
	const [selectedOrder, setSelectedOrder] = useState<Order>();
	const [searchInput, setSearchInput] = useState("");
	const [listOrder, setListOrder] = useState<Order[]>([]);
	const [startDate, setStartDate] = useState<string>("");
	const [endDate, setEndDate] = useState<string>("");
	const [sortCol, setSortCol] = useState<string>("");
	const [asc, setAsc] = useState<boolean>(true);

	// Zustand Action
	const showLoading = useLoadingStore((state) => state.showLoading);
	const hideLoading = useLoadingStore((state) => state.hideLoading);

	const fetchOrders = async () => {
		showLoading();
		try {
			const data: ResultPagination<Order> = await orderService.getList(
				currentPage,
				pageSize,
				searchInput,
				startDate,
				endDate,
				sortCol,
				asc
			);
			if (data.isSuccess) {
				setListOrder(data.data.items);
				setCurrentPage(data.data.page);
				setTotalPages(data.data.totalPages);
			} else {
				setListOrder([]);
			}
		} catch (error) {
			console.error(error);
			setListOrder([]);
		} finally {
			hideLoading();
		}
	};

	useEffect(() => {
		fetchOrders();
	}, [currentPage, startDate, endDate, sortCol, asc]);

	const handlePageChange = (page: number) => {
		setCurrentPage(page);
	};

	const handleRowClick = async (data: Order) => {
		setSelectedOrder(data);
		setIsOpenForm(true);
	};

	const handleCloseForm = () => {
		setIsOpenForm(false);
		setSelectedOrder(undefined);
	};

	const handleSaveOrder = async (updatedOrder: Order) => {
		await orderService.updateOrder(updatedOrder);
		fetchOrders();
		handleCloseForm();
	};

	const handleDeleteOrder = async (id: string) => {
		await orderService.deleteOrder(id);
		fetchOrders();
		handleCloseForm();
	};

	const handleSortChange = (value: string) => {
		setSortCol(value);
	};

	const handleSortDirectionChange = (value: string) => {
		setAsc(value === "asc");
	};

	const resetFilters = () => {
		setStartDate("");
		setEndDate("");
		setSortCol("");
		setAsc(true);
		setSearchInput("");
	};

	return (
		<>
			<div className="flex align-center justify-between p-4 bg-primary">
				<h2 className="font-bold text-2xl text-white ">
					Order Management
				</h2>
			</div>
			<div className="px-4 py-4">
				{/* Search and Filters */}
				<div className="space-y-4">
					<div className="flex items-center gap-4">
						<Input
							value={searchInput}
							onChange={(e) => setSearchInput(e.target.value)}
							placeholder="Search orders..."
						/>
						<Button
							className="lg:w-28"
							onClick={() => fetchOrders()}>
							<Search />
						</Button>
						<Button variant="outline" onClick={resetFilters}>
							Reset Filters
						</Button>
					</div>
					<div className="grid grid-cols-4 gap-4">
						<div className="space-y-2">
							<Label>Date Range</Label>
							<div className="flex gap-2">
								<Input
									type="date"
									value={startDate}
									onChange={(e) =>
										setStartDate(e.target.value)
									}
								/>
								<Input
									type="date"
									value={endDate}
									onChange={(e) => setEndDate(e.target.value)}
								/>
							</div>
						</div>

						<div className="space-y-2">
							<Label>Sort By</Label>
							<Select
								value={sortCol}
								onValueChange={handleSortChange}>
								<SelectTrigger>
									<SelectValue placeholder="Select column" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="amount">
										Amount
									</SelectItem>
									<SelectItem value="date">Date</SelectItem>
								</SelectContent>
							</Select>
						</div>
						<div className="space-y-2">
							<Label>Sort Direction</Label>
							<Select
								value={asc ? "asc" : "desc"}
								onValueChange={handleSortDirectionChange}>
								<SelectTrigger>
									<SelectValue placeholder="Select direction" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="asc">
										Ascending
									</SelectItem>
									<SelectItem value="desc">
										Descending
									</SelectItem>
								</SelectContent>
							</Select>
						</div>
					</div>
				</div>

				<div className="mt-4 rounded-md border">
					<CustomTable
						data={listOrder}
						columns={columns}
						onRowClick={handleRowClick}
					/>
				</div>
				<PaginationComponent
					page={currentPage}
					totalPages={totalPages}
					onPageChange={handlePageChange}
				/>
				{isOpenForm && selectedOrder && (
					<FormOrder
						data={selectedOrder}
						onClose={handleCloseForm}
						onSave={handleSaveOrder}
						onDelete={handleDeleteOrder}
					/>
				)}
			</div>
		</>
	);
}

export default AdminOrder;
