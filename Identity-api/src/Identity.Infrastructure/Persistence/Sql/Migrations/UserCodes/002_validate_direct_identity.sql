CREATE TEMPORARY TABLE user_identity_preflight (
    Code VARCHAR(50) COLLATE utf8mb4_unicode_ci NOT NULL,
    Email VARCHAR(254) COLLATE utf8mb4_unicode_ci NOT NULL,
    UNIQUE KEY UQPreflightCode (Code),
    UNIQUE KEY UQPreflightEmail (Email),
    CONSTRAINT CKPreflightCode CHECK (
        Code REGEXP '^[A-Za-z0-9._-]{1,50}$'
    ),
    CONSTRAINT CKPreflightEmail CHECK (
        CHAR_LENGTH(TRIM(Email)) BETWEEN 3 AND 254
        AND Email = TRIM(Email)
    )
);

INSERT INTO user_identity_preflight (Code, Email)
SELECT Code, Email
FROM users;

DROP TEMPORARY TABLE user_identity_preflight;
