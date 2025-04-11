// import { Loader } from "@chakra-ui/react";
import { LuSearch, LuShoppingCart, LuUser } from "react-icons/lu";
import { Outlet, Link } from "react-router-dom";

function MainLayout() {
	return (
		<div className="min-h-screen flex flex-col bg-white">
			{/* Header */}
			<header className="bg-white border-b border-gray-100 sticky top-0 z-50">
				<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
					<div className="flex justify-between items-center h-20">
						{/* Logo */}
						<Link to="/" className="flex items-center">
							<span className="text-3xl font-bold text-gradient-red-orange">
								KeyCraft
							</span>
						</Link>

						{/* Navigation */}
						<nav className="hidden md:flex space-x-8">
							<Link
								to="/"
								className="text-gray-700 hover:text-red-600 px-3 py-2 rounded-md text-sm font-medium transition-colors">
								Home
							</Link>
							<Link
								to="/products"
								className="text-gray-700 hover:text-red-600 px-3 py-2 rounded-md text-sm font-medium transition-colors">
								Keyboards
							</Link>
							<Link
								to="/categories"
								className="text-gray-700 hover:text-red-600 px-3 py-2 rounded-md text-sm font-medium transition-colors">
								Categories
							</Link>
							<Link
								to="/about"
								className="text-gray-700 hover:text-red-600 px-3 py-2 rounded-md text-sm font-medium transition-colors">
								About
							</Link>
						</nav>

						{/* Right side icons */}
						<div className="flex items-center space-x-6">
							<button className="p-2 text-gray-700 hover:text-red-600 rounded-full hover:bg-gray-50 transition-colors">
								<LuSearch size={20} />
							</button>
							<Link
								to="/cart"
								className="p-2 text-gray-700 hover:text-red-600 rounded-full hover:bg-gray-50 transition-colors relative">
								<LuShoppingCart size={20} />
								<span className="absolute -top-1 -right-1 bg-red-600 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
									0
								</span>
							</Link>
							<Link
								to="/account"
								className="p-2 text-gray-700 hover:text-red-600 rounded-full hover:bg-gray-50 transition-colors">
								<LuUser size={20} />
							</Link>
						</div>
					</div>
				</div>
			</header>

			{/* Main Content */}
			<main className="flex min-w-full items-center justify-center ">
				<Outlet />
			</main>

			{/* Footer */}
			<footer className="bg-white border-t border-gray-100">
				<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
					<div className="grid grid-cols-1 md:grid-cols-4 gap-8">
						{/* Company Info */}
						<div>
							<h3 className="text-xl font-bold text-red-600 mb-4">
								KeyCraft
							</h3>
							<p className="text-gray-600 text-sm">
								Your premium destination for mechanical
								keyboards. Quality craftsmanship, exceptional
								typing experience.
							</p>
						</div>

						{/* Quick Links */}
						<div>
							<h3 className="text-lg font-semibold text-gray-900 mb-4">
								Quick Links
							</h3>
							<ul className="space-y-2">
								<li>
									<Link
										to="/"
										className="text-gray-600 hover:text-red-600 transition-colors">
										Home
									</Link>
								</li>
								<li>
									<Link
										to="/products"
										className="text-gray-600 hover:text-red-600 transition-colors">
										Keyboards
									</Link>
								</li>
								<li>
									<Link
										to="/categories"
										className="text-gray-600 hover:text-red-600 transition-colors">
										Categories
									</Link>
								</li>
								<li>
									<Link
										to="/about"
										className="text-gray-600 hover:text-red-600 transition-colors">
										About Us
									</Link>
								</li>
							</ul>
						</div>

						{/* Customer Service */}
						<div>
							<h3 className="text-lg font-semibold text-gray-900 mb-4">
								Customer Service
							</h3>
							<ul className="space-y-2">
								<li>
									<Link
										to="/contact"
										className="text-gray-600 hover:text-red-600 transition-colors">
										Contact Us
									</Link>
								</li>
								<li>
									<Link
										to="/faq"
										className="text-gray-600 hover:text-red-600 transition-colors">
										FAQ
									</Link>
								</li>
								<li>
									<Link
										to="/shipping"
										className="text-gray-600 hover:text-red-600 transition-colors">
										Shipping Info
									</Link>
								</li>
								<li>
									<Link
										to="/returns"
										className="text-gray-600 hover:text-red-600 transition-colors">
										Returns
									</Link>
								</li>
							</ul>
						</div>

						{/* Newsletter */}
						<div>
							<h3 className="text-lg font-semibold text-gray-900 mb-4">
								Newsletter
							</h3>
							<p className="text-gray-600 text-sm mb-4">
								Subscribe for keyboard news and exclusive
								offers.
							</p>
							<div className="flex">
								<input
									type="email"
									placeholder="Your email"
									className="px-4 py-2 rounded-l-md text-gray-900 border border-gray-200 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
								/>
								<button className="bg-red-600 hover:bg-red-700 px-4 py-2 rounded-r-md text-white transition-colors">
									Subscribe
								</button>
							</div>
						</div>
					</div>

					{/* Bottom Bar */}
					<div className="border-t border-gray-100 mt-12 pt-8 text-center text-gray-600 text-sm">
						<p>
							&copy; {new Date().getFullYear()} KeyCraft. All
							rights reserved.
						</p>
					</div>
				</div>
			</footer>
		</div>
	);
}

export default MainLayout;
