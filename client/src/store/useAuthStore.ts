import { create } from "zustand";
import { persist } from "zustand/middleware";

interface User {
	id: string;
	email: string;
	// Add other user properties as needed
}

interface AuthState {
	user: User | null;
	isAuthenticated: boolean;
	setUser: (user: User | null) => void;
	logout: () => void;
}

export const useAuthStore = create<AuthState>()(
	persist(
		(set) => ({
			user: null,
			isAuthenticated: false,
			setUser: (user) =>
				set({ user, isAuthenticated: user ? true : false }),
			logout: () => set({ user: null, isAuthenticated: false }),
		}),
		{
			name: "auth-storage", // unique name for localStorage
		}
	)
);
