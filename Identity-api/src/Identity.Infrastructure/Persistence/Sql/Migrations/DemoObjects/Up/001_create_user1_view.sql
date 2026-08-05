CREATE OR REPLACE VIEW v_get_all_users AS
SELECT
    Id,
    Code,
    Name,
    CreatedDate,
    IsActive
FROM users;