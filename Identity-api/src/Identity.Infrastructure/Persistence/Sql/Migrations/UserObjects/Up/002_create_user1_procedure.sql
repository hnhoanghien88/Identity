CREATE PROCEDURE sp_get_all_users()
SELECT
    Id,
    Code,
    Name,
    CreatedDate,
    IsActive
FROM users;
