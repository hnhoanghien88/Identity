# Data Model: Quản lý User theo Role

## Role

Existing aggregate selected as membership owner.

| Field | Type | Rules |
|---|---|---|
| Id | unsigned integer | Positive identifier |
| Code | text | Visible stable sort key |
| Name | text | Supporting display text |
| IsActive | boolean | Must be true |
| IsDeleted | boolean | Must be false |

## User

Existing account used as member or candidate.

| Field | Type | Rules |
|---|---|---|
| Id | unsigned integer | Positive identifier |
| Code | text | Primary visible identity and sort key |
| Name | text | Display field |
| Email | text | Supporting field |
| IsActive | boolean | Must be true |
| IsDeleted | boolean | Must be false |

## User Role Membership (`user_roles`)

| Field | Type | Rules |
|---|---|---|
| Id | unsigned integer | Existing surrogate key |
| UserId | unsigned integer | Required FK to User |
| RoleId | unsigned integer | Required FK to Role |
| IsActive | boolean | New assignments are true |
| Audit fields | timestamps/text | Project conventions |

### Relationships and invariants

- Role and User each have zero or many memberships.
- `(UserId, RoleId)` is unique and is the concurrency boundary.
- Assignment requires active, non-deleted Role and Users.
- Existing assignment is idempotent success; removal only deletes the association.
- Batch assignment validates all input before one commit.

### State transitions

```text
Absent --assign--> Present(active)
Present(active) --assign/retry--> Present(active)
Present(active) --remove--> Absent
Absent --remove/retry--> Absent
```

## Read models

- **UserRoleMemberDto**: `userId`, `code`, `name`, `email`.
- **PagedUserRoleMembersDto**: `items`, `page`, `pageSize`, `totalCount`.
- **UserRoleCandidateDto**: public identity fields, excluding members.
- **AssignUsersToRoleResultDto**: `roleId`, `assignedUserIds`, `alreadyAssignedUserIds`.

Passwords, hashes, tokens and authorization details are never included.
