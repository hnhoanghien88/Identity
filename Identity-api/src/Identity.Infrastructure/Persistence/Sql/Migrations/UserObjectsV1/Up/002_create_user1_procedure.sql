CREATE PROCEDURE sp_get_users1()
SELECT
    Id,
    Code,
    Name,
    CreatedDate,
    IsActive
FROM users
WHERE IsActive = 1;
