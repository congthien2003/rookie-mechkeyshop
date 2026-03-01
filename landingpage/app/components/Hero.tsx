import Image from "next/image";
import { Github, ArrowRight } from "lucide-react";

export default function Hero() {
	return (
		<section className="relative min-h-screen flex items-center justify-center overflow-hidden pt-24">
			{/* Background gradient blobs */}
			<div className="absolute inset-0 -z-10">
				<div className="absolute top-1/4 left-1/4 w-96 h-96 bg-indigo-700/30 rounded-full filter blur-3xl" />
				<div className="absolute bottom-1/4 right-1/4 w-96 h-96 bg-purple-700/20 rounded-full filter blur-3xl" />
				<div className="absolute top-1/2 left-1/2 w-64 h-64 bg-cyan-700/20 rounded-full filter blur-3xl -translate-x-1/2 -translate-y-1/2" />
			</div>

			<div className="max-w-4xl mx-auto px-6 text-center">
				{/* Badge */}
				<div className="inline-flex items-center gap-2 px-3 py-1 mb-6 bg-indigo-500/10 border border-indigo-500/30 rounded-full text-indigo-300 text-sm font-medium">
					<span className="w-2 h-2 rounded-full bg-indigo-400 animate-pulse" />
					Phase 1 — Rookie Program
				</div>

				{/* Heading */}
				<h1 className="text-5xl md:text-7xl font-extrabold text-white leading-tight tracking-tight mb-6">
					Modern&nbsp;
					<span className="bg-gradient-to-r from-indigo-400 via-purple-400 to-cyan-400 bg-clip-text text-transparent block">
						Mechanical&nbsp;Keyboard
					</span>
					&nbsp;Store
				</h1>

				{/* Subheading */}
				<p className="text-lg md:text-xl text-slate-400 max-w-2xl mx-auto mb-10 leading-relaxed">
					A cloud-native e-commerce platform powered by{" "}
					<span className="text-white font-semibold">.NET 8 microservices</span>
					, <span className="text-white font-semibold">React</span>, Event
					Sourcing, RabbitMQ, Redis, and YARP — designed for scale and
					observability.
				</p>

				{/* CTA Buttons */}
				<div className="flex flex-col sm:flex-row items-center justify-center gap-4">
					<a
						href="#features"
						className="inline-flex items-center gap-2 px-6 py-3 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold rounded-xl transition-colors text-sm">
						Explore Features <ArrowRight size={16} />
					</a>
					<a
						href="https://github.com/congthien2003/rookie-mechkeyshop"
						target="_blank"
						rel="noopener noreferrer"
						className="inline-flex items-center gap-2 px-6 py-3 bg-white/5 hover:bg-white/10 border border-white/20 text-white font-semibold rounded-xl transition-colors text-sm">
						<Github size={16} /> View Source
					</a>
				</div>

				{/* Architecture image */}
				<div className="mt-16 rounded-2xl overflow-hidden border border-white/10 shadow-2xl shadow-indigo-900/40 relative w-full aspect-video">
					<Image
						src="https://ouwojlzirbbnufebtkgy.supabase.co/storage/v1/object/public/assets/MechKeyShop-Architecture.png"
						alt="MechKeyShop System Architecture"
						fill
						className="object-cover"
					/>
				</div>
			</div>
		</section>
	);
}
