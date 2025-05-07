import PaginationComponent from "@/components/app-pagination";
import CustomTable from "@/components/custom-table";
import { Category } from "@/interfaces/models/Category";
import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import FormCategory from "./form-category";
import { Badge } from "@/components/ui/badge";
import { categoryService } from "@/services/apiCategory";
import { ResultPagination } from "@/interfaces/common/ResultPagination";
import { ColumnDef } from "@tanstack/react-table";
import { PlusCircle, Search } from "lucide-react";
import { ToastError, ToastSuccess } from "@/lib/toast";
import { Input } from "@/components/ui/input";
// eslint-disable-next-line react-refresh/only-export-components
export const columns: ColumnDef<Category>[] = [
	{
		accessorKey: "name",
		header: "Name",
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
		accessorKey: "action",
		header: "Actions",
		cell: () => {
			return (
				<div className="flex gap-2">
					<Button variant={"outline"}>Edit</Button>
				</div>
			);
		},
	},
];

function AdminCategory() {
	const [listCategory, setListCategory] = useState<Category[]>([]);
	const [currentPage, setCurrentPage] = useState(1);
	const pageSize = 5;
	const [totalPage, setTotalPage] = useState(0);
	const [isOpenForm, setIsOpenForm] = useState(false);
	const [selectedCategory, setSelectedCategory] = useState<Category>();
	const [isEdit, setIsEdit] = useState(true);
	const fetch = async function () {
		const response: ResultPagination<Category> =
			await categoryService.getList(currentPage, pageSize);
		console.log(response);

		if (response.isSuccess) {
			setListCategory(response.data.items);
			setCurrentPage(response.data.page);
			setTotalPage(response.data.totalPages);
		}
	};

	const updateCategory = async (data: Category) => {
		const response = await categoryService.update(data);
		if (response?.isSuccess) {
			ToastSuccess("Updated success");
			await fetch();
		} else {
			ToastError("Updated failed");
		}
	};

	const addCategory = async (data: Category) => {
		const response = await categoryService.create(data);
		if (response?.isSuccess) {
			ToastSuccess("Updated success");
			await fetch();
		} else {
			ToastError("Updated failed");
		}
	};

	useEffect(() => {
		fetch();
	}, [currentPage]);

	const handlePageChange = (page: number) => {
		setCurrentPage(page);
	};

	const handleRowClick = (data: Category) => {
		setIsOpenForm(true);
		setSelectedCategory(data);
		setIsEdit(true);
	};

	const handleOpenCreateForm = () => {
		setIsOpenForm(true);
		setSelectedCategory(undefined);
		setIsEdit(false);
	};

	const handleCloseForm = () => {
		setIsOpenForm(false);
		setSelectedCategory(undefined);
	};

	const handleSaveCategory = async (category: Category) => {
		// Here you would typically make an API call to update the user
		if (isEdit) {
			console.log("Updated category:", category);
			await updateCategory(category);
		} else {
			console.log("Created category:", category);
			await addCategory(category);
		}
		handleCloseForm();
	};

	const handleDeleteCategory = (id: string) => {
		console.log("Deleted use", id);
		handleCloseForm();
	};

	return (
		<>
			<div className="flex align-center justify-between p-4 bg-primary h-16">
				<h2 className="font-bold text-2xl text-white">
					Category Management
				</h2>
				<Button
					className="bg-white text-black hover:bg-gray-300"
					onClick={() => handleOpenCreateForm()}>
					{" "}
					<PlusCircle /> Add new category
				</Button>
			</div>
			<div className="px-4 py-2">
				<div className="flex items-center gap-4">
					<Input />
					<Button className="lg:w-28">
						{" "}
						<Search />{" "}
					</Button>
				</div>
				<div className="mt-4 rounded-md border">
					<CustomTable
						data={listCategory}
						columns={columns}
						onRowClick={handleRowClick}
					/>
				</div>
				<PaginationComponent
					page={currentPage}
					totalPages={totalPage}
					onPageChange={handlePageChange}
				/>
				{isOpenForm && (
					<FormCategory
						data={selectedCategory}
						onClose={handleCloseForm}
						onSave={handleSaveCategory}
						onDelete={handleDeleteCategory}
					/>
				)}
			</div>
		</>
	);
}

export default AdminCategory;
