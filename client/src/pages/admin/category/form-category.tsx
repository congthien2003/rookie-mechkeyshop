import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { X } from "lucide-react";
import { Category } from "@/interfaces/models/Category";
import { Formik, Form, Field, ErrorMessage } from "formik";
import * as Yup from "yup";
import { Input } from "@/components/ui/input";
interface FormCategoryProps {
	data: Category | undefined;
	onClose: () => void;
	onSave: (data: Category) => void;
	onDelete: (id: string) => void;
}

const validationSchema = Yup.object({
	name: Yup.string().required("Name is required"),
});

function FormCategory({ data, onClose, onSave, onDelete }: FormCategoryProps) {
	const initValueForm: Category = {
		id: "",
		name: "",
		isDeleted: false,
	};
	if (data) {
		initValueForm.id = data.id;
		initValueForm.name = data.name;
		initValueForm.isDeleted = data.isDeleted;
	}

	console.log(data);

	const isEdit = data ? true : false;

	const handleSubmit = (values: Category) => {
		console.log(values);
		onSave(values);
	};

	return (
		<div className="fixed inset-0 bg-black/50 flex items-center justify-center z-40">
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
				<Formik
					initialValues={initValueForm}
					validationSchema={validationSchema}
					onSubmit={(value) => {
						handleSubmit(value);
					}}>
					<Form className="p-6 space-y-6">
						<div className="grid grid-cols-1 gap-6">
							<div className="space-y-2">
								<Label htmlFor="name">Name</Label>
								<Field
									as={Input}
									name="name"
									type="text"
									placeholder="Input category name"
									className="block border-[1px] w-full rounded-md p-2"
								/>
								<ErrorMessage
									name="name"
									component="div"
									className="text-red-400"
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
									onClick={() => onDelete(initValueForm.id)}>
									Delete
								</Button>
							)}

							<Button type="submit">
								{isEdit ? "Save Changes" : "Create"}
							</Button>
						</div>
					</Form>
				</Formik>
			</div>
		</div>
	);
}

export default FormCategory;
