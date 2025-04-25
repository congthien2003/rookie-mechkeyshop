import { configureStore } from "@reduxjs/toolkit";
import loadingReducer from "./loadingSlice";
import authReducer from "./authSlice";

export const store = configureStore({
	reducer: {
		loading: loadingReducer,
		auth: authReducer,
	},
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
