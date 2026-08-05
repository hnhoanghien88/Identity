CREATE TABLE `users`(
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `Email` VARCHAR(255) NOT NULL,
    `NormalizedEmail` VARCHAR(255) NOT NULL,
    `DisplayName` VARCHAR(255) NOT NULL,
    `PasswordHash` VARCHAR(500) NULL,
    `SecurityStamp` CHAR(36) NOT NULL,
    `PermissionVersion` INT NOT NULL DEFAULT 1,
    `CreatedBy` VARCHAR(255) NULL,
    `CreatedDate` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedBy` VARCHAR(255) NULL,
    `UpdatedDate` DATETIME(6) NULL,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    `IsDeleted` TINYINT(1) NOT NULL
);
ALTER TABLE
    `users` ADD UNIQUE `users_normalizedemail_unique`(`NormalizedEmail`);
CREATE TABLE `user_roles`(
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `UserId` BIGINT UNSIGNED NOT NULL,
    `RoleId` BIGINT UNSIGNED NOT NULL,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    `CreatedBy` VARCHAR(255) NULL,
    `CreatedDate` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedBy` VARCHAR(255) NULL,
    `UpdatedDate` DATETIME(6) NULL
);
ALTER TABLE
    `user_roles` ADD UNIQUE `user_roles_userid_roleid_unique`(`UserId`, `RoleId`);
ALTER TABLE
    `user_roles` ADD INDEX `user_roles_roleid_index`(`RoleId`);
CREATE TABLE `roles`(
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `ApplicationId` BIGINT UNSIGNED NOT NULL,
    `Code` VARCHAR(100) NOT NULL,
    `Name` VARCHAR(150) NOT NULL,
    `IsSystemRole` TINYINT(1) NOT NULL,
    `CreatedBy` VARCHAR(255) NULL,
    `CreatedDate` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedBy` VARCHAR(255) NULL,
    `UpdatedDate` DATETIME(6) NULL,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    `IsDeleted` TINYINT(1) NOT NULL
);
ALTER TABLE
    `roles` ADD UNIQUE `roles_applicationid_code_unique`(`ApplicationId`, `Code`);
CREATE TABLE `role_permissions`(
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `RoleId` BIGINT UNSIGNED NOT NULL,
    `PermissionId` BIGINT UNSIGNED NOT NULL,
    `CreatedBy` VARCHAR(255) NULL,
    `CreatedDate` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedBy` VARCHAR(255) NULL,
    `UpdatedDate` DATETIME(6) NULL
);
ALTER TABLE
    `role_permissions` ADD UNIQUE `role_permissions_roleid_permissionid_unique`(`RoleId`, `PermissionId`);
ALTER TABLE
    `role_permissions` ADD INDEX `role_permissions_permissionid_index`(`PermissionId`);
CREATE TABLE `resources`(
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `ApplicationId` BIGINT UNSIGNED NOT NULL,
    `Code` VARCHAR(120) NOT NULL,
    `Name` VARCHAR(150) NOT NULL,
    `ResourceType` VARCHAR(30) NOT NULL,
    `Description` VARCHAR(500) NULL,
    `CreatedBy` VARCHAR(255) NULL,
    `CreatedDate` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedBy` VARCHAR(255) NULL,
    `UpdatedDate` DATETIME(6) NULL,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    `IsDeleted` TINYINT(1) NOT NULL
);
ALTER TABLE
    `resources` ADD UNIQUE `resources_applicationid_code_unique`(`ApplicationId`, `Code`);
CREATE TABLE `refresh_tokens`(
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `UserId` BIGINT UNSIGNED NOT NULL,
    `ApplicationId` BIGINT UNSIGNED NOT NULL,
    `TokenHash` CHAR(64) NOT NULL,
    `JwtId` CHAR(36) NOT NULL,
    `FamilyId` CHAR(36) NOT NULL,
    `ExpiresDate` DATETIME(6) NOT NULL,
    `RevokedDate` DATETIME(6) NULL,
    `RevokedBy` BIGINT UNSIGNED NULL,
    `ReplacedByTokenId` BIGINT UNSIGNED NULL,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    `CreatedBy` VARCHAR(255) NULL,
    `CreatedDate` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedBy` VARCHAR(255) NULL,
    `UpdatedDate` DATETIME(6) NULL
);
ALTER TABLE
    `refresh_tokens` ADD INDEX `refresh_tokens_userid_index`(`UserId`);
ALTER TABLE
    `refresh_tokens` ADD INDEX `refresh_tokens_applicationid_index`(`ApplicationId`);
ALTER TABLE
    `refresh_tokens` ADD UNIQUE `refresh_tokens_tokenhash_unique`(`TokenHash`);
ALTER TABLE
    `refresh_tokens` ADD INDEX `refresh_tokens_replacedbytokenid_index`(`ReplacedByTokenId`);
