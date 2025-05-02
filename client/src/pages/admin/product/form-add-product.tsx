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
import { Formik, Form, Field, ErrorMessage } from "formik";
import * as Yup from "yup";

interface FormProductProps {
	onClose: () => void;
	onSave: (data: CreateProduct) => void;
}

const validationSchema = Yup.object({
	name: Yup.string().required("Name is required"),
	description: Yup.string(),
	price: Yup.number()
		.required("Price is required")
		.min(1, "Price must be greater than 0"),
	categoryId: Yup.string().required("Category is required"),
	imageData: Yup.string().required("Image is required"),
	variants: Yup.array().of(
		Yup.object().shape({
			name: Yup.string().required("Variant name is required"),
			value: Yup.array().of(
				Yup.string().required("Variant value is required")
			),
		})
	),
});

function FormAddProduct({ onClose, onSave }: FormProductProps) {
	const initFormData = {
		name: "",
		description: "",
		price: 0,
		categoryId: "",
		variants: [{ name: "", value: [""] }],
		imageData: "",
	};

	const [listCategory, setListCategory] = useState<Category[]>([]);
	const [preview, setPreview] = useState<string>("");

	const fetchListCategory = async () => {
		const response: ResultPagination<Category> =
			await categoryService.getList(1, 50, "", true);
		if (response.isSuccess) {
			setListCategory(response.data.items);
		}
	};

	useEffect(() => {
		fetchListCategory();
	}, []);

	const handleFileChange = async (
		e: React.ChangeEvent<HTMLInputElement>,
		setFieldValue: (field: string, value: any) => void
	) => {
		const file = e.target.files?.[0];
		if (!file) return;

		const reader = new FileReader();
		reader.onloadend = async () => {
			const base64 = reader.result?.toString() ?? "";
			setPreview(base64);
			setFieldValue("imageData", base64);
		};
		reader.readAsDataURL(file);
	};

	const handleSubmit = (values: CreateProduct) => {
		onSave(values);
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
				<Formik
					initialValues={initFormData}
					validationSchema={validationSchema}
					onSubmit={handleSubmit}>
					{({ values, setFieldValue, errors, touched }) => (
						<Form className="p-6 space-y-6">
							<div className="grid grid-cols-3 gap-6">
								<div className="space-y-2">
									<Label htmlFor="name">Name</Label>
									<Field
										as={Input}
										id="name"
										name="name"
										placeholder="Enter name"
									/>
									<ErrorMessage
										name="name"
										component="div"
										className="text-red-500 text-sm"
									/>
								</div>
								<div className="space-y-2">
									<Label htmlFor="price">Price</Label>
									<Field
										as={Input}
										id="price"
										name="price"
										type="number"
										placeholder="Enter price"
									/>
									<ErrorMessage
										name="price"
										component="div"
										className="text-red-500 text-sm"
									/>
								</div>
								<div className="space-y-2">
									<Label htmlFor="categoryId">Category</Label>
									<Field
										as="select"
										name="categoryId"
										className="p-2 px-3 bg-slate-200 rounded-lg text-black block w-full">
										<option value="">
											Select a category
										</option>
										{listCategory.map((category) => (
											<option
												key={category.id}
												value={category.id}
												className="p-1 text-black">
												{category.name}
											</option>
										))}
									</Field>
									<ErrorMessage
										name="categoryId"
										component="div"
										className="text-red-500 text-sm"
									/>
								</div>
								<div className="col-span-3 space-y-2">
									<Label htmlFor="description">
										Description
									</Label>
									<Field
										as={Textarea}
										id="description"
										name="description"
										placeholder="Enter description"
										rows={4}
									/>
									<ErrorMessage
										name="description"
										component="div"
										className="text-red-500 text-sm"
									/>
								</div>
							</div>

							{/* Variants section */}
							<div className="space-y-4">
								<div className="flex justify-between items-center">
									<Label className="text-lg">Variants</Label>
									<Button
										type="button"
										onClick={() => {
											const currentVariants =
												values.variants;
											setFieldValue("variants", [
												...currentVariants,
												{ name: "", value: [""] },
											]);
										}}
										variant="outline">
										<Plus className="h-4 w-4 mr-2" />
										Add Variant
									</Button>
								</div>
								{values.variants.map(
									(variant, variantIndex) => (
										<div
											key={variantIndex}
											className="border p-4 rounded-lg space-y-4">
											<div className="flex justify-between items-center">
												<Field
													as={Input}
													name={`variants.${variantIndex}.name`}
													placeholder="Variant name (e.g., Color)"
													className="w-1/3"
												/>
												<div className="flex gap-2">
													<Button
														type="button"
														onClick={() => {
															const currentValues =
																values.variants[
																	variantIndex
																].value;
															setFieldValue(
																`variants.${variantIndex}.value`,
																[
																	...currentValues,
																	"",
																]
															);
														}}
														variant="outline"
														size="sm">
														<Plus className="h-4 w-4" />
													</Button>
													<Button
														type="button"
														onClick={() => {
															const newVariants =
																values.variants.filter(
																	(_, i) =>
																		i !==
																		variantIndex
																);
															setFieldValue(
																"variants",
																newVariants
															);
														}}
														variant="destructive"
														size="sm">
														<Trash className="h-4 w-4" />
													</Button>
												</div>
											</div>
											<div className="grid grid-cols-2 gap-4">
												{variant.value.map(
													(_, valueIndex) => (
														<div
															key={valueIndex}
															className="flex gap-2">
															<Field
																as={Input}
																name={`variants.${variantIndex}.value.${valueIndex}`}
																placeholder="Value (e.g., Red)"
															/>
															<Button
																type="button"
																onClick={() => {
																	const newValues =
																		values.variants[
																			variantIndex
																		].value.filter(
																			(
																				_,
																				i
																			) =>
																				i !==
																				valueIndex
																		);
																	setFieldValue(
																		`variants.${variantIndex}.value`,
																		newValues
																	);
																}}
																variant="destructive"
																size="sm">
																<Trash className="h-4 w-4" />
															</Button>
														</div>
													)
												)}
											</div>
										</div>
									)
								)}
							</div>

							{/* Image section */}
							<div className="space-y-4">
								<Label className="text-lg">Image</Label>
								<div className="flex gap-4 items-center flex-col">
									<input
										type="file"
										accept="image/*"
										onChange={(e) =>
											handleFileChange(e, setFieldValue)
										}
										className="outline-none font-medium"
									/>
									{preview && (
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
									)}
									<ErrorMessage
										name="imageData"
										component="div"
										className="text-red-500 text-sm"
									/>
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
						</Form>
					)}
				</Formik>
			</div>
		</div>
	);
}

export default FormAddProduct;
