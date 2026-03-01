import { ArrowRight } from "lucide-react";

const stats = [
	{ value: "3", label: "Microservices" },
	{ value: "CQRS", label: "Event Sourcing Pattern" },
	{ value: "5+", label: "Infrastructure Services" },
	{ value: "100%", label: "Async Messaging" },
];

export default function Stats() {
	return (
		<section className="py-16 px-6 border-y border-white/10 bg-white/[0.02] mt-4">
			<div className="max-w-7xl mx-auto">
				<div className="grid grid-cols-2 md:grid-cols-4 gap-8">
					{stats.map((s, i) => (
						<div key={i} className="text-center">
							<div className="text-4xl font-extrabold text-white mb-2">
								{s.value}
							</div>
							<div className="text-sm text-slate-400">{s.label}</div>
						</div>
					))}
				</div>
			</div>
		</section>
	);
}

export function CTA() {
	return (
		<section className="py-24 px-6">
			<div className="max-w-3xl mx-auto text-center">
				<div className="rounded-3xl bg-gradient-to-br from-indigo-900/60 via-purple-900/40 to-cyan-900/30 border border-white/10 p-12">
					<h2 className="text-3xl md:text-4xl font-bold text-white mb-4">
						Ready to explore?
					</h2>
					<p className="text-slate-400 mb-8 text-lg">
						Clone the repository, spin up Docker services, and run the full
						microservices stack locally.
					</p>
					<div className="flex flex-col sm:flex-row gap-4 justify-center">
						<a
							href="https://github.com/congthien2003/rookie-mechkeyshop"
							target="_blank"
							rel="noopener noreferrer"
							className="inline-flex items-center gap-2 px-6 py-3 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold rounded-xl transition-colors">
							Get Started <ArrowRight size={16} />
						</a>
						<a
							href="#architecture"
							className="inline-flex items-center gap-2 px-6 py-3 bg-white/5 hover:bg-white/10 border border-white/20 text-white font-semibold rounded-xl transition-colors">
							View Architecture
						</a>
					</div>
				</div>
			</div>
		</section>
	);
}
