import { Link } from "react-router-dom";
import { LuHeart, LuShoppingCart } from "react-icons/lu";

interface ProductProps {
	product: {
		id: number;
		name: string;
		price: number;
		discount?: number;
		image: string;
		category?: string;
		description?: string;
	};
}

export default function ProductBestSeller({ product }: ProductProps) {
	return (
		<div className="group bg-white rounded-lg overflow-hidden border border-gray-150 hover:shadow-lg transition-all duration-300">
			{/* Product Info */}
			<div className="p-4">
				{product.category && (
					<span className="text-sm text-gradient-red-orange font-medium">
						{product.category}
					</span>
				)}
				<Link to={`/products/${product.id}`}>
					<h3 className="text-lg font-semibold text-gray-900 mt-2 hover:text-red-600 transition-colors">
						{product.name}
					</h3>
				</Link>
				{product.description && (
					<p className="text-gray-600 text-sm mt-2 line-clamp-2">
						{product.description}
					</p>
				)}

				{/* Price and Add to Cart */}
				<div className="mt-4 flex items-center justify-between">
					<div className="flex items-center gap-2">
						<span className="text-xl font-bold text-red-600">
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
				</div>
			</div>

			{/* Product Image Container */}
			<div className="relative overflow-hidden">
				<div className="flex items-center justify-center h-[250px]">
					<img
						src={product.image}
						alt={product.name}
						className="object-cover transition-transform duration-300 group-hover:scale-105"
					/>
				</div>
				{product.discount && (
					<div className="absolute top-4 right-4 bg-red-600  text-white px-3 py-1 rounded-full text-sm font-medium">
						-{product.discount}%
					</div>
				)}
				{/* Quick Add to Cart Button */}

				<div className="absolute bottom-4 left-1/2 -translate-x-1/2 flex items-center gap-3 opacity-0 group-hover:opacity-100 transition-opacity duration-300">
					<button className="hover:bg-red-100 py-2 px-2 rounded-[50%]">
						<LuHeart />
					</button>
					<button className=" bg-white text-red-600 px-4 py-2 rounded-full shadow-md hover:bg-red-600 hover:text-white">
						<LuShoppingCart className="w-5 h-5" />
					</button>
					{/* <button className="bg-red-600 border   text-white rounded-md px-3 py-2">
						<Link
							to={`/products/${product.id}`}
							className="text-sm font-medium hover:scale-110">
							View Details
						</Link>
					</button> */}
				</div>
			</div>
		</div>
	);
}
