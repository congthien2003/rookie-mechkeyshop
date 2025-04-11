import { Outlet, Link } from "react-router-dom";

const AdminLayout = () => {
	return (
		<div className="min-h-screen flex">
			<aside className="w-64 bg-gray-900 text-white p-4">
				<h2 className="text-xl font-bold">Admin Panel</h2>
				<nav>
					<Link to="/admin">Dashboard</Link> |{" "}
					<Link to="/admin/orders">Orders</Link>
				</nav>
			</aside>

			<main className="flex-grow p-4">
				<Outlet />
			</main>
		</div>
	);
};

export default AdminLayout;
