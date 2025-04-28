interface DataType<T> {
	items: T[];
	totalItems: number;
	page: number;
	pageSize: number;
	totalPages: number;
}

export interface ResultPagination<T> {
	isSuccess: boolean;
	message: string;
	data: DataType<T>;
}

// export const ResultPaginationFail: ResultPagination<T> = {
// 	isSuccess: false,
// 	message: "Get fail",
// 	data: {
// 		items: [] as T[],
// 		totalItems: 0,
// 		totalPages: 0,
// 		page: 0,
// 		pageSize: 0,
// 	},
// };
