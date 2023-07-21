import { UserInfo } from "./UserInfo";
import { LogoutButton } from "./LogoutButton";

export const AppBar = () => {
  return (
    <div className="bg-green-950 flex flex-row py-6 px-4 w-full justify-end gap-4">
      <UserInfo />
      <LogoutButton />
    </div>
  );
};
