import { useFormik } from "formik";
import * as Yup from "yup";
import { useNavigate } from "react-router-dom";
import { login } from "../../services/apiAuth";
import { toast } from "react-hot-toast";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
} from "@/components/ui/card";
import { Eye, EyeOff, Keyboard } from "lucide-react";
import { useEffect, useState } from "react";
import { useAuthStore } from "@/store/useAuthStore";
import { useLoadingStore } from "@/store/store";

const validationSchema = Yup.object({
	email: Yup.string()
		.email("Invalid email address")
		.required("Email is required"),
	password: Yup.string()
		.min(6, "Password must be at least 6 characters")
		.required("Password is required"),
});

export default function LoginPage() {
	const navigate = useNavigate();
	const [showPassword, setShowPassword] = useState(false);
	const { user, setUser } = useAuthStore();
	const { showLoading, hideLoading } = useLoadingStore();

	// Redirect if user is already logged in
	useEffect(() => {
		if (user) {
			navigate("/admin");
		}
	}, [user, navigate]);

	const formik = useFormik({
		initialValues: {
			email: "",
			password: "",
		},
		validationSchema,
		onSubmit: async (values) => {
			try {
				showLoading();
				const userData = await login(values);
				setUser(userData);
				toast.success("Login successful!");
				navigate("/admin");
			} catch {
				toast.error("Login failed. Please check your credentials.");
			} finally {
				hideLoading();
			}
		},
	});

	return (
		<div className=" flex items-center justify-center bg-gradient-to-br from-gray-900 via-gray-800 to-black p-4 rounded-md">
			<div className="w-full max-w-md space-y-8">
				<div className="text-center">
					<div className="flex justify-center mb-4">
						<Keyboard className="h-12 w-12 text-primary" />
					</div>
					<h2 className="text-3xl font-bold tracking-tight text-white">
						Welcome to MechKey
					</h2>
					<p className="mt-2 text-sm text-gray-400">
						Your premium mechanical keyboard destination
					</p>
				</div>

				<Card className="border-gray-800 bg-gray-900/50 backdrop-blur-sm">
					<CardHeader className="space-y-1">
						<CardTitle className="text-2xl font-bold text-center text-white">
							Sign in to your account
						</CardTitle>
						<CardDescription className="text-center text-gray-400">
							Enter your credentials to access your MechKey
							account
						</CardDescription>
					</CardHeader>
					<CardContent>
						<form
							onSubmit={formik.handleSubmit}
							className="space-y-4">
							<div className="space-y-2">
								<Label
									htmlFor="email"
									className="text-gray-300">
									Email
								</Label>
								<Input
									id="email"
									name="email"
									type="email"
									placeholder="name@example.com"
									autoComplete="email"
									onChange={formik.handleChange}
									onBlur={formik.handleBlur}
									value={formik.values.email}
									className={`bg-gray-800 border-gray-700 text-white placeholder:text-gray-500 ${
										formik.touched.email &&
										formik.errors.email
											? "border-destructive"
											: ""
									}`}
								/>
								{formik.touched.email &&
									formik.errors.email && (
										<p className="text-sm text-destructive">
											{formik.errors.email}
										</p>
									)}
							</div>
							<div className="space-y-2">
								<Label
									htmlFor="password"
									className="text-gray-300">
									Password
								</Label>
								<div className="relative">
									<Input
										id="password"
										name="password"
										type={
											showPassword ? "text" : "password"
										}
										placeholder="Enter your password"
										autoComplete="current-password"
										onChange={formik.handleChange}
										onBlur={formik.handleBlur}
										value={formik.values.password}
										className={`bg-gray-800 border-gray-700 text-white placeholder:text-gray-500 ${
											formik.touched.password &&
											formik.errors.password
												? "border-destructive"
												: ""
										}`}
									/>
									<Button
										type="button"
										variant="ghost"
										size="icon"
										className="absolute right-0 top-0 h-full px-3 py-2 hover:bg-transparent text-gray-400"
										onClick={() =>
											setShowPassword(!showPassword)
										}>
										{showPassword ? (
											<EyeOff className="h-4 w-4" />
										) : (
											<Eye className="h-4 w-4" />
										)}
									</Button>
								</div>
								{formik.touched.password &&
									formik.errors.password && (
										<p className="text-sm text-destructive">
											{formik.errors.password}
										</p>
									)}
							</div>
							<div className="flex items-center justify-between">
								<div className="flex items-center space-x-2">
									<input
										type="checkbox"
										id="remember"
										className="h-4 w-4 rounded border-gray-700 bg-gray-800 text-primary focus:ring-primary"
									/>
									<Label
										htmlFor="remember"
										className="text-sm font-normal text-gray-400">
										Remember me
									</Label>
								</div>
								<Button
									variant="link"
									className="px-0 font-normal text-primary hover:text-primary/80">
									Forgot password?
								</Button>
							</div>
							<Button
								type="submit"
								className="w-full bg-primary hover:bg-primary/90 text-white"
								disabled={formik.isSubmitting}>
								{formik.isSubmitting ? (
									<div className="flex items-center gap-2">
										<div className="h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent" />
										<span>Signing in...</span>
									</div>
								) : (
									"Sign in"
								)}
							</Button>
						</form>
					</CardContent>
					{/* <CardFooter className="flex flex-col space-y-4">
						<div className="relative w-full">
							<Separator className="absolute inset-0 flex items-center">
								<span className="w-full border-t border-gray-700" />
							</Separator>
							<div className="relative flex justify-center text-xs uppercase">
								<span className="bg-gray-900/50 px-2 text-gray-400">
									Or continue with
								</span>
							</div>
						</div>
						<div className="grid grid-cols-2 gap-4">
							<Button
								variant="outline"
								className="w-full border-gray-700 bg-gray-800 text-white hover:bg-gray-700">
								<svg
									className="mr-2 h-4 w-4"
									viewBox="0 0 24 24">
									<path
										d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
										fill="#4285F4"
									/>
									<path
										d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
										fill="#34A853"
									/>
									<path
										d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
										fill="#FBBC05"
									/>
									<path
										d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
										fill="#EA4335"
									/>
								</svg>
								Google
							</Button>
							<Button
								variant="outline"
								className="w-full border-gray-700 bg-gray-800 text-white hover:bg-gray-700">
								<svg
									className="mr-2 h-4 w-4"
									viewBox="0 0 24 24">
									<path
										d="M12 0C5.373 0 0 5.373 0 12c0 5.302 3.438 9.8 8.207 11.387.6.113.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23A11.509 11.509 0 0112 5.803c1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576C20.566 21.797 24 17.3 24 12c0-6.627-5.373-12-12-12z"
										fill="currentColor"
									/>
								</svg>
								GitHub
							</Button>
						</div>
					</CardFooter> */}
				</Card>
			</div>
		</div>
	);
}
