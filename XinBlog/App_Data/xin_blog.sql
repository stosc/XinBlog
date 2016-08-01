-- --------------------------------------------------------
-- 主机:                           127.0.0.1
-- 服务器版本:                        5.6.22-log - MySQL Community Server (GPL)
-- 服务器操作系统:                      Win64
-- HeidiSQL 版本:                  8.3.0.4694
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- 导出 xin_blog 的数据库结构
CREATE DATABASE IF NOT EXISTS `xin_blog` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `xin_blog`;


-- 导出  表 xin_blog.article 结构
CREATE TABLE IF NOT EXISTS `article` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `Guid` varchar(50) DEFAULT NULL,
  `Title` varchar(50) DEFAULT NULL,
  `Abstract` text,
  `AbstractImg` varchar(50) DEFAULT NULL,
  `Tags` varchar(100) DEFAULT NULL,
  `MetaTitle` varchar(50) DEFAULT NULL,
  `MetaDescription` varchar(50) DEFAULT NULL,
  `Content` longtext COMMENT '发布内容',
  `CreateDate` datetime DEFAULT NULL,
  `IsPublish` tinyint(1) NOT NULL DEFAULT '0' COMMENT '发布',
  `IsRecommend` tinyint(1) NOT NULL DEFAULT '0' COMMENT '推荐',
  `IsSeparate` tinyint(1) NOT NULL DEFAULT '0' COMMENT '独立页面',
  `ReadCount` int(11) DEFAULT NULL COMMENT '阅读计数',
  PRIMARY KEY (`id`),
  KEY `Index 2` (`Guid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 数据导出被取消选择。


-- 导出  视图 xin_blog.articleeditmeta 结构
-- 创建临时表以解决视图依赖性错误
CREATE TABLE `articleeditmeta` (
	`id` INT(11) NOT NULL,
	`Title` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`IsPublish` TINYINT(1) NOT NULL COMMENT '发布',
	`CreateDate` DATETIME NULL,
	`IsSeparate` TINYINT(1) NOT NULL COMMENT '独立页面',
	`IsRecommend` TINYINT(1) NOT NULL COMMENT '推荐'
) ENGINE=MyISAM;


-- 导出  视图 xin_blog.articleshowmeta 结构
-- 创建临时表以解决视图依赖性错误
CREATE TABLE `articleshowmeta` (
	`id` INT(11) NOT NULL,
	`Title` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`Abstract` TEXT NULL COLLATE 'utf8_general_ci',
	`AbstractImg` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`Tags` VARCHAR(100) NULL COLLATE 'utf8_general_ci',
	`CreateDate` DATETIME NULL,
	`IsRecommend` TINYINT(1) NOT NULL COMMENT '推荐',
	`ReadCount` INT(11) NULL COMMENT '阅读计数'
) ENGINE=MyISAM;


-- 导出  表 xin_blog.aspnetusers 结构
CREATE TABLE IF NOT EXISTS `aspnetusers` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Email` varchar(50) DEFAULT '0',
  `UserName` varchar(50) DEFAULT '0',
  `PasswordHash` varchar(50) DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `Index 2` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 数据导出被取消选择。


-- 导出  表 xin_blog.general 结构
CREATE TABLE IF NOT EXISTS `general` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(50) DEFAULT '0',
  `Description` varchar(500) DEFAULT '0',
  `PostsPerPage` int(11) DEFAULT '0',
  `Cover` varchar(500) DEFAULT NULL,
  `CommentPlugin` varchar(5000) DEFAULT NULL,
  KEY `Index 1` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 数据导出被取消选择。


-- 导出  表 xin_blog.navigation 结构
CREATE TABLE IF NOT EXISTS `navigation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(50) DEFAULT '0',
  `Link` varchar(50) DEFAULT '0',
  `Sequence` int(11) DEFAULT '0',
  KEY `Index 1` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 数据导出被取消选择。


-- 导出  表 xin_blog.resetpasswordlog 结构
CREATE TABLE IF NOT EXISTS `resetpasswordlog` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `key` varchar(100) DEFAULT NULL,
  `isUse` tinyint(4) DEFAULT '0',
  KEY `Index 1` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 数据导出被取消选择。


-- 导出  表 xin_blog.tags 结构
CREATE TABLE IF NOT EXISTS `tags` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) DEFAULT '0',
  `articles` varchar(100) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `Index 2` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 数据导出被取消选择。


-- 导出  表 xin_blog.user 结构
CREATE TABLE IF NOT EXISTS `user` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Email` varchar(256) DEFAULT NULL,
  `PasswordHash` longtext,
  `UserName` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) DEFAULT NULL,
  `SecurityStamp` longtext,
  `PhoneNumber` longtext,
  `PhoneNumberConfirmed` tinyint(1) DEFAULT NULL,
  `TwoFactorEnabled` tinyint(1) DEFAULT NULL,
  `LockoutEndDateUtc` datetime DEFAULT NULL,
  `CreateDate` datetime DEFAULT NULL,
  `LockoutEnabled` tinyint(1) DEFAULT NULL,
  `AccessFailedCount` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 数据导出被取消选择。


-- 导出  视图 xin_blog.articleeditmeta 结构
-- 移除临时表并创建最终视图结构
DROP TABLE IF EXISTS `articleeditmeta`;
CREATE ALGORITHM=MERGE DEFINER=`root`@`127.0.0.1` VIEW `articleeditmeta` AS select id,Title,IsPublish,CreateDate ,IsSeparate,IsRecommend from Article Order By Id desc ;


-- 导出  视图 xin_blog.articleshowmeta 结构
-- 移除临时表并创建最终视图结构
DROP TABLE IF EXISTS `articleshowmeta`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`127.0.0.1` VIEW `articleshowmeta` AS select id,Title,Abstract,AbstractImg,Tags,CreateDate,IsRecommend,ReadCount from article where IsPublish = 1 and IsSeparate = 0 order by id desc ;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;


-- --------------------------------------------------------
-- 主机:                           127.0.0.1
-- 服务器版本:                        5.6.22-log - MySQL Community Server (GPL)
-- 服务器操作系统:                      Win64
-- HeidiSQL 版本:                  8.3.0.4694
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
-- 正在导出表  xin_blog.general 的数据：~0 rows (大约)
DELETE FROM `general`;
/*!40000 ALTER TABLE `general` DISABLE KEYS */;
INSERT INTO `general` (`id`, `Title`, `Description`, `PostsPerPage`, `Cover`, `CommentPlugin`) VALUES
	(1, '信博客', '信博客系统', 10, NULL,NULL);
	/*!40000 ALTER TABLE `general` ENABLE KEYS */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

