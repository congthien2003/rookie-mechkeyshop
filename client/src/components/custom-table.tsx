import { ColumnDef } from "@tanstack/react-table";

interface CustomTableProps<T> {
	data: T[];
	columns: ColumnDef<T>[];
	onRowClick?: (data: T) => void;
}

function CustomTable<T>({ data, columns, onRowClick }: CustomTableProps<T>) {
	return (
		<div className="overflow-x-auto">
			<table className="min-w-full divide-y divide-gray-200">
				<thead className="bg-gray-50">
					<tr>
						{columns.map((column, index) => (
							<th
								key={index}
								scope="col"
								className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
								{column.header as string}
							</th>
						))}
					</tr>
				</thead>
				<tbody className="bg-white divide-y divide-gray-200">
					{data.length > 0 ? (
						data.map((row, rowIndex) => (
							<tr
								key={rowIndex}
								className={`hover:bg-gray-50 cursor-pointer transition-colors h-[100px] ${
									onRowClick ? "cursor-pointer" : ""
								}`}
								onClick={() => onRowClick && onRowClick(row)}>
								{columns.map((column, colIndex) => (
									<td
										key={colIndex}
										className="px-6 py-4 max-w-[300px] whitespace-nowrap text-sm font-medium text-gray-800">
										{column.cell
											? column.cell({
													row: { original: row },
											  })
											: (row as any)[
													column.accessorKey as string
											  ]}
									</td>
								))}
							</tr>
						))
					) : (
						<tr>
							<td
								colSpan={columns.length}
								className="px-6 py-4 text-center text-sm text-gray-500">
								No data available
							</td>
						</tr>
					)}
				</tbody>
			</table>
		</div>
	);
}

export default CustomTable;
