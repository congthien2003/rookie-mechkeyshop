import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";

const UnauthorizedPage = () => {
	const navigate = useNavigate();

	return (
		<div className="flex h-screen w-full flex-col items-center justify-center">
			<h1 className="text-9xl font-extrabold tracking-wider text-gray-900">
				401
			</h1>
			<div className="absolute rotate-12 rounded bg-primary px-2 text-sm text-white">
				Unauthorized Access
			</div>
			<p className="mt-4 text-gray-600">
				Sorry, you don't have permission to access this page.
			</p>
			<Button className="mt-5" onClick={() => navigate("/auth/login")}>
				Go to Login
			</Button>
		</div>
	);
};

export default UnauthorizedPage;
