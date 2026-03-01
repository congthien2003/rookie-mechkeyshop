import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  images: {
    remotePatterns: [
      {
        protocol: "https",
        hostname: "bsnnwuphmgsqfpvlqdwn.supabase.co",
      },
    ],
  },
};

export default nextConfig;
