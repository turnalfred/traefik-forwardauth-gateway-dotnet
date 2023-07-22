"use client";

import { useQuery } from "@tanstack/react-query";

export const useUserInfo = () =>
  useQuery<{
    userInfo: { userId: string; name: string };
    userEndpoints: { logout: string };
  }>(["user-info"], () =>
    fetch("/oauth2/user", { headers: { "CSRF-Token": "1" } }).then((res) =>
      res.json()
    )
  );

export const UserInfo = () => {
  const info = useUserInfo();

  if (!info.data) {
    return null;
  }

  return <div className="h-fit self-center">{info.data?.userInfo.name}</div>;
};
