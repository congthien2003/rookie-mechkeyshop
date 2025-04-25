import { createSlice } from "@reduxjs/toolkit";

interface LoadingState {
	isLoading: boolean;
}

const initialState: LoadingState = {
	isLoading: false,
};

const loadingSlice = createSlice({
	name: "loading",
	initialState,
	reducers: {
		ShowLoading(state) {
			state.isLoading = true;
		},
		HideLoading(state) {
			state.isLoading = false;
		},
	}, // Nếu cần reducer thường thì thêm vào đây
});
export const { ShowLoading, HideLoading } = loadingSlice.actions;
export default loadingSlice.reducer;
