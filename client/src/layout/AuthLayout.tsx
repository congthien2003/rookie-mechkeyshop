import { Outlet } from "react-router-dom";

const AuthLayout = () => {
	return (
		<div className="flex items-center justify-center min-h-screen bg-gray-100">
			<div className="w-full max-w-md bg-white p-6 rounded shadow-md">
				<Outlet />
			</div>
		</div>
	);
};

export default AuthLayout;
