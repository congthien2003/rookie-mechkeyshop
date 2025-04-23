export interface Result<T = void> {
	isSuccess: boolean;
	message: string;
	data?: T;
}
