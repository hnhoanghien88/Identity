import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { ApplicationsPage } from "../../src/features/applications";

const api = vi.hoisted(() => ({
  searchApplications: vi.fn(),
  createApplication: vi.fn(),
  updateApplication: vi.fn(),
  deleteApplication: vi.fn(),
}));

vi.mock("../../src/features/applications/api/applicationsApi", () => api);

const session = (
  permissions = [
    "Applications.Read",
    "Applications.Create",
    "Applications.Update",
    "Applications.Delete",
  ],
) => ({
  accessToken: "authenticated-token",
  authorization: { permissions },
});

describe("ApplicationsPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    api.searchApplications.mockResolvedValue({
      items: [
        {
          id: 1,
          code: "PORTAL",
          name: "Portal",
          audience: "portal-api",
          description: null,
          createdDate: "2026-08-11T00:00:00Z",
          isActive: true,
          version: 1,
        },
      ],
      totalCount: 1,
      page: 1,
      pageSize: 20,
    });
  });

  it("allows an authenticated user to load and manage Applications", async () => {
    render(<ApplicationsPage session={session()} onLogout={vi.fn()} />);

    expect(await screen.findByText("PORTAL")).toBeInTheDocument();
    expect(screen.getByText("portal-api")).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /create application/i }),
    ).toBeInTheDocument();
    expect(screen.getByLabelText(/edit PORTAL/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/delete PORTAL/i)).toBeInTheDocument();
  });

  it("sends Code, Name, Audience and status filters", async () => {
    const user = userEvent.setup();
    render(
      <ApplicationsPage
        session={session(["Applications.Read"])}
        onLogout={vi.fn()}
      />,
    );
    await screen.findByText("PORTAL");

    await user.type(screen.getByLabelText("Code"), "POR");
    await user.type(screen.getByLabelText("Name"), "Portal");
    await user.type(screen.getByLabelText("Audience"), "api");
    await user.click(screen.getByLabelText("Status"));
    await user.click(screen.getByRole("option", { name: "Active" }));
    await user.click(screen.getByRole("button", { name: "Apply filters" }));

    await waitFor(() =>
      expect(api.searchApplications).toHaveBeenLastCalledWith(
        expect.objectContaining({
          filter: {
            code: { contains: "POR" },
            name: { contains: "Portal" },
            audience: { contains: "api" },
            isActive: true,
          },
        }),
        expect.any(AbortSignal),
      ),
    );
  });
});
