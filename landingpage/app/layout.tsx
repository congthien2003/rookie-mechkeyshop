import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "MechKeyShop – Modern Mechanical Keyboard Store",
  description:
    "MechKeyShop is a cloud-native e-commerce platform for mechanical keyboards, built with .NET microservices, React, RabbitMQ, Redis, and more.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className="antialiased">{children}</body>
    </html>
  );
}
