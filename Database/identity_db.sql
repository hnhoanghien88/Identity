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
INSERT INTO `__efmigrationshistory` VALUES ('20260805063246_InitialDB','10.0.7'),('20260810070318_AddUserVersion','10.0.7'),('20260810085053_AddUserCodeIdentity','10.0.7'),('20260810090000_EnforceUserCodeIdentity','10.0.7'),('20260810093448_RemoveNormalizedUserIdentity','10.0.7'),('20260811054449_AddApplicationVersion','10.0.7'),('20260811094606_AddResourceVersion','10.0.7'),('20260812031658_AddPermissionActionVersion','10.0.7'),('20260812040001_AddMenuVersion','10.0.7'),('20260812055052_AddRoleVersion','10.0.7'),('20260812065014_RemovePermissionApplicationId','10.0.7'),('20260812080000_RemovePermissionIsDeleted','10.0.7'),('20260812081000_RemovePermissionIsActive','10.0.7');
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
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `applications`
--

LOCK TABLES `applications` WRITE;
/*!40000 ALTER TABLE `applications` DISABLE KEYS */;
INSERT INTO `applications` VALUES (1,'Identity-api','Identity API','Identity-api','Main authentication application','system@company.com','2026-08-05 14:20:19.881534','admin','2026-08-11 07:10:58.047105',1,0,2),(2,'Identity-client','Identity client','Identity-client','Identity client','admin','2026-08-11 07:11:28.557225',NULL,NULL,1,0,1);
/*!40000 ALTER TABLE `applications` ENABLE KEYS */;
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
  `IsVisible` tinyint(1) NOT NULL DEFAULT '1',
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
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menus`
--

LOCK TABLES `menus` WRITE;
/*!40000 ALTER TABLE `menus` DISABLE KEYS */;
INSERT INTO `menus` VALUES (1,1,NULL,5,'DASHBOARD','Dashboard','/dashboard','Dashboard',1,1,'system@company.com','2026-08-05 14:20:19.896678','admin','2026-08-13 03:24:06.702849',0,1,2),(2,1,NULL,NULL,'Identity','Identity',NULL,'AdminPanelSettings',1,1,'system@company.com','2026-08-05 14:20:19.896678','Codex','2026-08-13 08:08:42.000000',1,0,2),(3,1,2,1,'users','Users','/users','People',8,1,'system@company.com','2026-08-05 14:20:19.896678','Codex','2026-08-13 08:08:42.000000',1,0,5),(4,1,2,2,'roles','Roles','/roles','Security',6,1,'system@company.com','2026-08-05 14:20:19.896678','Codex','2026-08-13 08:08:42.000000',1,0,4),(5,1,2,3,'role-permissions','Role permissions','/role-permissions','Lock',7,1,'system@company.com','2026-08-05 14:20:19.896678','Codex','2026-08-13 08:08:42.000000',1,0,3),(6,1,2,5,'applications','Applications','/applications','Apps',2,1,NULL,'2026-08-13 03:32:27.606614','Codex','2026-08-13 08:08:42.000000',1,0,2),(7,1,2,4,'resources','Resources','/resources','Category',3,1,NULL,'2026-08-13 03:32:51.376781','Codex','2026-08-13 08:08:42.000000',1,0,2),(8,1,2,6,'menus','Menus','/menus','AccountTree',4,1,NULL,'2026-08-13 03:33:17.417406','Codex','2026-08-13 08:08:42.000000',1,0,2),(9,1,2,7,'actions','Actions','/actions','Bolt',5,1,NULL,'2026-08-13 03:33:56.016426','Codex','2026-08-13 08:08:42.000000',1,0,2),(10,1,2,8,'user-roles','User roles','/user-roles','ManageAccounts',9,1,NULL,'2026-08-13 03:34:25.588780','Codex','2026-08-13 08:08:42.000000',1,0,2);
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQPermissionActions` (`Code`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permission_actions`
--

LOCK TABLES `permission_actions` WRITE;
/*!40000 ALTER TABLE `permission_actions` DISABLE KEYS */;
INSERT INTO `permission_actions` VALUES (1,'Read','Read','system@company.com','2026-08-05 14:20:19.887446','admin','2026-08-12 03:34:04.399091',2),(2,'Create','Create','system@company.com','2026-08-05 14:20:19.887446','admin','2026-08-12 03:34:09.515977',2),(3,'Update','Update','system@company.com','2026-08-05 14:20:19.887446','admin','2026-08-12 03:34:13.805456',2),(4,'Delete','Delete','system@company.com','2026-08-05 14:20:19.887446','admin','2026-08-12 03:34:17.504327',2),(5,'Export','Export','system@company.com','2026-08-05 14:20:19.887446','admin','2026-08-12 08:46:20.910419',3),(6,'Import','Import','admin','2026-08-12 03:33:57.587650',NULL,NULL,1),(7,'ViewMenu','View menu',NULL,'2026-08-13 07:52:12.173669',NULL,NULL,1);
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQPermissions` (`ResourceId`,`ActionId`),
  KEY `IX_permissions_ActionId` (`ActionId`),
  CONSTRAINT `FK_permissions_permission_actions_ActionId` FOREIGN KEY (`ActionId`) REFERENCES `permission_actions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_permissions_resources_ResourceId` FOREIGN KEY (`ResourceId`) REFERENCES `resources` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=89 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permissions`
--

LOCK TABLES `permissions` WRITE;
/*!40000 ALTER TABLE `permissions` DISABLE KEYS */;
INSERT INTO `permissions` VALUES (31,1,2,'Users.Create','Create','admin','2026-08-12 08:45:12.638802',NULL,NULL),(32,1,4,'Users.Delete','Delete','admin','2026-08-12 08:45:13.294289',NULL,NULL),(35,1,1,'Users.Read','Read','admin','2026-08-12 08:45:14.678380',NULL,NULL),(36,1,3,'Users.Update','Update','admin','2026-08-12 08:45:15.129754',NULL,NULL),(37,1,5,'Users.Export','Export','admin','2026-08-12 08:46:26.825029',NULL,NULL),(38,1,6,'Users.Import','Import','admin','2026-08-12 08:46:27.181055',NULL,NULL),(39,2,2,'Roles.Create','Create','admin','2026-08-13 03:27:42.661385',NULL,NULL),(40,2,4,'Roles.Delete','Delete','admin','2026-08-13 03:27:43.173279',NULL,NULL),(41,2,5,'Roles.Export','Export','admin','2026-08-13 03:27:43.575718',NULL,NULL),(42,2,6,'Roles.Import','Import','admin','2026-08-13 03:27:43.998142',NULL,NULL),(43,2,1,'Roles.Read','Read','admin','2026-08-13 03:27:44.454131',NULL,NULL),(44,2,3,'Roles.Update','Update','admin','2026-08-13 03:27:44.953910',NULL,NULL),(45,3,2,'RolePermissions.Create','Create','admin','2026-08-13 03:27:46.397841',NULL,NULL),(46,3,4,'RolePermissions.Delete','Delete','admin','2026-08-13 03:27:46.704386',NULL,NULL),(47,3,5,'RolePermissions.Export','Export','admin','2026-08-13 03:27:47.164911',NULL,NULL),(48,3,6,'RolePermissions.Import','Import','admin','2026-08-13 03:27:47.513693',NULL,NULL),(49,3,1,'RolePermissions.Read','Read','admin','2026-08-13 03:27:47.911936',NULL,NULL),(50,3,3,'RolePermissions.Update','Update','admin','2026-08-13 03:27:48.297092',NULL,NULL),(51,4,2,'Resources.Create','Create','admin','2026-08-13 03:27:49.963218',NULL,NULL),(52,4,4,'Resources.Delete','Delete','admin','2026-08-13 03:27:50.273006',NULL,NULL),(53,4,5,'Resources.Export','Export','admin','2026-08-13 03:27:51.192149',NULL,NULL),(54,4,6,'Resources.Import','Import','admin','2026-08-13 03:27:51.523353',NULL,NULL),(55,4,1,'Resources.Read','Read','admin','2026-08-13 03:27:51.893946',NULL,NULL),(56,4,3,'Resources.Update','Update','admin','2026-08-13 03:27:52.235820',NULL,NULL),(57,5,2,'Applications.Create','Create','admin','2026-08-13 03:27:53.828000',NULL,NULL),(58,5,4,'Applications.Delete','Delete','admin','2026-08-13 03:27:54.142014',NULL,NULL),(59,5,5,'Applications.Export','Export','admin','2026-08-13 03:27:54.499950',NULL,NULL),(60,5,6,'Applications.Import','Import','admin','2026-08-13 03:27:54.907956',NULL,NULL),(61,5,1,'Applications.Read','Read','admin','2026-08-13 03:27:55.345847',NULL,NULL),(62,5,3,'Applications.Update','Update','admin','2026-08-13 03:27:55.737647',NULL,NULL),(63,6,2,'Menus.Create','Create','admin','2026-08-13 03:27:57.412933',NULL,NULL),(64,6,4,'Menus.Delete','Delete','admin','2026-08-13 03:27:57.710922',NULL,NULL),(65,6,5,'Menus.Export','Export','admin','2026-08-13 03:27:58.203098',NULL,NULL),(66,6,6,'Menus.Import','Import','admin','2026-08-13 03:27:58.545168',NULL,NULL),(67,6,1,'Menus.Read','Read','admin','2026-08-13 03:27:58.902569',NULL,NULL),(68,6,3,'Menus.Update','Update','admin','2026-08-13 03:27:59.345391',NULL,NULL),(69,7,2,'Actions.Create','Create','admin','2026-08-13 03:28:00.894350',NULL,NULL),(70,7,4,'Actions.Delete','Delete','admin','2026-08-13 03:28:01.225165',NULL,NULL),(71,7,5,'Actions.Export','Export','admin','2026-08-13 03:28:01.932708',NULL,NULL),(72,7,6,'Actions.Import','Import','admin','2026-08-13 03:28:02.251178',NULL,NULL),(73,7,1,'Actions.Read','Read','admin','2026-08-13 03:28:02.644090',NULL,NULL),(74,7,3,'Actions.Update','Update','admin','2026-08-13 03:28:03.020084',NULL,NULL),(75,8,2,'UserRoles.Create','Create','admin','2026-08-13 03:28:04.768074',NULL,NULL),(76,8,4,'UserRoles.Delete','Delete','admin','2026-08-13 03:28:05.119661',NULL,NULL),(77,8,5,'UserRoles.Export','Export','admin','2026-08-13 03:28:05.586041',NULL,NULL),(78,8,6,'UserRoles.Import','Import','admin','2026-08-13 03:28:05.953054',NULL,NULL),(79,8,1,'UserRoles.Read','Read','admin','2026-08-13 03:28:06.465977',NULL,NULL),(80,8,3,'UserRoles.Update','Update','admin','2026-08-13 03:28:06.782470',NULL,NULL),(81,1,7,'Users.ViewMenu','View menu',NULL,'2026-08-13 07:52:25.722384',NULL,NULL),(82,2,7,'Roles.ViewMenu','View menu',NULL,'2026-08-13 07:52:27.364906',NULL,NULL),(83,3,7,'RolePermissions.ViewMenu','View menu',NULL,'2026-08-13 07:52:28.606746',NULL,NULL),(84,4,7,'Resources.ViewMenu','View menu',NULL,'2026-08-13 07:52:30.023223',NULL,NULL),(85,5,7,'Applications.ViewMenu','View menu',NULL,'2026-08-13 07:52:31.247910',NULL,NULL),(86,6,7,'Menus.ViewMenu','View menu',NULL,'2026-08-13 07:52:32.530210',NULL,NULL),(87,7,7,'Actions.ViewMenu','View menu',NULL,'2026-08-13 07:52:33.879661',NULL,NULL),(88,8,7,'UserRoles.ViewMenu','View menu',NULL,'2026-08-13 07:52:35.286822',NULL,NULL);
/*!40000 ALTER TABLE `permissions` ENABLE KEYS */;
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQRefreshTokenHash` (`TokenHash`),
  KEY `IX_refresh_tokens_ApplicationId` (`ApplicationId`),
  KEY `IX_refresh_tokens_ReplacedByTokenId` (`ReplacedByTokenId`),
  KEY `IX_refresh_tokens_UserId` (`UserId`),
  CONSTRAINT `FK_refresh_tokens_applications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `applications` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_refresh_tokens_refresh_tokens_ReplacedByTokenId` FOREIGN KEY (`ReplacedByTokenId`) REFERENCES `refresh_tokens` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_refresh_tokens_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=150 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `refresh_tokens`
--

LOCK TABLES `refresh_tokens` WRITE;
/*!40000 ALTER TABLE `refresh_tokens` DISABLE KEYS */;
INSERT INTO `refresh_tokens` VALUES (31,1,1,'0309f6cde3578903e2a540a49097eea5c861b8f45447ee6bda9049dbe1e95738','5d3c16c9-7c51-4b48-a36b-4375668782cf','b72b98e7-f5a4-4dd6-9fe7-ec84d0c92b72','2026-08-17 06:00:40.483918','2026-08-10 06:00:49.027648',1,32,0,'admin','2026-08-10 06:00:40.483918','admin','2026-08-10 06:00:49.027648'),(32,1,1,'6e7eca5d8c978279c7c0e10c99cd72016b6777372341cc4990f2af9d406d53d6','dfb7c80c-7daf-4b5c-9195-3a03886ce242','b72b98e7-f5a4-4dd6-9fe7-ec84d0c92b72','2026-08-17 06:00:49.027648','2026-08-10 07:53:41.141955',1,NULL,0,'admin','2026-08-10 06:00:49.027648','admin','2026-08-10 07:53:41.141955'),(33,1,1,'ed4adaa09776a42a162beceb5955c375e8109d51cd965f2d183ae3b0fff1f929','92c87e9e-eaac-497d-a624-db1cf7a0f47b','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:53:41.141955','2026-08-10 07:54:12.034062',1,34,0,'admin','2026-08-10 07:53:41.141955','admin','2026-08-10 07:54:12.034062'),(34,1,1,'bf57d39ed50990c6af7c96f61b1cea0f610738696c56607a8cade1872f0cb018','b1667014-2e8a-47b1-8acd-dca12ca7503c','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:54:12.034062','2026-08-10 07:54:17.514404',1,35,0,'admin','2026-08-10 07:54:12.034062','admin','2026-08-10 07:54:17.514404'),(35,1,1,'75a7281997b4931b49977c2d832fc337ab9ba29a74cad8af01d0b2d282b1b05f','212a5afb-5a54-4eb2-b287-8a4762effbb2','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:54:17.514404','2026-08-10 07:54:30.644711',1,36,0,'admin','2026-08-10 07:54:17.514404','admin','2026-08-10 07:54:30.644711'),(36,1,1,'4d001579f6a38531630fb34f768887ea30b0f284b8a5f7503c6d6fdf789986cd','5c9b5368-d9fd-497b-86b2-1349ac213bf3','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:54:30.644711','2026-08-10 07:54:31.803163',1,37,0,'admin','2026-08-10 07:54:30.644711','admin','2026-08-10 07:54:31.803163'),(37,1,1,'3f68fdf93e01918a9ded778d1f0ecbdb7da9df8c0ba27f15ca665524ee1b6638','5d5f99ea-55a4-47d9-a4e8-0766bb891943','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:54:31.803163','2026-08-10 07:54:38.430777',1,38,0,'admin','2026-08-10 07:54:31.803163','admin','2026-08-10 07:54:38.430777'),(38,1,1,'5f3f3bf7b691b699e78d5308b46a5ef87f555f9f1a950457ae0660bcd0ea1ccb','8750dee2-f9c2-449e-bf4e-644bb2f2e648','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:54:38.430777','2026-08-10 07:54:41.549265',1,39,0,'admin','2026-08-10 07:54:38.430777','admin','2026-08-10 07:54:41.549265'),(39,1,1,'57adf6df9d9e81fa2fdbab699b6dfa021d9560a2616289108aab21f373024961','2b201cd5-8af4-4fdd-8f74-8ebe38a1ebcc','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:54:41.549265','2026-08-10 07:55:57.973373',1,40,0,'admin','2026-08-10 07:54:41.549265','admin','2026-08-10 07:55:57.973373'),(40,1,1,'7f945ad61c303eeb940c8a4d66bc836f17f5de737932aee8cd3aba7a11c22e7d','b9d7820b-2973-4172-9a02-cd252bc89070','2a16ca46-b074-43a3-9a65-189060836c9d','2026-08-17 07:55:57.973373','2026-08-10 07:56:08.752365',1,NULL,0,'admin','2026-08-10 07:55:57.973373','admin','2026-08-10 07:56:08.752365'),(41,1,1,'1b39692a97ca01c9b5e8fefcecb74345db7f52230e57278c06831dd68b165624','eaeef95e-10c0-42d4-84ad-2f5fe4bc270e','9589a287-2a6f-4f5a-a41b-611f8cc09c55','2026-08-17 07:56:08.752365','2026-08-10 07:56:09.818617',1,42,0,'admin','2026-08-10 07:56:08.752365','admin','2026-08-10 07:56:09.818617'),(42,1,1,'233754e1f4cddf20d492c659b49ca5e7178b140d43fc4664a317beb957018e81','33b598de-6d90-47bb-b41f-f7d3ca11d91f','9589a287-2a6f-4f5a-a41b-611f8cc09c55','2026-08-17 07:56:09.818617','2026-08-10 08:00:16.043298',1,43,0,'admin','2026-08-10 07:56:09.818617','admin','2026-08-10 08:00:16.043298'),(43,1,1,'14d4931463e2fe254a4317e85bcc47afab18be86bfea8b4c95e81eb8a8d99a45','82bcbb0b-11eb-43b1-9727-eb9d7cd83f03','9589a287-2a6f-4f5a-a41b-611f8cc09c55','2026-08-17 08:00:16.043298','2026-08-10 08:00:18.056555',1,NULL,0,'admin','2026-08-10 08:00:16.043298','admin','2026-08-10 08:00:18.056555'),(44,1,1,'1d201db16df821a5f1bffc3c6c5503fdd5c1f3fa50861d716c58a47d6ef21603','3e9397d6-1bb9-49ba-934f-cd76fd9d96e0','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:00:24.415556','2026-08-10 08:06:40.828914',1,45,0,'admin','2026-08-10 08:00:24.415556','admin','2026-08-10 08:06:40.828914'),(45,1,1,'44e83112f458876bbd4d2c090e20c8110905a19e6d28210982789b1265748d2e','2c7feef7-7cd4-4f39-a7fd-49dd1a4ff111','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:06:40.828914','2026-08-10 08:06:47.840867',1,46,0,'admin','2026-08-10 08:06:40.828914','admin','2026-08-10 08:06:47.840867'),(46,1,1,'e80361852a3b23622dad7f56f1326dcf36c9d6096d84bc11a81651976df67bea','4b6e6c11-622e-4845-9ec5-86bf2e832830','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:06:47.840867','2026-08-10 08:06:53.046377',1,47,0,'admin','2026-08-10 08:06:47.840867','admin','2026-08-10 08:06:53.046377'),(47,1,1,'c6792e8206e4dc9a6e2caf8336deebbe2adc8da25626a308facd21fe07aa0c24','47f85871-ad58-410d-83d7-40ed91e8a72b','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:06:53.046377','2026-08-10 08:08:39.952088',1,48,0,'admin','2026-08-10 08:06:53.046377','admin','2026-08-10 08:08:39.952088'),(48,1,1,'e8ed0fe66db7c1eb712aeac4f8905f1a239d9edb07f2631fba1033a351a98527','f7788784-ae2d-4928-8abb-678a2a2bc896','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:08:39.952088','2026-08-10 08:08:44.251989',1,49,0,'admin','2026-08-10 08:08:39.952088','admin','2026-08-10 08:08:44.251989'),(49,1,1,'22d9c08ce75640bbc3c0ac96ee7c4a1670d68251c79286a6b70a146f52e2279f','fb737ad8-1a55-4346-8a44-38a8b9d7bcba','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:08:44.251989','2026-08-10 08:35:55.841798',1,50,0,'admin','2026-08-10 08:08:44.251989','admin','2026-08-10 08:35:55.841798'),(50,1,1,'7b1ce2628939be33f388fb51a0fd1e900f4cff8d9f3655f3fe2545c34d76f460','cecbf86a-89e7-4296-b027-b1b2b06c2e2f','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:35:55.841798','2026-08-10 08:36:36.870881',1,51,0,'admin','2026-08-10 08:35:55.841798','admin','2026-08-10 08:36:36.870881'),(51,1,1,'f7a966dff6da865f9abf7fa8fc21b183b955445791ae3f2f10b4b6fbda11253b','baaac05a-e795-4e2e-b7a5-1e2857404414','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 08:36:36.870881','2026-08-10 09:10:02.664175',1,52,0,'admin','2026-08-10 08:36:36.870881','admin','2026-08-10 09:10:02.664175'),(52,1,1,'9df654afe0bdef3f62c1ba9baf76ce1844171de3dc98c5ac4583fd92e8e6b1c9','2ed7ebc9-03d2-40e5-ac4a-78af8687f24b','133fa434-5d86-4d40-a8fc-d51cc335d03e','2026-08-17 09:10:02.664175','2026-08-10 09:43:48.867969',1,NULL,0,'admin','2026-08-10 09:10:02.664175','admin@gmail.com','2026-08-10 09:43:48.867969'),(53,1,1,'229754d0316014434a2c6a785b67d7f733e86379e83a6c02d6fa0021ed835932','e28b157f-e806-48fb-91ad-fcfcda8ce61f','701c87aa-0030-46f4-8c8d-24e00d3a63d3','2026-08-17 09:43:48.867969','2026-08-10 09:45:18.799947',1,NULL,0,'admin@gmail.com','2026-08-10 09:43:48.867969','admin@gmail.com','2026-08-10 09:45:18.799947'),(54,1,1,'fe60ee24fa2186479a70f21b84ee0454634de74d0f28357dbf8e61482b08ddb3','db27636c-8e94-4c59-90b7-08e076973166','c982af3e-e904-4b18-84bd-9b308186da66','2026-08-17 09:45:18.799947','2026-08-10 09:45:33.728126',1,55,0,'admin@gmail.com','2026-08-10 09:45:18.799947','admin@gmail.com','2026-08-10 09:45:33.728126'),(55,1,1,'812817822eac97a98af66599bbd7e77a8f0fac780f5e6c94cfc6765dcaa764a1','729baa1b-0493-4d08-95bd-475f4bbb9c6a','c982af3e-e904-4b18-84bd-9b308186da66','2026-08-17 09:45:33.728126','2026-08-10 09:46:11.508113',1,56,0,'admin@gmail.com','2026-08-10 09:45:33.728126','admin@gmail.com','2026-08-10 09:46:11.508113'),(56,1,1,'6167509f80e047bd2d6df4da826ee2cfd1ce37adcfe991d2055b40b6773d4746','f67aac32-a65c-43a2-b9ca-67f3de11a878','c982af3e-e904-4b18-84bd-9b308186da66','2026-08-17 09:46:11.508113','2026-08-10 09:48:35.248233',1,57,0,'admin@gmail.com','2026-08-10 09:46:11.508113','admin@gmail.com','2026-08-10 09:48:35.248233'),(57,1,1,'923791e6697b9fe92d284fa26c92d218c6e787f3108780ac3576000aa443387f','de1193ee-dafb-491c-8f32-220d9934a93c','c982af3e-e904-4b18-84bd-9b308186da66','2026-08-17 09:48:35.248233','2026-08-10 09:48:51.560044',1,58,0,'admin@gmail.com','2026-08-10 09:48:35.248233','admin@gmail.com','2026-08-10 09:48:51.560044'),(58,1,1,'946d808282b71c996e31fbc8a4b3bdba8451813a2fa23cd31cde6bbf541f4cc5','f5ed2dc8-e365-419b-bd14-b7e2101e543f','c982af3e-e904-4b18-84bd-9b308186da66','2026-08-17 09:48:51.560044','2026-08-11 03:00:44.059363',1,59,0,'admin@gmail.com','2026-08-10 09:48:51.560044','admin@gmail.com','2026-08-11 03:00:44.059363'),(59,1,1,'91cbec56e6fc8c810953aa078924f4b452e0faba379ee024c895bc596babdb3b','fdfe7056-4932-4eb5-9ed6-563880c61c64','c982af3e-e904-4b18-84bd-9b308186da66','2026-08-18 03:00:44.059363','2026-08-11 03:00:46.484233',1,NULL,0,'admin@gmail.com','2026-08-11 03:00:44.059363','admin@gmail.com','2026-08-11 03:00:46.484233'),(60,1,1,'b7f1f5d298762ff43857064ac1c4d1d63023eabb3813784c80f703206e70d1f1','642339e7-a06f-4cdf-8199-2fcdbbb84e7a','f8427e99-7785-4972-b888-617f7235933d','2026-08-18 03:00:54.362231','2026-08-11 03:28:17.692419',1,61,0,'admin@gmail.com','2026-08-11 03:00:54.362231','admin@gmail.com','2026-08-11 03:28:17.692419'),(61,1,1,'76d1301012ccec1f0b386f4f9ebdf607da663937007e537b0fbbf2209b676858','bbb94ed2-273e-4e05-9999-af6b4387fd3d','f8427e99-7785-4972-b888-617f7235933d','2026-08-18 03:28:17.692419','2026-08-11 03:31:37.487915',1,62,0,'admin@gmail.com','2026-08-11 03:28:17.692419','admin@gmail.com','2026-08-11 03:31:37.487915'),(62,1,1,'5fb10126a937386a223f703a0cec89a0675c6ca6ba483fd2fccdce0e82601a67','e3bfc999-d150-43e4-b4fb-50ccd55be73f','f8427e99-7785-4972-b888-617f7235933d','2026-08-18 03:31:37.487915','2026-08-11 04:25:42.281696',1,63,0,'admin@gmail.com','2026-08-11 03:31:37.487915','admin@gmail.com','2026-08-11 04:25:42.281696'),(63,1,1,'e3356d27f34c5c029af054dc7e2e753fac5bad4402d0f389a9a038b76ee80993','5c710119-8223-4834-b607-fdf54f4fdb65','f8427e99-7785-4972-b888-617f7235933d','2026-08-18 04:25:42.281696','2026-08-11 06:54:55.960014',1,NULL,0,'admin@gmail.com','2026-08-11 04:25:42.281696','admin@gmail.com','2026-08-11 06:54:55.960014'),(64,1,1,'14a82f1278b034b3dc5aecd1ec72b0a155a92327f1336bd625a7e904f01c35ed','9a9da57a-0a0d-4863-9cb1-8813e2637a42','5e1f81e0-4289-4a03-8b7d-fe59b6ffeefa','2026-08-18 06:54:55.960014','2026-08-11 06:57:21.337569',1,65,0,'admin@gmail.com','2026-08-11 06:54:55.960014','admin@gmail.com','2026-08-11 06:57:21.337569'),(65,1,1,'a1687727e8feb25924024dde74bbe14fcc981d2b6dba22f07bbe3f42d0b0ebee','99c47196-c352-4870-af46-32f49cfbfeef','5e1f81e0-4289-4a03-8b7d-fe59b6ffeefa','2026-08-18 06:57:21.337569','2026-08-11 07:09:15.938945',1,NULL,0,'admin@gmail.com','2026-08-11 06:57:21.337569','admin@gmail.com','2026-08-11 07:09:15.938945'),(66,1,1,'fb2cdffd3e70ce82781a080196db2522d0744e38e7628fe15fc3ea11a7970496','efd89083-a9fb-4721-aefd-905dcfe28bf9','e0b95f23-c8dd-4368-a72f-47f39d0ad837','2026-08-18 07:09:15.938945','2026-08-11 07:50:25.166152',1,67,0,'admin@gmail.com','2026-08-11 07:09:15.938945','admin@gmail.com','2026-08-11 07:50:25.166152'),(67,1,1,'81b1dacbf1e9690526d40bf7b99ef62279124eaae9f119fd2c2bb10bf6d1ce07','d24a9f06-c8b2-40da-81d2-c2f615a27feb','e0b95f23-c8dd-4368-a72f-47f39d0ad837','2026-08-18 07:50:25.166152','2026-08-11 08:08:37.818937',1,68,0,'admin@gmail.com','2026-08-11 07:50:25.166152','admin@gmail.com','2026-08-11 08:08:37.818937'),(68,1,1,'9d8706e0a53ff2a1f9a91e36242c38dbcba490918586e20820ef9dc3259bf9f8','1f4f5b1b-0549-457b-b1c5-235c1f538c3f','e0b95f23-c8dd-4368-a72f-47f39d0ad837','2026-08-18 08:08:37.818937','2026-08-11 08:35:39.769399',1,69,0,'admin@gmail.com','2026-08-11 08:08:37.818937','admin@gmail.com','2026-08-11 08:35:39.769399'),(69,1,1,'c188201253011c1fb8e79ce0cb0779652576a13e1799ad1e8923b9cbb2fbc136','252a851d-0353-4d9a-95ee-4c24a68129bf','e0b95f23-c8dd-4368-a72f-47f39d0ad837','2026-08-18 08:35:39.769399','2026-08-11 09:20:47.605127',1,70,0,'admin@gmail.com','2026-08-11 08:35:39.769399','admin@gmail.com','2026-08-11 09:20:47.605127'),(70,1,1,'f16097d224db4dccecc94ac1d0f524eec2f49891f958566b014c577b10b83e00','ae72a980-bfa9-47d7-a669-29fb7601bcf6','e0b95f23-c8dd-4368-a72f-47f39d0ad837','2026-08-18 09:20:47.605127','2026-08-12 02:28:18.285070',1,71,0,'admin@gmail.com','2026-08-11 09:20:47.605127','admin@gmail.com','2026-08-12 02:28:18.285070'),(71,1,1,'047f620737d31e1900d5516edc2f5de6f8951631da65868798cf263dde9b54df','c5e88737-c8ac-4eb5-8f2f-92ed0e5598a8','e0b95f23-c8dd-4368-a72f-47f39d0ad837','2026-08-19 02:28:18.285070','2026-08-12 02:28:28.828310',1,NULL,0,'admin@gmail.com','2026-08-12 02:28:18.285070','admin@gmail.com','2026-08-12 02:28:28.828310'),(72,1,1,'84a906db7b9b02db00281f5dcef68e781de5634343c25a3f70b06ab30201600f','9c005f33-291d-465e-a59f-6fc7a91e8b38','d2f46ef3-39ce-43f9-b69a-dfbfa3ce9009','2026-08-19 02:35:39.419820','2026-08-12 02:35:40.976046',1,NULL,0,'admin@gmail.com','2026-08-12 02:35:39.419820','admin@gmail.com','2026-08-12 02:35:40.976046'),(73,1,1,'08e5be62727862e9978a8cc00fcf65cc9c5d7cdeb0f73e1d8acd7dedd2a3b51e','f7657790-2080-489b-a813-53e1af0c30de','5a761d5e-4f4a-4a12-84ca-10e0fac439c1','2026-08-19 02:35:40.976046','2026-08-12 02:36:25.366871',1,NULL,0,'admin@gmail.com','2026-08-12 02:35:40.976046','admin@gmail.com','2026-08-12 02:36:25.366871'),(74,1,1,'f7e81e23cc464b8a29ce4fcab99059f4b8ee3477e0dd712979aaaad6c1940b29','abd4832c-b9a4-460c-b96b-c00e3c192b83','7e649c4e-7337-4efc-8241-d8d139218d22','2026-08-19 02:36:25.366871','2026-08-12 02:39:24.393073',1,NULL,0,'admin@gmail.com','2026-08-12 02:36:25.366871','admin@gmail.com','2026-08-12 02:39:24.393073'),(75,1,1,'0946136ae7c8eda22b57deef3a9ad7069527fa6e5e479d31335b71d7ab4a455d','5af0fe58-c558-4004-adcc-74d6be3fec18','9fa5269f-f96c-4524-97bc-62d5a14e8d8d','2026-08-19 02:39:24.393073','2026-08-12 03:14:41.988335',1,76,0,'admin@gmail.com','2026-08-12 02:39:24.393073','admin@gmail.com','2026-08-12 03:14:41.988335'),(76,1,1,'37b9b41ecbfc6b0d8802656a1645c3a05668fa20201c4a94712a7ad99820b9bf','0ff1b20c-a56d-46d6-857c-0cfdacee7069','9fa5269f-f96c-4524-97bc-62d5a14e8d8d','2026-08-19 03:14:41.988335','2026-08-12 03:32:36.506011',1,NULL,0,'admin@gmail.com','2026-08-12 03:14:41.988335','admin@gmail.com','2026-08-12 03:32:36.506011'),(77,1,1,'ca9c3ffcdc699e74fc609957f917209c496bd50d07004b0db8de908c48f780d0','d135e8a1-9b62-40af-b922-9411352bf35a','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 03:32:36.506011','2026-08-12 03:32:44.756888',1,78,0,'admin@gmail.com','2026-08-12 03:32:36.506011','admin@gmail.com','2026-08-12 03:32:44.756888'),(78,1,1,'b4fd80d3c4445b5b0907bd1fee5c9420a0d3ec1dfdaacd3d129ae5f9f23bcb19','227fe7de-e3b3-48a1-bcbe-cd76f855c4ac','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 03:32:44.756888','2026-08-12 04:16:47.550057',1,79,0,'admin@gmail.com','2026-08-12 03:32:44.756888','admin@gmail.com','2026-08-12 04:16:47.550057'),(79,1,1,'2ce846f7c0ae205734ed46ab861079f3f0d5a70c1e26adad28b1f6343ea22034','dfe04a74-e9e4-4b5c-9be8-53811889e301','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 04:16:47.550057','2026-08-12 04:26:03.242933',1,80,0,'admin@gmail.com','2026-08-12 04:16:47.550057','admin@gmail.com','2026-08-12 04:26:03.242933'),(80,1,1,'9f60ab7637e787a83048517c1c8ad58ba4de72b46f2e3dfe692ed39c452ff00f','959824d4-e306-4637-b4f3-8950d44ea3ae','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 04:26:03.242933','2026-08-12 04:26:15.320184',1,81,0,'admin@gmail.com','2026-08-12 04:26:03.242933','admin@gmail.com','2026-08-12 04:26:15.320184'),(81,1,1,'771a28ca87dc38d903ac7abdad406db9e4cec43cc8f335648610ed747a58ea32','f4d05916-9bec-4916-b3c9-aec5652e7e1d','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 04:26:15.320184','2026-08-12 04:27:12.137430',1,82,0,'admin@gmail.com','2026-08-12 04:26:15.320184','admin@gmail.com','2026-08-12 04:27:12.137430'),(82,1,1,'2dafd4555002440cba6f781f33ae7fe72d5ea57115c8c52a0b88988478e25568','9cb18aad-011e-4dac-a563-644247cae189','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 04:27:12.137430','2026-08-12 05:30:11.444087',1,83,0,'admin@gmail.com','2026-08-12 04:27:12.137430','admin@gmail.com','2026-08-12 05:30:11.444087'),(83,1,1,'e55ac24bbe71d81603be309a086435e6d67d96131ecf30159f17290f8eacb927','684c9b92-7a02-417c-888c-93ece1c3047f','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 05:30:11.444087','2026-08-12 05:53:14.934852',1,84,0,'admin@gmail.com','2026-08-12 05:30:11.444087','admin@gmail.com','2026-08-12 05:53:14.934852'),(84,1,1,'328692e8be1e832736b6472175ce64aa3df3dfa92d1d63599582307600027616','aa23c21c-e4f2-422b-87c3-a706d0ead680','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 05:53:14.934852','2026-08-12 06:02:43.891396',1,85,0,'admin@gmail.com','2026-08-12 05:53:14.934852','admin@gmail.com','2026-08-12 06:02:43.891396'),(85,1,1,'e3bbc1c10c64b681894437fd5d4dc385a96914a096157578e132dd415bc7c444','4f726fa4-06a0-4a9e-9bcb-7f3935c7d6a1','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 06:02:43.891396','2026-08-12 06:03:36.987303',1,86,0,'admin@gmail.com','2026-08-12 06:02:43.891396','admin@gmail.com','2026-08-12 06:03:36.987303'),(86,1,1,'0b8cacdbc9559c75d90eeb649f4eff98431b820cacee50c0cef3886d94899659','63168bf5-2059-4149-ae4c-099280741d37','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 06:03:36.987303','2026-08-12 06:45:46.948607',1,87,0,'admin@gmail.com','2026-08-12 06:03:36.987303','admin@gmail.com','2026-08-12 06:45:46.948607'),(87,1,1,'d90c0c8670c5a3979b9fab0ebcde7731d9fba494b8e424352608903824a991f7','aa48246b-6403-42dc-b219-a509c6962fe6','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 06:45:46.948607','2026-08-12 08:28:20.177803',1,88,0,'admin@gmail.com','2026-08-12 06:45:46.948607','admin@gmail.com','2026-08-12 08:28:20.177803'),(88,1,1,'4764bed38cfb406232e8b23bd7c6550c6b47b8cb7a603e65c4fde3c0f730acb4','dc08acbf-65a9-428f-8d1f-16cd95671cba','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 08:28:20.177803','2026-08-12 08:44:20.564306',1,89,0,'admin@gmail.com','2026-08-12 08:28:20.177803','admin@gmail.com','2026-08-12 08:44:20.564306'),(89,1,1,'eb649fac9457f39f27f9824ed692f80b8989a57933e2e782414d52916674304b','2adc18c9-603f-4617-a696-1c7c0ee04e19','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 08:44:20.564306','2026-08-12 08:45:08.598357',1,90,0,'admin@gmail.com','2026-08-12 08:44:20.564306','admin@gmail.com','2026-08-12 08:45:08.598357'),(90,1,1,'aa57575edd467d3eed76795aac30cbbb27fe056fe35f7f5720283722aad9dbd3','afcca4bf-2db3-4356-87e3-8411abcabc59','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 08:45:08.598357','2026-08-12 08:52:17.342680',1,91,0,'admin@gmail.com','2026-08-12 08:45:08.598357','admin@gmail.com','2026-08-12 08:52:17.342680'),(91,1,1,'3d2e90c90e5152e5622220771e0f99a7da804677073d0a0b04826a496ddb1855','2cad14b3-c4b0-4e46-adcf-00c8e2fd3317','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-19 08:52:17.342680','2026-08-13 03:17:33.315385',1,92,0,'admin@gmail.com','2026-08-12 08:52:17.342680','admin@gmail.com','2026-08-13 03:17:33.315385'),(92,1,1,'bd3340b49d94a79e20f303f060e34718e044ef694c177239bd9511745e3405a5','536f7586-07c1-4731-89bb-98641440ef9d','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-20 03:17:33.315385','2026-08-13 03:35:00.504949',1,93,0,'admin@gmail.com','2026-08-13 03:17:33.315385','admin@gmail.com','2026-08-13 03:35:00.504949'),(93,1,1,'e732a462eeb6497f64539d6c9e4f3f4de1eb4be6655e780ad17e34c3f95ab8e9','440a4c51-a1dc-465b-9989-34c12a9f32b1','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-20 03:35:00.504949','2026-08-13 05:29:34.217509',1,94,0,'admin@gmail.com','2026-08-13 03:35:00.504949','admin@gmail.com','2026-08-13 05:29:34.217509'),(94,1,1,'77bc28aab4aa508ca3fe79d199419b86d72de83a81d0163fd14deeb79316cb4e','467667a7-8433-4568-8771-27852209e963','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-20 05:29:34.217509','2026-08-13 05:29:56.726908',1,95,0,'admin@gmail.com','2026-08-13 05:29:34.217509','admin@gmail.com','2026-08-13 05:29:56.726908'),(95,1,1,'a9dcbdfbb040e190cbda9ad50779c37992e1f5874d304e2a8e99d008cee88492','6a1b1fdf-3982-47c8-a8b3-cca089720e3a','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-20 05:29:56.726908','2026-08-13 05:36:11.125568',1,96,0,'admin@gmail.com','2026-08-13 05:29:56.726908','admin@gmail.com','2026-08-13 05:36:11.125568'),(96,1,1,'b542ac8aba673b2f6d513775635cdeefaf9d06bf06249003acc66b349126aa9b','40215a32-e6e8-410d-85e3-c71ad488ef00','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-20 05:36:11.125568','2026-08-13 05:40:57.652181',1,97,0,'admin@gmail.com','2026-08-13 05:36:11.125568','admin@gmail.com','2026-08-13 05:40:57.652181'),(97,1,1,'3a016ed9702891fcb6fb2d942355295a177cad8bc3bdc44833b818a7122ea30a','e4921114-4894-4441-acd6-0e7787e03337','523a3580-07ab-476e-9081-ba64f42ede88','2026-08-20 05:40:57.652181','2026-08-13 05:46:02.660630',1,NULL,0,'admin@gmail.com','2026-08-13 05:40:57.652181','admin@gmail.com','2026-08-13 05:46:02.660630'),(98,1,1,'d403fe6370eb3fa16963fd2cc9c337e3458ed7567b9211144610a0f7af655bad','c2928c6b-bdab-41dc-bf88-98fc9cfd8593','a11bc4cd-9af3-4771-9afa-20b108245dcb','2026-08-20 05:53:50.361699','2026-08-13 06:17:07.355163',1,99,0,'admin@gmail.com','2026-08-13 05:53:50.361699','admin@gmail.com','2026-08-13 06:17:07.355163'),(99,1,1,'f415c46f0dd3f9e23317328f7cbb856819c746e45bff3908fd125ddcb3757d3f','bf9b9ec3-4241-4661-8515-180a58f6d5e6','a11bc4cd-9af3-4771-9afa-20b108245dcb','2026-08-20 06:17:07.355163','2026-08-13 06:38:12.018343',1,100,0,'admin@gmail.com','2026-08-13 06:17:07.355163','admin@gmail.com','2026-08-13 06:38:12.018343'),(100,1,1,'ffe92e0a89fa037cb63d2b2ee52bd2791dd57cc7de42250ae20e28c0168881f8','b31e60aa-b81f-435f-9c6a-4d96944a5121','a11bc4cd-9af3-4771-9afa-20b108245dcb','2026-08-20 06:38:12.018343','2026-08-13 06:38:15.017396',1,NULL,0,'admin@gmail.com','2026-08-13 06:38:12.018343','admin@gmail.com','2026-08-13 06:38:15.017396'),(101,1,1,'74fd231a308a0017a2477a218a4b627de2029da600f6f9947a241e83e37cc5f2','2ab82812-78d8-468b-990c-19b4187e3b66','2c05bd57-fd86-4064-9825-1f31a70205b3','2026-08-20 06:38:24.007328','2026-08-13 06:38:38.902703',1,102,0,'admin@gmail.com','2026-08-13 06:38:24.007328','admin@gmail.com','2026-08-13 06:38:38.902703'),(102,1,1,'bdca414adceb14405fba9c4926e8c806fc461d6cd040b092c9a78519e4384805','92c16738-442f-4fd1-95dc-bcdddb6661cb','2c05bd57-fd86-4064-9825-1f31a70205b3','2026-08-20 06:38:38.902703','2026-08-13 06:58:33.350382',1,103,0,'admin@gmail.com','2026-08-13 06:38:38.902703','admin@gmail.com','2026-08-13 06:58:33.350382'),(103,1,1,'d2015267ffc73882f73b66f6d0052e206b116ffb1ca677a096767b2fb6db55ab','0088c62a-97e9-4f83-9d62-76d4c20c43ab','2c05bd57-fd86-4064-9825-1f31a70205b3','2026-08-20 06:58:33.350382','2026-08-13 06:58:36.755863',1,NULL,0,'admin@gmail.com','2026-08-13 06:58:33.350382','admin@gmail.com','2026-08-13 06:58:36.755863'),(104,1,1,'3a442e023f8929e46adf17beb83748afacd22bb72d4b338b004e8a2c25d7c8a4','7e8abdc1-4186-4f34-9f52-afca18358962','096230f8-8fb8-475c-acd8-4ac55b16ca75','2026-08-20 06:58:58.640229','2026-08-13 07:05:43.442645',1,105,0,'admin@gmail.com','2026-08-13 06:58:58.640229','admin@gmail.com','2026-08-13 07:05:43.442645'),(105,1,1,'a13dccad4fec624eaf50039b715d63c0b443214e76d3fc179381aaf37e5b8104','5ffb7f15-20e3-42c5-b91e-fecb7eafa168','096230f8-8fb8-475c-acd8-4ac55b16ca75','2026-08-20 07:05:43.442645','2026-08-13 07:07:58.537406',1,106,0,'admin@gmail.com','2026-08-13 07:05:43.442645','admin@gmail.com','2026-08-13 07:07:58.537406'),(106,1,1,'536a18e777927f10c13ccc0e3e8297f244672ff8d1207c2f4690fef9602a60f4','f019442c-707a-45c7-bd47-fd1c30455dd5','096230f8-8fb8-475c-acd8-4ac55b16ca75','2026-08-20 07:07:58.537406','2026-08-13 07:09:24.012231',1,NULL,0,'admin@gmail.com','2026-08-13 07:07:58.537406','admin@gmail.com','2026-08-13 07:09:24.012231'),(107,8,1,'bc04b24b6811a9de9c70f4628b39924f0bd42767c622dd3de2ba9300fea05b47','a09384e7-845b-4b44-8aa0-ac9c11d8b102','43253a93-d33d-4671-9ece-3da10156f8a3','2026-08-20 07:09:30.755280','2026-08-13 07:11:58.981037',8,108,0,'hien@gmail.com','2026-08-13 07:09:30.755280','hien@gmail.com','2026-08-13 07:11:58.981037'),(108,8,1,'71945a1cd01d3b8a38a86833a3ccb28ff685b77d151c99dccfcb172aef5b1783','601a7eb1-82a9-4482-9d88-4c86b47999c6','43253a93-d33d-4671-9ece-3da10156f8a3','2026-08-20 07:11:58.981037','2026-08-13 07:16:58.080650',8,NULL,0,'hien@gmail.com','2026-08-13 07:11:58.981037','hien@gmail.com','2026-08-13 07:16:58.080650'),(109,8,1,'78cf649d2563f859cb250547bae1922b25061d0a298df9bab19c03cfee84b9db','7a708357-d08c-43fc-99f6-cb3b918699af','8c0ad540-2445-400f-9feb-3b36d0f60ac2','2026-08-20 07:17:09.225243','2026-08-13 07:28:53.043376',8,110,0,'hien@gmail.com','2026-08-13 07:17:09.225243','hien@gmail.com','2026-08-13 07:28:53.043376'),(110,8,1,'5a6b5f5c99f22a95a88bfd9e7173f639691b7c018b97fc8f2245b25179ab3c1b','a28b8a6a-6e1a-4337-b328-e0ac588881a0','8c0ad540-2445-400f-9feb-3b36d0f60ac2','2026-08-20 07:28:53.043376','2026-08-13 07:29:51.532564',8,111,0,'hien@gmail.com','2026-08-13 07:28:53.043376','hien@gmail.com','2026-08-13 07:29:51.532564'),(111,8,1,'22dcf8ca0b027164b1e275a69b9627dabb9adb7d288fdbae2c5fb71af2cc7f1c','9e74d608-1c06-42b4-848c-c91888f55dca','8c0ad540-2445-400f-9feb-3b36d0f60ac2','2026-08-20 07:29:51.532564','2026-08-13 07:31:15.402358',8,112,0,'hien@gmail.com','2026-08-13 07:29:51.532564','hien@gmail.com','2026-08-13 07:31:15.402358'),(112,8,1,'1deb937ecc6290f3b88319f2f2fa006da4321c16068f2b43ef0da1a9192f3451','94253773-1224-4e6d-b1b1-8c8ca04d7a6d','8c0ad540-2445-400f-9feb-3b36d0f60ac2','2026-08-20 07:31:15.402358','2026-08-13 07:37:53.497242',8,113,0,'hien@gmail.com','2026-08-13 07:31:15.402358','hien@gmail.com','2026-08-13 07:37:53.497242'),(113,8,1,'33a8dc33a1ef43e1395e32bb790a3cc355f18200996b6ed215dc23a7e816fff9','905feb06-a6c0-4a5b-90e8-3d10230cc81f','8c0ad540-2445-400f-9feb-3b36d0f60ac2','2026-08-20 07:37:53.497242','2026-08-13 07:38:44.426877',8,NULL,0,'hien@gmail.com','2026-08-13 07:37:53.497242','hien@gmail.com','2026-08-13 07:38:44.426877'),(114,1,1,'bb45b50b0ea4e68e092f29e2b337d957a2391ee911067875ebd912af1cfd8fe7','e677cca2-194f-4508-b76f-e982c74bbc29','d110c8cc-5364-4ece-9295-2a45975e446a','2026-08-20 07:38:51.009526','2026-08-13 07:39:30.082953',1,NULL,0,'admin@gmail.com','2026-08-13 07:38:51.009526','admin@gmail.com','2026-08-13 07:39:30.082953'),(115,1,1,'e56e88118efacf0a08b34fe5c11be9ba01c737d8c2c11def3199a12e6e3b2340','eedb4d13-b42f-42bc-8934-e7262111ee57','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:51:51.503497','2026-08-13 07:52:26.671788',1,116,0,'admin@gmail.com','2026-08-13 07:51:51.503497','admin@gmail.com','2026-08-13 07:52:26.671788'),(116,1,1,'de2dfa2a13fddbf0cf6f4188ae3eec574b1726fc15fc4c9cf80e523adfceba36','c500b04b-f5c6-426b-997f-e6de98d36273','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:26.671788','2026-08-13 07:52:27.995156',1,117,0,'admin@gmail.com','2026-08-13 07:52:26.671788','admin@gmail.com','2026-08-13 07:52:27.995156'),(117,1,1,'0bb551969201fcf01d6617b4b5f8ff9a983ce26a32a735d200a2edfd2d67a2e7','78655342-b04e-4715-a2d8-3be2cc1cd32d','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:27.995156','2026-08-13 07:52:29.355124',1,118,0,'admin@gmail.com','2026-08-13 07:52:27.995156','admin@gmail.com','2026-08-13 07:52:29.355124'),(118,1,1,'205930895aabd4e34145f858415a427106a73055a3afb9c4670675ae8f6e6887','fb58bbe0-899c-4b02-9da6-33854831543e','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:29.355124','2026-08-13 07:52:30.622345',1,119,0,'admin@gmail.com','2026-08-13 07:52:29.355124','admin@gmail.com','2026-08-13 07:52:30.622345'),(119,1,1,'78e3854528d3935e5bde099f59929d5724ca803e78adebc009b83c5818a5ec62','70515398-3b50-4ac4-8311-383a8d4016a4','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:30.622345','2026-08-13 07:52:31.922413',1,120,0,'admin@gmail.com','2026-08-13 07:52:30.622345','admin@gmail.com','2026-08-13 07:52:31.922413'),(120,1,1,'0b24eab4a392f2b2733c666f2a772cb2eaa00640c181f9931ae4d14b454d1eef','ff836551-6d4c-4f72-bfcf-f1f5f7e5997f','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:31.922413','2026-08-13 07:52:33.234234',1,121,0,'admin@gmail.com','2026-08-13 07:52:31.922413','admin@gmail.com','2026-08-13 07:52:33.234234'),(121,1,1,'fd4b7b7cc49cd50aae7062cd8c07eb00af941bb98fbe507864800cbbf08324f5','472bab4f-4631-4fb6-a17d-daec66f9c62e','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:33.234234','2026-08-13 07:52:34.679474',1,122,0,'admin@gmail.com','2026-08-13 07:52:33.234234','admin@gmail.com','2026-08-13 07:52:34.679474'),(122,1,1,'cf2a8c4a06205e69bd03a41b50066c1d9cdc1da0a9e2354ccf17889ccf0bc496','76d13866-065f-452e-8a61-f9b838f2856b','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:34.679474','2026-08-13 07:52:37.446743',1,123,0,'admin@gmail.com','2026-08-13 07:52:34.679474','admin@gmail.com','2026-08-13 07:52:37.446743'),(123,1,1,'447f2d635660822af7555fa7146f4e55a59edbf70836e40a51659754eaac959c','64897aa5-a197-447f-a9e1-99fdc656181a','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 07:52:37.446743','2026-08-13 08:00:21.365164',1,124,0,'admin@gmail.com','2026-08-13 07:52:37.446743','admin@gmail.com','2026-08-13 08:00:21.365164'),(124,1,1,'10cf35eb8340d110e4bcf7630851d5a01343b9f1b35eba97eb3ff05ffc191234','f2ff841e-1768-48f5-b27d-6f893be3bf86','f2f4e938-a7e1-44b2-be38-ae7d850e1970','2026-08-20 08:00:21.365164','2026-08-13 08:00:29.356042',1,NULL,0,'admin@gmail.com','2026-08-13 08:00:21.365164','admin@gmail.com','2026-08-13 08:00:29.356042'),(125,1,1,'34409f44c2cd9a978d1a95dd8b848c153b893145464ed0a212b5fda451f85fff','9a0cd42c-8ecd-40ed-9c69-41c9e821ae4b','c855c202-e8c1-4b72-8d19-382878be9c5b','2026-08-20 08:00:29.356042','2026-08-13 08:00:41.152450',1,126,0,'admin@gmail.com','2026-08-13 08:00:29.356042','admin@gmail.com','2026-08-13 08:00:41.152450'),(126,1,1,'54eab4822bb4cde9456ff5cbf884b6bc774638b8375dbb0f04162949183d6a5a','86a32380-9853-46e9-9e6f-cbe7a5e7a4fb','c855c202-e8c1-4b72-8d19-382878be9c5b','2026-08-20 08:00:41.152450','2026-08-13 08:00:51.584932',1,NULL,0,'admin@gmail.com','2026-08-13 08:00:41.152450','admin@gmail.com','2026-08-13 08:00:51.584932'),(127,1,1,'20349820530814fdd231e16cf787e85ac906b3ad2cac229fe28b469048a54890','bade81d1-281c-4658-9422-1c8e49f0829b','e880b88c-5215-4121-9cec-0b0a172bc4cf','2026-08-20 08:00:51.584932','2026-08-13 08:03:46.371105',1,128,0,'admin@gmail.com','2026-08-13 08:00:51.584932','admin@gmail.com','2026-08-13 08:03:46.371105'),(128,1,1,'eab91a5a74852973da6088b3d278995d99f26ccfe229bb489127a1f54c3d8ae4','64b45855-1967-43f7-b0f0-bb9063de1bc1','e880b88c-5215-4121-9cec-0b0a172bc4cf','2026-08-20 08:03:46.371105','2026-08-13 08:04:15.647211',1,NULL,0,'admin@gmail.com','2026-08-13 08:03:46.371105','admin@gmail.com','2026-08-13 08:04:15.647211'),(129,8,1,'3f9b2f6afec67593f3a1e4d16117112d5547ecfab9f2c985e38016693f905f33','12b88731-6ea0-40b4-8a3f-589a190af7cc','0a21cf15-6492-406c-85ab-7b31e0993122','2026-08-20 08:04:23.514476','2026-08-13 08:06:18.821631',8,NULL,0,'hien@gmail.com','2026-08-13 08:04:23.514476','hien@gmail.com','2026-08-13 08:06:18.821631'),(130,1,1,'8af1106dbcedaa6d5cf1f4ab2b91c26466951f5cad17d368770fa9c559c86dd8','a57d3388-faa0-48b8-b7e3-8c889c0588ce','19f7a965-eedb-44fb-aadb-6d1ea9d8a391','2026-08-20 08:06:31.632122','2026-08-13 08:09:32.160084',1,131,0,'admin@gmail.com','2026-08-13 08:06:31.632122','admin@gmail.com','2026-08-13 08:09:32.160084'),(131,1,1,'f5ded225f4b0c74a3da79d073e41105ade2eac6661ff2f53fa0f837a58d027f9','3998c63d-1272-4c9b-9adf-9a7f0d880b66','19f7a965-eedb-44fb-aadb-6d1ea9d8a391','2026-08-20 08:09:32.160084','2026-08-13 08:09:58.004641',1,NULL,0,'admin@gmail.com','2026-08-13 08:09:32.160084','admin@gmail.com','2026-08-13 08:09:58.004641'),(132,1,1,'114806ab0217fb330a7caeccc0509892d5a4f88ef79dcf8e5e4fc9cf74f972c6','8fc8fcf5-d93e-447d-b0cf-2c57c1f9442f','8cb346b6-dc91-49b8-b05d-49a28dd231fa','2026-08-20 08:10:04.117577','2026-08-13 08:10:30.539326',1,NULL,0,'admin@gmail.com','2026-08-13 08:10:04.117577','admin@gmail.com','2026-08-13 08:10:30.539326'),(133,8,1,'7b2dca91c90e333529f9fef056346500d631230f164d73360c51af34a282f19b','625af29b-7261-419b-9506-a69b543efc3f','af6e74e7-4309-4391-a22d-e14c34ae45b4','2026-08-20 08:10:41.221003','2026-08-13 08:11:06.143550',8,NULL,0,'hien@gmail.com','2026-08-13 08:10:41.221003','hien@gmail.com','2026-08-13 08:11:06.143550'),(134,1,1,'d9221e60a6914b381a3b11a2e242dfda7df88cf4002d973aba26f8e798a3f635','54df5b12-2a0b-43d5-a924-88d81d73f756','7f84d697-00c8-41af-916e-d0e72c470e45','2026-08-20 08:12:01.373551','2026-08-13 08:12:39.553366',1,NULL,0,'admin@gmail.com','2026-08-13 08:12:01.373551','admin@gmail.com','2026-08-13 08:12:39.553366'),(135,8,1,'187b4b7f6d6b74d711271f2971fcc650626bbeb1f9f4640690514e449c8a2a80','6a400dd5-f5f9-416f-855f-5b8566983a5b','06f3cec1-9372-4107-a3cb-bc8a2e2c271b','2026-08-20 08:12:46.568542','2026-08-13 08:15:40.912933',8,136,0,'hien@gmail.com','2026-08-13 08:12:46.568542','hien@gmail.com','2026-08-13 08:15:40.912933'),(136,8,1,'087cbf251ad6892ebd6dc74cca8484c5af772491262c64c6e6eea8a29cee2b0f','74fe9025-6c33-48aa-b1aa-c5ac3081eb07','06f3cec1-9372-4107-a3cb-bc8a2e2c271b','2026-08-20 08:15:40.912933','2026-08-13 08:47:59.442610',8,137,0,'hien@gmail.com','2026-08-13 08:15:40.912933','hien@gmail.com','2026-08-13 08:47:59.442610'),(137,8,1,'f995330d8765bf61556c4be1e3b32dafbd7a8ba30f38bd5f6618b5c3b313cf68','f65d408d-b0e7-4ae2-9770-dacf47c0843e','06f3cec1-9372-4107-a3cb-bc8a2e2c271b','2026-08-20 08:47:59.442610','2026-08-13 08:48:07.718993',8,138,0,'hien@gmail.com','2026-08-13 08:47:59.442610','hien@gmail.com','2026-08-13 08:48:07.718993'),(138,8,1,'141c6fc6c91e832984baa1f96fa0c5198e27f0251d2e8c1e1d33fcc378e7e8ea','8bcfaf6f-f7a8-4fd0-8291-3768192aa91d','06f3cec1-9372-4107-a3cb-bc8a2e2c271b','2026-08-20 08:48:07.718993','2026-08-13 08:48:12.712932',8,NULL,0,'hien@gmail.com','2026-08-13 08:48:07.718993','hien@gmail.com','2026-08-13 08:48:12.712932'),(139,1,1,'414d709dcf7ed6fc60b8fce9be3b4f833662e52cb60d7d777dcc98549e148715','d16dfae1-c20e-483b-9dc8-66a75de59aad','83a7e6e8-791e-426f-9c0b-7d91cc865771','2026-08-20 08:48:20.749472','2026-08-13 09:01:33.329029',1,140,0,'admin@gmail.com','2026-08-13 08:48:20.749472','admin@gmail.com','2026-08-13 09:01:33.329029'),(140,1,1,'d78b02ffdf401168315c291c84d40991aa6dd0a5cdd7b26b607b9f1141dc2e01','98107dda-a7ec-457f-8f34-0dc3a70a8b29','83a7e6e8-791e-426f-9c0b-7d91cc865771','2026-08-20 09:01:33.329029','2026-08-13 09:53:44.458180',1,141,0,'admin@gmail.com','2026-08-13 09:01:33.329029','admin@gmail.com','2026-08-13 09:53:44.458180'),(141,1,1,'9cf311fee6b30799ab44669c1f52807f252b2ba4a08ac5978f7668075b517d7c','3fbe2658-8225-4742-94f5-95f026b51bdd','83a7e6e8-791e-426f-9c0b-7d91cc865771','2026-08-20 09:53:44.458180','2026-08-14 08:26:18.881892',1,142,0,'admin@gmail.com','2026-08-13 09:53:44.458180','admin@gmail.com','2026-08-14 08:26:18.881892'),(142,1,1,'85edab881d5e9babb5f681d2af36b02d4a3a79b6e63dbda43b0490e29de49190','0d2a738c-f60c-4eb8-be2c-1675f76305f7','83a7e6e8-791e-426f-9c0b-7d91cc865771','2026-08-21 08:26:18.881892','2026-08-14 08:26:22.946120',1,NULL,0,'admin@gmail.com','2026-08-14 08:26:18.881892','admin@gmail.com','2026-08-14 08:26:22.946120'),(143,1,1,'3fc4e66330493c71fdd9557719991a828a2798fe7f3962626720da94fe668660','06d54202-89bf-4228-b8bb-1ff5791a955d','1b0971b7-5df3-46ed-ad53-012cb4a5fe22','2026-08-21 09:10:55.779806','2026-08-14 09:11:03.642926',1,NULL,0,'admin@gmail.com','2026-08-14 09:10:55.779806','admin@gmail.com','2026-08-14 09:11:03.642926'),(144,1,1,'8286b070624dc3e843a6da38c2940ecac7a137dd8f6061fec7cc8a83bb0e1aaa','db84d653-dec7-468f-9588-4be551418ff1','f0d8f248-da94-4c48-878f-a4c3f8997ad8','2026-08-25 06:26:26.870236','2026-08-18 06:26:43.272938',1,NULL,0,'admin@gmail.com','2026-08-18 06:26:26.870236','admin@gmail.com','2026-08-18 06:26:43.272938'),(145,1,1,'c7943c894d3df009c9fbe9cb7c4d098b6dc4f178733d4ee5cf64b38e72d06bbd','1363be4f-882d-44ea-8375-167451036c06','94624891-6bed-41ff-a899-b8b7260bc8ac','2026-08-25 09:17:43.682573','2026-08-18 09:22:52.641295',1,146,0,'admin@gmail.com','2026-08-18 09:17:43.682573','admin@gmail.com','2026-08-18 09:22:52.641295'),(146,1,1,'16c6608fae38af10bb43ec76fdc09d7cb62bf3beb206b0ac5fcb912f2cdbce1d','69b8fde2-b818-4572-b199-3566581beabf','94624891-6bed-41ff-a899-b8b7260bc8ac','2026-08-25 09:22:52.641295','2026-08-18 09:23:15.374388',1,147,0,'admin@gmail.com','2026-08-18 09:22:52.641295','admin@gmail.com','2026-08-18 09:23:15.374388'),(147,1,1,'c95c347de2c947323bd7d1196d8877b47b70f03dd4d8421036b039437180bc4a','f6a78dd6-cf5b-44b5-a596-6aa94db7b572','94624891-6bed-41ff-a899-b8b7260bc8ac','2026-08-25 09:23:15.374388','2026-08-18 09:24:42.645815',1,148,0,'admin@gmail.com','2026-08-18 09:23:15.374388','admin@gmail.com','2026-08-18 09:24:42.645815'),(148,1,1,'252faefcb33c2f5d1dfae49d155004062c95cfa066398c341966963c3b9ac586','a9e0f6f4-83d8-47b8-88fa-48d6660b921e','94624891-6bed-41ff-a899-b8b7260bc8ac','2026-08-25 09:24:42.645815','2026-08-18 09:25:17.026666',1,NULL,0,'admin@gmail.com','2026-08-18 09:24:42.645815','admin@gmail.com','2026-08-18 09:25:17.026666'),(149,1,1,'8971cb3dfd5427eb81a9db4f81032fdc513741f37da4b296da45e3339005a2cc','a7c97e8c-73d3-4493-8d43-948eaba4107f','f6e1b02a-b770-47eb-8e65-0cf9be6e6062','2026-08-25 09:25:23.940769',NULL,NULL,NULL,1,'admin@gmail.com','2026-08-18 09:25:23.940769',NULL,NULL);
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
) ENGINE=InnoDB AUTO_INCREMENT=30010 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `resources`
--

