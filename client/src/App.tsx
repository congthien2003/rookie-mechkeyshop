import "./App.css";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import LoginPage from "./pages/auth/LoginPage.tsx";
import AdminLayout from "./layout/AdminLayout.tsx";
import AuthLayout from "./layout/AuthLayout.tsx";
import AdminDashboard from "./pages/admin/AdminDashboard.tsx";
import AdminOrder from "./pages/admin/AdminOrder.tsx";
import RegisterPage from "./pages/auth/RegisterPage.tsx";
import AdminProduct from "./pages/admin/AdminProduct.tsx";
import AdminCategory from "./pages/admin/AdminCategory.tsx";
import AdminUser from "./pages/admin/AdminUser.tsx";
import { Toaster } from "react-hot-toast";
function App() {
	return (
		<>
			<BrowserRouter>
				<Routes>
					{/* Layout Admin */}
					<Route path="/admin" element={<AdminLayout />}>
						<Route index element={<AdminDashboard />} />
						<Route path="products" element={<AdminProduct />} />
						<Route path="categories" element={<AdminCategory />} />
						<Route path="users" element={<AdminUser />} />
						<Route path="orders" element={<AdminOrder />} />
					</Route>

					{/* Layout Auth */}
					<Route path="/auth" element={<AuthLayout />}>
						<Route path="login" element={<LoginPage />} />
						<Route path="register" element={<RegisterPage />} />
					</Route>
				</Routes>
			</BrowserRouter>
			<Toaster />
		</>
	);
}

export default App;
