import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { ResourcesPage } from "../../src/features/resources";

const resourceApi = vi.hoisted(() => ({
  searchResources: vi.fn(),
  createResource: vi.fn(),
  updateResource: vi.fn(),
  deleteResource: vi.fn(),
}));
const applicationApi = vi.hoisted(() => ({
  searchApplications: vi.fn(),
}));

vi.mock("../../src/features/resources/api/resourcesApi", () => resourceApi);
vi.mock(
  "../../src/features/applications/api/applicationsApi",
  () => applicationApi,
);

const session = (permissions) => ({
  accessToken: "token",
  authorization: {
    permissions,
  },
});

describe("ResourcesPage list", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    resourceApi.searchResources.mockResolvedValue({
      items: [
        {
          id: 1,
          applicationId: 2,
          applicationCode: "PORTAL",
          applicationName: "Portal",
          code: "USERS",
          name: "Users",
          resourceType: "Api",
          description: null,
          createdDate: "2026-08-11T00:00:00Z",
          isActive: true,
          version: 1,
        },
      ],
      totalCount: 1,
    });
    applicationApi.searchApplications.mockResolvedValue({
      items: [
        {
          id: 2,
          code: "PORTAL",
          name: "Portal",
        },
      ],
    });
  });

  it("loads rows and applies scoped filters", async () => {
    const user = userEvent.setup();
    render(
      <ResourcesPage
        session={session([
          "Resources.Read",
          "Resources.Create",
          "Resources.Update",
          "Resources.Delete",
        ])}
      />,
    );

    expect(await screen.findByText("USERS")).toBeInTheDocument();
    expect(applicationApi.searchApplications).not.toHaveBeenCalled();
    await user.type(screen.getByLabelText("Code"), "USE");
    await user.type(screen.getByLabelText("Resource Type"), "Api");
    await user.click(screen.getByRole("button", { name: "Apply filters" }));

    await waitFor(() =>
      expect(resourceApi.searchResources).toHaveBeenLastCalledWith(
        expect.objectContaining({
          filter: expect.objectContaining({
            code: {
              contains: "USE",
            },
            resourceType: {
              contains: "Api",
            },
          }),
        }),
        expect.any(AbortSignal),
      ),
    );
  });

  it("denies access when Resources.Read permission is absent", () => {
    render(<ResourcesPage session={session([])} />);
    expect(
      screen.getByText("You do not have permission to view Resources."),
    ).toBeInTheDocument();
    expect(resourceApi.searchResources).not.toHaveBeenCalled();
  });
});
