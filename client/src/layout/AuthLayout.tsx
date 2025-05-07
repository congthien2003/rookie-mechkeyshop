import { Outlet } from "react-router-dom";
import { Keyboard } from "lucide-react";

const AuthLayout = () => {
	return (
		<div className="min-h-screen bg-gradient-to-br from-gray-900 via-gray-800 to-black">
			{/* Decorative elements */}
			<div className="absolute inset-0 overflow-hidden">
				<div className="absolute -top-40 -right-40 w-80 h-80 bg-primary/20 rounded-full blur-3xl" />
				<div className="absolute -bottom-40 -left-40 w-80 h-80 bg-primary/20 rounded-full blur-3xl" />
			</div>

			{/* Header */}
			<header className="fixed top-0 left-0 right-0 p-4">
				<div className="flex items-center gap-2 text-white">
					<Keyboard className="h-6 w-6 text-primary" />
					<span className="text-xl font-bold">MechKey</span>
				</div>
			</header>

			{/* Main content */}
			<main className="relative min-h-screen flex items-center justify-center p-4">
				<Outlet />
			</main>

			{/* Footer */}
			<footer className="fixed bottom-0 left-0 right-0 p-4 text-center text-gray-500 text-sm">
				© {new Date().getFullYear()} MechKey. All rights reserved.
			</footer>
		</div>
	);
};

export default AuthLayout;
