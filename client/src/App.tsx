import "./App.css";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import MainLayout from "./layout/MainLayout.tsx";
import LoginPage from "./pages/auth/LoginPage.tsx";
import AdminLayout from "./layout/AdminLayout.tsx";
import AuthLayout from "./layout/AuthLayout.tsx";
import AdminDashboard from "./pages/admin/AdminDashboard.tsx";
import HomePage from "./pages/store/HomePage.tsx";
import ProductPage from "./pages/store/ProductPage.tsx";
import AdminOrder from "./pages/admin/AdminOrder.tsx";
import RegisterPage from "./pages/auth/RegisterPage.tsx";

function App() {
	return (
		<>
			<BrowserRouter>
				<Routes>
					{/* Layout Khách Hàng */}
					<Route path="/" element={<MainLayout />}>
						<Route index element={<HomePage />} />
						<Route path="products" element={<ProductPage />} />
					</Route>

					{/* Layout Admin */}
					<Route path="/admin" element={<AdminLayout />}>
						<Route index element={<AdminDashboard />} />
						<Route path="orders" element={<AdminOrder />} />
					</Route>

					{/* Layout Auth */}
					<Route path="/auth" element={<AuthLayout />}>
						<Route path="login" element={<LoginPage />} />
						<Route path="register" element={<RegisterPage />} />
					</Route>
				</Routes>
			</BrowserRouter>
		</>
	);
}

export default App;
