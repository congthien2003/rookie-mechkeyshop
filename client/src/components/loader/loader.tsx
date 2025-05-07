import { useLoadingStore } from "@/store/store";
import "./loader.css";
const Loader = () => {
	const isLoading = useLoadingStore((state) => state.isLoading);
	if (!isLoading) return null;

	return (
		<div className="fixed inset-0 h-screen w-screen flex items-center justify-center z-50">
			{/* Overlay đen mờ */}
			<div className="absolute inset-0 bg-black bg-opacity-60"></div>

			{/* Nội dung loader */}
			<div className="relative z-50">
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
