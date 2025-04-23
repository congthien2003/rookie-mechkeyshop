import toast from "react-hot-toast";

export const ToastSuccess = function (message: string) {
	return toast.success(message, {
		duration: 2500,
	});
};

export const ToastError = function (message: string) {
	return toast.error(message, {
		duration: 2500,
	});
};
