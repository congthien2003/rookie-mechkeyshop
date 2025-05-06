import { Button } from "@/components/ui/button";
import { useAuthStore } from "@/store/useAuthStore";
import { LogOut } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { logout } from "@/services/apiAuth";
import { toast } from "react-hot-toast";

const UserBlock = () => {
	const { user, logout: logoutStore } = useAuthStore();
	const navigate = useNavigate();

	const handleLogout = async () => {
		try {
			await logout();
			logoutStore();
			toast.success("Logged out successfully");
			navigate("/auth/login");
		} catch (error) {
			toast.error("Failed to logout");
		}
	};

	return (
		<div className="flex flex-col gap-2 border-t py-4 px-2">
			<span className="text-sm text-muted-foreground">{user?.email}</span>
			<Button
				variant="outline"
				size="sm"
				className=" gap-2"
				onClick={handleLogout}>
				<LogOut className="h-4 w-4" />
				Logout
			</Button>
		</div>
	);
};

export default UserBlock;
