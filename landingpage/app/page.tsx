import Navbar from "./components/Navbar";
import Hero from "./components/Hero";
import Stats, { CTA } from "./components/Stats";
import Features from "./components/Features";
import Architecture from "./components/Architecture";
import TechStack from "./components/TechStack";
import Footer from "./components/Footer";

export default function Home() {
  return (
    <main className="min-h-screen bg-[#0f0f1a] text-white">
      <Navbar />
      <Hero />
      <Stats />
      <Features />
      <Architecture />
      <TechStack />
      <CTA />
      <Footer />
    </main>
  );
}
