import { ClientComponent } from "../features/client-component/client-component";
import { ServerComponent } from "../features/server-component/server-component";

export default function Home() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-between p-24">
      <div className="flex flex-row justify-evenly w-full gap-8">
        <ClientComponent />
        <ServerComponent />
      </div>
    </main>
  );
}