CREATE TABLE `permissions`(
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `ApplicationId` BIGINT UNSIGNED NOT NULL,
    `ResourceId` BIGINT UNSIGNED NOT NULL,
    `ActionId` BIGINT UNSIGNED NOT NULL,
    `Code` VARCHAR(200) NOT NULL,
    `Name` VARCHAR(150) NOT NULL,
    `CreatedBy` VARCHAR(255) NULL,
    `CreatedDate` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedBy` VARCHAR(255) NULL,
    `UpdatedDate` DATETIME(6) NULL,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    `IsDeleted` TINYINT(1) NOT NULL
);
ALTER TABLE
    `permissions` ADD UNIQUE `permissions_resourceid_actionid_unique`(`ResourceId`, `ActionId`);
ALTER TABLE
    `permissions` ADD INDEX `permissions_applicationid_index`(`ApplicationId`);
ALTER TABLE
    `permissions` ADD INDEX `permissions_actionid_index`(`ActionId`);
CREATE TABLE `permission_actions`(
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `Code` VARCHAR(50) NOT NULL,
    `Name` VARCHAR(100) NOT NULL,
    `CreatedBy` VARCHAR(255) NULL,
    `CreatedDate` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedBy` VARCHAR(255) NULL,
    `UpdatedDate` DATETIME(6) NULL
);
ALTER TABLE
    `permission_actions` ADD UNIQUE `permission_actions_code_unique`(`Code`);
CREATE TABLE `menus`(
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `ApplicationId` BIGINT UNSIGNED NOT NULL,
    `ParentId` BIGINT UNSIGNED NULL,
    `ResourceId` BIGINT UNSIGNED NULL,
    `Code` VARCHAR(120) NOT NULL,
    `Name` VARCHAR(150) NOT NULL,
    `Route` VARCHAR(300) NULL,
    `Icon` VARCHAR(100) NULL,
    `SortOrder` INT NOT NULL,
    `IsVisible` TINYINT(1) NOT NULL DEFAULT 1,
    `CreatedBy` VARCHAR(255) NULL,
    `CreatedDate` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedBy` VARCHAR(255) NULL,
    `UpdatedDate` DATETIME(6) NULL,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    `IsDeleted` TINYINT(1) NOT NULL
);
ALTER TABLE
    `menus` ADD UNIQUE `menus_applicationid_code_unique`(`ApplicationId`, `Code`);
ALTER TABLE
    `menus` ADD INDEX `menus_parentid_index`(`ParentId`);
ALTER TABLE
    `menus` ADD INDEX `menus_resourceid_index`(`ResourceId`);
CREATE TABLE `applications`(
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `Code` VARCHAR(50) NOT NULL,
    `Name` VARCHAR(150) NOT NULL,
    `Audience` VARCHAR(150) NOT NULL,
    `Description` VARCHAR(500) NULL,
    `CreatedBy` VARCHAR(255) NULL,
    `CreatedDate` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedBy` VARCHAR(255) NULL,
    `UpdatedDate` DATETIME(6) NULL,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    `IsDeleted` TINYINT(1) NOT NULL
);
ALTER TABLE
    `applications` ADD UNIQUE `applications_code_unique`(`Code`);
ALTER TABLE
    `roles` ADD CONSTRAINT `roles_applicationid_foreign` FOREIGN KEY(`ApplicationId`) REFERENCES `applications`(`Id`);
ALTER TABLE
    `user_roles` ADD CONSTRAINT `user_roles_userid_foreign` FOREIGN KEY(`UserId`) REFERENCES `users`(`Id`);
ALTER TABLE
    `permissions` ADD CONSTRAINT `permissions_actionid_foreign` FOREIGN KEY(`ActionId`) REFERENCES `permission_actions`(`Id`);
ALTER TABLE
    `resources` ADD CONSTRAINT `resources_applicationid_foreign` FOREIGN KEY(`ApplicationId`) REFERENCES `applications`(`Id`);
ALTER TABLE
    `menus` ADD CONSTRAINT `menus_resourceid_foreign` FOREIGN KEY(`ResourceId`) REFERENCES `resources`(`Id`);
ALTER TABLE
    `permissions` ADD CONSTRAINT `permissions_resourceid_foreign` FOREIGN KEY(`ResourceId`) REFERENCES `resources`(`Id`);
ALTER TABLE
    `user_roles` ADD CONSTRAINT `user_roles_roleid_foreign` FOREIGN KEY(`RoleId`) REFERENCES `roles`(`Id`);
ALTER TABLE
    `refresh_tokens` ADD CONSTRAINT `refresh_tokens_applicationid_foreign` FOREIGN KEY(`ApplicationId`) REFERENCES `applications`(`Id`);
ALTER TABLE
    `role_permissions` ADD CONSTRAINT `role_permissions_roleid_foreign` FOREIGN KEY(`RoleId`) REFERENCES `roles`(`Id`);
ALTER TABLE
    `menus` ADD CONSTRAINT `menus_parentid_foreign` FOREIGN KEY(`ParentId`) REFERENCES `menus`(`Id`);
ALTER TABLE
    `permissions` ADD CONSTRAINT `permissions_applicationid_foreign` FOREIGN KEY(`ApplicationId`) REFERENCES `applications`(`Id`);
ALTER TABLE
    `refresh_tokens` ADD CONSTRAINT `refresh_tokens_replacedbytokenid_foreign` FOREIGN KEY(`ReplacedByTokenId`) REFERENCES `refresh_tokens`(`Id`);
ALTER TABLE
    `role_permissions` ADD CONSTRAINT `role_permissions_permissionid_foreign` FOREIGN KEY(`PermissionId`) REFERENCES `permissions`(`Id`);
ALTER TABLE
    `menus` ADD CONSTRAINT `menus_applicationid_foreign` FOREIGN KEY(`ApplicationId`) REFERENCES `applications`(`Id`);