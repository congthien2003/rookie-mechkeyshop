import "./App.css";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import LoginPage from "./pages/auth/LoginPage.tsx";
import AdminLayout from "./layout/AdminLayout.tsx";
import AuthLayout from "./layout/AuthLayout.tsx";
import AdminDashboard from "./pages/admin/dashboard/AdminDashboard.tsx";
import AdminOrder from "./pages/admin/order/AdminOrder.tsx";
import RegisterPage from "./pages/auth/RegisterPage.tsx";
import AdminProduct from "./pages/admin/product/AdminProduct.tsx";
import AdminCategory from "./pages/admin/category/AdminCategory.tsx";
import AdminUser from "./pages/admin/user/AdminUser.tsx";
import { Toaster } from "react-hot-toast";
import Loader from "./components/loader/loader.tsx";
import PrivateRoute from "./components/PrivateRoute";
import NotFoundPage from "./pages/error/NotFoundPage";
import UnauthorizedPage from "./pages/error/UnauthorizedPage";

function App() {
	return (
		<>
			<Loader />

			<BrowserRouter>
				<Routes>
					<Route
						path="/"
						element={<Navigate to="/admin" replace={true} />}
					/>
					{/* Protected Admin Routes */}
					<Route element={<PrivateRoute />}>
						<Route path="/admin" element={<AdminLayout />}>
							<Route index element={<AdminDashboard />} />
							<Route path="products" element={<AdminProduct />} />
							<Route
								path="categories"
								element={<AdminCategory />}
							/>
							<Route path="users" element={<AdminUser />} />
							<Route path="orders" element={<AdminOrder />} />
						</Route>
					</Route>

					{/* Public Auth Routes */}
					<Route path="/auth" element={<AuthLayout />}>
						<Route path="login" element={<LoginPage />} />
						<Route path="register" element={<RegisterPage />} />
					</Route>

					{/* Error Pages */}
					<Route path="/401" element={<UnauthorizedPage />} />
					<Route path="/404" element={<NotFoundPage />} />

					{/* Catch all route - redirect to 404 */}
					<Route path="*" element={<Navigate to="/404" replace />} />
				</Routes>
			</BrowserRouter>
			<Toaster />
		</>
	);
}

export default App;
