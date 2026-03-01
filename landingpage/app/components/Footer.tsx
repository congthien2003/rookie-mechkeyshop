import { Github, Keyboard } from "lucide-react";

export default function Footer() {
  return (
    <footer className="border-t border-white/10 py-12 px-6">
      <div className="max-w-7xl mx-auto flex flex-col md:flex-row items-center justify-between gap-6">
        <div className="flex items-center gap-2 text-white font-bold">
          <Keyboard className="text-indigo-400" size={20} />
          <span>MechKeyShop</span>
        </div>

        <p className="text-slate-500 text-sm text-center">
          Built with ❤️ during the NashTech Rookie Program &mdash; Phase 1.
        </p>

        <a
          href="https://github.com/congthien2003/rookie-mechkeyshop"
          target="_blank"
          rel="noopener noreferrer"
          className="flex items-center gap-2 text-slate-400 hover:text-white text-sm transition-colors"
        >
          <Github size={16} />
          congthien2003/rookie-mechkeyshop
        </a>
      </div>
    </footer>
  );
}
