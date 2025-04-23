import { Box, Cuboid, File, Home, User } from "lucide-react";
import { Link, useLocation } from "react-router-dom";
import { cn } from "@/lib/utils";

// Menu items.
const items = [
	{
		title: "Dashboard",
		url: "/admin",
		icon: Home,
	},
	{
		title: "Users",
		url: "/admin/users",
		icon: User,
	},
	{
		title: "Categories",
		url: "/admin/categories",
		icon: Cuboid,
	},
	{
		title: "Products",
		url: "/admin/products",
		icon: Box,
	},
	{
		title: "Orders",
		url: "/admin/orders",
		icon: File,
	},
];

export function AppSidebar() {
	const location = useLocation();

	return (
		<div className="flex h-[100vh] m-w-64 w-64 flex-col border-r bg-background">
			<div className="flex h-14 items-center border-b px-4">
				<h1 className="text-lg font-semibold">Admin Panel</h1>
			</div>
			<nav className="flex-1 space-y-1 p-2">
				{items.map((item) => {
					const Icon = item.icon;
					return (
						<Link
							key={item.url}
							to={item.url}
							className={cn(
								"flex items-center gap-3 rounded-lg px-3 py-2 text-md font-medium transition-colors",
								location.pathname === item.url
									? "bg-primary text-primary-foreground"
									: "hover:bg-muted"
							)}>
							<Icon className="h-4 w-4" />
							{item.title}
						</Link>
					);
				})}
			</nav>
		</div>
	);
}
