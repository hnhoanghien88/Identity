# Quickstart Validation: Quản lý Menus dạng cây

## Prerequisites

- MySQL development database configured for `Identity.Api`.
- Valid administrator account with `Menus.View/Create/Update/Delete` capabilities.
- At least two active Applications and Resources belonging to each.

## Build and automated checks

```powershell
dotnet build Identity-api/Identity-api.slnx
dotnet test Identity-api/Identity-api.slnx
Set-Location Identity-client
npm test -- --run
npm run lint
npm run build
```

## End-to-end validation

1. Start API and client using repository development configuration; sign in as an authorized administrator.
2. Open `/menus`, select Application A and verify only its tree appears.
3. Create a root, child and grandchild; assign an Application A Resource to the child and reload.
4. Expand/collapse each branch and all branches using mouse and keyboard. Verify order is `SortOrder`, then Name.
5. Move the child to another valid root. Verify its grandchild follows and retains its relationship.
6. Attempt to set the child parent to its grandchild, select an Application B Resource, and reuse a duplicate Code. Each must fail without partial change.
7. Edit a leaf Menu, change Application from A to B, verify Parent and Resource choices reload for B and stale selections clear, then save and confirm the Menu moves from tree A to tree B.
8. Attempt the same Application change for a Menu with children and verify it is rejected without changing the branch.
9. Change a row's Order and move focus away without pressing Enter; verify one update request is sent and only that row receives the saved Order/Version.
10. Change Order and press Enter, then move focus; verify no duplicate request is sent, the tree is not refetched, expansion remains unchanged, and another row can be edited immediately.
7. Open the same Menu in two sessions, update one and then submit the stale form. Verify the stale update is rejected.
8. Attempt to delete a parent and verify it is blocked; delete a leaf and verify it disappears after reload.
9. Remove each Menus capability in turn and verify both UI actions and API endpoint access fail closed.
10. Seed or select a Menu with both Resource and Route set to `NULL`. Verify both index cells show `—`, no mojibake is present, open Edit, verify Resource is None and Route is empty, then save successfully without converting either value into display text.

Expected shapes and error statuses are defined in [contracts/menus.openapi.yaml](contracts/menus.openapi.yaml); entity rules are in [data-model.md](data-model.md).