LOCK TABLES `resources` WRITE;
/*!40000 ALTER TABLE `resources` DISABLE KEYS */;
INSERT INTO `resources` VALUES (1,1,'Users','Users','Menu','User management','system@company.com','2026-08-05 14:20:19.891601','admin','2026-08-13 03:20:32.899863',1,0,3),(2,1,'Roles','Roles','Menu','Role management','system@company.com','2026-08-05 14:20:19.891601','admin','2026-08-13 03:20:36.121314',1,0,3),(3,1,'RolePermissions','Role permissions','Menu','Role permissions','system@company.com','2026-08-05 14:20:19.891601','admin','2026-08-13 03:22:02.640154',1,0,4),(4,1,'Resources','Resources','Menu','Resources','system@company.com','2026-08-05 14:20:19.891601','admin','2026-08-13 03:21:32.041225',1,0,4),(5,1,'Applications','Applications','Menu','Applications','system@company.com','2026-08-05 14:20:19.891601','admin','2026-08-13 03:21:18.317691',1,0,4),(6,1,'Menus','Menus','Menu','Menus','admin','2026-08-13 03:22:55.317241',NULL,NULL,1,0,1),(7,1,'Actions','Actions','Menu','Actions','admin','2026-08-13 03:23:17.538084',NULL,NULL,1,0,1),(8,1,'UserRoles','User roles','Menu','User roles','admin','2026-08-13 03:23:44.062674',NULL,NULL,1,0,1),(30009,2,'test','test','test','test',NULL,'2026-08-13 09:55:54.233274',NULL,NULL,1,0,1);
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQRolePermissions` (`RoleId`,`PermissionId`),
  KEY `IX_role_permissions_PermissionId` (`PermissionId`),
  CONSTRAINT `FK_role_permissions_permissions_PermissionId` FOREIGN KEY (`PermissionId`) REFERENCES `permissions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_role_permissions_roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=136 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role_permissions`
--

LOCK TABLES `role_permissions` WRITE;
/*!40000 ALTER TABLE `role_permissions` DISABLE KEYS */;
INSERT INTO `role_permissions` VALUES (53,1,31,'admin','2026-08-12 08:45:12.650271',NULL,NULL),(54,1,32,'admin','2026-08-12 08:45:13.298406',NULL,NULL),(57,1,35,'admin','2026-08-12 08:45:14.681293',NULL,NULL),(58,1,36,'admin','2026-08-12 08:45:15.131999',NULL,NULL),(59,1,37,'admin','2026-08-12 08:46:26.835594',NULL,NULL),(60,1,38,'admin','2026-08-12 08:46:27.186334',NULL,NULL),(61,1,39,'admin','2026-08-13 03:27:42.703290',NULL,NULL),(62,1,40,'admin','2026-08-13 03:27:43.178351',NULL,NULL),(63,1,41,'admin','2026-08-13 03:27:43.579361',NULL,NULL),(64,1,42,'admin','2026-08-13 03:27:44.001635',NULL,NULL),(65,1,43,'admin','2026-08-13 03:27:44.457040',NULL,NULL),(66,1,44,'admin','2026-08-13 03:27:44.956471',NULL,NULL),(67,1,45,'admin','2026-08-13 03:27:46.400916',NULL,NULL),(68,1,46,'admin','2026-08-13 03:27:46.707008',NULL,NULL),(69,1,47,'admin','2026-08-13 03:27:47.168303',NULL,NULL),(70,1,48,'admin','2026-08-13 03:27:47.517066',NULL,NULL),(71,1,49,'admin','2026-08-13 03:27:47.914942',NULL,NULL),(72,1,50,'admin','2026-08-13 03:27:48.299784',NULL,NULL),(73,1,51,'admin','2026-08-13 03:27:49.966860',NULL,NULL),(74,1,52,'admin','2026-08-13 03:27:50.277132',NULL,NULL),(75,1,53,'admin','2026-08-13 03:27:51.194972',NULL,NULL),(76,1,54,'admin','2026-08-13 03:27:51.526735',NULL,NULL),(77,1,55,'admin','2026-08-13 03:27:51.897021',NULL,NULL),(78,1,56,'admin','2026-08-13 03:27:52.240542',NULL,NULL),(79,1,57,'admin','2026-08-13 03:27:53.831551',NULL,NULL),(80,1,58,'admin','2026-08-13 03:27:54.144768',NULL,NULL),(81,1,59,'admin','2026-08-13 03:27:54.503379',NULL,NULL),(82,1,60,'admin','2026-08-13 03:27:54.910151',NULL,NULL),(83,1,61,'admin','2026-08-13 03:27:55.348601',NULL,NULL),(84,1,62,'admin','2026-08-13 03:27:55.740369',NULL,NULL),(85,1,63,'admin','2026-08-13 03:27:57.416445',NULL,NULL),(86,1,64,'admin','2026-08-13 03:27:57.713540',NULL,NULL),(87,1,65,'admin','2026-08-13 03:27:58.205362',NULL,NULL),(88,1,66,'admin','2026-08-13 03:27:58.548206',NULL,NULL),(89,1,67,'admin','2026-08-13 03:27:58.906215',NULL,NULL),(90,1,68,'admin','2026-08-13 03:27:59.348984',NULL,NULL),(91,1,69,'admin','2026-08-13 03:28:00.896721',NULL,NULL),(92,1,70,'admin','2026-08-13 03:28:01.227347',NULL,NULL),(93,1,71,'admin','2026-08-13 03:28:01.934790',NULL,NULL),(94,1,72,'admin','2026-08-13 03:28:02.253839',NULL,NULL),(95,1,73,'admin','2026-08-13 03:28:02.646651',NULL,NULL),(96,1,74,'admin','2026-08-13 03:28:03.022193',NULL,NULL),(97,1,75,'admin','2026-08-13 03:28:04.770815',NULL,NULL),(98,1,76,'admin','2026-08-13 03:28:05.122006',NULL,NULL),(99,1,77,'admin','2026-08-13 03:28:05.588844',NULL,NULL),(100,1,78,'admin','2026-08-13 03:28:05.955582',NULL,NULL),(101,1,79,'admin','2026-08-13 03:28:06.468025',NULL,NULL),(102,1,80,'admin','2026-08-13 03:28:06.784425',NULL,NULL),(103,2,69,NULL,'2026-08-13 07:08:34.083523',NULL,NULL),(104,2,70,NULL,'2026-08-13 07:08:48.608914',NULL,NULL),(105,2,71,NULL,'2026-08-13 07:08:49.573537',NULL,NULL),(106,2,72,NULL,'2026-08-13 07:08:49.998068',NULL,NULL),(107,2,73,NULL,'2026-08-13 07:08:50.421984',NULL,NULL),(108,2,74,NULL,'2026-08-13 07:08:50.787960',NULL,NULL),(109,2,35,NULL,'2026-08-13 07:09:04.777593',NULL,NULL),(110,2,43,NULL,'2026-08-13 07:09:12.463250',NULL,NULL),(111,2,49,NULL,'2026-08-13 07:09:14.392518',NULL,NULL),(112,2,55,NULL,'2026-08-13 07:09:16.135259',NULL,NULL),(113,2,61,NULL,'2026-08-13 07:39:04.302649',NULL,NULL),(114,2,67,NULL,'2026-08-13 07:39:07.038050',NULL,NULL),(115,2,79,NULL,'2026-08-13 07:39:09.956150',NULL,NULL),(116,3,79,NULL,'2026-08-13 07:39:16.219847',NULL,NULL),(117,3,73,NULL,'2026-08-13 07:39:17.832211',NULL,NULL),(118,3,67,NULL,'2026-08-13 07:39:19.357903',NULL,NULL),(119,3,61,NULL,'2026-08-13 07:39:20.844910',NULL,NULL),(120,3,55,NULL,'2026-08-13 07:39:22.559425',NULL,NULL),(121,3,49,NULL,'2026-08-13 07:39:24.125695',NULL,NULL),(122,3,43,NULL,'2026-08-13 07:39:25.538628',NULL,NULL),(123,3,35,NULL,'2026-08-13 07:39:27.055560',NULL,NULL),(124,1,81,NULL,'2026-08-13 07:52:25.733481',NULL,NULL),(125,1,82,NULL,'2026-08-13 07:52:27.367237',NULL,NULL),(126,1,83,NULL,'2026-08-13 07:52:28.609536',NULL,NULL),(127,1,84,NULL,'2026-08-13 07:52:30.025287',NULL,NULL),(128,1,85,NULL,'2026-08-13 07:52:31.250699',NULL,NULL),(129,1,86,NULL,'2026-08-13 07:52:32.532945',NULL,NULL),(130,1,87,NULL,'2026-08-13 07:52:33.882264',NULL,NULL),(131,1,88,NULL,'2026-08-13 07:52:35.290604',NULL,NULL),(132,2,87,NULL,'2026-08-13 07:52:54.299784',NULL,NULL),(133,2,86,NULL,'2026-08-13 07:52:56.046671',NULL,NULL),(134,2,85,NULL,'2026-08-13 07:52:57.691550',NULL,NULL),(135,2,84,NULL,'2026-08-13 07:52:59.352372',NULL,NULL);
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
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,1,'Admin','Admin',1,'system@company.com','2026-08-05 14:20:19.906653',NULL,NULL,1,0,1),(2,1,'Manager','Manager',1,'system@company.com','2026-08-05 14:20:19.906653',NULL,NULL,1,0,1),(3,1,'Employee','Employee',1,'system@company.com','2026-08-05 14:20:19.906653',NULL,NULL,1,0,1),(4,2,'testrole','testrole',1,NULL,'2026-08-13 09:56:26.114345',NULL,NULL,1,0,1);
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
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQUserRoles` (`UserId`,`RoleId`),
  KEY `IX_user_roles_RoleId` (`RoleId`),
  CONSTRAINT `FK_user_roles_roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_user_roles_users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_roles`
