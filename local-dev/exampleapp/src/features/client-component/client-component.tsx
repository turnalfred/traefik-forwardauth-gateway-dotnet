"use client";

import { useQuery } from "@tanstack/react-query";

export const ClientComponent = () => {
  const { data } = useQuery(["client-component"], () =>
    fetch("/api/hello", { headers: { "CSRF-Token": "1" } }).then((res) =>
      res.json()
    )
  );
  return (
    <div className="w-[50%] flex flex-col gap-4 outline-dashed outline-1 outline-green-50 p-4 rounded-lg">
      <h2 className="text-xl">Client component</h2>
      <p>
        This data was fetched from a client component to the example-service at
        /api/hello. The API service is behind the Auth proxy but the API call
        originated from client side (i.e. appears in network tab)
      </p>
      {data ? (
        <pre className="rounded-lg bg-green-800 p-4 overflow-auto">
          {JSON.stringify(data, null, 2)}
        </pre>
      ) : (
        <div>loading...</div>
      )}
    </div>
  );
};
