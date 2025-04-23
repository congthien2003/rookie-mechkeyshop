import {
	Pagination,
	PaginationContent,
	PaginationEllipsis,
	PaginationItem,
	PaginationLink,
	PaginationNext,
	PaginationPrevious,
} from "@/components/ui/pagination";

interface PaginationProps {
	page: number;
	totalPages: number;
	onPageChange: (page: number) => void;
}

function PaginationComponent({
	page,
	totalPages,
	onPageChange,
}: PaginationProps) {
	const handlePageChange = (newPage: number) => {
		if (newPage >= 0 && newPage <= totalPages) {
			onPageChange(newPage);
		}
		onPageChange(newPage);
	};

	const renderPageNumbers = () => {
		const pages = [];
		const maxVisiblePages = 5;
		let startPage = Math.max(1, page - Math.floor(maxVisiblePages / 2));
		const endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);

		if (endPage - startPage + 1 < maxVisiblePages) {
			startPage = Math.max(1, endPage - maxVisiblePages + 1);
		}

		// Add first page
		if (startPage > 1) {
			pages.push(
				<PaginationItem key={1}>
					<PaginationLink
						href="#"
						onClick={(e) => {
							e.preventDefault();
							handlePageChange(1);
						}}>
						1
					</PaginationLink>
				</PaginationItem>
			);
			if (startPage > 2) {
				pages.push(
					<PaginationItem key="start-ellipsis">
						<PaginationEllipsis />
					</PaginationItem>
				);
			}
		}

		// Add page numbers
		for (let i = startPage; i <= endPage; i++) {
			pages.push(
				<PaginationItem key={i}>
					<PaginationLink
						onClick={(e) => {
							e.preventDefault();
							handlePageChange(i);
						}}
						isActive={i === page}>
						{i}
					</PaginationLink>
				</PaginationItem>
			);
		}

		// Add last page
		if (endPage < totalPages) {
			if (endPage < totalPages - 1) {
				pages.push(
					<PaginationItem key="end-ellipsis">
						<PaginationEllipsis />
					</PaginationItem>
				);
			}
			pages.push(
				<PaginationItem key={totalPages}>
					<PaginationLink
						onClick={(e) => {
							e.preventDefault();
							handlePageChange(totalPages);
						}}>
						{totalPages}
					</PaginationLink>
				</PaginationItem>
			);
		}

		return pages;
	};

	return (
		<div className="w-full flex items-center justify-center mt-4">
			<Pagination>
				<PaginationContent>
					<PaginationItem>
						<PaginationPrevious
							onClick={(e) => {
								e.preventDefault();
								handlePageChange(page - 1);
							}}
							className={
								page === 1
									? "pointer-events-none opacity-50"
									: ""
							}
						/>
					</PaginationItem>
					{renderPageNumbers()}
					<PaginationItem>
						<PaginationNext
							href="#"
							onClick={(e) => {
								e.preventDefault();
								handlePageChange(page + 1);
							}}
							className={
								page === totalPages
									? "pointer-events-none opacity-50"
									: ""
							}
						/>
					</PaginationItem>
				</PaginationContent>
			</Pagination>
		</div>
	);
}

export default PaginationComponent;
