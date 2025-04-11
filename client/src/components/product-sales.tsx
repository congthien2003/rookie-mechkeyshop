import { Link } from "react-router-dom";

interface ProductSalesProps {
	product: {
		id: number;
		name: string;
		description: string;
		price: number;
		discount?: number;
		image: string;
		category?: string;
	};
	variant?: "default" | "compact";
}

export default function ProductSales({
	product,
	variant = "default",
}: ProductSalesProps) {
	const isCompact = variant === "compact";

	return (
		<div
			className={`bg-white rounded-lg overflow-hidden border border-gray-100 hover:shadow-md transition-shadow ${
				isCompact ? "max-w-2xl" : "w-full"
			}`}>
			<div className="grid grid-cols-1 md:grid-cols-2 gap-6 p-6">
				{/* Product Info */}
				<div className="flex flex-col justify-between">
					<div>
						{product.category && (
							<span className="text-sm text-red-600 font-medium">
								{product.category}
							</span>
						)}
						<h3 className="text-xl font-bold text-gray-900 mt-2">
							{product.name}
						</h3>
						<p className="text-gray-600 mt-2 line-clamp-3">
							{product.description}
						</p>
					</div>

					<div className="mt-4">
						<div className="flex items-center gap-2 ">
							<span className="text-2xl font-bold text-red-600">
								${product.price}
							</span>
							{product.discount && (
								<span className="text-sm text-gray-500 line-through">
									$
									{(
										product.price *
										(1 + product.discount / 100)
									).toFixed(2)}
								</span>
							)}
						</div>
						<Link
							to={`/products/${product.id}`}
							className="mt-4 inline-block bg-red-600 text-white px-6 py-2 rounded-md font-medium hover:bg-red-700 transition-colors">
							View Details
						</Link>
					</div>
				</div>

				{/* Product Image */}
				<div className="relative">
					<img
						src={product.image}
						alt={product.name}
						className="w-full h-full object-cover rounded-lg"
					/>
					{product.discount && (
						<div className="absolute top-4 right-4 bg-red-600 text-white px-3 py-1 rounded-full text-sm font-medium">
							-{product.discount}%
						</div>
					)}
				</div>
			</div>
		</div>
	);
}
