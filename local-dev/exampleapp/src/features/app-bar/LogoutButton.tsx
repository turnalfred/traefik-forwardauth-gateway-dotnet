"use client";

import { Popover } from "@headlessui/react";
import { useQuery } from "@tanstack/react-query";
import Link from "next/link";
import { useState } from "react";
import { usePopper } from "react-popper";
import { useUserInfo } from "./UserInfo";

export const LogoutButton = () => {
  let [referenceElement, setReferenceElement] =
    useState<HTMLButtonElement | null>(null);
  let [popperElement, setPopperElement] = useState<HTMLDivElement | null>(null);
  let { styles, attributes } = usePopper(referenceElement, popperElement, {
    placement: "bottom-start",
  });

  const info = useUserInfo();

  if (!info.data) {
    return null;
  }

  return (
    <Popover className="relative">
      {({ close }) => (
        <>
          <Popover.Button ref={setReferenceElement}>
            <button className="rounded-lg bg-green-800 hover:bg-green-700 px-4 py-2">
              Logout
            </button>
          </Popover.Button>

          <Popover.Panel
            className="absolute z-10 bg-green-800 p-4 mt-2 w-[200px] rounded-lg"
            ref={setPopperElement}
            style={styles.popper}
            {...attributes.popper}
          >
            <p className="mb-4">Are you sure?</p>
            <div className="flex flex-row gap-4 justify-end">
              <button
                onClick={() => close()}
                className="py-2 px-4 rounded-lg bg-gray-500"
              >
                No
              </button>
              <Link
                href={info.data?.userEndpoints.logout ?? ""}
                className="py-2 px-4 rounded-lg bg-blue-500"
                prefetch={false}
              >
                Yes
              </Link>
            </div>
          </Popover.Panel>
        </>
      )}
    </Popover>
  );
};
