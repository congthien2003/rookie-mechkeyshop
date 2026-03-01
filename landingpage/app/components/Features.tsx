import {
  ShoppingCart,
  ClipboardList,
  Bell,
  MessageSquare,
  Database,
  Shield,
  Zap,
  Eye,
} from "lucide-react";

const features = [
  {
    icon: <ShoppingCart className="text-indigo-400" size={28} />,
    title: "Catalog Service",
    description:
      "Full CRUD for categories, products, and product images. Clean Architecture with Entity Framework Core and SQL Server.",
  },
  {
    icon: <ClipboardList className="text-purple-400" size={28} />,
    title: "Order Service",
    description:
      "Handles order creation, payment flow, and state management using Event Sourcing with PostgreSQL as the event store.",
  },
  {
    icon: <Bell className="text-cyan-400" size={28} />,
    title: "Notification Service",
    description:
      "Listens to domain events (OrderCreated, UserRegistered) and sends transactional emails asynchronously via MongoDB logs.",
  },
  {
    icon: <MessageSquare className="text-emerald-400" size={28} />,
    title: "Async Messaging",
    description:
      "RabbitMQ with MassTransit decouples services and enables reliable, scalable event-driven communication.",
  },
  {
    icon: <Zap className="text-yellow-400" size={28} />,
    title: "Redis Caching",
    description:
      "Frequently-queried data like product listings and categories are cached in Redis to reduce DB load and improve response times.",
  },
  {
    icon: <Shield className="text-rose-400" size={28} />,
    title: "YARP Reverse Proxy",
    description:
      "All admin portal requests are routed through YARP, which handles rate limiting, load balancing, and service discovery.",
  },
  {
    icon: <Database className="text-orange-400" size={28} />,
    title: "Cloud Storage",
    description:
      "Product images are uploaded and served via Supabase Storage Buckets with fast CDN delivery.",
  },
  {
    icon: <Eye className="text-teal-400" size={28} />,
    title: "Observability",
    description:
      ".NET Aspire provides distributed tracing, metrics collection, and health check dashboards across all microservices.",
  },
];

export default function Features() {
  return (
    <section id="features" className="py-24 px-6">
      <div className="max-w-7xl mx-auto">
        <div className="text-center mb-16">
          <h2 className="text-3xl md:text-5xl font-bold text-white mb-4">
            Platform Features
          </h2>
          <p className="text-slate-400 text-lg max-w-2xl mx-auto">
            Built for real-world scale with a clean separation of concerns and
            production-ready patterns.
          </p>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {features.map((f, i) => (
            <div
              key={i}
              className="group p-6 rounded-2xl bg-white/[0.03] border border-white/10 hover:border-indigo-500/40 hover:bg-white/[0.06] transition-all duration-300"
            >
              <div className="mb-4">{f.icon}</div>
              <h3 className="text-white font-semibold text-base mb-2">
                {f.title}
              </h3>
              <p className="text-slate-400 text-sm leading-relaxed">
                {f.description}
              </p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
