import { useEffect, useState } from "react";
import { productService } from "@/services/apiProduct";
import {
	CreateProduct,
	EditProduct,
	Product,
} from "@/interfaces/models/Product";
import { ResultPagination } from "@/interfaces/common/ResultPagination";
import { ColumnDef } from "@tanstack/react-table";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { PlusCircle, Search } from "lucide-react";
import CustomTable from "@/components/custom-table";
import PaginationComponent from "@/components/app-pagination";
import FormProduct from "./form-product";
import { ToastError, ToastSuccess } from "@/lib/toast";
import FormAddProduct from "./form-add-product";
import { Input } from "@/components/ui/input";
import { Category } from "@/interfaces/models/Category";
import { categoryService } from "@/services/apiCategory";
import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
} from "@/components/ui/select";
import { useLoadingStore } from "@/store/store";

// eslint-disable-next-line react-refresh/only-export-components
export const columns: ColumnDef<Product>[] = [
	{
		accessorKey: "imageUrl",
		header: "Image",
		cell: ({ row }) => {
			return <img width={"70px"} src={row.original.imageUrl} />;
		},
	},
	{
		accessorKey: "name",
		header: "Name",
	},
	{
		accessorKey: "price",
		header: "Price",
		cell: ({ row }) => {
			return `$${row.original.price}`;
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

function AdminProduct() {
	const [listProduct, setListProduct] = useState<Product[]>([]);
	const [listCategory, setListCategory] = useState<Category[]>([]);
	const [currentPage, setCurrentPage] = useState(1);
	const pageSize = 5;
	const [totalPage, setTotalPage] = useState(0);
	const [isOpenEditForm, setIsOpenEditForm] = useState(false);
	const [selectedProduct, setSelectedProduct] = useState<Product>();
	const [isOpenAddForm, setIsOpenAddForm] = useState(false);
	const [selectedCategory, setSelectedCategory] = useState<string>("");
	const [searchTerm, setSearchTerm] = useState("");

	// Zustand Action
	const showLoading = useLoadingStore((state) => state.showLoading);
	const hideLoading = useLoadingStore((state) => state.hideLoading);

	const fetchCategories = async () => {
		try {
			const response = await categoryService.getList(1, 100, "", true);
			if (response.isSuccess) {
				setListCategory(response.data.items);
			}
		} catch (error) {
			console.error("Failed to fetch categories:", error);
		}
	};

	const fetch = async function () {
		showLoading();

		const response: ResultPagination<Product> =
			await productService.getList(
				currentPage,
				pageSize,
				searchTerm,
				selectedCategory,
				"",
				true
			);
		console.log(response);

		if (response.isSuccess) {
			setListProduct(response.data.items);
			setCurrentPage(response.data.page);
			setTotalPage(response.data.totalPages);
		}
		hideLoading();
	};

	const updateProduct = async (data: EditProduct) => {
		console.log("Update product:", data);
		const response = await productService.edit(data);
		if (response.isSuccess) {
			await fetch();
			ToastSuccess("Success");
		} else {
			ToastError("Failed");
		}
	};

	const addProduct = async (data: CreateProduct) => {
		console.log("Add product:", data);
		const response = await productService.create(data);
		if (response.isSuccess) {
			await fetch();
			ToastSuccess("Success");
		} else {
			ToastError("Failed");
		}
	};

	const deleteProduct = async (id: string) => {
		const response = await productService.deleteById(id);
		if (response.isSuccess) {
			await fetch();
			ToastSuccess("Success");
		} else {
			ToastError("Failed");
		}
	};

	useEffect(() => {
		fetchCategories();
	}, []);

	useEffect(() => {
		fetch();
	}, [currentPage, selectedCategory]);

	const handleSearch = () => {
		setCurrentPage(1);
		fetch();
	};

	const handleCategoryChange = (value: string) => {
		setSelectedCategory(value);
		setCurrentPage(1);
	};

	const handlePageChange = (page: number) => {
		setCurrentPage(page);
	};

	const handleRowClick = (data: Product) => {
		setIsOpenEditForm(true);
		setSelectedProduct(data);
	};

	const handleOpenCreateForm = () => {
		setIsOpenAddForm(true);
		setSelectedProduct(undefined);
	};

	const handleCloseForm = () => {
		setIsOpenEditForm(false);
		setIsOpenAddForm(false);
		setSelectedProduct(undefined);
	};

	const handleSaveProduct = async (product: EditProduct) => {
		await updateProduct(product);
		handleCloseForm();
	};

	const handleCreateProduct = async (product: CreateProduct) => {
		await addProduct(product);
		handleCloseForm();
	};

	const handleDeleteProduct = async (id: string) => {
		console.log("Delete product:", id);
		await deleteProduct(id);
		handleCloseForm();
	};

	return (
		<>
			<div className="flex align-center justify-between p-4 bg-primary h-16">
				<h2 className="font-bold text-2xl text-white">
					Product Management
				</h2>
				<Button
					className="bg-white text-black hover:bg-gray-300"
					onClick={() => handleOpenCreateForm()}>
					{" "}
					<PlusCircle /> Add new product
				</Button>
			</div>
			<div className="px-4 py-2">
				<div className="flex items-center gap-4">
					<Input
						value={searchTerm}
						onChange={(e) => setSearchTerm(e.target.value)}
						placeholder="Search products..."
					/>
					<Button className="lg:w-28" onClick={handleSearch}>
						<Search />
					</Button>
					{}{" "}
					<Select
						value={selectedCategory}
						onValueChange={handleCategoryChange}>
						<SelectTrigger className="w-[180px]">
							<SelectValue placeholder="Select category" />
						</SelectTrigger>
						<SelectContent>
							<SelectItem value=" ">All Categories</SelectItem>
							{listCategory.map((category) => (
								<SelectItem
									key={category.id}
									value={category.id}>
									{category.name}
								</SelectItem>
							))}
						</SelectContent>
					</Select>
				</div>
				<div className="mt-4 rounded-md border">
					<CustomTable
						data={listProduct}
						columns={columns}
						onRowClick={handleRowClick}
					/>
				</div>
				<PaginationComponent
					page={currentPage}
					totalPages={totalPage}
					onPageChange={handlePageChange}
				/>
				{isOpenEditForm && selectedProduct && (
					<FormProduct
						data={selectedProduct}
						onClose={handleCloseForm}
						onSave={handleSaveProduct}
						onDelete={handleDeleteProduct}
					/>
				)}
			</div>

			{isOpenAddForm && (
				<FormAddProduct
					onClose={handleCloseForm}
					onSave={handleCreateProduct}
				/>
			)}
		</>
	);
}

export default AdminProduct;
