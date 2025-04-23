import { AppSidebar } from "@/components/app-sidebar";
import { Outlet } from "react-router-dom";

const AdminLayout = () => {
	return (
		<div className="min-h-screen flex">
			<AppSidebar />
			<main className="flex-grow p-4">
				<Outlet />
			</main>
		</div>
	);
};

export default AdminLayout;
