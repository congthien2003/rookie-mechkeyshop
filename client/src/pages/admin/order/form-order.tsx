import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { X } from "lucide-react";
import { Order } from "@/interfaces/models/Order";
import { Badge } from "@/components/ui/badge";
import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
} from "@/components/ui/select";

interface FormOrderProps {
	data: Order;
	onClose: () => void;
	onSave: (data: Order) => void;
	onDelete: (id: string) => void;
}

function FormOrder({ data, onClose, onSave, onDelete }: FormOrderProps) {
	const [formData, setFormData] = useState<Order>(data);

	const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const { name, value } = e.target;
		setFormData((prev) => ({
			...prev,
			[name]: value,
		}));
	};

	const handleStatusChange = (value: string) => {
		setFormData((prev) => ({
			...prev,
			status: parseInt(value),
		}));
	};

	const handleSubmit = (e: React.FormEvent) => {
		e.preventDefault();
		onSave(formData);
	};

	const getStatusBadge = (status: number) => {
		switch (status) {
			case 0:
				return <Badge variant="outline">Pending</Badge>;
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
	};

	return (
		<div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
			<div className="bg-white rounded-lg shadow-lg w-full max-w-4xl max-h-[90vh] overflow-y-auto">
				<div className="flex items-center justify-between p-4 border-b sticky top-0 bg-white">
					<h2 className="text-xl font-semibold">Order Details</h2>
					<button
						onClick={onClose}
						className="text-gray-500 hover:text-gray-700">
						<X className="h-5 w-5" />
					</button>
				</div>
				<form onSubmit={handleSubmit} className="p-6 space-y-6">
					{/* Order Status and Info */}
					<div className="flex gap-4 items-center">
						<div className="bg-blue-100 p-2 rounded-lg h-[50px] flex items-center">
							<span className="font-medium">
								Total Amount: ${formData.totalAmount}
							</span>
						</div>
						<div className="ml-auto">
							{getStatusBadge(formData.status)}
						</div>
					</div>

					{/* Order Details */}
					<div className="grid grid-cols-2 gap-6">
						<div className="space-y-2">
							<Label htmlFor="name">Customer Name</Label>
							<Input
								id="name"
								name="name"
								value={formData.name}
								onChange={handleChange}
								readOnly
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
								readOnly
							/>
						</div>
						<div className="space-y-2">
							<Label htmlFor="phone">Phone</Label>
							<Input
								id="phone"
								name="phone"
								value={formData.phone}
								onChange={handleChange}
								readOnly
							/>
						</div>
						<div className="space-y-2">
							<Label htmlFor="address">Address</Label>
							<Input
								id="address"
								name="address"
								value={formData.address}
								onChange={handleChange}
								readOnly
							/>
						</div>
						<div className="space-y-2">
							<Label>Order Date</Label>
							<div className="text-sm text-gray-500">
								{new Date(formData.orderDate).toLocaleString()}
							</div>
						</div>
						<div className="space-y-2">
							<Label>Order Status</Label>
							<Select
								value={formData.status.toString()}
								onValueChange={handleStatusChange}>
								<SelectTrigger>
									<SelectValue placeholder="Select status" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="0">Pending</SelectItem>
									<SelectItem value="1">Accepted</SelectItem>
									<SelectItem value="2">Cancelled</SelectItem>
									<SelectItem value="3">Completed</SelectItem>
								</SelectContent>
							</Select>
						</div>
					</div>

					{/* Order Items */}
					<div className="space-y-4 border-t pt-4">
						<h3 className="text-lg font-semibold">Order Items</h3>
						<div className="space-y-4">
							{data.orderItems.map((item, index) => (
								<div
									key={index}
									className="flex items-center gap-4 p-4 border rounded-lg">
									<img
										src={item.imageUrl}
										alt={item.productName}
										className="w-20 h-20 object-cover rounded"
									/>
									<div className="flex-1">
										<h4 className="font-medium">
											{item.productName}
										</h4>
										<p className="text-sm text-gray-500">
											{item?.option?.name}
											{item?.option?.value}
										</p>
										<p className="text-sm">
											Quantity: {item.quantity}
										</p>
										<p className="text-sm font-medium">
											Price: ${item.price}
										</p>
									</div>
									<div className="text-right">
										<p className="font-medium">
											${item.price * item.quantity}
										</p>
									</div>
								</div>
							))}
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

export default FormOrder;
