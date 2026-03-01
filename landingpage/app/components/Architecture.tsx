export default function Architecture() {
  return (
    <section id="architecture" className="py-24 px-6 bg-white/[0.02]">
      <div className="max-w-7xl mx-auto">
        <div className="text-center mb-16">
          <h2 className="text-3xl md:text-5xl font-bold text-white mb-4">
            System Architecture
          </h2>
          <p className="text-slate-400 text-lg max-w-2xl mx-auto">
            A microservices architecture designed for scalability, resilience,
            and full observability.
          </p>
        </div>

        {/* Architecture Highlights */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-16">
          {[
            {
              title: "Microservices",
              color: "indigo",
              items: [
                "Catalog Service — products & categories",
                "Order Service — orders & payments",
                "Notification Service — async emails",
              ],
            },
            {
              title: "Infrastructure",
              color: "purple",
              items: [
                "RabbitMQ + MassTransit messaging",
                "Redis caching layer",
                "Supabase cloud storage",
              ],
            },
            {
              title: "Cross-Cutting",
              color: "cyan",
              items: [
                "YARP reverse proxy & rate-limiting",
                ".NET Aspire observability",
                "Clean Architecture patterns",
              ],
            },
          ].map((col, i) => (
            <div
              key={i}
              className="p-6 rounded-2xl border border-white/10 bg-white/[0.03]"
            >
              <h3
                className={`text-lg font-semibold mb-4 ${
                  col.color === "indigo"
                    ? "text-indigo-400"
                    : col.color === "purple"
                    ? "text-purple-400"
                    : "text-cyan-400"
                }`}
              >
                {col.title}
              </h3>
              <ul className="space-y-2">
                {col.items.map((item, j) => (
                  <li key={j} className="flex items-start gap-2 text-sm text-slate-300">
                    <span
                      className={`mt-1.5 w-1.5 h-1.5 rounded-full flex-shrink-0 ${
                        col.color === "indigo"
                          ? "bg-indigo-400"
                          : col.color === "purple"
                          ? "bg-purple-400"
                          : "bg-cyan-400"
                      }`}
                    />
                    {item}
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>

        {/* Event Flow Diagram */}
        <div id="services" className="rounded-2xl border border-white/10 bg-white/[0.02] p-8">
          <h3 className="text-xl font-bold text-white mb-2">
            Order Service — Event Sourcing Flow
          </h3>
          <p className="text-slate-400 text-sm mb-8">
            CQRS + MediatR with PostgreSQL as event store and MongoDB as the
            read model.
          </p>

          <div className="overflow-x-auto">
            <div className="min-w-[640px] flex items-start gap-0">
              {[
                {
                  label: "Customer",
                  sub: "HTTP Request",
                  color: "bg-indigo-600",
                },
                {
                  label: "YARP",
                  sub: "Reverse Proxy",
                  color: "bg-purple-600",
                },
                {
                  label: "Order Service",
                  sub: "Validate & Build",
                  color: "bg-violet-600",
                },
                {
                  label: "Event Store",
                  sub: "PostgreSQL",
                  color: "bg-blue-600",
                },
                {
                  label: "RabbitMQ",
                  sub: "Publish Event",
                  color: "bg-orange-600",
                },
                {
                  label: "Read Model",
                  sub: "MongoDB",
                  color: "bg-emerald-600",
                },
              ].map((step, i, arr) => (
                <div key={i} className="flex items-center">
                  <div className="flex flex-col items-center gap-2">
                    <div
                      className={`${step.color} text-white text-xs font-semibold px-4 py-3 rounded-xl text-center min-w-[100px]`}
                    >
                      <div>{step.label}</div>
                      <div className="font-normal opacity-75 mt-0.5">
                        {step.sub}
                      </div>
                    </div>
                  </div>
                  {i < arr.length - 1 && (
                    <div className="flex items-center mx-2">
                      <div className="w-8 h-px bg-slate-600" />
                      <div className="w-0 h-0 border-t-4 border-b-4 border-l-4 border-t-transparent border-b-transparent border-l-slate-500" />
                    </div>
                  )}
                </div>
              ))}
            </div>
          </div>

          <div className="mt-8 grid grid-cols-1 sm:grid-cols-3 gap-4">
            {[
              {
                step: "1",
                text: "Customer sends HTTP request → YARP forwards to Order Service",
                color: "border-indigo-500/40 bg-indigo-500/5",
              },
              {
                step: "2",
                text: "Order Service validates the aggregate and appends domain events to PostgreSQL event store",
                color: "border-purple-500/40 bg-purple-500/5",
              },
              {
                step: "3",
                text: "Integration event published to RabbitMQ, consumed by Read Model projections in MongoDB",
                color: "border-emerald-500/40 bg-emerald-500/5",
              },
            ].map((s) => (
              <div
                key={s.step}
                className={`rounded-xl border p-4 ${s.color}`}
              >
                <span className="text-xs font-bold text-slate-400 uppercase tracking-wider">
                  Step {s.step}
                </span>
                <p className="text-sm text-slate-300 mt-1">{s.text}</p>
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
}
