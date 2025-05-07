import ApplicationUser from "@/interfaces/models/ApplicationUser";
import { useEffect, useState } from "react";
import { ColumnDef } from "@tanstack/react-table";
import { Badge } from "@/components/ui/badge";
import PaginationComponent from "@/components/app-pagination";
import CustomTable from "@/components/custom-table";
import { Button } from "@/components/ui/button";
import FormUser from "./form-user";
import { Input } from "@/components/ui/input";
import { Search } from "lucide-react";
import { userService } from "@/services/apiUser";
import { ResultPagination } from "@/interfaces/common/ResultPagination";
import { useLoadingStore } from "@/store/store";
import { ToastError, ToastSuccess } from "@/lib/toast";

export const columns: ColumnDef<ApplicationUser>[] = [
	{
		accessorKey: "name",
		header: "Name",
	},
	{
		accessorKey: "email",
		header: "Email",
	},
	{
		accessorKey: "isEmailConfirmed",
		header: "Email Status",
		cell: ({ row }) => {
			return (
				<Badge
					variant={
						row.original.isEmailConfirmed
							? "default"
							: "destructive"
					}>
					{row.original.isEmailConfirmed
						? "Confirmed"
						: "Unconfirmed"}
				</Badge>
			);
		},
	},
	{
		accessorKey: "isDeleted",
		header: "Status",
		cell: ({ row }) => {
			return (
				<Badge
					variant={
						row.original.isDeleted ? "destructive" : "default"
					}>
					{row.original.isDeleted ? "Deleted" : "Active"}
				</Badge>
			);
		},
	},
	{
		accessorKey: "phones",
		header: "Phone",
	},
	// {
	// 	accessorKey: "address",
	// 	header: "Address",
	// },
	// {
	// 	accessorKey: "roleId",
	// 	header: "Role",
	// 	cell: ({ row }) => {
	// 		return row.original.RoleId === 1 ? "Admin" : "User";
	// 	},
	// },
];

function AdminUser() {
	const [currentPage, setCurrentPage] = useState(1);
	const [pageSize] = useState(10);
	const [totalPages, setTotalPages] = useState(0);
	const [isOpenForm, setIsOpenForm] = useState(false);
	const [selectedUser, setSelectedUser] = useState<ApplicationUser>();
	const [searchInput, setSearchInput] = useState("");
	const [listUser, setListUser] = useState<ApplicationUser[]>([]);

	// Zustand Action
	const showLoading = useLoadingStore((state) => state.showLoading);
	const hideLoading = useLoadingStore((state) => state.hideLoading);

	const fetch = async function () {
		showLoading();
		const data: ResultPagination<ApplicationUser> =
			await userService.getList(currentPage, pageSize, searchInput);
		console.log(data);
		if (data.isSuccess) {
			setListUser(data.data.items);
			setCurrentPage(data.data.page);
			setTotalPages(data.data.totalPages);
		} else {
			setListUser([]);
		}

		hideLoading();
	};

	useEffect(() => {
		fetch();
	}, []);

	const handlePageChange = (page: number) => {
		setCurrentPage(page);
		console.log(page);
	};

	const handleRowClick = (data: ApplicationUser) => {
		setIsOpenForm(true);
		setSelectedUser(data);
	};

	const handleCloseForm = () => {
		setIsOpenForm(false);
		setSelectedUser(undefined);
	};

	const handleSaveUser = async (updatedUser: ApplicationUser) => {
		showLoading();
		try {
			const result = await userService.updateById(
				updatedUser.id,
				updatedUser
			);
			if (result.isSuccess) {
				await fetch(); // Refresh the data after successful update
				handleCloseForm();
				ToastSuccess(result.message);
			} else {
				ToastError(result.message);
			}
		} catch (error) {
			console.error("Error updating user:", error);
		} finally {
			hideLoading();
		}
	};

	const handleDeleteUser = (id: string) => {
		console.log("Deleted use", id);
		handleCloseForm();
	};

	return (
		<>
			<div className="flex align-center justify-between p-4 bg-primary h-16">
				<h2 className="font-bold text-2xl text-white">
					User Management
				</h2>
			</div>
			<div className="px-4 py-2">
				<div className="flex items-center gap-4">
					<Input
						value={searchInput}
						onChange={(e) => setSearchInput(e.target.value)}
					/>
					<Button className="lg:w-28" onClick={() => fetch()}>
						{" "}
						<Search />{" "}
					</Button>
				</div>
				<div className="mt-4 rounded-md border">
					<CustomTable
						data={listUser}
						columns={columns}
						onRowClick={handleRowClick}
					/>
				</div>
				<PaginationComponent
					page={currentPage}
					totalPages={totalPages}
					onPageChange={handlePageChange}
				/>
				{isOpenForm && selectedUser && (
					<FormUser
						data={selectedUser}
						onClose={handleCloseForm}
						onSave={handleSaveUser}
						onDelete={handleDeleteUser}
					/>
				)}
			</div>
		</>
	);
}

export default AdminUser;
