import { Outlet, Link } from "react-router-dom";

const AuthLayout = () => {
	return (
		<div className="flex items-center justify-center min-h-screen bg-gray-100">
			<div className="w-full max-w-md bg-white p-6 rounded shadow-md">
				<Outlet />
				<p className="mt-4 text-center">
					<Link to="/auth/register" className="text-blue-500">
						Register
					</Link>{" "}
					|
					<Link to="/auth/login" className="text-blue-500">
						{" "}
						Login
					</Link>
				</p>
			</div>
		</div>
	);
};

export default AuthLayout;
