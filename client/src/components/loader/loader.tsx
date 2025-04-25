import "./loader.css";
import { useSelector } from "react-redux";
import { RootState } from "@/store/store";
const Loader = () => {
	const isLoading = useSelector(
		(state: RootState) => state.loading.isLoading
	);

	if (!isLoading) return null;

	return (
		<div className="fixed inset-0 h-screen w-screen flex items-center justify-center z-50">
			{/* Overlay đen mờ */}
			<div className="absolute inset-0 bg-black bg-opacity-60"></div>

			{/* Nội dung loader */}
			<div className="relative z-10">
				<div id="box-outer">
					<div id="box-inner">
						<div id="box"></div>
					</div>
				</div>
			</div>
		</div>
	);
};

export default Loader;