--

LOCK TABLES `user_roles` WRITE;
/*!40000 ALTER TABLE `user_roles` DISABLE KEYS */;
INSERT INTO `user_roles` VALUES (10,1,1,1,'system@company.com','2026-08-05 14:25:21.528941',NULL,NULL),(11,2,2,1,'system@company.com','2026-08-05 14:25:21.528941',NULL,NULL),(12,3,3,1,'system@company.com','2026-08-05 14:25:21.528941',NULL,NULL),(13,8,2,1,NULL,'2026-08-13 07:08:18.604569',NULL,NULL);
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
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin@gmail.com','admin','pbkdf2-sha256$100000$G9pK9JbSLNAv07+Do8Bw5g==$ysLL9Ga0haI+hrnt9gf9m+vc1Vm1ZUH573WmCHq2jHg=','d0201823-2d5d-43e1-97ba-20e4056cdf47',9,NULL,'2026-08-05 06:57:56.914471',NULL,NULL,1,0,1,'admin'),(2,'superadmin@company.com','Super Admin','$2a$12$samplebcrypthash','516ad0e6-b98f-403b-8324-a4c4d6e8b258',19,'system@company.com','2026-08-05 14:25:01.468932',NULL,'2026-08-11 07:12:02.162436',0,1,2,'user-2'),(3,'admin@company.com','System Admin','$2a$12$samplebcrypthash','8759e8d6-9d7e-4e05-a25d-01668736d2c1',10,'system@company.com','2026-08-05 14:25:01.468932',NULL,'2026-08-11 07:12:05.997542',0,1,2,'user-3'),(4,'user1@company.com','John Smith','$2a$12$samplebcrypthash','6a29552d-4ef2-499f-a947-c62636deeb15',2,'system@company.com','2026-08-05 14:25:01.468932',NULL,'2026-08-11 07:12:07.980428',0,1,2,'user-4'),(5,'admin2@gmail.com','admin 2','pbkdf2-sha256$100000$O7xkI/9NOqaArrbSipzPnQ==$1z9Bihrj7xSCzkJnXdv3CUHv8rqJgzHsnurS+6ufXes=','11e0399f-f852-4b4c-aa55-5c720ed98cf0',2,NULL,'2026-08-05 07:59:36.828165',NULL,'2026-08-10 08:02:16.505903',0,1,3,'user-5'),(6,'admin1@gmail.com','admin 1','pbkdf2-sha256$100000$Ndz/cmqNZomrwxcDrCg4KA==$KstjLqaFqNXw6tPT0e5A9SRTczuv1FoTegZF6ru94xg=','eea107e3-6ff9-4547-a514-ed46491aa61a',2,NULL,'2026-08-10 08:01:35.402933',NULL,'2026-08-11 07:12:00.208214',0,1,2,'user-6'),(7,'smoke-1786355029@example.test','Smoke User Updated','pbkdf2-sha256$100000$dO034n5jjZjBLV/cfLG4ZA==$Dhu7F91IA6Xra/K3LNnZH7YaR/u9ROMFR3l0lmbCXJs=','94fdac86-9de8-422c-8a4b-4ba1dabb5572',2,NULL,'2026-08-10 09:43:49.413483',NULL,'2026-08-10 09:43:49.523610',0,1,3,'smoke-1786355029-updated'),(8,'hien@gmail.com','hoang hien 1','pbkdf2-sha256$100000$HkoITmvkHW12o5gpHaoqwg==$9r8Aud7F/L2qHPOKYQvJ9dZA3mPG5uoDeuaD0ZE9soI=','0426a105-d10d-43cc-8570-69e8a41cdd01',19,NULL,'2026-08-11 03:01:35.426183',NULL,'2026-08-11 07:12:43.801130',1,0,2,'hien');
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

-- Dump completed on 2026-08-18 16:54:08
