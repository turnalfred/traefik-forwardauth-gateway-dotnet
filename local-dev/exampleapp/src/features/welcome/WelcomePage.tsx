import Link from "next/link";

export function WelcomePage() {
  return (
    <div>
      <h1 className="text-xl mb-4">Welcome to the example app</h1>
      <p>
        This is the welcome page. This page will be open to all users as our
        traefik config does not apply the auth middleware to pages under /p/. To
        get started visit the{" "}
        <Link href="/p/signin" className="text-blue-400">
          sign-in page
        </Link>
      </p>
    </div>
  );
}
