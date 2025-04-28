import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { cn } from "@/lib/utils";

interface DashboardCardProps {
	title: string;
	value: number;
	icon: React.ReactNode;
	description?: string;
	className?: string;
	isLoading?: boolean;
}

export function DashboardCard({
	title,
	value,
	icon,
	description,
	className,
	isLoading = false,
}: DashboardCardProps) {
	return (
		<Card className={cn("h-full", className)}>
			<CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
				<CardTitle className="text-sm font-medium">{title}</CardTitle>
				{icon}
			</CardHeader>
			<CardContent>
				{isLoading ? (
					<Skeleton className="h-8 w-24" />
				) : (
					<div className="text-2xl font-bold">
						{value.toLocaleString()}
					</div>
				)}
				{description && (
					<p className="text-xs text-muted-foreground">
						{description}
					</p>
				)}
			</CardContent>
		</Card>
	);
}
