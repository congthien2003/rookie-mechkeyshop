import ApplicationUser from "@/interfaces/models/ApplicationUser";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { X } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
} from "@/components/ui/select";

interface FormUserProps {
	data: ApplicationUser;
	onClose: () => void;
	onSave: (data: ApplicationUser) => void;
	onDelete: (id: string) => void;
}

function FormUser({ data, onClose, onSave, onDelete }: FormUserProps) {
	const [formData, setFormData] = useState<ApplicationUser>(data);
	console.log(data);
	const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const { name, value } = e.target;
		setFormData((prev) => ({
			...prev,
			[name]: value,
		}));
	};

	const handleSelectChange = (name: string, value: string) => {
		setFormData((prev) => ({
			...prev,
			[name]: value,
		}));
	};

	const handleSubmit = (e: React.FormEvent) => {
		e.preventDefault();
		onSave(formData);
	};

	return (
		<div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
			<div className="bg-white rounded-lg shadow-lg w-full max-w-2xl">
				<div className="flex items-center justify-between p-4 border-b">
					<h2 className="text-xl font-semibold">Edit User</h2>
					<button
						onClick={onClose}
						className="text-gray-500 hover:text-gray-700">
						<X className="h-5 w-5" />
					</button>
				</div>
				<form onSubmit={handleSubmit} className="p-6 space-y-6">
					<div className="grid grid-cols-2 gap-6">
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
							<Label htmlFor="email">Email</Label>
							<Input
								id="email"
								name="email"
								type="email"
								value={formData.email}
								onChange={handleChange}
								placeholder="Enter email"
							/>
						</div>
						<div className="space-y-2">
							<Label htmlFor="phones">Phone</Label>
							<Input
								id="phones"
								name="phones"
								value={formData.phones}
								onChange={handleChange}
								placeholder="Enter phone number"
							/>
						</div>
						<div className="space-y-2">
							<Label htmlFor="address">Address</Label>
							<Input
								id="address"
								name="address"
								value={formData.address}
								onChange={handleChange}
								placeholder="Enter address"
							/>
						</div>
						<div className="space-y-2">
							<Label className="mr-2">Email Status:</Label>
							<Badge
								variant={
									formData.isEmailConfirmed
										? "default"
										: "destructive"
								}>
								{formData.isEmailConfirmed
									? "Confirmed"
									: "Unconfirmed"}
							</Badge>
						</div>
						<div className="space-y-2">
							<Label className="mr-2">Account Status:</Label>
							<Badge
								variant={
									formData.isDeleted
										? "destructive"
										: "default"
								}>
								{formData.isDeleted ? "Deleted" : "Active"}
							</Badge>
						</div>
						<div className="space-y-2">
							<Label htmlFor="roleId">Role</Label>
							<Select
								value={formData.roleId.toString()}
								onValueChange={(value) =>
									handleSelectChange("roleId", value)
								}>
								<SelectTrigger>
									<SelectValue placeholder="Select role" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="1">Admin</SelectItem>
									<SelectItem value="2">User</SelectItem>
								</SelectContent>
							</Select>
						</div>
					</div>
					<div className="grid grid-cols-2 gap-6 pt-4 border-t">
						<div className="space-y-2">
							<Label>Created At</Label>
							<div className="text-sm text-gray-500">
								{new Date(formData.createdAt).toLocaleString()}
							</div>
						</div>
						<div className="space-y-2">
							<Label>Last Updated</Label>
							<div className="text-sm text-gray-500">
								{new Date(
									formData.lastUpdated
								).toLocaleString()}
							</div>
						</div>
					</div>
					<div className="flex justify-end space-x-4 pt-4 border-t">
						<Button
							variant="outline"
							onClick={onClose}
							type="button">
							Cancel
						</Button>
						<Button
							type="button"
							className="bg-red-600 hover:bg-red-500"
							onClick={() => onDelete(formData.id)}>
							Delete
						</Button>
						<Button type="submit">Save Changes</Button>
					</div>
				</form>
			</div>
		</div>
	);
}

export default FormUser;
