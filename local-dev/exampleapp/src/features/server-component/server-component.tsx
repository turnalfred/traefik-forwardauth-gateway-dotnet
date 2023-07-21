import { headers } from "next/headers";

const serverSideFetcher = async () => {
  const authHeaders: [string, string][] = [];
  headers().forEach((value, key) => {
    if (key.toLocaleLowerCase().startsWith("x-auth")) {
      authHeaders.push([key, value]);
    }
  });

  return { authHeaders };
};

export const ServerComponent = async () => {
  const { authHeaders } = await serverSideFetcher();
  return (
    <div className="w-[50%] flex flex-col gap-4 outline-dashed outline-1 outline-green-50 p-4 rounded-lg">
      <h2 className="text-xl">Server component</h2>
      <p>
        These were the X-Auth headers were present in a react server component
        (behind the auth server proxy). No API call will appear in the network
        tab for this.
      </p>

      <pre className="rounded-lg bg-green-800 p-4 overflow-auto">
        {authHeaders.map((x) => `${x[0]}: ${x[1]}`).join("\n")}
      </pre>
    </div>
  );
};
