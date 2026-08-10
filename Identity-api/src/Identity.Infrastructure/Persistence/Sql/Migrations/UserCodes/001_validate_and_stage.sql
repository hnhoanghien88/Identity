-- Run after migrating to 20260810085053_AddUserCodeIdentity.
-- Replace the example values with a reviewed mapping for every existing user.
-- Do not derive production Codes automatically from Email.

INSERT INTO user_code_mappings (UserId, Code)
VALUES
    (1, 'admin')
ON DUPLICATE KEY UPDATE
    Code = VALUES(Code);

SELECT
    u.Id,
    u.Email
FROM users AS u
LEFT JOIN user_code_mappings AS m ON m.UserId = u.Id
WHERE m.UserId IS NULL;

SELECT
    UserId,
    Code
FROM user_code_mappings
WHERE Code NOT REGEXP '^[A-Za-z0-9._-]{1,50}$';

SELECT
    UPPER(Code) AS NormalizedCode,
    COUNT(*) AS DuplicateCount
FROM user_code_mappings
GROUP BY UPPER(Code)
HAVING COUNT(*) > 1;
