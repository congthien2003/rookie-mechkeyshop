import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { X } from "lucide-react";
import { Category } from "@/interfaces/models/Category";

interface FormCategoryProps {
	data: Category | undefined;
	onClose: () => void;
	onSave: (data: Category) => void;
	onDelete: (id: string) => void;
}

function FormCategory({ data, onClose, onSave, onDelete }: FormCategoryProps) {
	const newCategory: Category = {
		id: "",
		name: "",
		isDeleted: false,
	};

	const isEdit = data ? true : false;
	const [formData, setFormData] = useState<Category>(data ?? newCategory);

	const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const { name, value } = e.target;
		console.log({ name, value });
		setFormData((prev) => ({
			...prev,
			[name]: value,
		}));
	};

	const handleSubmit = (e: React.FormEvent) => {
		e.preventDefault();
		if (formData) {
			onSave(formData);
		}
	};

	return (
		<div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
			<div className="bg-white rounded-lg shadow-lg w-full max-w-2xl">
				<div className="flex items-center justify-between p-4 border-b">
					<h2 className="text-xl font-semibold">
						{isEdit ? "Edit" : "Create"} Category
					</h2>
					<button
						onClick={onClose}
						className="text-gray-500 hover:text-gray-700">
						<X className="h-5 w-5" />
					</button>
				</div>
				<form onSubmit={handleSubmit} className="p-6 space-y-6">
					<div className="grid grid-cols-1 gap-6">
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
					</div>
					<div className="flex justify-end space-x-4 pt-4 border-t">
						<Button
							variant="outline"
							onClick={onClose}
							type="button">
							Cancel
						</Button>
						{isEdit && (
							<Button
								type="button"
								className="bg-red-600 hover:bg-red-500"
								onClick={() => onDelete(formData.id)}>
								Delete
							</Button>
						)}

						<Button type="submit">
							{isEdit ? "Save Changes" : "Create"}
						</Button>
					</div>
				</form>
			</div>
		</div>
	);
}

export default FormCategory;
