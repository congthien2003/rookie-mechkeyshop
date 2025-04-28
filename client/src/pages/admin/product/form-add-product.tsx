import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { X, Plus, Trash } from "lucide-react";
import { CreateProduct } from "@/interfaces/models/Product";
import { Variant } from "@/interfaces/models/Variant";
import { ResultPagination } from "@/interfaces/common/ResultPagination";
import { Category } from "@/interfaces/models/Category";
import { categoryService } from "@/services/apiCategory";

interface FormProductProps {
	onClose: () => void;
	onSave: (data: CreateProduct) => void;
}

function FormAddProduct({ onClose, onSave }: FormProductProps) {
	const newProduct: CreateProduct = {
		name: "",
		description: "",
		price: 0,
		categoryId: "",
		variants: [],
		imageData: "",
	};

	const [formData, setFormData] = useState<CreateProduct>(newProduct);
	const [variants, setVariants] = useState<Variant[]>([
		{ name: "", value: [] },
	]);
	const [listCategory, setListCategory] = useState<Category[]>([]);

	const fetchListCategory = async () => {
		const response: ResultPagination<Category> =
			await categoryService.getList(1, 50, "", true);
		if (response.isSuccess) {
			setListCategory(response.data.items);
			console.log(response.data.items);
		}
	};

	useEffect(() => {
		fetchListCategory();
	}, []);

	const [preview, setPreview] = useState<string>("");

	// eslint-disable-next-line @typescript-eslint/no-explicit-any
	const handleFileChange = async (e: any) => {
		const file = e.target.files[0];
		if (!file) return;

		const reader = new FileReader();
		reader.onloadend = async () => {
			const base64 = await reader.result;
			setPreview(base64?.toString() ?? ""); // Hiển thị ảnh preview nếu cần
			setFormData((prev) => ({
				...prev,
				imageData: base64?.toString() ?? "",
			}));
			console.log(formData);
		};
		reader.readAsDataURL(file); // chuyển file thành base64 string
	};

	const handleChange = (
		e: React.ChangeEvent<
			HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
		>
	) => {
		const { name, value } = e.target;
		setFormData((prev) => ({
			...prev,
			[name]: value,
		}));
	};

	const handleVariantNameChange = (index: number, value: string) => {
		const newVariants = [...variants];
		newVariants[index].name = value;
		setVariants(newVariants);
	};

	const handleVariantValueChange = (
		variantIndex: number,
		valueIndex: number,
		value: string
	) => {
		const newVariants = [...variants];
		newVariants[variantIndex].value[valueIndex] = value;
		setVariants(newVariants);
	};

	const addVariant = () => {
		setVariants([...variants, { name: "", value: [""] }]);
	};

	const addVariantValue = (index: number) => {
		const newVariants = [...variants];
		newVariants[index].value.push("");
		setVariants(newVariants);
	};

	const removeVariant = (index: number) => {
		const newVariants = variants.filter((_, i) => i !== index);
		setVariants(newVariants);
	};

	const removeVariantValue = (variantIndex: number, valueIndex: number) => {
		const newVariants = [...variants];
		newVariants[variantIndex].value = newVariants[
			variantIndex
		].value.filter((_, i) => i !== valueIndex);
		setVariants(newVariants);
	};

	const handleSubmit = (e: React.FormEvent) => {
		e.preventDefault();
		const productVariants: Variant[] = variants.map((variant) => ({
			name: variant.name,
			value: variant.value,
		}));

		const productToSave: CreateProduct = {
			...formData,
			variants: productVariants,
		};

		onSave(productToSave);
	};

	return (
		<div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
			<div className="bg-white rounded-lg shadow-lg w-full max-w-4xl max-h-[90vh] overflow-y-auto">
				<div className="flex items-center justify-between p-4 border-b sticky top-0 bg-white">
					<h2 className="text-xl font-semibold">Create Product</h2>
					<button
						onClick={onClose}
						className="text-gray-500 hover:text-gray-700">
						<X className="h-5 w-5" />
					</button>
				</div>
				<form onSubmit={handleSubmit} className="p-6 space-y-6">
					<div className="grid grid-cols-3 gap-6">
						<div className="space-y-2">
							<Label htmlFor="name">Name</Label>
							<Input
								id="name"
								name="name"
								value={formData.name}
								onChange={handleChange}
								placeholder="Enter name"
							/>
						</div>
						<div className="space-y-2">
							<Label htmlFor="price">Price</Label>
							<Input
								id="price"
								name="price"
								type="number"
								value={formData.price}
								onChange={handleChange}
								placeholder="Enter price"
							/>
						</div>
						<div className="space-y-2">
							<Label htmlFor="categoryId">Category</Label>
							<select
								name="categoryId"
								className="p-2 px-3 bg-slate-200 rounded-lg text-black block"
								onChange={(e) => handleChange(e)}>
								{listCategory.map((category) => {
									return (
										<option
											value={category.id}
											selected={
												category.id ===
												formData.categoryId
											}
											className="p-1 text-black">
											{category.name}
										</option>
									);
								})}
							</select>
						</div>
						<div className="col-span-3 space-y-2">
							<Label htmlFor="description">Description</Label>
							<Textarea
								id="description"
								name="description"
								value={formData.description}
								onChange={handleChange}
								placeholder="Enter description"
								rows={4}></Textarea>
						</div>
					</div>

					{/* Variants section */}
					<div className="space-y-4">
						<div className="flex justify-between items-center">
							<Label className="text-lg">Variants</Label>
							<Button
								type="button"
								onClick={addVariant}
								variant="outline">
								<Plus className="h-4 w-4 mr-2" />
								Add Variant
							</Button>
						</div>
						{variants.map((variant, variantIndex) => (
							<div
								key={variantIndex}
								className="border p-4 rounded-lg space-y-4">
								<div className="flex justify-between items-center">
									<Input
										value={variant.name}
										onChange={(e) =>
											handleVariantNameChange(
												variantIndex,
												e.target.value
											)
										}
										placeholder="Variant name (e.g., Color)"
										className="w-1/3"
									/>
									<div className="flex gap-2">
										<Button
											type="button"
											onClick={() =>
												addVariantValue(variantIndex)
											}
											variant="outline"
											size="sm">
											<Plus className="h-4 w-4" />
										</Button>
										<Button
											type="button"
											onClick={() =>
												removeVariant(variantIndex)
											}
											variant="destructive"
											size="sm">
											<Trash className="h-4 w-4" />
										</Button>
									</div>
								</div>
								<div className="grid grid-cols-2 gap-4">
									{variant.value.map((value, valueIndex) => (
										<div
											key={valueIndex}
											className="flex gap-2">
											<Input
												value={value}
												onChange={(e) =>
													handleVariantValueChange(
														variantIndex,
														valueIndex,
														e.target.value
													)
												}
												placeholder="Value (e.g., Red)"
											/>
											<Button
												type="button"
												onClick={() =>
													removeVariantValue(
														variantIndex,
														valueIndex
													)
												}
												variant="destructive"
												size="sm">
												<Trash className="h-4 w-4" />
											</Button>
										</div>
									))}
								</div>
							</div>
						))}
					</div>

					{/* Image section */}
					<div className="space-y-4">
						<Label className="text-lg">Image</Label>
						<div className="flex gap-4 items-center flex-col">
							<input
								type="file"
								accept="image/*"
								onChange={handleFileChange}
								className="outline-none font-medium"
							/>
							{preview && (
								<>
									<div>
										<h5 className="font-bold text-center">
											Live Preview
										</h5>
										<img
											src={preview}
											alt="Preview"
											style={{ width: 200 }}
										/>
									</div>
								</>
							)}
						</div>
					</div>

					<div className="flex justify-end space-x-4 pt-4 border-t">
						<Button
							variant="outline"
							onClick={onClose}
							type="button">
							Cancel
						</Button>

						<Button type="submit">Create</Button>
					</div>
				</form>
			</div>
		</div>
	);
}

export default FormAddProduct;
