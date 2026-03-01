const technologies = [
  {
    name: ".NET 8",
    role: "Backend development framework",
    category: "Backend",
    color: "indigo",
  },
  {
    name: ".NET Aspire",
    role: "Observability & diagnostics",
    category: "Backend",
    color: "indigo",
  },
  {
    name: "ASP.NET Core MVC",
    role: "Admin & API web framework",
    category: "Backend",
    color: "indigo",
  },
  {
    name: "Entity Framework Core",
    role: "ORM for relational data",
    category: "Backend",
    color: "indigo",
  },
  {
    name: "React.js",
    role: "Customer-facing frontend",
    category: "Frontend",
    color: "cyan",
  },
  {
    name: "Next.js",
    role: "Landing page framework",
    category: "Frontend",
    color: "cyan",
  },
  {
    name: "Tailwind CSS",
    role: "Utility-first styling",
    category: "Frontend",
    color: "cyan",
  },
  {
    name: "SQL Server",
    role: "Primary relational database",
    category: "Database",
    color: "emerald",
  },
  {
    name: "PostgreSQL",
    role: "Event store for Order Service",
    category: "Database",
    color: "emerald",
  },
  {
    name: "MongoDB",
    role: "Read model & email logs",
    category: "Database",
    color: "emerald",
  },
  {
    name: "Redis",
    role: "In-memory cache for fast access",
    category: "Infrastructure",
    color: "orange",
  },
  {
    name: "RabbitMQ",
    role: "Message broker for async communication",
    category: "Infrastructure",
    color: "orange",
  },
  {
    name: "MassTransit",
    role: "Abstraction layer for messaging",
    category: "Infrastructure",
    color: "orange",
  },
  {
    name: "YARP",
    role: "Reverse proxy & routing",
    category: "Infrastructure",
    color: "orange",
  },
  {
    name: "Supabase",
    role: "Cloud storage for images",
    category: "Infrastructure",
    color: "orange",
  },
  {
    name: "Clean Architecture",
    role: "Project structure & separation of concerns",
    category: "Pattern",
    color: "purple",
  },
];

const colorMap: Record<string, string> = {
  indigo: "border-indigo-500/30 bg-indigo-500/10 text-indigo-300",
  cyan: "border-cyan-500/30 bg-cyan-500/10 text-cyan-300",
  emerald: "border-emerald-500/30 bg-emerald-500/10 text-emerald-300",
  orange: "border-orange-500/30 bg-orange-500/10 text-orange-300",
  purple: "border-purple-500/30 bg-purple-500/10 text-purple-300",
};

export default function TechStack() {
  const categories = Array.from(new Set(technologies.map((t) => t.category)));

  return (
    <section id="techstack" className="py-24 px-6">
      <div className="max-w-7xl mx-auto">
        <div className="text-center mb-16">
          <h2 className="text-3xl md:text-5xl font-bold text-white mb-4">
            Technology Stack
          </h2>
          <p className="text-slate-400 text-lg max-w-2xl mx-auto">
            Modern, battle-tested technologies working together to deliver a
            resilient and observable platform.
          </p>
        </div>

        <div className="space-y-10">
          {categories.map((cat) => (
            <div key={cat}>
              <h3 className="text-sm font-semibold uppercase tracking-widest text-slate-500 mb-4">
                {cat}
              </h3>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                {technologies
                  .filter((t) => t.category === cat)
                  .map((tech, i) => (
                    <div
                      key={i}
                      className={`rounded-xl border px-5 py-4 ${colorMap[tech.color]}`}
                    >
                      <div className="font-semibold text-sm">{tech.name}</div>
                      <div className="text-xs opacity-70 mt-1">{tech.role}</div>
                    </div>
                  ))}
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
