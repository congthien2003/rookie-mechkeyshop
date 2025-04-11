import Product from "@/components/product";
import ProductBestSeller from "@/components/product-best-seller";
import ProductSales from "@/components/product-sales";
import { useState, useEffect } from "react";
import { LuTruck, LuCreditCard, LuHeadphones, LuStar } from "react-icons/lu";

// Sample images - replace with your actual images
const sliderImages = [
	"https://placehold.co/600x600",
	"https://placehold.co/600x600",
	"https://placehold.co/600x600",
];

// Sample flash sale products - replace with your actual data
const flashSaleProducts = [
	{
		id: 1,
		name: "Mechanical Keyboard X1",
		description: "Description 1",
		price: 99.99,
		discount: 20,
		image: "https://demo.nextmerce.com/_next/image?url=https%3A%2F%2Fcdn.sanity.io%2Fimages%2Frpq7htxl%2Fproduction%2Fd5e6cc8a016057b6ef12174fd9fac9c64f1a3263-175x213.png&w=256&q=75",
	},
	{
		id: 2,
		name: "RGB Gaming Keyboard",
		description: "Description 2",
		price: 149.99,
		discount: 15,
		image: "https://demo.nextmerce.com/_next/image?url=https%3A%2F%2Fcdn.sanity.io%2Fimages%2Frpq7htxl%2Fproduction%2Fd5e6cc8a016057b6ef12174fd9fac9c64f1a3263-175x213.png&w=256&q=75",
	},
];

function HomePage() {
	const [currentSlide, setCurrentSlide] = useState(0);

	useEffect(() => {
		const timer = setInterval(() => {
			setCurrentSlide((prev) => (prev + 1) % sliderImages.length);
		}, 5000);

		return () => clearInterval(timer);
	}, []);

	return (
		<>
			<div className="w-screen flex flex-col items-center">
				<div className="container pt-[40px] pb-[80px]">
					{/* Hero Section with Slider and Flash Sale */}
					<section className="grid grid-cols-1 lg:grid-cols-3 gap-6 ">
						{/* Slider Section */}
						<div className="lg:col-span-2 h-[100%] relative overflow-hidden rounded-lg">
							{sliderImages.map((image, index) => (
								<div
									key={index}
									className={`absolute inset-0 transition-opacity duration-500 ${
										index === currentSlide
											? "opacity-100"
											: "opacity-0"
									}`}>
									<img
										src={image}
										alt={`Slide ${index + 1}`}
										className="w-full h-full object-cover"
									/>
								</div>
							))}
							{/* Slider Dots */}
							<div className="absolute bottom-4 left-0 right-0 flex justify-center space-x-2">
								{sliderImages.map((_, index) => (
									<button
										key={index}
										onClick={() => setCurrentSlide(index)}
										className={`w-2 h-2 rounded-full transition-colors ${
											index === currentSlide
												? "bg-red-600"
												: "bg-gray-300"
										}`}
									/>
								))}
							</div>
						</div>

						{/* Flash Sale Section */}
						<div className="space-y-4">
							<h2 className="text-2xl font-bold text-red-600">
								Flash Sale
							</h2>
							{flashSaleProducts.map((product) => (
								<ProductSales product={product} />
							))}
						</div>
					</section>
					{/* Services Section */}
					<section className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mt-4">
						{/* Shipping */}
						<div className="bg-white p-6 rounded-lg border border-gray-100 flex items-center space-x-4">
							<div className="bg-red-50 p-3 rounded-full">
								<LuTruck className="w-6 h-6 text-red-600" />
							</div>
							<div>
								<h3 className="font-semibold text-gray-900">
									Free Shipping
								</h3>
								<p className="text-sm text-gray-600">
									On all orders over $100
								</p>
							</div>
						</div>

						{/* Payment */}
						<div className="bg-white p-6 rounded-lg border border-gray-100 flex items-center space-x-4">
							<div className="bg-red-50 p-3 rounded-full">
								<LuCreditCard className="w-6 h-6 text-red-600" />
							</div>
							<div>
								<h3 className="font-semibold text-gray-900">
									Secure Payment
								</h3>
								<p className="text-sm text-gray-600">
									100% secure payment
								</p>
							</div>
						</div>

						{/* Services */}
						<div className="bg-white p-6 rounded-lg border border-gray-100 flex items-center space-x-4">
							<div className="bg-red-50 p-3 rounded-full">
								<LuHeadphones className="w-6 h-6 text-red-600" />
							</div>
							<div>
								<h3 className="font-semibold text-gray-900">
									24/7 Support
								</h3>
								<p className="text-sm text-gray-600">
									Dedicated support
								</p>
							</div>
						</div>

						{/* Rating */}
						<div className="bg-white p-6 rounded-lg border border-gray-100 flex items-center space-x-4">
							<div className="bg-red-50 p-3 rounded-full">
								<LuStar className="w-6 h-6 text-red-600" />
							</div>
							<div>
								<h3 className="font-semibold text-gray-900">
									Best Quality
								</h3>
								<p className="text-sm text-gray-600">
									Quality guaranteed
								</p>
							</div>
						</div>
					</section>
				</div>
				<div className="bg-gradient-blue-purple w-full py-[80px] flex items-center justify-center">
					<div className="container">
						<h2 className="font-bold text-center text-3xl text-white">
							BEST SELLER
						</h2>

						<div className="grid grid-cols-1 lg:grid-cols-4 gap-6 mt-8">
							{flashSaleProducts.map((product) => (
								<ProductBestSeller product={product} />
							))}
							{flashSaleProducts.map((product) => (
								<ProductBestSeller product={product} />
							))}
						</div>
					</div>
				</div>
				<div className="w-full py-[80px] flex items-center justify-center">
					<div className="container">
						<h2 className="font-bold text-center text-3xl text-gray-800">
							New Arrivals
						</h2>

						<div className="grid grid-cols-1 lg:grid-cols-4 gap-6 mt-8">
							{flashSaleProducts.map((product) => (
								<Product product={product} />
							))}
							{flashSaleProducts.map((product) => (
								<Product product={product} />
							))}
						</div>
					</div>
				</div>
			</div>
		</>
	);
}

export default HomePage;
