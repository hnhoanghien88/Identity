-- MySQL dump 10.13  Distrib 8.0.46, for Win64 (x86_64)
--
-- Host: localhost    Database: identity_db
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20260805063246_InitialDB','10.0.7'),('20260810070318_AddUserVersion','10.0.7'),('20260810085053_AddUserCodeIdentity','10.0.7'),('20260810090000_EnforceUserCodeIdentity','10.0.7'),('20260810093448_RemoveNormalizedUserIdentity','10.0.7'),('20260811054449_AddApplicationVersion','10.0.7'),('20260811094606_AddResourceVersion','10.0.7'),('20260812031658_AddPermissionActionVersion','10.0.7'),('20260812040001_AddMenuVersion','10.0.7'),('20260812055052_AddRoleVersion','10.0.7'),('20260812065014_RemovePermissionApplicationId','10.0.7'),('20260812080000_RemovePermissionIsDeleted','10.0.7'),('20260812081000_RemovePermissionIsActive','10.0.7'),('20260819033916_ChangeMenuVisibilitySemantics','10.0.7'),('20260819042959_AddRateLimitPolicies','10.0.7'),('20260819062104_AddRateLimitManagementAccess','10.0.7'),('20260819062356_AddLoginRateLimitPolicy','10.0.7'),('20260819090932_AddCascadeSoftDelete','10.0.7'),('20260820035757_AddGoogleExternalIdentity','10.0.7');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `applications`
--

DROP TABLE IF EXISTS `applications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `applications` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Code` varchar(50) NOT NULL,
  `Name` varchar(150) NOT NULL,
  `Audience` varchar(150) NOT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `CreatedBy` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `Version` bigint unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQApplicationsCode` (`Code`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `applications`
--

LOCK TABLES `applications` WRITE;
/*!40000 ALTER TABLE `applications` DISABLE KEYS */;
INSERT INTO `applications` VALUES (1,'Identity','Identity API','Identity','Main authentication application','system@company.com','2026-08-05 14:20:19.881534','admin','2026-08-11 07:10:58.047105',1,0,2),(2,'Identity-client','Identity client','Identity-client','Identity client','admin','2026-08-11 07:11:28.557225',NULL,NULL,1,0,1);
/*!40000 ALTER TABLE `applications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `external_identities`
--

DROP TABLE IF EXISTS `external_identities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `external_identities` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` bigint unsigned NOT NULL,
  `Provider` varchar(30) NOT NULL,
  `ProviderSubject` varchar(255) NOT NULL,
  `EmailAtLinkTime` varchar(254) NOT NULL,
  `DisplayName` varchar(255) NOT NULL,
  `AvatarUrl` varchar(2048) DEFAULT NULL,
  `LastLoginDate` datetime(6) NOT NULL,
  `CreatedBy` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQExternalIdentitiesProviderSubject` (`Provider`,`ProviderSubject`),
  UNIQUE KEY `UQExternalIdentitiesUserProvider` (`UserId`,`Provider`),
  CONSTRAINT `FK_external_identities_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `external_identities`
--

