import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";

const NotFoundPage = () => {
	const navigate = useNavigate();

	return (
		<div className="flex h-screen w-full flex-col items-center justify-center">
			<h1 className="text-9xl font-extrabold tracking-wider text-gray-900">
				404
			</h1>
			<div className="absolute rotate-12 rounded bg-primary px-2 text-sm text-white">
				Page Not Found
			</div>
			<Button className="mt-5" onClick={() => navigate(-1)}>
				Go Back
			</Button>
		</div>
	);
};

export default NotFoundPage;
