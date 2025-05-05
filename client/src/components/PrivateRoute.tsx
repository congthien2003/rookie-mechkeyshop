import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuthStore } from "@/store/useAuthStore";

const PrivateRoute = () => {
	const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
	const location = useLocation();

	// If not authenticated, redirect to login
	if (!isAuthenticated) {
		// Save the attempted URL for redirecting after login
		return <Navigate to="/auth/login" state={{ from: location }} replace />;
	}

	// If authenticated, render the child routes
	return <Outlet />;
};

export default PrivateRoute;
