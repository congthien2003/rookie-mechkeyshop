import { createSlice, PayloadAction, createAsyncThunk } from "@reduxjs/toolkit";
interface AuthState {
	isLoggedIn: boolean;
	loading: boolean;
	error: string | null;
	token: string | null;
}
const initialState: AuthState = {
	isLoggedIn: false,
	loading: false,
	error: "",
	token: null,
};

export const Login = createAsyncThunk(
	"auth/login",
	async (
		{ email, password }: { email: string; password: string },
		{ rejectWithValue }
	) => {
		console.log(email, password);

		// Call Api

		return rejectWithValue("Error");
	}
);

export const Register = createAsyncThunk(
	"auth/register",
	async (
		{ email, password }: { email: string; password: string },
		{ rejectWithValue }
	) => {
		console.log(email, password);
		// Call Api

		return rejectWithValue("Error");
	}
);

export const Logout = createAsyncThunk("auth/logout", async () => {
	console.log("Log out");
	// Call Api
});

// Create Slice
const authSlice = createSlice({
	name: "auth",
	initialState,
	reducers: {}, // Nếu cần reducer thường thì thêm vào đây
	extraReducers: (builder) => {
		builder
			.addCase(Login.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(Login.fulfilled, (state, action: PayloadAction<any>) => {
				state.loading = false;
				state.isLoggedIn = true;
				//
			})
			.addCase(Login.rejected, (state, action) => {
				state.loading = false;
				state.error = action.payload as string;
			})
			.addCase(Register.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(
				Register.fulfilled,
				(state, action: PayloadAction<any>) => {
					state.loading = false;
					state.isLoggedIn = false;
					// state.user = action.payload.user;
					// state.session = action.payload.session;
				}
			)
			.addCase(Register.rejected, (state, action) => {
				state.loading = false;
				state.error = action.payload as string;
			})
			.addCase(Logout.fulfilled, (state) => {
				state.token = "";
				state.isLoggedIn = false;
			});
	},
});

// Export các action và reducer

export default authSlice.reducer;
