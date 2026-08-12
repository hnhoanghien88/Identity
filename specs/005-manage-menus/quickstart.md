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
7. Open the same Menu in two sessions, update one and then submit the stale form. Verify the stale update is rejected.
8. Attempt to delete a parent and verify it is blocked; delete a leaf and verify it disappears after reload.
9. Remove each Menus capability in turn and verify both UI actions and API endpoint access fail closed.

Expected shapes and error statuses are defined in [contracts/menus.openapi.yaml](contracts/menus.openapi.yaml); entity rules are in [data-model.md](data-model.md).

