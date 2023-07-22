"use client";

import { useQuery } from "@tanstack/react-query";
import Link from "next/link";

const useGetIdps = () =>
  useQuery<{ name: string; loginUrl: string }[]>(["idps"], () =>
    fetch("/oauth2/providers", { headers: { "CSRF-Token": "1" } }).then((res) =>
      res.json()
    )
  );

export function SigninPage() {
  const idps = useGetIdps();
  if (!idps.data) {
    return <div>Loading...</div>;
  }

  return (
    <div className="flex flex-col gap-4 w-fit">
      <h1 className="text-xl">Sign-in with:</h1>
      {idps.data.map((idp) => {
        return (
          <Link
            key={idp.name}
            href={`${idp.loginUrl}?returnPath=${encodeURIComponent("/")}`}
            className="text-blue-500 self-end"
          >
            {idp.name}
          </Link>
        );
      })}
    </div>
  );
}