LOCK TABLES `external_identities` WRITE;
/*!40000 ALTER TABLE `external_identities` DISABLE KEYS */;
INSERT INTO `external_identities` VALUES (1,9,'Google','103860866659095721435','hnhoanghien88@gmail.com','Hiển Hoàng','https://lh3.googleusercontent.com/a/ACg8ocJ55W_jBiCZxnlpLSobXrnss5nTJ2JrinNLFJDeclDe24zqOXM=s96-c','2026-08-20 07:01:04.612796','external:google','2026-08-20 07:01:04.612796',NULL,NULL,1,0);
/*!40000 ALTER TABLE `external_identities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `menus`
--

DROP TABLE IF EXISTS `menus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menus` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ApplicationId` bigint unsigned NOT NULL,
  `ParentId` bigint unsigned DEFAULT NULL,
  `ResourceId` bigint unsigned DEFAULT NULL,
  `Code` varchar(120) NOT NULL,
  `Name` varchar(150) NOT NULL,
  `Route` varchar(300) DEFAULT NULL,
  `Icon` varchar(100) DEFAULT NULL,
  `SortOrder` int NOT NULL DEFAULT '0',
  `IsVisible` tinyint(1) NOT NULL DEFAULT '0',
  `CreatedBy` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `Version` bigint unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQMenus` (`ApplicationId`,`Code`),
  KEY `IX_menus_ParentId` (`ParentId`),
  KEY `IX_menus_ResourceId` (`ResourceId`),
  CONSTRAINT `FK_menus_applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `applications` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_menus_menus_ParentId` FOREIGN KEY (`ParentId`) REFERENCES `menus` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_menus_resources_ResourceId` FOREIGN KEY (`ResourceId`) REFERENCES `resources` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menus`
--

LOCK TABLES `menus` WRITE;
/*!40000 ALTER TABLE `menus` DISABLE KEYS */;
INSERT INTO `menus` VALUES (1,1,NULL,5,'DASHBOARD','Dashboard','/dashboard','Dashboard',1,1,'system@company.com','2026-08-05 14:20:19.896678','admin','2026-08-13 03:24:06.702849',0,1,2),(2,1,NULL,NULL,'Identity','Identity',NULL,'AdminPanelSettings',1,1,'system@company.com','2026-08-05 14:20:19.896678',NULL,'2026-08-19 03:43:06.999959',1,0,5),(3,1,2,1,'users','Users','/users','People',8,0,'system@company.com','2026-08-05 14:20:19.896678','Codex','2026-08-13 08:08:42.000000',1,0,5),(4,1,2,2,'roles','Roles','/roles','Security',6,0,'system@company.com','2026-08-05 14:20:19.896678','Codex','2026-08-13 08:08:42.000000',1,0,4),(5,1,2,3,'role-permissions','Role permissions','/role-permissions','Lock',7,0,'system@company.com','2026-08-05 14:20:19.896678','Codex','2026-08-13 08:08:42.000000',1,0,3),(6,1,2,5,'applications','Applications','/applications','Apps',2,0,NULL,'2026-08-13 03:32:27.606614','Codex','2026-08-13 08:08:42.000000',1,0,2),(7,1,2,4,'resources','Resources','/resources','Category',3,0,NULL,'2026-08-13 03:32:51.376781','Codex','2026-08-13 08:08:42.000000',1,0,2),(8,1,2,6,'menus','Menus','/menus','AccountTree',4,0,NULL,'2026-08-13 03:33:17.417406','Codex','2026-08-13 08:08:42.000000',1,0,2),(9,1,2,7,'actions','Actions','/actions','Bolt',5,0,NULL,'2026-08-13 03:33:56.016426','Codex','2026-08-13 08:08:42.000000',1,0,2),(10,1,2,8,'user-roles','User roles','/user-roles','ManageAccounts',9,0,NULL,'2026-08-13 03:34:25.588780',NULL,'2026-08-19 03:23:25.745132',1,0,6),(11,1,2,30010,'rate-limiting','Rate limiting','/rate-limiting',NULL,10,0,NULL,'2026-08-19 06:09:11.106776',NULL,'2026-08-19 07:19:24.853375',1,0,2),(12,1,2,30011,'rate-limits','Rate Limiting','/rate-limits','Speed',9,0,'migration','2026-08-19 06:21:57.558009',NULL,'2026-08-19 09:31:32.675689',0,1,2);
/*!40000 ALTER TABLE `menus` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `permission_actions`
--

DROP TABLE IF EXISTS `permission_actions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permission_actions` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Code` varchar(50) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `CreatedBy` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `Version` bigint unsigned NOT NULL DEFAULT '1',
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQPermissionActions` (`Code`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permission_actions`
--

LOCK TABLES `permission_actions` WRITE;
/*!40000 ALTER TABLE `permission_actions` DISABLE KEYS */;
INSERT INTO `permission_actions` VALUES (1,'Read','Read','system@company.com','2026-08-05 14:20:19.887446','admin','2026-08-12 03:34:04.399091',2,1,0),(2,'Create','Create','system@company.com','2026-08-05 14:20:19.887446','admin','2026-08-12 03:34:09.515977',2,1,0),(3,'Update','Update','system@company.com','2026-08-05 14:20:19.887446','admin','2026-08-12 03:34:13.805456',2,1,0),(4,'Delete','Delete','system@company.com','2026-08-05 14:20:19.887446','admin','2026-08-12 03:34:17.504327',2,1,0),(5,'Export','Export','system@company.com','2026-08-05 14:20:19.887446','admin','2026-08-12 08:46:20.910419',3,1,0),(6,'Import','Import','admin','2026-08-12 03:33:57.587650',NULL,NULL,1,1,0),(7,'ViewMenu','View menu',NULL,'2026-08-13 07:52:12.173669',NULL,NULL,1,1,0);
/*!40000 ALTER TABLE `permission_actions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `permissions`
--

DROP TABLE IF EXISTS `permissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permissions` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ResourceId` bigint unsigned NOT NULL,
  `ActionId` bigint unsigned NOT NULL,
  `Code` varchar(200) NOT NULL,
  `Name` varchar(150) NOT NULL,
  `CreatedBy` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQPermissions` (`ResourceId`,`ActionId`),
  KEY `IX_permissions_ActionId` (`ActionId`),
  CONSTRAINT `FK_permissions_permission_actions_ActionId` FOREIGN KEY (`ActionId`) REFERENCES `permission_actions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_permissions_resources_ResourceId` FOREIGN KEY (`ResourceId`) REFERENCES `resources` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=103 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permissions`
--

LOCK TABLES `permissions` WRITE;
/*!40000 ALTER TABLE `permissions` DISABLE KEYS */;
INSERT INTO `permissions` VALUES (31,1,2,'Users.Create','Create','admin','2026-08-12 08:45:12.638802',NULL,NULL,1,0),(32,1,4,'Users.Delete','Delete','admin','2026-08-12 08:45:13.294289',NULL,NULL,1,0),(35,1,1,'Users.Read','Read','admin','2026-08-12 08:45:14.678380',NULL,NULL,1,0),(36,1,3,'Users.Update','Update','admin','2026-08-12 08:45:15.129754',NULL,NULL,1,0),(37,1,5,'Users.Export','Export','admin','2026-08-12 08:46:26.825029',NULL,NULL,1,0),(38,1,6,'Users.Import','Import','admin','2026-08-12 08:46:27.181055',NULL,NULL,1,0),(39,2,2,'Roles.Create','Create','admin','2026-08-13 03:27:42.661385',NULL,NULL,1,0),(40,2,4,'Roles.Delete','Delete','admin','2026-08-13 03:27:43.173279',NULL,NULL,1,0),(41,2,5,'Roles.Export','Export','admin','2026-08-13 03:27:43.575718',NULL,NULL,1,0),(42,2,6,'Roles.Import','Import','admin','2026-08-13 03:27:43.998142',NULL,NULL,1,0),(43,2,1,'Roles.Read','Read','admin','2026-08-13 03:27:44.454131',NULL,NULL,1,0),(44,2,3,'Roles.Update','Update','admin','2026-08-13 03:27:44.953910',NULL,NULL,1,0),(45,3,2,'RolePermissions.Create','Create','admin','2026-08-13 03:27:46.397841',NULL,NULL,1,0),(46,3,4,'RolePermissions.Delete','Delete','admin','2026-08-13 03:27:46.704386',NULL,NULL,1,0),(47,3,5,'RolePermissions.Export','Export','admin','2026-08-13 03:27:47.164911',NULL,NULL,1,0),(48,3,6,'RolePermissions.Import','Import','admin','2026-08-13 03:27:47.513693',NULL,NULL,1,0),(49,3,1,'RolePermissions.Read','Read','admin','2026-08-13 03:27:47.911936',NULL,NULL,1,0),(50,3,3,'RolePermissions.Update','Update','admin','2026-08-13 03:27:48.297092',NULL,NULL,1,0),(51,4,2,'Resources.Create','Create','admin','2026-08-13 03:27:49.963218',NULL,NULL,1,0),(52,4,4,'Resources.Delete','Delete','admin','2026-08-13 03:27:50.273006',NULL,NULL,1,0),(53,4,5,'Resources.Export','Export','admin','2026-08-13 03:27:51.192149',NULL,NULL,1,0),(54,4,6,'Resources.Import','Import','admin','2026-08-13 03:27:51.523353',NULL,NULL,1,0),(55,4,1,'Resources.Read','Read','admin','2026-08-13 03:27:51.893946',NULL,NULL,1,0),(56,4,3,'Resources.Update','Update','admin','2026-08-13 03:27:52.235820',NULL,NULL,1,0),(57,5,2,'Applications.Create','Create','admin','2026-08-13 03:27:53.828000',NULL,NULL,1,0),(58,5,4,'Applications.Delete','Delete','admin','2026-08-13 03:27:54.142014',NULL,NULL,1,0),(59,5,5,'Applications.Export','Export','admin','2026-08-13 03:27:54.499950',NULL,NULL,1,0),(60,5,6,'Applications.Import','Import','admin','2026-08-13 03:27:54.907956',NULL,NULL,1,0),(61,5,1,'Applications.Read','Read','admin','2026-08-13 03:27:55.345847',NULL,NULL,1,0),(62,5,3,'Applications.Update','Update','admin','2026-08-13 03:27:55.737647',NULL,NULL,1,0),(63,6,2,'Menus.Create','Create','admin','2026-08-13 03:27:57.412933',NULL,NULL,1,0),(64,6,4,'Menus.Delete','Delete','admin','2026-08-13 03:27:57.710922',NULL,NULL,1,0),(65,6,5,'Menus.Export','Export','admin','2026-08-13 03:27:58.203098',NULL,NULL,1,0),(66,6,6,'Menus.Import','Import','admin','2026-08-13 03:27:58.545168',NULL,NULL,1,0),(67,6,1,'Menus.Read','Read','admin','2026-08-13 03:27:58.902569',NULL,NULL,1,0),(68,6,3,'Menus.Update','Update','admin','2026-08-13 03:27:59.345391',NULL,NULL,1,0),(69,7,2,'Actions.Create','Create','admin','2026-08-13 03:28:00.894350',NULL,NULL,1,0),(70,7,4,'Actions.Delete','Delete','admin','2026-08-13 03:28:01.225165',NULL,NULL,1,0),(71,7,5,'Actions.Export','Export','admin','2026-08-13 03:28:01.932708',NULL,NULL,1,0),(72,7,6,'Actions.Import','Import','admin','2026-08-13 03:28:02.251178',NULL,NULL,1,0),(73,7,1,'Actions.Read','Read','admin','2026-08-13 03:28:02.644090',NULL,NULL,1,0),(74,7,3,'Actions.Update','Update','admin','2026-08-13 03:28:03.020084',NULL,NULL,1,0),(75,8,2,'UserRoles.Create','Create','admin','2026-08-13 03:28:04.768074',NULL,NULL,1,0),(76,8,4,'UserRoles.Delete','Delete','admin','2026-08-13 03:28:05.119661',NULL,NULL,1,0),(77,8,5,'UserRoles.Export','Export','admin','2026-08-13 03:28:05.586041',NULL,NULL,1,0),(78,8,6,'UserRoles.Import','Import','admin','2026-08-13 03:28:05.953054',NULL,NULL,1,0),(79,8,1,'UserRoles.Read','Read','admin','2026-08-13 03:28:06.465977',NULL,NULL,1,0),(80,8,3,'UserRoles.Update','Update','admin','2026-08-13 03:28:06.782470',NULL,NULL,1,0),(81,1,7,'Users.ViewMenu','View menu',NULL,'2026-08-13 07:52:25.722384',NULL,NULL,1,0),(82,2,7,'Roles.ViewMenu','View menu',NULL,'2026-08-13 07:52:27.364906',NULL,NULL,1,0),(83,3,7,'RolePermissions.ViewMenu','View menu',NULL,'2026-08-13 07:52:28.606746',NULL,NULL,1,0),(84,4,7,'Resources.ViewMenu','View menu',NULL,'2026-08-13 07:52:30.023223',NULL,NULL,1,0),(85,5,7,'Applications.ViewMenu','View menu',NULL,'2026-08-13 07:52:31.247910',NULL,NULL,1,0),(86,6,7,'Menus.ViewMenu','View menu',NULL,'2026-08-13 07:52:32.530210',NULL,NULL,1,0),(87,7,7,'Actions.ViewMenu','View menu',NULL,'2026-08-13 07:52:33.879661',NULL,NULL,1,0),(88,8,7,'UserRoles.ViewMenu','View menu',NULL,'2026-08-13 07:52:35.286822',NULL,NULL,1,0),(89,30010,2,'RateLimiting.Create','Create',NULL,'2026-08-19 06:12:45.142408',NULL,NULL,1,0),(90,30010,4,'RateLimiting.Delete','Delete',NULL,'2026-08-19 06:12:45.579499',NULL,NULL,1,0),(91,30010,5,'RateLimiting.Export','Export',NULL,'2026-08-19 06:12:45.883561',NULL,NULL,1,0),(92,30010,6,'RateLimiting.Import','Import',NULL,'2026-08-19 06:12:46.259808',NULL,NULL,1,0),(93,30010,1,'RateLimiting.Read','Read',NULL,'2026-08-19 06:12:46.647913',NULL,NULL,1,0),(94,30010,3,'RateLimiting.Update','Update',NULL,'2026-08-19 06:12:47.246024',NULL,NULL,1,0),(95,30010,7,'RateLimiting.ViewMenu','View menu',NULL,'2026-08-19 06:12:47.606050',NULL,NULL,1,0),(96,30011,1,'RateLimits.Read','Read','migration','2026-08-19 06:21:57.560756',NULL,'2026-08-19 09:31:32.675689',0,1),(97,30011,2,'RateLimits.Create','Create','migration','2026-08-19 06:21:57.560756',NULL,'2026-08-19 09:31:32.675689',0,1),(98,30011,3,'RateLimits.Update','Update','migration','2026-08-19 06:21:57.560756',NULL,'2026-08-19 09:31:32.675689',0,1),(99,30011,4,'RateLimits.Delete','Delete','migration','2026-08-19 06:21:57.560756',NULL,'2026-08-19 09:31:32.675689',0,1),(100,30011,7,'RateLimits.ViewMenu','View menu','migration','2026-08-19 06:21:57.560756',NULL,'2026-08-19 09:31:32.675689',0,1);
/*!40000 ALTER TABLE `permissions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rate_limit_policies`
--

DROP TABLE IF EXISTS `rate_limit_policies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rate_limit_policies` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ApplicationId` bigint unsigned DEFAULT NULL,
  `Name` varchar(150) NOT NULL,
  `RoutePattern` varchar(300) NOT NULL,
  `HttpMethods` varchar(100) DEFAULT NULL,
  `PartitionBy` varchar(100) NOT NULL,
  `Algorithm` varchar(30) NOT NULL,
  `PermitLimit` int unsigned NOT NULL,
  `WindowSeconds` int unsigned NOT NULL,
  `BurstLimit` int unsigned DEFAULT NULL,
  `Priority` int NOT NULL,
  `Version` bigint unsigned NOT NULL DEFAULT '1',
  `CreatedBy` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQRateLimitPoliciesName` (`Name`),
  KEY `IXRateLimitPoliciesActivePriority` (`IsActive`,`Priority`),
  KEY `IXRateLimitPoliciesMatch` (`ApplicationId`,`RoutePattern`),
  CONSTRAINT `FK_rate_limit_policies_applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `applications` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rate_limit_policies`
--

LOCK TABLES `rate_limit_policies` WRITE;
/*!40000 ALTER TABLE `rate_limit_policies` DISABLE KEYS */;
INSERT INTO `rate_limit_policies` VALUES (1,1,'Identity API - Default','*',NULL,'User,Application,Endpoint','TokenBucket',300,60,30,0,1,'migration','2026-08-19 04:30:47.468764',NULL,NULL,1,0),(2,1,'Identity API - Authorization','authorization','GET','IpAddress,Application,Endpoint','SlidingWindow',30,60,NULL,100,1,'migration','2026-08-19 04:30:47.468764',NULL,NULL,1,0),(3,1,'Identity API - Write Operations','api/**','POST,PUT,PATCH,DELETE','User,IpAddress,Application,Endpoint','TokenBucket',100,60,20,50,1,'migration','2026-08-19 04:30:47.468764',NULL,NULL,1,0),(4,1,'Identity API - Login','login','POST','IpAddress,Application,Endpoint','FixedWindow',5,60,NULL,200,1,'migration','2026-08-19 06:24:26.766555',NULL,NULL,1,0),(5,1,'Identity API - Google External Login','external-login/**','GET','IpAddress,Application,Endpoint','SlidingWindow',20,300,NULL,190,1,'migration','2026-08-20 04:18:50.138755',NULL,NULL,1,0);
/*!40000 ALTER TABLE `rate_limit_policies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `refresh_tokens`
--

DROP TABLE IF EXISTS `refresh_tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `refresh_tokens` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` bigint unsigned NOT NULL,
  `ApplicationId` bigint unsigned NOT NULL,
  `TokenHash` char(64) NOT NULL,
  `JwtId` char(36) NOT NULL,
  `FamilyId` char(36) NOT NULL,
  `ExpiresDate` datetime(6) NOT NULL,
  `RevokedDate` datetime(6) DEFAULT NULL,
  `RevokedBy` bigint unsigned DEFAULT NULL,
  `ReplacedByTokenId` bigint unsigned DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedBy` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQRefreshTokenHash` (`TokenHash`),
  KEY `IX_refresh_tokens_ApplicationId` (`ApplicationId`),
  KEY `IX_refresh_tokens_ReplacedByTokenId` (`ReplacedByTokenId`),
  KEY `IX_refresh_tokens_UserId` (`UserId`),
  CONSTRAINT `FK_refresh_tokens_applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `applications` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_refresh_tokens_refresh_tokens_ReplacedByTokenId` FOREIGN KEY (`ReplacedByTokenId`) REFERENCES `refresh_tokens` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_refresh_tokens_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=214 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `refresh_tokens`
--

LOCK TABLES `refresh_tokens` WRITE;
/*!40000 ALTER TABLE `refresh_tokens` DISABLE KEYS */;
INSERT INTO `refresh_tokens` VALUES (31,1,1,'0309f6cde3578903e2a540a49097eea5c861b8f45447ee6bda9049dbe1e95738','5d3c16c9-7c51-4b48-a36b-4375668782cf','b72b98e7-f5a4-4dd6-9fe7-ec84d0c92b72','2026-08-17 06:00:40.483918','2026-08-10 06:00:49.027648',1,32,0,'admin','2026-08-10 06:00:40.483918','admin','2026-08-10 06:00:49.027648',0),(32,1,1,'6e7eca5d8c978279c7c0e10c99cd72016b6777372341cc4990f2af9d406d53d6','dfb7c80c-7daf-4b5c-9195-3a03886ce242','b72b98e7-f5a4-4dd6-9fe7-ec84d0c92b72','2026-08-17 06:00:49.027648','2026-08-10 07:53:41.141955',1,NULL,0,'admin','2026-08-10 06:00:49.027648','admin','2026-08-10 07:53:41.141955',0),(33,1,1,'ed4adaa09776a42a162beceb5955c375e8109d51cd965f2d183ae3b0fff1f929','92c87e9e-eaac-497d-a624-db1cf7a0f47b','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:53:41.141955','2026-08-10 07:54:12.034062',1,34,0,'admin','2026-08-10 07:53:41.141955','admin','2026-08-10 07:54:12.034062',0),(34,1,1,'bf57d39ed50990c6af7c96f61b1cea0f610738696c56607a8cade1872f0cb018','b1667014-2e8a-47b1-8acd-dca12ca7503c','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:54:12.034062','2026-08-10 07:54:17.514404',1,35,0,'admin','2026-08-10 07:54:12.034062','admin','2026-08-10 07:54:17.514404',0),(35,1,1,'75a7281997b4931b49977c2d832fc337ab9ba29a74cad8af01d0b2d282b1b05f','212a5afb-5a54-4eb2-b287-8a4762effbb2','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:54:17.514404','2026-08-10 07:54:30.644711',1,36,0,'admin','2026-08-10 07:54:17.514404','admin','2026-08-10 07:54:30.644711',0),(36,1,1,'4d001579f6a38531630fb34f768887ea30b0f284b8a5f7503c6d6fdf789986cd','5c9b5368-d9fd-497b-86b2-1349ac213bf3','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:54:30.644711','2026-08-10 07:54:31.803163',1,37,0,'admin','2026-08-10 07:54:30.644711','admin','2026-08-10 07:54:31.803163',0),(37,1,1,'3f68fdf93e01918a9ded778d1f0ecbdb7da9df8c0ba27f15ca665524ee1b6638','5d5f99ea-55a4-47d9-a4e8-0766bb891943','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:54:31.803163','2026-08-10 07:54:38.430777',1,38,0,'admin','2026-08-10 07:54:31.803163','admin','2026-08-10 07:54:38.430777',0),(38,1,1,'5f3f3bf7b691b699e78d5308b46a5ef87f555f9f1a950457ae0660bcd0ea1ccb','8750dee2-f9c2-449e-bf4e-644bb2f2e648','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:54:38.430777','2026-08-10 07:54:41.549265',1,39,0,'admin','2026-08-10 07:54:38.430777','admin','2026-08-10 07:54:41.549265',0),(39,1,1,'57adf6df9d9e81fa2fdbab699b6dfa021d9560a2616289108aab21f373024961','2b201cd5-8af4-4fdd-8f74-8ebe38a1ebcc','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:54:41.549265','2026-08-10 07:55:57.973373',1,40,0,'admin','2026-08-10 07:54:41.549265','admin','2026-08-10 07:55:57.973373',0),(40,1,1,'7f945ad61c303eeb940c8a4d66bc836f17f5de737932aee8cd3aba7a11c22e7d','b9d7820b-2973-4172-9a02-cd252bc89070','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:55:57.973373','2026-08-10 07:56:08.752365',1,NULL,0,'admin','2026-08-10 07:55:57.973373','admin','2026-08-10 07:56:08.752365',0),(41,1,1,'1b39692a97ca01c9b5e8fefcecb74345db7f52230e57278c06831dd68b165624','eaeef95e-10c0-42d4-84ad-2f5fe4bc270e','9589a287-2a6f-4f5a-a41b-611f8cc09c55','2026-08-17 07:56:08.752365','2026-08-10 07:56:09.818617',1,42,0,'admin','2026-08-10 07:56:08.752365','admin','2026-08-10 07:56:09.818617',0),(42,1,1,'233754e1f4cddf20d492c659b49ca5e7178b140d43fc4664a317beb957018e81','33b598de-6d90-47bb-b41f-f7d3ca11d91f','9589a287-2a6f-4f5a-a41b-611f8cc09c55','2026-08-17 07:56:09.818617','2026-08-10 08:00:16.043298',1,43,0,'admin','2026-08-10 07:56:09.818617','admin','2026-08-10 08:00:16.043298',0),(43,1,1,'14d4931463e2fe254a4317e85bcc47afab18be86bfea8b4c95e81eb8a8d99a45','82bcbb0b-11eb-43b1-9727-eb9d7cd83f03','9589a287-2a6f-4f5a-a41b-611f8cc09c55','2026-08-17 08:00:16.043298','2026-08-10 08:00:18.056555',1,NULL,0,'admin','2026-08-10 08:00:16.043298','admin','2026-08-10 08:00:18.056555',0),(44,1,1,'1d201db16df821a5f1bffc3c6c5503fdd5c1f3fa50861d716c58a47d6ef21603','3e9397d6-1bb9-49ba-934f-cd76fd9d96e0','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:00:24.415556','2026-08-10 08:06:40.828914',1,45,0,'admin','2026-08-10 08:00:24.415556','admin','2026-08-10 08:06:40.828914',0),(45,1,1,'44e83112f458876bbd4d2c090e20c8110905a19e6d28210982789b1265748d2e','2c7feef7-7cd4-4f39-a7fd-49dd1a4ff111','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:06:40.828914','2026-08-10 08:06:47.840867',1,46,0,'admin','2026-08-10 08:06:40.828914','admin','2026-08-10 08:06:47.840867',0),(46,1,1,'e80361852a3b23622dad7f56f1326dcf36c9d6096d84bc11a81651976df67bea','4b6e6c11-622e-4845-9ec5-86bf2e832830','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:06:47.840867','2026-08-10 08:06:53.046377',1,47,0,'admin','2026-08-10 08:06:47.840867','admin','2026-08-10 08:06:53.046377',0),(47,1,1,'c6792e8206e4dc9a6e2caf8336deebbe2adc8da25626a308facd21fe07aa0c24','47f85871-ad58-410d-83d7-40ed91e8a72b','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:06:53.046377','2026-08-10 08:08:39.952088',1,48,0,'admin','2026-08-10 08:06:53.046377','admin','2026-08-10 08:08:39.952088',0),(48,1,1,'e8ed0fe66db7c1eb712aeac4f8905f1a239d9edb07f2631fba1033a351a98527','f7788784-ae2d-4928-8abb-678a2a2bc896','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:08:39.952088','2026-08-10 08:08:44.251989',1,49,0,'admin','2026-08-10 08:08:39.952088','admin','2026-08-10 08:08:44.251989',0),(49,1,1,'22d9c08ce75640bbc3c0ac96ee7c4a1670d68251c79286a6b70a146f52e2279f','fb737ad8-1a55-4346-8a44-38a8b9d7bcba','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:08:44.251989','2026-08-10 08:35:55.841798',1,50,0,'admin','2026-08-10 08:08:44.251989','admin','2026-08-10 08:35:55.841798',0),(50,1,1,'7b1ce2628939be33f388fb51a0fd1e900f4cff8d9f3655f3fe2545c34d76f460','cecbf86a-89e7-4296-b027-b1b2b06c2e2f','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:35:55.841798','2026-08-10 08:36:36.870881',1,51,0,'admin','2026-08-10 08:35:55.841798','admin','2026-08-10 08:36:36.870881',0),(51,1,1,'f7a966dff6da865f9abf7fa8fc21b183b955445791ae3f2f10b4b6fbda11253b','baaac05a-e795-4e2e-b7a5-1e2857404414','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:36:36.870881','2026-08-10 09:10:02.664175',1,52,0,'admin','2026-08-10 08:36:36.870881','admin','2026-08-10 09:10:02.664175',0),(52,1,1,'9df654afe0bdef3f62c1ba9baf76ce1844171de3dc98c5ac4583fd92e8e6b1c9','2ed7ebc9-03d2-40e5-ac4a-78af8687f24b','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 09:10:02.664175','2026-08-10 09:43:48.867969',1,NULL,0,'admin','2026-08-10 09:10:02.664175','admin@gmail.com','2026-08-10 09:43:48.867969',0),(53,1,1,'229754d0316014434a2c6a785b67d7f733e86379e83a6c02d6fa0021ed835932','e28b157f-e806-48fb-91ad-fcfcda8ce61f','701c87aa-0030-46f4-8c8d-24e00d3a63d3','2026-08-17 09:43:48.867969','2026-08-10 09:45:18.799947',1,NULL,0,'admin@gmail.com','2026-08-10 09:43:48.867969','admin@gmail.com','2026-08-10 09:45:18.799947',0),(54,1,1,'fe60ee24fa2186479a70f21b84ee0454634de74d0f28357dbf8e61482b08ddb3','db27636c-8e94-4c59-90b7-08e076973166','c982af3e-e904-4b18-84bd-9b308186da66','2026-08-17 09:45:18.799947','2026-08-10 09:45:33.728126',1,55,0,'admin@gmail.com','2026-08-10 09:45:18.799947','admin@gmail.com','2026-08-10 09:45:33.728126',0),(55,1,1,'812817822eac97a98af66599bbd7e77a8f0fac780f5e6c94cfc6765dcaa764a1','729baa1b-0493-4d08-95bd-475f4bbb9c6a','c982af3e-e904-4b18-84bd-9b308186da66','2026-08-17 09:45:33.728126','2026-08-10 09:46:11.508113',1,56,0,'admin@gmail.com','2026-08-10 09:45:33.728126','admin@gmail.com','2026-08-10 09:46:11.508113',0),(56,1,1,'6167509f80e047bd2d6df4da826ee2cfd1ce37adcfe991d2055b40b6773d4746','f67aac32-a65c-43a2-b9ca-67f3de11a878','c982af3e-e904-4b18-84bd-9b308186da66','2026-08-17 09:46:11.508113','2026-08-10 09:48:35.248233',1,57,0,'admin@gmail.com','2026-08-10 09:46:11.508113','admin@gmail.com','2026-08-10 09:48:35.248233',0),(57,1,1,'923791e6697b9fe92d284fa26c92d218c6e787f3108780ac3576000aa443387f','de1193ee-dafb-491c-8f32-220d9934a93c','c982af3e-e904-4b18-84bd-9b308186da66','2026-08-17 09:48:35.248233','2026-08-10 09:48:51.560044',1,58,0,'admin@gmail.com','2026-08-10 09:48:35.248233','admin@gmail.com','2026-08-10 09:48:51.560044',0),(58,1,1,'946d808282b71c996e31fbc8a4b3bdba8451813a2fa23cd31cde6bbf541f4cc5','f5ed2dc8-e365-419b-bd14-b7e2101e543f','c982af3e-e904-4b18-84bd-9b308186da66','2026-08-17 09:48:51.560044','2026-08-11 03:00:44.059363',1,59,0,'admin@gmail.com','2026-08-10 09:48:51.560044','admin@gmail.com','2026-08-11 03:00:44.059363',0),(59,1,1,'91cbec56e6fc8c810953aa078924f4b452e0faba379ee024c895bc596babdb3b','fdfe7056-4932-4eb5-9ed6-563880c61c64','c982af3e-e904-4b18-84bd-9b308186da66','2026-08-18 03:00:44.059363','2026-08-11 03:00:46.484233',1,NULL,0,'admin@gmail.com','2026-08-11 03:00:44.059363','admin@gmail.com','2026-08-11 03:00:46.484233',0),(60,1,1,'b7f1f5d298762ff43857064ac1c4d1d63023eabb3813784c80f703206e70d1f1','642339e7-a06f-4cdf-8199-2fcdbbb84e7a','f8427e99-7785-4972-b888-617f7235933d','2026-08-18 03:00:54.362231','2026-08-11 03:28:17.692419',1,61,0,'admin@gmail.com','2026-08-11 03:00:54.362231','admin@gmail.com','2026-08-11 03:28:17.692419',0),(61,1,1,'76d1301012ccec1f0b386f4f9ebdf607da663937007e537b0fbbf2209b676858','bbb94ed2-273e-4e05-9999-af6b4387fd3d','f8427e99-7785-4972-b888-617f7235933d','2026-08-18 03:28:17.692419','2026-08-11 03:31:37.487915',1,62,0,'admin@gmail.com','2026-08-11 03:28:17.692419','admin@gmail.com','2026-08-11 03:31:37.487915',0),(62,1,1,'5fb10126a937386a223f703a0cec89a0675c6ca6ba483fd2fccdce0e82601a67','e3bfc999-d150-43e4-b4fb-50ccd55be73f','f8427e99-7785-4972-b888-617f7235933d','2026-08-18 03:31:37.487915','2026-08-11 04:25:42.281696',1,63,0,'admin@gmail.com','2026-08-11 03:31:37.487915','admin@gmail.com','2026-08-11 04:25:42.281696',0),(63,1,1,'e3356d27f34c5c029af054dc7e2e753fac5bad4402d0f389a9a038b76ee80993','5c710119-8223-4834-b607-fdf54f4fdb65','f8427e99-7785-4972-b888-617f7235933d','2026-08-18 04:25:42.281696','2026-08-11 06:54:55.960014',1,NULL,0,'admin@gmail.com','2026-08-11 04:25:42.281696','admin@gmail.com','2026-08-11 06:54:55.960014',0),(64,1,1,'14a82f1278b034b3dc5aecd1ec72b0a155a92327f1336bd625a7e904f01c35ed','9a9da57a-0a0d-4863-9cb1-8813e2637a42','5e1f81e0-4289-4a03-8b7d-fe59b6ffeefa','2026-08-18 06:54:55.960014','2026-08-11 06:57:21.337569',1,65,0,'admin@gmail.com','2026-08-11 06:54:55.960014','admin@gmail.com','2026-08-11 06:57:21.337569',0),(65,1,1,'a1687727e8feb25924024dde74bbe14fcc981d2b6dba22f07bbe3f42d0b0ebee','99c47196-c352-4870-af46-32f49cfbfeef','5e1f81e0-4289-4a03-8b7d-fe59b6ffeefa','2026-08-18 06:57:21.337569','2026-08-11 07:09:15.938945',1,NULL,0,'admin@gmail.com','2026-08-11 06:57:21.337569','admin@gmail.com','2026-08-11 07:09:15.938945',0),(66,1,1,'fb2cdffd3e70ce82781a080196db2522d0744e38e7628fe15fc3ea11a7970496','efd89083-a9fb-4721-aefd-905dcfe28bf9','e0b95f23-c8dd-4368-a72f-47f39d0ad837','2026-08-18 07:09:15.938945','2026-08-11 07:50:25.166152',1,67,0,'admin@gmail.com','2026-08-11 07:09:15.938945','admin@gmail.com','2026-08-11 07:50:25.166152',0),(67,1,1,'81b1dacbf1e9690526d40bf7b99ef62279124eaae9f119fd2c2bb10bf6d1ce07','d24a9f06-c8b2-40da-81d2-c2f615a27feb','e0b95f23-c8dd-4368-a72f-47f39d0ad837','2026-08-18 07:50:25.166152','2026-08-11 08:08:37.818937',1,68,0,'admin@gmail.com','2026-08-11 07:50:25.166152','admin@gmail.com','2026-08-11 08:08:37.818937',0),(68,1,1,'9d8706e0a53ff2a1f9a91e36242c38dbcba490918586e20820ef9dc3259bf9f8','1f4f5b1b-0549-457b-b1c5-235c1f538c3f','e0b95f23-c8dd-4368-a72f-47f39d0ad837','2026-08-18 08:08:37.818937','2026-08-11 08:35:39.769399',1,69,0,'admin@gmail.com','2026-08-11 08:08:37.818937','admin@gmail.com','2026-08-11 08:35:39.769399',0),(69,1,1,'c188201253011c1fb8e79ce0cb0779652576a13e1799ad1e8923b9cbb2fbc136','252a851d-0353-4d9a-95ee-4c24a68129bf','e0b95f23-c8dd-4368-a72f-47f39d0ad837','2026-08-18 08:35:39.769399','2026-08-11 09:20:47.605127',1,70,0,'admin@gmail.com','2026-08-11 08:35:39.769399','admin@gmail.com','2026-08-11 09:20:47.605127',0),(70,1,1,'f16097d224db4dccecc94ac1d0f524eec2f49891f958566b014c577b10b83e00','ae72a980-bfa9-47d7-a669-29fb7601bcf6','e0b95f23-c8dd-4368-a72f-47f39d0ad837','2026-08-18 09:20:47.605127','2026-08-12 02:28:18.285070',1,71,0,'admin@gmail.com','2026-08-11 09:20:47.605127','admin@gmail.com','2026-08-12 02:28:18.285070',0),(71,1,1,'047f620737d31e1900d5516edc2f5de6f8951631da65868798cf263dde9b54df','c5e88737-c8ac-4eb5-8f2f-92ed0e5598a8','e0b95f23-c8dd-4368-a72f-47f39d0ad837','2026-08-19 02:28:18.285070','2026-08-12 02:28:28.828310',1,NULL,0,'admin@gmail.com','2026-08-12 02:28:18.285070','admin@gmail.com','2026-08-12 02:28:28.828310',0),(72,1,1,'84a906db7b9b02db00281f5dcef68e781de5634343c25a3f70b06ab30201600f','9c005f33-291d-465e-a59f-6fc7a91e8b38','d2f46ef3-39ce-43f9-b69a-dfbfa3ce9009','2026-08-19 02:35:39.419820','2026-08-12 02:35:40.976046',1,NULL,0,'admin@gmail.com','2026-08-12 02:35:39.419820','admin@gmail.com','2026-08-12 02:35:40.976046',0),(73,1,1,'08e5be62727862e9978a8cc00fcf65cc9c5d7cdeb0f73e1d8acd7dedd2a3b51e','f7657790-2080-489b-a813-53e1af0c30de','5a761d5e-4f4a-4a12-84ca-10e0fac439c1','2026-08-19 02:35:40.976046','2026-08-12 02:36:25.366871',1,NULL,0,'admin@gmail.com','2026-08-12 02:35:40.976046','admin@gmail.com','2026-08-12 02:36:25.366871',0),(74,1,1,'f7e81e23cc464b8a29ce4fcab99059f4b8ee3477e0dd712979aaaad6c1940b29','abd4832c-b9a4-460c-b96b-c00e3c192b83','7e649c4e-7337-4efc-8241-d8d139218d22','2026-08-19 02:36:25.366871','2026-08-12 02:39:24.393073',1,NULL,0,'admin@gmail.com','2026-08-12 02:36:25.366871','admin@gmail.com','2026-08-12 02:39:24.393073',0),(75,1,1,'0946136ae7c8eda22b57deef3a9ad7069527fa6e5e479d31335b71d7ab4a455d','5af0fe58-c558-4004-adcc-74d6be3fec18','9fa5269f-f96c-4524-97bc-62d5a14e8d8d','2026-08-19 02:39:24.393073','2026-08-12 03:14:41.988335',1,76,0,'admin@gmail.com','2026-08-12 02:39:24.393073','admin@gmail.com','2026-08-12 03:14:41.988335',0),(76,1,1,'37b9b41ecbfc6b0d8802656a1645c3a05668fa20201c4a94712a7ad99820b9bf','0ff1b20c-a56d-46d6-857c-0cfdacee7069','9fa5269f-f96c-4524-97bc-62d5a14e8d8d','2026-08-19 03:14:41.988335','2026-08-12 03:32:36.506011',1,NULL,0,'admin@gmail.com','2026-08-12 03:14:41.988335','admin@gmail.com','2026-08-12 03:32:36.506011',0),(77,1,1,'ca9c3ffcdc699e74fc609957f917209c496bd50d07004b0db8de908c48f780d0','d135e8a1-9b62-40af-b922-9411352bf35a','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 03:32:36.506011','2026-08-12 03:32:44.756888',1,78,0,'admin@gmail.com','2026-08-12 03:32:36.506011','admin@gmail.com','2026-08-12 03:32:44.756888',0),(78,1,1,'b4fd80d3c4445b5b0907bd1fee5c9420a0d3ec1dfdaacd3d129ae5f9f23bcb19','227fe7de-e3b3-48a1-bcbe-cd76f855c4ac','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 03:32:44.756888','2026-08-12 04:16:47.550057',1,79,0,'admin@gmail.com','2026-08-12 03:32:44.756888','admin@gmail.com','2026-08-12 04:16:47.550057',0),(79,1,1,'2ce846f7c0ae205734ed46ab861079f3f0d5a70c1e26adad28b1f6343ea22034','dfe04a74-e9e4-4b5c-9be8-53811889e301','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 04:16:47.550057','2026-08-12 04:26:03.242933',1,80,0,'admin@gmail.com','2026-08-12 04:16:47.550057','admin@gmail.com','2026-08-12 04:26:03.242933',0),(80,1,1,'9f60ab7637e787a83048517c1c8ad58ba4de72b46f2e3dfe692ed39c452ff00f','959824d4-e306-4637-b4f3-8950d44ea3ae','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 04:26:03.242933','2026-08-12 04:26:15.320184',1,81,0,'admin@gmail.com','2026-08-12 04:26:03.242933','admin@gmail.com','2026-08-12 04:26:15.320184',0),(81,1,1,'771a28ca87dc38d903ac7abdad406db9e4cec43cc8f335648610ed747a58ea32','f4d05916-9bec-4916-b3c9-aec5652e7e1d','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 04:26:15.320184','2026-08-12 04:27:12.137430',1,82,0,'admin@gmail.com','2026-08-12 04:26:15.320184','admin@gmail.com','2026-08-12 04:27:12.137430',0),(82,1,1,'2dafd4555002440cba6f781f33ae7fe72d5ea57115c8c52a0b88988478e25568','9cb18aad-011e-4dac-a563-644247cae189','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 04:27:12.137430','2026-08-12 05:30:11.444087',1,83,0,'admin@gmail.com','2026-08-12 04:27:12.137430','admin@gmail.com','2026-08-12 05:30:11.444087',0),(83,1,1,'e55ac24bbe71d81603be309a086435e6d67d96131ecf30159f17290f8eacb927','684c9b92-7a02-417c-888c-93ece1c3047f','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 05:30:11.444087','2026-08-12 05:53:14.934852',1,84,0,'admin@gmail.com','2026-08-12 05:30:11.444087','admin@gmail.com','2026-08-12 05:53:14.934852',0),(84,1,1,'328692e8be1e832736b6472175ce64aa3df3dfa92d1d63599582307600027616','aa23c21c-e4f2-422b-87c3-a706d0ead680','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 05:53:14.934852','2026-08-12 06:02:43.891396',1,85,0,'admin@gmail.com','2026-08-12 05:53:14.934852','admin@gmail.com','2026-08-12 06:02:43.891396',0),(85,1,1,'e3bbc1c10c64b681894437fd5d4dc385a96914a096157578e132dd415bc7c444','4f726fa4-06a0-4a9e-9bcb-7f3935c7d6a1','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 06:02:43.891396','2026-08-12 06:03:36.987303',1,86,0,'admin@gmail.com','2026-08-12 06:02:43.891396','admin@gmail.com','2026-08-12 06:03:36.987303',0),(86,1,1,'0b8cacdbc9559c75d90eeb649f4eff98431b820cacee50c0cef3886d94899659','63168bf5-2059-4149-ae4c-099280741d37','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 06:03:36.987303','2026-08-12 06:45:46.948607',1,87,0,'admin@gmail.com','2026-08-12 06:03:36.987303','admin@gmail.com','2026-08-12 06:45:46.948607',0),(87,1,1,'d90c0c8670c5a3979b9fab0ebcde7731d9fba494b8e424352608903824a991f7','aa48246b-6403-42dc-b219-a509c6962fe6','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 06:45:46.948607','2026-08-12 08:28:20.177803',1,88,0,'admin@gmail.com','2026-08-12 06:45:46.948607','admin@gmail.com','2026-08-12 08:28:20.177803',0),(88,1,1,'4764bed38cfb406232e8b23bd7c6550c6b47b8cb7a603e65c4fde3c0f730acb4','dc08acbf-65a9-428f-8d1f-16cd95671cba','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 08:28:20.177803','2026-08-12 08:44:20.564306',1,89,0,'admin@gmail.com','2026-08-12 08:28:20.177803','admin@gmail.com','2026-08-12 08:44:20.564306',0),(89,1,1,'eb649fac9457f39f27f9824ed692f80b8989a57933e2e782414d52916674304b','2adc18c9-603f-4617-a696-1c7c0ee04e19','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 08:44:20.564306','2026-08-12 08:45:08.598357',1,90,0,'admin@gmail.com','2026-08-12 08:44:20.564306','admin@gmail.com','2026-08-12 08:45:08.598357',0),(90,1,1,'aa57575edd467d3eed76795aac30cbbb27fe056fe35f7f5720283722aad9dbd3','afcca4bf-2db3-4356-87e3-8411abcabc59','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 08:45:08.598357','2026-08-12 08:52:17.342680',1,91,0,'admin@gmail.com','2026-08-12 08:45:08.598357','admin@gmail.com','2026-08-12 08:52:17.342680',0),(91,1,1,'3d2e90c90e5152e5622220771e0f99a7da804677073d0a0b04826a496ddb1855','2cad14b3-c4b0-4e46-adcf-00c8e2fd3317','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 08:52:17.342680','2026-08-13 03:17:33.315385',1,92,0,'admin@gmail.com','2026-08-12 08:52:17.342680','admin@gmail.com','2026-08-13 03:17:33.315385',0),(92,1,1,'bd3340b49d94a79e20f303f060e34718e044ef694c177239bd9511745e3405a5','536f7586-07c1-4731-89bb-98641440ef9d','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-20 03:17:33.315385','2026-08-13 03:35:00.504949',1,93,0,'admin@gmail.com','2026-08-13 03:17:33.315385','admin@gmail.com','2026-08-13 03:35:00.504949',0),(93,1,1,'e732a462eeb6497f64539d6c9e4f3f4de1eb4be6655e780ad17e34c3f95ab8e9','440a4c51-a1dc-465b-9989-34c12a9f32b1','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-20 03:35:00.504949','2026-08-13 05:29:34.217509',1,94,0,'admin@gmail.com','2026-08-13 03:35:00.504949','admin@gmail.com','2026-08-13 05:29:34.217509',0),(94,1,1,'77bc28aab4aa508ca3fe79d199419b86d72de83a81d0163fd14deeb79316cb4e','467667a7-8433-4568-8771-27852209e963','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-20 05:29:34.217509','2026-08-13 05:29:56.726908',1,95,0,'admin@gmail.com','2026-08-13 05:29:34.217509','admin@gmail.com','2026-08-13 05:29:56.726908',0),(95,1,1,'a9dcbdfbb040e190cbda9ad50779c37992e1f5874d304e2a8e99d008cee88492','6a1b1fdf-3982-47c8-a8b3-cca089720e3a','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-20 05:29:56.726908','2026-08-13 05:36:11.125568',1,96,0,'admin@gmail.com','2026-08-13 05:29:56.726908','admin@gmail.com','2026-08-13 05:36:11.125568',0),(96,1,1,'b542ac8aba673b2f6d513775635cdeefaf9d06bf06249003acc66b349126aa9b','40215a32-e6e8-410d-85e3-c71ad488ef00','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-20 05:36:11.125568','2026-08-13 05:40:57.652181',1,97,0,'admin@gmail.com','2026-08-13 05:36:11.125568','admin@gmail.com','2026-08-13 05:40:57.652181',0),(97,1,1,'3a016ed9702891fcb6fb2d942355295a177cad8bc3bdc44833b818a7122ea30a','e4921114-4894-4441-acd6-0e7787e03337','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-20 05:40:57.652181','2026-08-13 05:46:02.660630',1,NULL,0,'admin@gmail.com','2026-08-13 05:40:57.652181','admin@gmail.com','2026-08-13 05:46:02.660630',0),(98,1,1,'d403fe6370eb3fa16963fd2cc9c337e3458ed7567b9211144610a0f7af655bad','c2928c6b-bdab-41dc-bf88-98fc9cfd8593','a11bc4cd-9af3-4771-9afa-20b108245dcb','2026-08-20 05:53:50.361699','2026-08-13 06:17:07.355163',1,99,0,'admin@gmail.com','2026-08-13 05:53:50.361699','admin@gmail.com','2026-08-13 06:17:07.355163',0),(99,1,1,'f415c46f0dd3f9e23317328f7cbb856819c746e45bff3908fd125ddcb3757d3f','bf9b9ec3-4241-4661-8515-180a58f6d5e6','a11bc4cd-9af3-4771-9afa-20b108245dcb','2026-08-20 06:17:07.355163','2026-08-13 06:38:12.018343',1,100,0,'admin@gmail.com','2026-08-13 06:17:07.355163','admin@gmail.com','2026-08-13 06:38:12.018343',0),(100,1,1,'ffe92e0a89fa037cb63d2b2ee52bd2791dd57cc7de42250ae20e28c0168881f8','b31e60aa-b81f-435f-9c6a-4d96944a5121','a11bc4cd-9af3-4771-9afa-20b108245dcb','2026-08-20 06:38:12.018343','2026-08-13 06:38:15.017396',1,NULL,0,'admin@gmail.com','2026-08-13 06:38:12.018343','admin@gmail.com','2026-08-13 06:38:15.017396',0),(101,1,1,'74fd231a308a0017a2477a218a4b627de2029da600f6f9947a241e83e37cc5f2','2ab82812-78d8-468b-990c-19b4187e3b66','2c05bd57-fd86-4064-9825-1f31a70205b3','2026-08-20 06:38:24.007328','2026-08-13 06:38:38.902703',1,102,0,'admin@gmail.com','2026-08-13 06:38:24.007328','admin@gmail.com','2026-08-13 06:38:38.902703',0),(102,1,1,'bdca414adceb14405fba9c4926e8c806fc461d6cd040b092c9a78519e4384805','92c16738-442f-4fd1-95dc-bcdddb6661cb','2c05bd57-fd86-4064-9825-1f31a70205b3','2026-08-20 06:38:38.902703','2026-08-13 06:58:33.350382',1,103,0,'admin@gmail.com','2026-08-13 06:38:38.902703','admin@gmail.com','2026-08-13 06:58:33.350382',0),(103,1,1,'d2015267ffc73882f73b66f6d0052e206b116ffb1ca677a096767b2fb6db55ab','0088c62a-97e9-4f83-9d62-76d4c20c43ab','2c05bd57-fd86-4064-9825-1f31a70205b3','2026-08-20 06:58:33.350382','2026-08-13 06:58:36.755863',1,NULL,0,'admin@gmail.com','2026-08-13 06:58:33.350382','admin@gmail.com','2026-08-13 06:58:36.755863',0),(104,1,1,'3a442e023f8929e46adf17beb83748afacd22bb72d4b338b004e8a2c25d7c8a4','7e8abdc1-4186-4f34-9f52-afca18358962','096230f8-8fb8-475c-acd8-4ac55b16ca75','2026-08-20 06:58:58.640229','2026-08-13 07:05:43.442645',1,105,0,'admin@gmail.com','2026-08-13 06:58:58.640229','admin@gmail.com','2026-08-13 07:05:43.442645',0),(105,1,1,'a13dccad4fec624eaf50039b715d63c0b443214e76d3fc179381aaf37e5b8104','5ffb7f15-20e3-42c5-b91e-fecb7eafa168','096230f8-8fb8-475c-acd8-4ac55b16ca75','2026-08-20 07:05:43.442645','2026-08-13 07:07:58.537406',1,106,0,'admin@gmail.com','2026-08-13 07:05:43.442645','admin@gmail.com','2026-08-13 07:07:58.537406',0),(106,1,1,'536a18e777927f10c13ccc0e3e8297f244672ff8d1207c2f4690fef9602a60f4','f019442c-707a-45c7-bd47-fd1c30455dd5','096230f8-8fb8-475c-acd8-4ac55b16ca75','2026-08-20 07:07:58.537406','2026-08-13 07:09:24.012231',1,NULL,0,'admin@gmail.com','2026-08-13 07:07:58.537406','admin@gmail.com','2026-08-13 07:09:24.012231',0),(107,8,1,'bc04b24b6811a9de9c70f4628b39924f0bd42767c622dd3de2ba9300fea05b47','a09384e7-845b-4b44-8aa0-ac9c11d8b102','43253a93-d33d-4671-9ece-3da10156f8a3','2026-08-20 07:09:30.755280','2026-08-13 07:11:58.981037',8,108,0,'hien@gmail.com','2026-08-13 07:09:30.755280','hien@gmail.com','2026-08-13 07:11:58.981037',0),(108,8,1,'71945a1cd01d3b8a38a86833a3ccb28ff685b77d151c99dccfcb172aef5b1783','601a7eb1-82a9-4482-9d88-4c86b47999c6','43253a93-d33d-4671-9ece-3da10156f8a3','2026-08-20 07:11:58.981037','2026-08-13 07:16:58.080650',8,NULL,0,'hien@gmail.com','2026-08-13 07:11:58.981037','hien@gmail.com','2026-08-13 07:16:58.080650',0),(109,8,1,'78cf649d2563f859cb250547bae1922b25061d0a298df9bab19c03cfee84b9db','7a708357-d08c-43fc-99f6-cb3b918699af','8c0ad540-2445-400f-9feb-3b36d0f60ac2','2026-08-20 07:17:09.225243','2026-08-13 07:28:53.043376',8,110,0,'hien@gmail.com','2026-08-13 07:17:09.225243','hien@gmail.com','2026-08-13 07:28:53.043376',0),(110,8,1,'5a6b5f5c99f22a95a88bfd9e7173f639691b7c018b97fc8f2245b25179ab3c1b','a28b8a6a-6e1a-4337-b328-e0ac588881a0','8c0ad540-2445-400f-9feb-3b36d0f60ac2','2026-08-20 07:28:53.043376','2026-08-13 07:29:51.532564',8,111,0,'hien@gmail.com','2026-08-13 07:28:53.043376','hien@gmail.com','2026-08-13 07:29:51.532564',0),(111,8,1,'22dcf8ca0b027164b1e275a69b9627dabb9adb7d288fdbae2c5fb71af2cc7f1c','9e74d608-1c06-42b4-848c-c91888f55dca','8c0ad540-2445-400f-9feb-3b36d0f60ac2','2026-08-20 07:29:51.532564','2026-08-13 07:31:15.402358',8,112,0,'hien@gmail.com','2026-08-13 07:29:51.532564','hien@gmail.com','2026-08-13 07:31:15.402358',0),(112,8,1,'1deb937ecc6290f3b88319f2f2fa006da4321c16068f2b43ef0da1a9192f3451','94253773-1224-4e6d-b1b1-8c8ca04d7a6d','8c0ad540-2445-400f-9feb-3b36d0f60ac2','2026-08-20 07:31:15.402358','2026-08-13 07:37:53.497242',8,113,0,'hien@gmail.com','2026-08-13 07:31:15.402358','hien@gmail.com','2026-08-13 07:37:53.497242',0),(113,8,1,'33a8dc33a1ef43e1395e32bb790a3cc355f18200996b6ed215dc23a7e816fff9','905feb06-a6c0-4a5b-90e8-3d10230cc81f','8c0ad540-2445-400f-9feb-3b36d0f60ac2','2026-08-20 07:37:53.497242','2026-08-13 07:38:44.426877',8,NULL,0,'hien@gmail.com','2026-08-13 07:37:53.497242','hien@gmail.com','2026-08-13 07:38:44.426877',0),(114,1,1,'bb45b50b0ea4e68e092f29e2b337d957a2391ee911067875ebd912af1cfd8fe7','e677cca2-194f-4508-b76f-e982c74bbc29','d110c8cc-5364-4ece-9295-2a45975e446a','2026-08-20 07:38:51.009526','2026-08-13 07:39:30.082953',1,NULL,0,'admin@gmail.com','2026-08-13 07:38:51.009526','admin@gmail.com','2026-08-13 07:39:30.082953',0),(115,1,1,'e56e88118efacf0a08b34fe5c11be9ba01c737d8c2c11def3199a12e6e3b2340','eedb4d13-b42f-42bc-8934-e7262111ee57','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:51:51.503497','2026-08-13 07:52:26.671788',1,116,0,'admin@gmail.com','2026-08-13 07:51:51.503497','admin@gmail.com','2026-08-13 07:52:26.671788',0),(116,1,1,'de2dfa2a13fddbf0cf6f4188ae3eec574b1726fc15fc4c9cf80e523adfceba36','c500b04b-f5c6-426b-997f-e6de98d36273','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:26.671788','2026-08-13 07:52:27.995156',1,117,0,'admin@gmail.com','2026-08-13 07:52:26.671788','admin@gmail.com','2026-08-13 07:52:27.995156',0),(117,1,1,'0bb551969201fcf01d6617b4b5f8ff9a983ce26a32a735d200a2edfd2d67a2e7','78655342-b04e-4715-a2d8-3be2cc1cd32d','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:27.995156','2026-08-13 07:52:29.355124',1,118,0,'admin@gmail.com','2026-08-13 07:52:27.995156','admin@gmail.com','2026-08-13 07:52:29.355124',0),(118,1,1,'205930895aabd4e34145f858415a427106a73055a3afb9c4670675ae8f6e6887','fb58bbe0-899c-4b02-9da6-33854831543e','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:29.355124','2026-08-13 07:52:30.622345',1,119,0,'admin@gmail.com','2026-08-13 07:52:29.355124','admin@gmail.com','2026-08-13 07:52:30.622345',0),(119,1,1,'78e3854528d3935e5bde099f59929d5724ca803e78adebc009b83c5818a5ec62','70515398-3b50-4ac4-8311-383a8d4016a4','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:30.622345','2026-08-13 07:52:31.922413',1,120,0,'admin@gmail.com','2026-08-13 07:52:30.622345','admin@gmail.com','2026-08-13 07:52:31.922413',0),(120,1,1,'0b24eab4a392f2b2733c666f2a772cb2eaa00640c181f9931ae4d14b454d1eef','ff836551-6d4c-4f72-bfcf-f1f5f7e5997f','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:31.922413','2026-08-13 07:52:33.234234',1,121,0,'admin@gmail.com','2026-08-13 07:52:31.922413','admin@gmail.com','2026-08-13 07:52:33.234234',0),(121,1,1,'fd4b7b7cc49cd50aae7062cd8c07eb00af941bb98fbe507864800cbbf08324f5','472bab4f-4631-4fb6-a17d-daec66f9c62e','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:33.234234','2026-08-13 07:52:34.679474',1,122,0,'admin@gmail.com','2026-08-13 07:52:33.234234','admin@gmail.com','2026-08-13 07:52:34.679474',0),(122,1,1,'cf2a8c4a06205e69bd03a41b50066c1d9cdc1da0a9e2354ccf17889ccf0bc496','76d13866-065f-452e-8a61-f9b838f2856b','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:34.679474','2026-08-13 07:52:37.446743',1,123,0,'admin@gmail.com','2026-08-13 07:52:34.679474','admin@gmail.com','2026-08-13 07:52:37.446743',0),(123,1,1,'447f2d635660822af7555fa7146f4e55a59edbf70836e40a51659754eaac959c','64897aa5-a197-447f-a9e1-99fdc656181a','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:37.446743','2026-08-13 08:00:21.365164',1,124,0,'admin@gmail.com','2026-08-13 07:52:37.446743','admin@gmail.com','2026-08-13 08:00:21.365164',0),(124,1,1,'10cf35eb8340d110e4bcf7630851d5a01343b9f1b35eba97eb3ff05ffc191234','f2ff841e-1768-48f5-b27d-6f893be3bf86','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 08:00:21.365164','2026-08-13 08:00:29.356042',1,NULL,0,'admin@gmail.com','2026-08-13 08:00:21.365164','admin@gmail.com','2026-08-13 08:00:29.356042',0),(125,1,1,'34409f44c2cd9a978d1a95dd8b848c153b893145464ed0a212b5fda451f85fff','9a0cd42c-8ecd-40ed-9c69-41c9e821ae4b','c855c202-e8c1-4b72-8d19-382878be9c5b','2026-08-20 08:00:29.356042','2026-08-13 08:00:41.152450',1,126,0,'admin@gmail.com','2026-08-13 08:00:29.356042','admin@gmail.com','2026-08-13 08:00:41.152450',0),(126,1,1,'54eab4822bb4cde9456ff5cbf884b6bc774638b8375dbb0f04162949183d6a5a','86a32380-9853-46e9-9e6f-cbe7a5e7a4fb','c855c202-e8c1-4b72-8d19-382878be9c5b','2026-08-20 08:00:41.152450','2026-08-13 08:00:51.584932',1,NULL,0,'admin@gmail.com','2026-08-13 08:00:41.152450','admin@gmail.com','2026-08-13 08:00:51.584932',0),(127,1,1,'20349820530814fdd231e16cf787e85ac906b3ad2cac229fe28b469048a54890','bade81d1-281c-4658-9422-1c8e49f0829b','e880b88c-5215-4121-9cec-0b0a172bc4cf','2026-08-20 08:00:51.584932','2026-08-13 08:03:46.371105',1,128,0,'admin@gmail.com','2026-08-13 08:00:51.584932','admin@gmail.com','2026-08-13 08:03:46.371105',0),(128,1,1,'eab91a5a74852973da6088b3d278995d99f26ccfe229bb489127a1f54c3d8ae4','64b45855-1967-43f7-b0f0-bb9063de1bc1','e880b88c-5215-4121-9cec-0b0a172bc4cf','2026-08-20 08:03:46.371105','2026-08-13 08:04:15.647211',1,NULL,0,'admin@gmail.com','2026-08-13 08:03:46.371105','admin@gmail.com','2026-08-13 08:04:15.647211',0),(129,8,1,'3f9b2f6afec67593f3a1e4d16117112d5547ecfab9f2c985e38016693f905f33','12b88731-6ea0-40b4-8a3f-589a190af7cc','0a21cf15-6492-406c-85ab-7b31e0993122','2026-08-20 08:04:23.514476','2026-08-13 08:06:18.821631',8,NULL,0,'hien@gmail.com','2026-08-13 08:04:23.514476','hien@gmail.com','2026-08-13 08:06:18.821631',0),(130,1,1,'8af1106dbcedaa6d5cf1f4ab2b91c26466951f5cad17d368770fa9c559c86dd8','a57d3388-faa0-48b8-b7e3-8c889c0588ce','19f7a965-eedb-44fb-aadb-6d1ea9d8a391','2026-08-20 08:06:31.632122','2026-08-13 08:09:32.160084',1,131,0,'admin@gmail.com','2026-08-13 08:06:31.632122','admin@gmail.com','2026-08-13 08:09:32.160084',0),(131,1,1,'f5ded225f4b0c74a3da79d073e41105ade2eac6661ff2f53fa0f837a58d027f9','3998c63d-1272-4c9b-9adf-9a7f0d880b66','19f7a965-eedb-44fb-aadb-6d1ea9d8a391','2026-08-20 08:09:32.160084','2026-08-13 08:09:58.004641',1,NULL,0,'admin@gmail.com','2026-08-13 08:09:32.160084','admin@gmail.com','2026-08-13 08:09:58.004641',0),(132,1,1,'114806ab0217fb330a7caeccc0509892d5a4f88ef79dcf8e5e4fc9cf74f972c6','8fc8fcf5-d93e-447d-b0cf-2c57c1f9442f','8cb346b6-dc91-49b8-b05d-49a28dd231fa','2026-08-20 08:10:04.117577','2026-08-13 08:10:30.539326',1,NULL,0,'admin@gmail.com','2026-08-13 08:10:04.117577','admin@gmail.com','2026-08-13 08:10:30.539326',0),(133,8,1,'7b2dca91c90e333529f9fef056346500d631230f164d73360c51af34a282f19b','625af29b-7261-419b-9506-a69b543efc3f','af6e74e7-4309-4391-a22d-e14c34ae45b4','2026-08-20 08:10:41.221003','2026-08-13 08:11:06.143550',8,NULL,0,'hien@gmail.com','2026-08-13 08:10:41.221003','hien@gmail.com','2026-08-13 08:11:06.143550',0),(134,1,1,'d9221e60a6914b381a3b11a2e242dfda7df88cf4002d973aba26f8e798a3f635','54df5b12-2a0b-43d5-a924-88d81d73f756','7f84d697-00c8-41af-916e-d0e72c470e45','2026-08-20 08:12:01.373551','2026-08-13 08:12:39.553366',1,NULL,0,'admin@gmail.com','2026-08-13 08:12:01.373551','admin@gmail.com','2026-08-13 08:12:39.553366',0),(135,8,1,'187b4b7f6d6b74d711271f2971fcc650626bbeb1f9f4640690514e449c8a2a80','6a400dd5-f5f9-416f-855f-5b8566983a5b','06f3cec1-9372-4107-a3cb-bc8a2e2c271b','2026-08-20 08:12:46.568542','2026-08-13 08:15:40.912933',8,136,0,'hien@gmail.com','2026-08-13 08:12:46.568542','hien@gmail.com','2026-08-13 08:15:40.912933',0),(136,8,1,'087cbf251ad6892ebd6dc74cca8484c5af772491262c64c6e6eea8a29cee2b0f','74fe9025-6c33-48aa-b1aa-c5ac3081eb07','06f3cec1-9372-4107-a3cb-bc8a2e2c271b','2026-08-20 08:15:40.912933','2026-08-13 08:47:59.442610',8,137,0,'hien@gmail.com','2026-08-13 08:15:40.912933','hien@gmail.com','2026-08-13 08:47:59.442610',0),(137,8,1,'f995330d8765bf61556c4be1e3b32dafbd7a8ba30f38bd5f6618b5c3b313cf68','f65d408d-b0e7-4ae2-9770-dacf47c0843e','06f3cec1-9372-4107-a3cb-bc8a2e2c271b','2026-08-20 08:47:59.442610','2026-08-13 08:48:07.718993',8,138,0,'hien@gmail.com','2026-08-13 08:47:59.442610','hien@gmail.com','2026-08-13 08:48:07.718993',0),(138,8,1,'141c6fc6c91e832984baa1f96fa0c5198e27f0251d2e8c1e1d33fcc378e7e8ea','8bcfaf6f-f7a8-4fd0-8291-3768192aa91d','06f3cec1-9372-4107-a3cb-bc8a2e2c271b','2026-08-20 08:48:07.718993','2026-08-13 08:48:12.712932',8,NULL,0,'hien@gmail.com','2026-08-13 08:48:07.718993','hien@gmail.com','2026-08-13 08:48:12.712932',0),(139,1,1,'414d709dcf7ed6fc60b8fce9be3b4f833662e52cb60d7d777dcc98549e148715','d16dfae1-c20e-483b-9dc8-66a75de59aad','83a7e6e8-791e-426f-9c0b-7d91cc865771','2026-08-20 08:48:20.749472','2026-08-13 09:01:33.329029',1,140,0,'admin@gmail.com','2026-08-13 08:48:20.749472','admin@gmail.com','2026-08-13 09:01:33.329029',0),(140,1,1,'d78b02ffdf401168315c291c84d40991aa6dd0a5cdd7b26b607b9f1141dc2e01','98107dda-a7ec-457f-8f34-0dc3a70a8b29','83a7e6e8-791e-426f-9c0b-7d91cc865771','2026-08-20 09:01:33.329029','2026-08-13 09:53:44.458180',1,141,0,'admin@gmail.com','2026-08-13 09:01:33.329029','admin@gmail.com','2026-08-13 09:53:44.458180',0),(141,1,1,'9cf311fee6b30799ab44669c1f52807f252b2ba4a08ac5978f7668075b517d7c','3fbe2658-8225-4742-94f5-95f026b51bdd','83a7e6e8-791e-426f-9c0b-7d91cc865771','2026-08-20 09:53:44.458180','2026-08-14 08:26:18.881892',1,142,0,'admin@gmail.com','2026-08-13 09:53:44.458180','admin@gmail.com','2026-08-14 08:26:18.881892',0),(142,1,1,'85edab881d5e9babb5f681d2af36b02d4a3a79b6e63dbda43b0490e29de49190','0d2a738c-f60c-4eb8-be2c-1675f76305f7','83a7e6e8-791e-426f-9c0b-7d91cc865771','2026-08-21 08:26:18.881892','2026-08-14 08:26:22.946120',1,NULL,0,'admin@gmail.com','2026-08-14 08:26:18.881892','admin@gmail.com','2026-08-14 08:26:22.946120',0),(143,1,1,'3fc4e66330493c71fdd9557719991a828a2798fe7f3962626720da94fe668660','06d54202-89bf-4228-b8bb-1ff5791a955d','1b0971b7-5df3-46ed-ad53-012cb4a5fe22','2026-08-21 09:10:55.779806','2026-08-14 09:11:03.642926',1,NULL,0,'admin@gmail.com','2026-08-14 09:10:55.779806','admin@gmail.com','2026-08-14 09:11:03.642926',0),(144,1,1,'8286b070624dc3e843a6da38c2940ecac7a137dd8f6061fec7cc8a83bb0e1aaa','db84d653-dec7-468f-9588-4be551418ff1','f0d8f248-da94-4c48-878f-a4c3f8997ad8','2026-08-25 06:26:26.870236','2026-08-18 06:26:43.272938',1,NULL,0,'admin@gmail.com','2026-08-18 06:26:26.870236','admin@gmail.com','2026-08-18 06:26:43.272938',0),(145,1,1,'c7943c894d3df009c9fbe9cb7c4d098b6dc4f178733d4ee5cf64b38e72d06bbd','1363be4f-882d-44ea-8375-167451036c06','94624891-6bed-41ff-a899-b8b7260bc8ac','2026-08-25 09:17:43.682573','2026-08-18 09:22:52.641295',1,146,0,'admin@gmail.com','2026-08-18 09:17:43.682573','admin@gmail.com','2026-08-18 09:22:52.641295',0),(146,1,1,'16c6608fae38af10bb43ec76fdc09d7cb62bf3beb206b0ac5fcb912f2cdbce1d','69b8fde2-b818-4572-b199-3566581beabf','94624891-6bed-41ff-a899-b8b7260bc8ac','2026-08-25 09:22:52.641295','2026-08-18 09:23:15.374388',1,147,0,'admin@gmail.com','2026-08-18 09:22:52.641295','admin@gmail.com','2026-08-18 09:23:15.374388',0),(147,1,1,'c95c347de2c947323bd7d1196d8877b47b70f03dd4d8421036b039437180bc4a','f6a78dd6-cf5b-44b5-a596-6aa94db7b572','94624891-6bed-41ff-a899-b8b7260bc8ac','2026-08-25 09:23:15.374388','2026-08-18 09:24:42.645815',1,148,0,'admin@gmail.com','2026-08-18 09:23:15.374388','admin@gmail.com','2026-08-18 09:24:42.645815',0),(148,1,1,'252faefcb33c2f5d1dfae49d155004062c95cfa066398c341966963c3b9ac586','a9e0f6f4-83d8-47b8-88fa-48d6660b921e','94624891-6bed-41ff-a899-b8b7260bc8ac','2026-08-25 09:24:42.645815','2026-08-18 09:25:17.026666',1,NULL,0,'admin@gmail.com','2026-08-18 09:24:42.645815','admin@gmail.com','2026-08-18 09:25:17.026666',0),(149,1,1,'8971cb3dfd5427eb81a9db4f81032fdc513741f37da4b296da45e3339005a2cc','a7c97e8c-73d3-4493-8d43-948eaba4107f','f6e1b02a-b770-47eb-8e65-0cf9be6e6062','2026-08-25 09:25:23.940769','2026-08-19 02:41:41.205572',1,150,0,'admin@gmail.com','2026-08-18 09:25:23.940769','admin@gmail.com','2026-08-19 02:41:41.205572',0),(150,1,1,'d9682cdaa8a6c3ec7edfe7b8dd289c5f77325c506d0884e9df4473205676c901','c49e2095-4c23-47b2-a53d-8cc23e21426c','f6e1b02a-b770-47eb-8e65-0cf9be6e6062','2026-08-26 02:41:41.205572','2026-08-19 02:53:31.245753',1,151,0,'admin@gmail.com','2026-08-19 02:41:41.205572','admin@gmail.com','2026-08-19 02:53:31.245753',0),(151,1,1,'f66d79c5bb992d6e31fe913c05b118f3971a4a3ad001d5e1623b9d74eb6c50aa','7279d96b-e8e8-4d49-a37d-bec88b9b057b','f6e1b02a-b770-47eb-8e65-0cf9be6e6062','2026-08-26 02:53:31.245753','2026-08-19 03:06:58.786485',1,152,0,'admin@gmail.com','2026-08-19 02:53:31.245753','admin@gmail.com','2026-08-19 03:06:58.786485',0),(152,1,1,'3293401b89fcb0f13514422a814ddbfd30faf25dd1d5ae3cd2251dc16851b319','a7ba0f1e-f063-433b-9e2e-2705736ae5c0','f6e1b02a-b770-47eb-8e65-0cf9be6e6062','2026-08-26 03:06:58.786485','2026-08-19 03:07:05.736023',1,NULL,0,'admin@gmail.com','2026-08-19 03:06:58.786485','admin@gmail.com','2026-08-19 03:07:05.736023',0),(153,1,1,'69c1798b7857bef4a21394eb43725a0485e8ddc11b304e7b5657d8e294a8f04d','f294aded-dd32-4779-be75-22425179c2a2','3f5f986c-fb2e-4ad5-af62-3221e0bbe555','2026-08-26 03:07:05.736023','2026-08-19 03:07:09.756361',1,NULL,0,'admin@gmail.com','2026-08-19 03:07:05.736023','admin@gmail.com','2026-08-19 03:07:09.756361',0),(154,1,1,'eb498373ceaac581b40ee95c911251fc31dd22f0785b7d913f8f562acdd3fdb7','1d31a9fe-b8ce-4bad-a252-63bff5a40e89','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:07:09.756361','2026-08-19 03:07:52.337340',1,155,0,'admin@gmail.com','2026-08-19 03:07:09.756361','admin@gmail.com','2026-08-19 03:07:52.337340',0),(155,1,1,'71b84d5a9a63b4d73b6606346807f212518a2dd2fa35f9edb03920d5e83974ed','0f9c3b16-ca71-4f56-a059-dad3e0867a43','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:07:52.337340','2026-08-19 03:08:40.109108',1,156,0,'admin@gmail.com','2026-08-19 03:07:52.337340','admin@gmail.com','2026-08-19 03:08:40.109108',0),(156,1,1,'0a68fa227c88a57d7d6ee48119c5f19ff5877805e94dc76cc4e0bfc03027f121','69289da0-9955-4870-a6b6-3bae4b7f87e7','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:08:40.109108','2026-08-19 03:08:52.278320',1,157,0,'admin@gmail.com','2026-08-19 03:08:40.109108','admin@gmail.com','2026-08-19 03:08:52.278320',0),(157,1,1,'5ce96799309ed8fb8de35c5d19c25c3b5ad8cd0da5afd753fccb4764119bb844','b20dfced-830b-4da5-b182-86ac347d8edd','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:08:52.278320','2026-08-19 03:23:25.660484',1,158,0,'admin@gmail.com','2026-08-19 03:08:52.278320','admin@gmail.com','2026-08-19 03:23:25.660484',0),(158,1,1,'698878f2fd15f834c0bc252c75063782bc743f8863d251fad7f5a14b10242125','adf9714e-6f5f-428a-827b-9bbd3f6430b6','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:23:25.660484','2026-08-19 03:31:12.058228',1,159,0,'admin@gmail.com','2026-08-19 03:23:25.660484','admin@gmail.com','2026-08-19 03:31:12.058228',0),(159,1,1,'8999ed82ef862a03b8ecddbb86070c76cc54c88993313fa5f83392b20a0dc78e','82e922dc-1cf0-4729-a79b-9adf4fea9156','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:31:12.058228','2026-08-19 03:31:16.425482',1,160,0,'admin@gmail.com','2026-08-19 03:31:12.058228','admin@gmail.com','2026-08-19 03:31:16.425482',0),(160,1,1,'f319347d352adbc101b7548b4574a8b1501466ea6625f8133ca90b8edccbc1f4','37d75c31-e3ac-4a22-ad67-4a221e81e14a','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:31:16.425482','2026-08-19 03:31:23.385900',1,161,0,'admin@gmail.com','2026-08-19 03:31:16.425482','admin@gmail.com','2026-08-19 03:31:23.385900',0),(161,1,1,'87e16dce0ee6052b1886d8ffa8080c467968c732e2bdb6158cf65f9b600caf6c','284cf213-a4c3-4734-bce6-7a9d562b832d','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:31:23.385900','2026-08-19 03:31:26.495779',1,162,0,'admin@gmail.com','2026-08-19 03:31:23.385900','admin@gmail.com','2026-08-19 03:31:26.495779',0),(162,1,1,'8e5d2161cfbdca7b5bc69e94e516ceb77e63dc7a480a425a3f2c4c715d409f88','83d9d95d-5b21-4b60-83bd-01d9f1976390','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:31:26.495779','2026-08-19 03:31:30.127152',1,163,0,'admin@gmail.com','2026-08-19 03:31:26.495779','admin@gmail.com','2026-08-19 03:31:30.127152',0),(163,1,1,'358b895ebc92dd9e595a3ebf01e16496ef88c049e1e1faa69b7cd384bc180b37','f4050b6c-e98e-4baa-aaa8-3a183c2d3321','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:31:30.127152','2026-08-19 03:31:54.382638',1,164,0,'admin@gmail.com','2026-08-19 03:31:30.127152','admin@gmail.com','2026-08-19 03:31:54.382638',0),(164,1,1,'71d0a038c2cade9769f307832fb3350bf505c18d6c2a4b701269693428e499f9','7cec5abf-0f76-48a0-9149-a2c578d5e612','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:31:54.382638','2026-08-19 03:31:57.642175',1,165,0,'admin@gmail.com','2026-08-19 03:31:54.382638','admin@gmail.com','2026-08-19 03:31:57.642175',0),(165,1,1,'927d717f4ddc715ca0cb51cb7f3764e395dd52600976a0619c7deb9a643730e1','5503e5a9-9667-4bc8-8bc9-9ef492213754','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:31:57.642175','2026-08-19 03:32:01.809158',1,166,0,'admin@gmail.com','2026-08-19 03:31:57.642175','admin@gmail.com','2026-08-19 03:32:01.809158',0),(166,1,1,'359cce007d7d07142f17b56d54a60c8b505e87354fc759e885f72609ece978ab','42576c2d-1d65-4f98-91d2-29f9fc5ddf3a','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:32:01.809158','2026-08-19 03:32:05.583477',1,167,0,'admin@gmail.com','2026-08-19 03:32:01.809158','admin@gmail.com','2026-08-19 03:32:05.583477',0),(167,1,1,'6a3302396e031739fe478142d62439595a75da70d87a443031e3214d02b23f3c','69d61253-efb4-4dab-bee4-bcf2aab8c850','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:32:05.583477','2026-08-19 03:32:08.885018',1,168,0,'admin@gmail.com','2026-08-19 03:32:05.583477','admin@gmail.com','2026-08-19 03:32:08.885018',0),(168,1,1,'0bee91cb5e1db6f5a44a7a9629fc29c62361dfbf5a7c79cb68e26ec5dbbdc35f','4c188448-93d8-4565-8820-2af091aea8a8','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:32:08.885018','2026-08-19 03:33:08.616826',1,169,0,'admin@gmail.com','2026-08-19 03:32:08.885018','admin@gmail.com','2026-08-19 03:33:08.616826',0),(169,1,1,'732c52137bce7488704d29b67f6142a97a172a5f2942fe7285658b7a83e2640d','973c4885-bd69-4c4b-ac68-e825c28f209b','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:33:08.616826','2026-08-19 03:33:12.632082',1,170,0,'admin@gmail.com','2026-08-19 03:33:08.616826','admin@gmail.com','2026-08-19 03:33:12.632082',0),(170,1,1,'8e8a39553970359909543be60a97bc4a1c14e579734ca0c849bdba6396aa5af3','614ebba6-d5bc-428d-84d2-d1bb9fba294c','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:33:12.632082','2026-08-19 03:33:19.281517',1,171,0,'admin@gmail.com','2026-08-19 03:33:12.632082','admin@gmail.com','2026-08-19 03:33:19.281517',0),(171,1,1,'0e30c7eeeec56772b94726407344f2d040cb4d6711912a4bf6de554b180d8436','7f6cf885-fa4f-431a-bf8b-0078e4cfc126','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:33:19.281517','2026-08-19 03:33:22.544280',1,172,0,'admin@gmail.com','2026-08-19 03:33:19.281517','admin@gmail.com','2026-08-19 03:33:22.544280',0),(172,1,1,'8a8bc32cf9e2145d50f21bdedf582b4d1251fcb953d3268c5e2b7011d9fcfd36','e7da95f0-69f8-46a9-a177-8ffd4f7cbc29','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:33:22.544280','2026-08-19 03:33:25.269686',1,173,0,'admin@gmail.com','2026-08-19 03:33:22.544280','admin@gmail.com','2026-08-19 03:33:25.269686',0),(173,1,1,'2b1afa8fb258b10e7311da591bc81c3d36112a7a13f22c3560823471825c30e5','9952590e-5792-4503-9feb-38788de1204a','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:33:25.269686','2026-08-19 03:33:27.832327',1,174,0,'admin@gmail.com','2026-08-19 03:33:25.269686','admin@gmail.com','2026-08-19 03:33:27.832327',0),(174,1,1,'6c00aca1c24ce76188f632128a2e293ede9f78c6e1224f1eb341bb659f9b3e1c','da618110-f062-4b94-91df-dbc1e650a533','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:33:27.832327','2026-08-19 03:33:30.694733',1,175,0,'admin@gmail.com','2026-08-19 03:33:27.832327','admin@gmail.com','2026-08-19 03:33:30.694733',0),(175,1,1,'832c5b2fdde787c2c53ecba6e2a195a29e4a630dd4ba8c6e738e1792043b79b2','524d76b7-5c85-4e02-9874-9ed56ace268b','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:33:30.694733','2026-08-19 03:33:33.784938',1,176,0,'admin@gmail.com','2026-08-19 03:33:30.694733','admin@gmail.com','2026-08-19 03:33:33.784938',0),(176,1,1,'0737aa1f0c0078273203d0ad0fe80bd87ee08bc751ab5945119119442d7eb731','9d06024a-4e25-414c-9063-fa53c1f8f641','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:33:33.784938','2026-08-19 03:33:36.978536',1,177,0,'admin@gmail.com','2026-08-19 03:33:33.784938','admin@gmail.com','2026-08-19 03:33:36.978536',0),(177,1,1,'633a2e9eb7353b743385ca7a52e56f59f38ebda100244ab0d85f7b65f336655e','ecb409f2-43cc-4e62-8c76-b5f5201f2adf','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:33:36.978536','2026-08-19 03:33:40.092173',1,178,0,'admin@gmail.com','2026-08-19 03:33:36.978536','admin@gmail.com','2026-08-19 03:33:40.092173',0),(178,1,1,'40203ac4b59d46622af41ad15e35b335f37987f319733498d86838897eb2aa1e','2cea8db9-c0fb-4515-a5a8-b381b14046e2','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:33:40.092173','2026-08-19 03:33:52.802142',1,179,0,'admin@gmail.com','2026-08-19 03:33:40.092173','admin@gmail.com','2026-08-19 03:33:52.802142',0),(179,1,1,'8aa8a7c22ba2af0190e04de499b10bfc91d46886c5f307e6c64030b2db233eb2','f21c134c-85e6-43d5-b976-be801bcd457a','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:33:52.802142','2026-08-19 03:37:15.622954',1,180,0,'admin@gmail.com','2026-08-19 03:33:52.802142','admin@gmail.com','2026-08-19 03:37:15.622954',0),(180,1,1,'5c209fd3a112f9c8b9816b64a85c309c37395feb1127363c33710bd4eb460bf9','0741993d-e909-4005-86da-e603712c4c66','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:37:15.622954','2026-08-19 03:42:57.217924',1,181,0,'admin@gmail.com','2026-08-19 03:37:15.622954','admin@gmail.com','2026-08-19 03:42:57.217924',0),(181,1,1,'d44c0d8b068cbcd9817d7d533721df3474150c7b23debccf2f9640ba9ea79ec9','ffed1e55-0e02-446f-b7fa-60c907620e0d','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:42:57.217924','2026-08-19 03:43:10.874135',1,182,0,'admin@gmail.com','2026-08-19 03:42:57.217924','admin@gmail.com','2026-08-19 03:43:10.874135',0),(182,1,1,'7a4638b9aad4a8b7d3b6e39bd4a90d6fcaa9532766811da38c08880e847a8733','dc784a79-40e0-4f01-b963-967642fb142a','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:43:10.874135','2026-08-19 03:51:41.197499',1,183,0,'admin@gmail.com','2026-08-19 03:43:10.874135','admin@gmail.com','2026-08-19 03:51:41.197499',0),(183,1,1,'5c6e3748125b5947151e59f225baf92981054565917aee10ed9c0de2d0eccc4d','3cc937fc-67bb-448d-b44d-9cbbbb89cf3b','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 03:51:41.197499','2026-08-19 04:08:59.887846',1,184,0,'admin@gmail.com','2026-08-19 03:51:41.197499','admin@gmail.com','2026-08-19 04:08:59.887846',0),(184,1,1,'c09769a98768b657a7a555b9f09a195b22aa085bfa27dedf7fff52019efd55b4','1c7d5ab5-eda3-4bcd-b59e-599798a7e47f','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 04:08:59.887846','2026-08-19 06:04:31.289773',1,185,0,'admin@gmail.com','2026-08-19 04:08:59.887846','admin@gmail.com','2026-08-19 06:04:31.289773',0),(185,1,1,'2413018275189c377dae1686f571fa43f42f932f477fc1ec414aeb82bdf3641e','c97935b8-198a-4d24-8d7c-6f2d10b304fb','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 06:04:31.289773','2026-08-19 06:12:31.001919',1,186,0,'admin@gmail.com','2026-08-19 06:04:31.289773','admin@gmail.com','2026-08-19 06:12:31.001919',0),(186,1,1,'c87e95770c805e0aba65d0f9e35d9743b8a3b1b9026735c65998b1c87854c544','a1bc52bd-49c5-4923-808f-ecc2e8f1b428','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 06:12:31.001919','2026-08-19 06:12:45.485941',1,187,0,'admin@gmail.com','2026-08-19 06:12:31.001919','admin@gmail.com','2026-08-19 06:12:45.485941',0),(187,1,1,'5c960ad6420742475d40f54815ea7f0d7cdc40f86fa2fb689a652c049cd15ab6','603ed600-2a4a-41bd-a78c-49962fdfd368','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 06:12:45.485941','2026-08-19 06:12:45.808745',1,188,0,'admin@gmail.com','2026-08-19 06:12:45.485941','admin@gmail.com','2026-08-19 06:12:45.808745',0),(188,1,1,'16ba9ce285e69d87627668ae4f2be4f215f553f28c6c8885df97d938d954980c','f0cf5673-8fe2-49f4-a887-1cb365954611','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 06:12:45.808745','2026-08-19 06:12:46.194967',1,189,0,'admin@gmail.com','2026-08-19 06:12:45.808745','admin@gmail.com','2026-08-19 06:12:46.194967',0),(189,1,1,'c8f2a50f1acb4327b9348c355403042ba4d51a82654d0298b4aa7e278f4926e8','9f44b4f6-7881-42f0-96e8-6b333f8f1163','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 06:12:46.194967','2026-08-19 06:12:46.583496',1,190,0,'admin@gmail.com','2026-08-19 06:12:46.194967','admin@gmail.com','2026-08-19 06:12:46.583496',0),(190,1,1,'a7de11db3b1eb44143832834be882bbd4180f9012c9de1cd55efa9d2d4f91b8a','8a44b37e-7cbe-4d27-a3ef-b11c98874a11','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 06:12:46.583496','2026-08-19 06:12:47.187107',1,191,0,'admin@gmail.com','2026-08-19 06:12:46.583496','admin@gmail.com','2026-08-19 06:12:47.187107',0),(191,1,1,'0d5c38e3b71533f6f66bd6caebf02e58c7d51ae9c7ad566147cc0a34ec8b6f56','3bb8773d-9bc0-42b1-b282-271cdf4fc99b','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 06:12:47.187107','2026-08-19 06:12:47.536344',1,192,0,'admin@gmail.com','2026-08-19 06:12:47.187107','admin@gmail.com','2026-08-19 06:12:47.536344',0),(192,1,1,'c5c7ed19dab007a58c87bf8382a8124352f2dc5677b0754c3a992ceb42bae8fa','79911642-7f6e-49c2-8ee2-174eecae75ae','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 06:12:47.536344','2026-08-19 06:12:52.650394',1,193,0,'admin@gmail.com','2026-08-19 06:12:47.536344','admin@gmail.com','2026-08-19 06:12:52.650394',0),(193,1,1,'f2d9ecce38438180202afa7b7363724ddd756049c23e009bd924fcfa40493797','06da4c43-58a2-47f5-9376-5e95cfe9ba70','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 06:12:52.650394','2026-08-19 06:20:19.421165',1,194,0,'admin@gmail.com','2026-08-19 06:12:52.650394','admin@gmail.com','2026-08-19 06:20:19.421165',0),(194,1,1,'43fa87f3e68f1ccc8211812014267002d9fb01ab200206614d69c4126a42617e','240e2a35-d957-4d28-bf04-b15446d82d8b','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 06:20:19.421165','2026-08-19 07:16:06.675922',1,195,0,'admin@gmail.com','2026-08-19 06:20:19.421165','admin@gmail.com','2026-08-19 07:16:06.675922',0),(195,1,1,'454e8794c063cffdc1e6a1bf5171a113711d42b0848baeda503dd5e3bd9ff658','4df40636-1461-4564-9bcc-2aeab1c59b09','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 07:16:06.675922','2026-08-19 07:36:40.404261',1,196,0,'admin@gmail.com','2026-08-19 07:16:06.675922','admin@gmail.com','2026-08-19 07:36:40.404261',0),(196,1,1,'1e8ff40daca086115fe664d0467df6710b1bae6e7db1e773a8b4da9d6c1370eb','b8f1fb50-5334-4905-ae21-47690aadf060','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 07:36:40.404261','2026-08-19 07:37:01.828321',1,197,0,'admin@gmail.com','2026-08-19 07:36:40.404261','admin@gmail.com','2026-08-19 07:37:01.828321',0),(197,1,1,'02830e74f409101b150d3657e84121d4386b0c51e2e32799e289cd261644f114','1f342441-7a59-463c-8859-29724038e0a9','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 07:37:01.828321','2026-08-19 07:38:03.922332',1,198,0,'admin@gmail.com','2026-08-19 07:37:01.828321','admin@gmail.com','2026-08-19 07:38:03.922332',0),(198,1,1,'a4b11c5888c00fd3bee2d7d141b66e2cb22ebab7713121c1f115e9b3775ad7bd','b1cbe57e-8062-4213-89d8-5d929234424d','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 07:38:03.922332','2026-08-19 07:38:04.795256',1,199,0,'admin@gmail.com','2026-08-19 07:38:03.922332','admin@gmail.com','2026-08-19 07:38:04.795256',0),(199,1,1,'dc8bb04beaeba3e7cb51c2d58f4111beaaefb42afef47ac603f436c64efb8a0c','085e7fc3-35ea-4be1-ba58-21a1df89139f','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 07:38:04.795256','2026-08-19 07:38:05.214266',1,200,0,'admin@gmail.com','2026-08-19 07:38:04.795256','admin@gmail.com','2026-08-19 07:38:05.214266',0),(200,1,1,'aded1a52e9184c0d4b90a6bc1c57f4a353218767d29b04d0519724697dd66240','38dd74dc-c028-4149-9a56-aa11a00ebc5d','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 07:38:05.214266','2026-08-19 07:38:05.752139',1,201,0,'admin@gmail.com','2026-08-19 07:38:05.214266','admin@gmail.com','2026-08-19 07:38:05.752139',0),(201,1,1,'0c4d554d6d3723a99c319f73e150aa4678e9e1fc2d1455e2cfd7dad3f2b9bd8b','a18c88f8-a156-4d81-9f52-5c427994d963','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 07:38:05.752139','2026-08-19 07:38:07.420848',1,202,0,'admin@gmail.com','2026-08-19 07:38:05.752139','admin@gmail.com','2026-08-19 07:38:07.420848',0),(202,1,1,'9dd3cb321cfe9549e815dbc38b33f60e50a19871026ae29ce9c52fdde289c344','8559b662-d13a-4fb6-9106-a6bc25463b4e','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 07:38:07.420848','2026-08-19 08:23:26.765923',1,203,0,'admin@gmail.com','2026-08-19 07:38:07.420848','admin@gmail.com','2026-08-19 08:23:26.765923',0),(203,1,1,'77a2011c91c9c61a5828677fd4f994315ced668f67f7b665855fdfcfcbf6ea91','7f870dd0-17da-4bfc-a8b5-57644788e264','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 08:23:26.765923','2026-08-19 08:56:38.778706',1,204,0,'admin@gmail.com','2026-08-19 08:23:26.765923','admin@gmail.com','2026-08-19 08:56:38.778706',0),(204,1,1,'dbad24dd44b0ddce1874510a42411b9dd884dc091a0c61e95f102c73e8709d69','368147a6-09cd-4c4c-aac1-5db1f0f41e94','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 08:56:38.778706','2026-08-19 09:31:21.761270',1,205,0,'admin@gmail.com','2026-08-19 08:56:38.778706','admin@gmail.com','2026-08-19 09:31:21.761270',0),(205,1,1,'9062fca26cfe1362476c3e0c200f68bcc21f03edbc05433437c8a62493299ba1','9bb30c75-3749-442d-b5ec-e76427b35375','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 09:31:21.761270','2026-08-19 09:44:35.028366',1,206,0,'admin@gmail.com','2026-08-19 09:31:21.761270','admin@gmail.com','2026-08-19 09:44:35.028366',0),(206,1,1,'42dbd70690fa9fc84b3b40e54781ecfac71caa9e89efa3a72c681ce62090b207','6629951d-d83e-4a19-87af-72bf7c27fd28','a9efcc71-64fb-44cf-b48a-ca2d11eb3cbd','2026-08-26 09:44:35.028366','2026-08-19 09:44:39.060897',1,NULL,0,'admin@gmail.com','2026-08-19 09:44:35.028366','admin@gmail.com','2026-08-19 09:44:39.060897',0),(207,1,1,'485fe2a8a3df2deb2039f2ea6d2c82dd4c1e8f2d1511cf58e73224152aef7b1f','2323a613-479b-4cf0-a8b2-5d799d091c86','13327489-4076-4bdd-8883-04bd5502d87e','2026-08-26 09:45:56.053030','2026-08-19 09:46:43.920341',1,NULL,0,'admin@gmail.com','2026-08-19 09:45:56.053030','admin@gmail.com','2026-08-19 09:46:43.920341',0),(208,1,1,'83ca322b556f3e5abde785fa0b30a9f984e34f00d50f11fc27e3e350ccd137a2','f208ef8c-c35a-491a-a08b-002f8f205535','c9a522cf-c13b-4bff-9389-12f57c8db10c','2026-08-26 09:46:57.865777','2026-08-19 09:49:35.323189',1,NULL,0,'admin@gmail.com','2026-08-19 09:46:57.865777','admin@gmail.com','2026-08-19 09:49:35.323189',0),(209,1,1,'f5a7b220634687c9080ac8cb3aeeaa0c1686c5a5b862ae926e69b74e27d39c6f','ef452f7d-7442-47e0-b9f5-3f75fcb277c0','c376c4fe-1738-4883-8f7e-e6a556277670','2026-08-26 09:55:52.476732','2026-08-20 04:14:47.082903',1,210,0,'admin@gmail.com','2026-08-19 09:55:52.476732','admin@gmail.com','2026-08-20 04:14:47.082903',0),(210,1,1,'81908323d34921b5f9d1e1ccb913ea9977fc665d530f1970d00fa29de269277f','7fd06224-20ed-48f5-91f5-3d558844a5e3','c376c4fe-1738-4883-8f7e-e6a556277670','2026-08-27 04:14:47.082903','2026-08-20 04:14:51.378072',1,NULL,0,'admin@gmail.com','2026-08-20 04:14:47.082903','admin@gmail.com','2026-08-20 04:14:51.378072',0),(211,1,1,'1efdb86871819a7e55b281bda3d118182b0d2dbdc99ef30072965ae74603b316','7c74dc42-39ef-48b8-af06-aa6eae341962','1b7db308-9121-4cca-892e-d8aef7d4f4bf','2026-08-27 06:16:42.481025','2026-08-20 06:17:42.133673',1,NULL,0,'admin@gmail.com','2026-08-20 06:16:42.481025','admin@gmail.com','2026-08-20 06:17:42.133673',0),(212,9,1,'027c2fc57aad9a3ce20d3c4e670856fe75242a47b51b07d61e107363b610dd4f','6675a8d5-3068-45fd-94d7-c2a078944255','09317bea-ee3e-4c40-83cb-14202344d210','2026-08-27 07:01:12.852272','2026-08-20 07:01:17.637264',9,213,0,'hnhoanghien88@gmail.com','2026-08-20 07:01:12.852272','hnhoanghien88@gmail.com','2026-08-20 07:01:17.637264',0),(213,9,1,'2e0028224266bdad97381b5c3a329c7d38a8b0670082a3ac1a9e72f67743caa6','faf3e0aa-e64b-49d2-b67b-f6c045f1db87','09317bea-ee3e-4c40-83cb-14202344d210','2026-08-27 07:01:17.637264',NULL,NULL,NULL,1,'hnhoanghien88@gmail.com','2026-08-20 07:01:17.637264',NULL,NULL,0);
/*!40000 ALTER TABLE `refresh_tokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `resources`
--

DROP TABLE IF EXISTS `resources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `resources` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ApplicationId` bigint unsigned NOT NULL,
  `Code` varchar(120) NOT NULL,
  `Name` varchar(150) NOT NULL,
  `ResourceType` varchar(30) NOT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `CreatedBy` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `Version` bigint unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQResources` (`ApplicationId`,`Code`),
  CONSTRAINT `FK_resources_applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `applications` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=50012 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `resources`
--

LOCK TABLES `resources` WRITE;
/*!40000 ALTER TABLE `resources` DISABLE KEYS */;
INSERT INTO `resources` VALUES (1,1,'Users','Users','Menu','User management','system@company.com','2026-08-05 14:20:19.891601','admin','2026-08-13 03:20:32.899863',1,0,3),(2,1,'Roles','Roles','Menu','Role management','system@company.com','2026-08-05 14:20:19.891601','admin','2026-08-13 03:20:36.121314',1,0,3),(3,1,'RolePermissions','Role permissions','Menu','Role permissions','system@company.com','2026-08-05 14:20:19.891601','admin','2026-08-13 03:22:02.640154',1,0,4),(4,1,'Resources','Resources','Menu','Resources','system@company.com','2026-08-05 14:20:19.891601','admin','2026-08-13 03:21:32.041225',1,0,4),(5,1,'Applications','Applications','Menu','Applications','system@company.com','2026-08-05 14:20:19.891601','admin','2026-08-13 03:21:18.317691',1,0,4),(6,1,'Menus','Menus','Menu','Menus','admin','2026-08-13 03:22:55.317241',NULL,NULL,1,0,1),(7,1,'Actions','Actions','Menu','Actions','admin','2026-08-13 03:23:17.538084',NULL,NULL,1,0,1),(8,1,'UserRoles','User roles','Menu','User roles','admin','2026-08-13 03:23:44.062674',NULL,NULL,1,0,1),(30009,2,'test','test','test','test',NULL,'2026-08-13 09:55:54.233274',NULL,NULL,1,0,1),(30010,1,'RateLimiting','Rate limiting','Menu','Rate limiting',NULL,'2026-08-19 06:06:39.430269',NULL,'2026-08-19 06:09:26.688565',1,0,2),(30011,1,'RateLimits','Rate Limiting','Menu','Configure API rate limit policies','migration','2026-08-19 06:21:57.553787',NULL,'2026-08-19 09:31:32.675689',0,1,2);
/*!40000 ALTER TABLE `resources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `role_permissions`
--

DROP TABLE IF EXISTS `role_permissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `role_permissions` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `RoleId` bigint unsigned NOT NULL,
  `PermissionId` bigint unsigned NOT NULL,
  `CreatedBy` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQRolePermissions` (`RoleId`,`PermissionId`),
  KEY `IX_role_permissions_PermissionId` (`PermissionId`),
  CONSTRAINT `FK_role_permissions_permissions_PermissionId` FOREIGN KEY (`PermissionId`) REFERENCES `permissions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_role_permissions_roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=191 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role_permissions`
--

LOCK TABLES `role_permissions` WRITE;
/*!40000 ALTER TABLE `role_permissions` DISABLE KEYS */;
INSERT INTO `role_permissions` VALUES (53,1,31,'admin','2026-08-12 08:45:12.650271',NULL,NULL,1,0),(54,1,32,'admin','2026-08-12 08:45:13.298406',NULL,NULL,1,0),(57,1,35,'admin','2026-08-12 08:45:14.681293',NULL,NULL,1,0),(58,1,36,'admin','2026-08-12 08:45:15.131999',NULL,NULL,1,0),(59,1,37,'admin','2026-08-12 08:46:26.835594',NULL,NULL,1,0),(60,1,38,'admin','2026-08-12 08:46:27.186334',NULL,NULL,1,0),(61,1,39,'admin','2026-08-13 03:27:42.703290',NULL,NULL,1,0),(62,1,40,'admin','2026-08-13 03:27:43.178351',NULL,NULL,1,0),(63,1,41,'admin','2026-08-13 03:27:43.579361',NULL,NULL,1,0),(64,1,42,'admin','2026-08-13 03:27:44.001635',NULL,NULL,1,0),(65,1,43,'admin','2026-08-13 03:27:44.457040',NULL,NULL,1,0),(66,1,44,'admin','2026-08-13 03:27:44.956471',NULL,NULL,1,0),(67,1,45,'admin','2026-08-13 03:27:46.400916',NULL,NULL,1,0),(68,1,46,'admin','2026-08-13 03:27:46.707008',NULL,NULL,1,0),(69,1,47,'admin','2026-08-13 03:27:47.168303',NULL,NULL,1,0),(70,1,48,'admin','2026-08-13 03:27:47.517066',NULL,NULL,1,0),(71,1,49,'admin','2026-08-13 03:27:47.914942',NULL,NULL,1,0),(72,1,50,'admin','2026-08-13 03:27:48.299784',NULL,NULL,1,0),(73,1,51,'admin','2026-08-13 03:27:49.966860',NULL,NULL,1,0),(74,1,52,'admin','2026-08-13 03:27:50.277132',NULL,NULL,1,0),(75,1,53,'admin','2026-08-13 03:27:51.194972',NULL,NULL,1,0),(76,1,54,'admin','2026-08-13 03:27:51.526735',NULL,NULL,1,0),(77,1,55,'admin','2026-08-13 03:27:51.897021',NULL,NULL,1,0),(78,1,56,'admin','2026-08-13 03:27:52.240542',NULL,NULL,1,0),(79,1,57,'admin','2026-08-13 03:27:53.831551',NULL,NULL,1,0),(80,1,58,'admin','2026-08-13 03:27:54.144768',NULL,NULL,1,0),(81,1,59,'admin','2026-08-13 03:27:54.503379',NULL,NULL,1,0),(82,1,60,'admin','2026-08-13 03:27:54.910151',NULL,NULL,1,0),(83,1,61,'admin','2026-08-13 03:27:55.348601',NULL,NULL,1,0),(84,1,62,'admin','2026-08-13 03:27:55.740369',NULL,NULL,1,0),(85,1,63,'admin','2026-08-13 03:27:57.416445',NULL,NULL,1,0),(86,1,64,'admin','2026-08-13 03:27:57.713540',NULL,NULL,1,0),(87,1,65,'admin','2026-08-13 03:27:58.205362',NULL,NULL,1,0),(88,1,66,'admin','2026-08-13 03:27:58.548206',NULL,NULL,1,0),(89,1,67,'admin','2026-08-13 03:27:58.906215',NULL,NULL,1,0),(90,1,68,'admin','2026-08-13 03:27:59.348984',NULL,NULL,1,0),(91,1,69,'admin','2026-08-13 03:28:00.896721',NULL,NULL,1,0),(92,1,70,'admin','2026-08-13 03:28:01.227347',NULL,NULL,1,0),(93,1,71,'admin','2026-08-13 03:28:01.934790',NULL,NULL,1,0),(94,1,72,'admin','2026-08-13 03:28:02.253839',NULL,NULL,1,0),(95,1,73,'admin','2026-08-13 03:28:02.646651',NULL,NULL,1,0),(96,1,74,'admin','2026-08-13 03:28:03.022193',NULL,NULL,1,0),(97,1,75,'admin','2026-08-13 03:28:04.770815',NULL,NULL,1,0),(98,1,76,'admin','2026-08-13 03:28:05.122006',NULL,NULL,1,0),(99,1,77,'admin','2026-08-13 03:28:05.588844',NULL,NULL,1,0),(100,1,78,'admin','2026-08-13 03:28:05.955582',NULL,NULL,1,0),(101,1,79,'admin','2026-08-13 03:28:06.468025',NULL,NULL,1,0),(102,1,80,'admin','2026-08-13 03:28:06.784425',NULL,NULL,1,0),(103,2,69,NULL,'2026-08-13 07:08:34.083523',NULL,NULL,1,0),(104,2,70,NULL,'2026-08-13 07:08:48.608914',NULL,NULL,1,0),(105,2,71,NULL,'2026-08-13 07:08:49.573537',NULL,NULL,1,0),(106,2,72,NULL,'2026-08-13 07:08:49.998068',NULL,NULL,1,0),(107,2,73,NULL,'2026-08-13 07:08:50.421984',NULL,NULL,1,0),(108,2,74,NULL,'2026-08-13 07:08:50.787960',NULL,NULL,1,0),(109,2,35,NULL,'2026-08-13 07:09:04.777593',NULL,NULL,1,0),(110,2,43,NULL,'2026-08-13 07:09:12.463250',NULL,NULL,1,0),(111,2,49,NULL,'2026-08-13 07:09:14.392518',NULL,NULL,1,0),(112,2,55,NULL,'2026-08-13 07:09:16.135259',NULL,NULL,1,0),(113,2,61,NULL,'2026-08-13 07:39:04.302649',NULL,NULL,1,0),(114,2,67,NULL,'2026-08-13 07:39:07.038050',NULL,NULL,1,0),(115,2,79,NULL,'2026-08-13 07:39:09.956150',NULL,NULL,1,0),(116,3,79,NULL,'2026-08-13 07:39:16.219847',NULL,NULL,1,0),(118,3,67,NULL,'2026-08-13 07:39:19.357903',NULL,NULL,1,0),(119,3,61,NULL,'2026-08-13 07:39:20.844910',NULL,NULL,1,0),(120,3,55,NULL,'2026-08-13 07:39:22.559425',NULL,NULL,1,0),(122,3,43,NULL,'2026-08-13 07:39:25.538628',NULL,NULL,1,0),(123,3,35,NULL,'2026-08-13 07:39:27.055560',NULL,NULL,1,0),(124,1,81,NULL,'2026-08-13 07:52:25.733481',NULL,NULL,1,0),(125,1,82,NULL,'2026-08-13 07:52:27.367237',NULL,NULL,1,0),(126,1,83,NULL,'2026-08-13 07:52:28.609536',NULL,NULL,1,0),(127,1,84,NULL,'2026-08-13 07:52:30.025287',NULL,NULL,1,0),(128,1,85,NULL,'2026-08-13 07:52:31.250699',NULL,NULL,1,0),(129,1,86,NULL,'2026-08-13 07:52:32.532945',NULL,NULL,1,0),(130,1,87,NULL,'2026-08-13 07:52:33.882264',NULL,NULL,1,0),(131,1,88,NULL,'2026-08-13 07:52:35.290604',NULL,NULL,1,0),(132,2,87,NULL,'2026-08-13 07:52:54.299784',NULL,NULL,1,0),(133,2,86,NULL,'2026-08-13 07:52:56.046671',NULL,NULL,1,0),(134,2,85,NULL,'2026-08-13 07:52:57.691550',NULL,NULL,1,0),(135,2,84,NULL,'2026-08-13 07:52:59.352372',NULL,NULL,1,0),(136,3,73,NULL,'2026-08-19 03:51:49.660784',NULL,NULL,1,0),(139,3,85,NULL,'2026-08-19 03:52:01.544894',NULL,NULL,1,0),(140,3,87,NULL,'2026-08-19 03:52:03.849291',NULL,NULL,1,0),(141,3,86,NULL,'2026-08-19 03:52:06.182330',NULL,NULL,1,0),(142,3,84,NULL,'2026-08-19 03:52:07.610715',NULL,NULL,1,0),(144,3,83,NULL,'2026-08-19 03:52:10.057436',NULL,NULL,1,0),(145,3,49,NULL,'2026-08-19 03:52:11.125011',NULL,NULL,1,0),(146,3,82,NULL,'2026-08-19 03:52:12.431700',NULL,NULL,1,0),(147,3,88,NULL,'2026-08-19 03:52:14.646531',NULL,NULL,1,0),(148,3,81,NULL,'2026-08-19 03:52:15.754856',NULL,NULL,1,0),(149,1,89,NULL,'2026-08-19 06:12:45.169228',NULL,NULL,1,0),(150,1,90,NULL,'2026-08-19 06:12:45.582864',NULL,NULL,1,0),(151,1,91,NULL,'2026-08-19 06:12:45.886752',NULL,NULL,1,0),(152,1,92,NULL,'2026-08-19 06:12:46.262424',NULL,NULL,1,0),(153,1,93,NULL,'2026-08-19 06:12:46.652212',NULL,NULL,1,0),(154,1,94,NULL,'2026-08-19 06:12:47.248082',NULL,NULL,1,0),(155,1,95,NULL,'2026-08-19 06:12:47.609279',NULL,NULL,1,0),(156,3,93,NULL,'2026-08-19 06:12:55.122403',NULL,NULL,1,0),(157,2,93,NULL,'2026-08-19 06:12:58.754059',NULL,NULL,1,0),(173,5,73,NULL,'2026-08-20 06:17:16.189606',NULL,NULL,1,0),(174,5,61,NULL,'2026-08-20 06:17:17.859385',NULL,NULL,1,0),(175,5,67,NULL,'2026-08-20 06:17:19.229191',NULL,NULL,1,0),(176,5,86,NULL,'2026-08-20 06:17:20.016563',NULL,NULL,1,0),(177,5,87,NULL,'2026-08-20 06:17:21.498627',NULL,NULL,1,0),(178,5,85,NULL,'2026-08-20 06:17:23.175846',NULL,NULL,1,0),(179,5,93,NULL,'2026-08-20 06:17:25.805177',NULL,NULL,1,0),(180,5,95,NULL,'2026-08-20 06:17:26.394089',NULL,NULL,1,0),(181,5,55,NULL,'2026-08-20 06:17:27.897900',NULL,NULL,1,0),(182,5,84,NULL,'2026-08-20 06:17:28.407974',NULL,NULL,1,0),(183,5,49,NULL,'2026-08-20 06:17:29.774157',NULL,NULL,1,0),(184,5,83,NULL,'2026-08-20 06:17:30.309539',NULL,NULL,1,0),(185,5,43,NULL,'2026-08-20 06:17:31.785486',NULL,NULL,1,0),(186,5,82,NULL,'2026-08-20 06:17:32.401947',NULL,NULL,1,0),(187,5,79,NULL,'2026-08-20 06:17:34.065428',NULL,NULL,1,0),(188,5,88,NULL,'2026-08-20 06:17:34.586388',NULL,NULL,1,0),(189,5,35,NULL,'2026-08-20 06:17:35.999236',NULL,NULL,1,0),(190,5,81,NULL,'2026-08-20 06:17:36.627061',NULL,NULL,1,0);
/*!40000 ALTER TABLE `role_permissions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ApplicationId` bigint unsigned NOT NULL,
  `Code` varchar(100) NOT NULL,
  `Name` varchar(150) NOT NULL,
  `IsSystemRole` tinyint(1) NOT NULL,
  `CreatedBy` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `Version` bigint unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQRoles` (`ApplicationId`,`Code`),
  CONSTRAINT `FK_roles_applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `applications` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,1,'Admin','Admin',1,'system@company.com','2026-08-05 14:20:19.906653',NULL,NULL,1,0,1),(2,1,'Manager','Manager',1,'system@company.com','2026-08-05 14:20:19.906653',NULL,NULL,1,0,1),(3,1,'Employee','Employee',1,'system@company.com','2026-08-05 14:20:19.906653',NULL,NULL,1,0,1),(4,2,'testrole','testrole',1,NULL,'2026-08-13 09:56:26.114345',NULL,NULL,1,0,1),(5,1,'Viewer','Viewer',1,NULL,'2026-08-20 06:17:01.647658',NULL,NULL,1,0,1);
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_roles`
--

DROP TABLE IF EXISTS `user_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_roles` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` bigint unsigned NOT NULL,
  `RoleId` bigint unsigned NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedBy` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQUserRoles` (`UserId`,`RoleId`),
  KEY `IX_user_roles_RoleId` (`RoleId`),
  CONSTRAINT `FK_user_roles_roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_user_roles_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_roles`
--

LOCK TABLES `user_roles` WRITE;
/*!40000 ALTER TABLE `user_roles` DISABLE KEYS */;
INSERT INTO `user_roles` VALUES (10,1,1,1,'system@company.com','2026-08-05 14:25:21.528941',NULL,NULL,0),(11,2,2,1,'system@company.com','2026-08-05 14:25:21.528941',NULL,NULL,0),(12,3,3,1,'system@company.com','2026-08-05 14:25:21.528941',NULL,NULL,0),(13,8,2,1,NULL,'2026-08-13 07:08:18.604569',NULL,NULL,0),(14,9,5,1,'external:google','2026-08-20 07:01:04.612796',NULL,NULL,0);
/*!40000 ALTER TABLE `user_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Email` varchar(254) NOT NULL,
  `DisplayName` varchar(255) NOT NULL,
  `PasswordHash` varchar(500) DEFAULT NULL,
  `SecurityStamp` char(36) NOT NULL,
  `PermissionVersion` int NOT NULL DEFAULT '1',
  `CreatedBy` varchar(255) DEFAULT NULL,
  `CreatedDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `UpdatedDate` datetime(6) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `Version` bigint unsigned NOT NULL DEFAULT '1',
  `Code` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQUsersCode` (`Code`),
  UNIQUE KEY `UQUsersEmail` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin@gmail.com','admin','pbkdf2-sha256$100000$G9pK9JbSLNAv07+Do8Bw5g==$ysLL9Ga0haI+hrnt9gf9m+vc1Vm1ZUH573WmCHq2jHg=','d0201823-2d5d-43e1-97ba-20e4056cdf47',21,NULL,'2026-08-05 06:57:56.914471',NULL,NULL,1,0,1,'admin'),(2,'superadmin@company.com','Super Admin','$2a$12$samplebcrypthash','516ad0e6-b98f-403b-8324-a4c4d6e8b258',22,'system@company.com','2026-08-05 14:25:01.468932',NULL,'2026-08-11 07:12:02.162436',0,1,2,'user-2'),(3,'admin@company.com','System Admin','$2a$12$samplebcrypthash','8759e8d6-9d7e-4e05-a25d-01668736d2c1',31,'system@company.com','2026-08-05 14:25:01.468932',NULL,'2026-08-11 07:12:05.997542',0,1,2,'user-3'),(4,'user1@company.com','John Smith','$2a$12$samplebcrypthash','6a29552d-4ef2-499f-a947-c62636deeb15',2,'system@company.com','2026-08-05 14:25:01.468932',NULL,'2026-08-11 07:12:07.980428',0,1,2,'user-4'),(5,'admin2@gmail.com','admin 2','pbkdf2-sha256$100000$O7xkI/9NOqaArrbSipzPnQ==$1z9Bihrj7xSCzkJnXdv3CUHv8rqJgzHsnurS+6ufXes=','11e0399f-f852-4b4c-aa55-5c720ed98cf0',2,NULL,'2026-08-05 07:59:36.828165',NULL,'2026-08-10 08:02:16.505903',0,1,3,'user-5'),(6,'admin1@gmail.com','admin 1','pbkdf2-sha256$100000$Ndz/cmqNZomrwxcDrCg4KA==$KstjLqaFqNXw6tPT0e5A9SRTczuv1FoTegZF6ru94xg=','eea107e3-6ff9-4547-a514-ed46491aa61a',2,NULL,'2026-08-10 08:01:35.402933',NULL,'2026-08-11 07:12:00.208214',0,1,2,'user-6'),(7,'smoke-1786355029@example.test','Smoke User Updated','pbkdf2-sha256$100000$dO034n5jjZjBLV/cfLG4ZA==$Dhu7F91IA6Xra/K3LNnZH7YaR/u9ROMFR3l0lmbCXJs=','94fdac86-9de8-422c-8a4b-4ba1dabb5572',2,NULL,'2026-08-10 09:43:49.413483',NULL,'2026-08-10 09:43:49.523610',0,1,3,'smoke-1786355029-updated'),(8,'hien@gmail.com','hoang hien 1','pbkdf2-sha256$100000$HkoITmvkHW12o5gpHaoqwg==$9r8Aud7F/L2qHPOKYQvJ9dZA3mPG5uoDeuaD0ZE9soI=','0426a105-d10d-43cc-8570-69e8a41cdd01',22,NULL,'2026-08-11 03:01:35.426183',NULL,'2026-08-11 07:12:43.801130',1,0,2,'hien'),(9,'hnhoanghien88@gmail.com','Hiển Hoàng',NULL,'37df2f52-fc8a-4fbe-890e-5d4abf912c5a',1,'external:google','2026-08-20 07:01:04.612796',NULL,NULL,1,0,1,'g_e976d7d3beb7ae9bfde1f25708fa6178802887e7cc807d3a');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-08-20 14:06:38
