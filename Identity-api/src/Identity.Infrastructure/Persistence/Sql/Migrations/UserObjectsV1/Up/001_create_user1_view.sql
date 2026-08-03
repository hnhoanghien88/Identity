CREATE OR REPLACE VIEW v_get_users1 AS
SELECT
    Id,
    Code,
    Name,
    CreatedDate,
    IsActive
FROM users
WHERE IsActive = 1;