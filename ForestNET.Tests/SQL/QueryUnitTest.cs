using ForestNET.Lib.SQL;

namespace ForestNET.Tests.SQL
{
    public class QueryUnitTest
    {
        [Test]
        public void TestQuery()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                Dictionary<string, int> a_baseGateways = new()
                {
                    [BaseGateway.MARIADB.ToString()] = 0,
                    [BaseGateway.SQLITE.ToString()] = 1,
                    [BaseGateway.MSSQL.ToString()] = 2,
                    [BaseGateway.PGSQL.ToString()] = 3,
                    [BaseGateway.ORACLE.ToString()] = 4,
                    [BaseGateway.NOSQLMDB.ToString()] = 5
                };

                string[][] a_expectedQueries = new string[][] {
				    /* MARIADB */ new string[] {
                        "CREATE TABLE `sys_forestnet_testddl` (`Id` INT(10) NOT NULL PRIMARY KEY AUTO_INCREMENT, `UUID` VARCHAR(36) NOT NULL UNIQUE, `ShortText` VARCHAR(255) NULL, `Text` TEXT NULL, `SmallInt` SMALLINT(6) NULL, `Int` INT(10) NULL, `BigInt` BIGINT(20) NULL, `DateTime` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP, `Date` TIMESTAMP NOT NULL DEFAULT '2020-04-06 08:10:12', `Time` TIME NULL, `LocalDateTime` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP, `LocalDate` TIMESTAMP NULL, `LocalTime` TIME DEFAULT '12:24:46', `DoubleCol` DOUBLE NULL, `Decimal` DECIMAL(38,9) NULL, `Bool` BIT(1) NULL)",
                        "CREATE TABLE `sys_forestnet_testddl2` (`Id` INT(10) NOT NULL PRIMARY KEY AUTO_INCREMENT, `DoubleCol` DOUBLE NULL)",
                        "ALTER TABLE `sys_forestnet_testddl` ADD `Text2` VARCHAR(36) NULL, ADD `ShortText2` VARCHAR(255) NULL",
                        "ALTER TABLE `sys_forestnet_testddl` ADD UNIQUE `new_index_Int` (`Int`)",
                        "ALTER TABLE `sys_forestnet_testddl` ADD UNIQUE `new_index_SmallInt_Bool` (`SmallInt`, `Bool`), DROP INDEX `new_index_Int`",
                        "ALTER TABLE `sys_forestnet_testddl` ADD INDEX `new_index_Text2` (`Text2`)",
                        "ALTER TABLE `sys_forestnet_testddl` DROP INDEX `new_index_Text2`",
                        "ALTER TABLE `sys_forestnet_testddl` CHANGE `Text2` `Text2Changed` VARCHAR(255) NOT NULL DEFAULT 'Das ist das Haus vom Nikolaus'",
                        "INSERT INTO `sys_forestnet_testddl` (`sys_forestnet_testddl`.`UUID`, `sys_forestnet_testddl`.`ShortText`, `sys_forestnet_testddl`.`Text`, `sys_forestnet_testddl`.`SmallInt`, `sys_forestnet_testddl`.`Int`, `sys_forestnet_testddl`.`BigInt`, `sys_forestnet_testddl`.`DateTime`, `sys_forestnet_testddl`.`Date`, `sys_forestnet_testddl`.`Time`, `sys_forestnet_testddl`.`LocalDateTime`, `sys_forestnet_testddl`.`LocalDate`, `sys_forestnet_testddl`.`LocalTime`, `sys_forestnet_testddl`.`DoubleCol`, `sys_forestnet_testddl`.`Decimal`, `sys_forestnet_testddl`.`Bool`, `sys_forestnet_testddl`.`Text2Changed`, `sys_forestnet_testddl`.`ShortText2`) VALUES ('123e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 123, 1234567890, 1234567890123, '2003-12-15 08:33:03', '2009-06-29', '11:01:43', '2010-09-02 05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, true, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "INSERT INTO `sys_forestnet_testddl` (`sys_forestnet_testddl`.`UUID`, `sys_forestnet_testddl`.`ShortText`, `sys_forestnet_testddl`.`Text`, `sys_forestnet_testddl`.`SmallInt`, `sys_forestnet_testddl`.`Int`, `sys_forestnet_testddl`.`BigInt`, `sys_forestnet_testddl`.`DateTime`, `sys_forestnet_testddl`.`Date`, `sys_forestnet_testddl`.`Time`, `sys_forestnet_testddl`.`LocalDateTime`, `sys_forestnet_testddl`.`LocalDate`, `sys_forestnet_testddl`.`LocalTime`, `sys_forestnet_testddl`.`DoubleCol`, `sys_forestnet_testddl`.`Decimal`, `sys_forestnet_testddl`.`Bool`, `sys_forestnet_testddl`.`Text2Changed`, `sys_forestnet_testddl`.`ShortText2`) VALUES ('223e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 223, 1234567890, 2234567890123, '2003-12-15 08:33:03', '2009-06-29', '11:01:43', '2010-09-02 05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, false, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "INSERT INTO `sys_forestnet_testddl` (`sys_forestnet_testddl`.`UUID`, `sys_forestnet_testddl`.`ShortText`, `sys_forestnet_testddl`.`Text`, `sys_forestnet_testddl`.`SmallInt`, `sys_forestnet_testddl`.`Int`, `sys_forestnet_testddl`.`BigInt`, `sys_forestnet_testddl`.`DateTime`, `sys_forestnet_testddl`.`Date`, `sys_forestnet_testddl`.`Time`, `sys_forestnet_testddl`.`LocalDateTime`, `sys_forestnet_testddl`.`LocalDate`, `sys_forestnet_testddl`.`LocalTime`, `sys_forestnet_testddl`.`DoubleCol`, `sys_forestnet_testddl`.`Decimal`, `sys_forestnet_testddl`.`Bool`, `sys_forestnet_testddl`.`Text2Changed`, `sys_forestnet_testddl`.`ShortText2`) VALUES ('323e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 323, 1234567890, 3234567890123, '2003-12-15 08:33:03', '2009-06-29', '11:01:43', '2010-09-02 05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, true, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "SELECT `sys_forestnet_testddl`.`ShortText`, MIN(`sys_forestnet_testddl`.`SmallInt`), `sys_forestnet_testddl`.`LocalDate` AS 'Spalte C', `sys_forestnet_testddl`.`Int`, `sys_forestnet_testddl2`.`Id` FROM `sys_forestnet_testddl` INNER JOIN `sys_forestnet_testddl2` ON (`sys_forestnet_testddl2`.`Id` = `sys_forestnet_testddl`.`Id` AND `sys_forestnet_testddl2`.`DoubleCol` <= `sys_forestnet_testddl`.`DoubleCol`) WHERE `sys_forestnet_testddl`.`ShortText` <> 'Wert' OR `sys_forestnet_testddl2`.`Id` >= 123 AND `sys_forestnet_testddl`.`SmallInt` > 25353 GROUP BY `sys_forestnet_testddl`.`ShortText`, `sys_forestnet_testddl`.`LocalDate`, `sys_forestnet_testddl`.`Int`, `sys_forestnet_testddl2`.`Id` HAVING (`sys_forestnet_testddl`.`Int` <= 456.0 AND `sys_forestnet_testddl`.`ShortText` = 'Trew' AND `sys_forestnet_testddl`.`LocalDate` <> '2018-11-16') ORDER BY `sys_forestnet_testddl2`.`Id` ASC, `sys_forestnet_testddl`.`ShortText` DESC LIMIT 0, 10",
                        "UPDATE `sys_forestnet_testddl` SET `sys_forestnet_testddl`.`ShortText` = 'Wert', `sys_forestnet_testddl`.`Int` = 1337, `sys_forestnet_testddl`.`DoubleCol` = 35.67, `sys_forestnet_testddl`.`DateTime` = '2003-12-15 08:33:03' WHERE `sys_forestnet_testddl`.`ShortText` <> 'Wert' OR `sys_forestnet_testddl`.`SmallInt` >= 123 AND `sys_forestnet_testddl`.`DateTime` >= '2003-12-15 08:33:03'",
                        "SELECT * FROM `sys_forestnet_testddl` WHERE `sys_forestnet_testddl`.`DateTime` <> '2003-12-15 08:33:03' OR `sys_forestnet_testddl`.`Date` >= '2009-06-29' AND `sys_forestnet_testddl`.`Time` > '11:01:43'",
                        "SELECT * FROM `sys_forestnet_testddl` WHERE `sys_forestnet_testddl`.`LocalDateTime` <> '2010-09-02 05:55:13' OR `sys_forestnet_testddl`.`LocalDate` >= '2018-11-16' AND `sys_forestnet_testddl`.`LocalTime` > '17:42:23'",
                        "SELECT * FROM `sys_forestnet_testddl`",
                        "DELETE FROM `sys_forestnet_testddl` WHERE `sys_forestnet_testddl`.`ShortText` <> 'Wert' OR `sys_forestnet_testddl`.`SmallInt` >= 32.45 AND `sys_forestnet_testddl`.`DateTime` > '2003-12-15 08:33:03'",
                        "ALTER TABLE `sys_forestnet_testddl` DROP `ShortText2`",
                        "ALTER TABLE `sys_forestnet_testddl` DROP `BigInt`, DROP `Int`",
                        "ALTER TABLE `sys_forestnet_testddl` DROP INDEX `new_index_SmallInt_Bool`",
                        "TRUNCATE TABLE `sys_forestnet_testddl`",
                        "DROP TABLE `sys_forestnet_testddl`",
                        "DROP TABLE `sys_forestnet_testddl2`"
                    },
				    /* SQLITE */ new string[] {
                        "CREATE TABLE `sys_forestnet_testddl` (`Id` integer NOT NULL PRIMARY KEY AUTOINCREMENT, `UUID` varchar(36) NOT NULL UNIQUE, `ShortText` varchar(255) NULL, `Text` text NULL, `SmallInt` smallint NULL, `Int` integer NULL, `BigInt` bigint NULL, `DateTime` datetime NULL DEFAULT CURRENT_TIMESTAMP, `Date` datetime NOT NULL DEFAULT '2020-04-06 08:10:12', `Time` time NULL, `LocalDateTime` datetime NULL DEFAULT CURRENT_TIMESTAMP, `LocalDate` datetime NULL, `LocalTime` time DEFAULT '12:24:46', `DoubleCol` double NULL, `Decimal` decimal(38,9) NULL, `Bool` bit(1) NULL)",
                        "CREATE TABLE `sys_forestnet_testddl2` (`Id` integer NOT NULL PRIMARY KEY AUTOINCREMENT, `DoubleCol` double NULL)",
                        "ALTER TABLE `sys_forestnet_testddl` ADD `Text2` varchar(36) NULL::forestnetSQLQuerySeparator::ALTER TABLE `sys_forestnet_testddl` ADD `ShortText2` varchar(255) NULL",
                        "CREATE UNIQUE INDEX `new_index_Int` ON `sys_forestnet_testddl` (`Int`)",
                        "DROP INDEX `new_index_Int`::forestnetSQLQuerySeparator::CREATE UNIQUE INDEX `new_index_SmallInt_Bool` ON `sys_forestnet_testddl` (`SmallInt`, `Bool`)",
                        "CREATE INDEX `new_index_Text2` ON `sys_forestnet_testddl` (`Text2`)",
                        "DROP INDEX `new_index_Text2`",
                        "CREATE TABLE `REPLACE_RANDOM_sys_forestnet_testddl` (`Id` integer NOT NULL PRIMARY KEY AUTOINCREMENT, `UUID` varchar(36) NOT NULL UNIQUE, `ShortText` varchar(255) NULL, `Text` text NULL, `SmallInt` smallint NULL, `Int` integer NULL, `BigInt` bigint NULL, `DateTime` datetime NULL DEFAULT CURRENT_TIMESTAMP, `Date` datetime NOT NULL DEFAULT '2020-04-06 08:10:12', `Time` time NULL, `LocalDateTime` datetime NULL DEFAULT CURRENT_TIMESTAMP, `LocalDate` datetime NULL, `LocalTime` time DEFAULT '12:24:46', `DoubleCol` double NULL, `Decimal` decimal(38,9) NULL, `Bool` bit(1) NULL, `Text2Changed` varchar(255) NULL DEFAULT 'Das ist das Haus vom Nikolaus', `ShortText2` varchar(255) NULL)::forestnetSQLQuerySeparator::INSERT INTO `REPLACE_RANDOM_sys_forestnet_testddl` (`Id`,`UUID`,`ShortText`,`Text`,`SmallInt`,`Int`,`BigInt`,`DateTime`,`Date`,`Time`,`LocalDateTime`,`LocalDate`,`LocalTime`,`DoubleCol`,`Decimal`,`Bool`,`Text2Changed`,`ShortText2`) SELECT `Id`,`UUID`,`ShortText`,`Text`,`SmallInt`,`Int`,`BigInt`,`DateTime`,`Date`,`Time`,`LocalDateTime`,`LocalDate`,`LocalTime`,`DoubleCol`,`Decimal`,`Bool`,`Text2`,`ShortText2` FROM `sys_forestnet_testddl`::forestnetSQLQuerySeparator::DROP TABLE `sys_forestnet_testddl`::forestnetSQLQuerySeparator::ALTER TABLE `REPLACE_RANDOM_sys_forestnet_testddl` RENAME TO `sys_forestnet_testddl`::forestnetSQLQuerySeparator::",
                        "INSERT INTO `sys_forestnet_testddl` (`UUID`, `ShortText`, `Text`, `SmallInt`, `Int`, `BigInt`, `DateTime`, `Date`, `Time`, `LocalDateTime`, `LocalDate`, `LocalTime`, `DoubleCol`, `Decimal`, `Bool`, `Text2Changed`, `ShortText2`) VALUES ('123e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 123, 1234567890, 1234567890123, '2003-12-15 08:33:03', '2009-06-29', '11:01:43', '2010-09-02 05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, true, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "INSERT INTO `sys_forestnet_testddl` (`UUID`, `ShortText`, `Text`, `SmallInt`, `Int`, `BigInt`, `DateTime`, `Date`, `Time`, `LocalDateTime`, `LocalDate`, `LocalTime`, `DoubleCol`, `Decimal`, `Bool`, `Text2Changed`, `ShortText2`) VALUES ('223e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 223, 1234567890, 2234567890123, '2003-12-15 08:33:03', '2009-06-29', '11:01:43', '2010-09-02 05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, false, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "INSERT INTO `sys_forestnet_testddl` (`UUID`, `ShortText`, `Text`, `SmallInt`, `Int`, `BigInt`, `DateTime`, `Date`, `Time`, `LocalDateTime`, `LocalDate`, `LocalTime`, `DoubleCol`, `Decimal`, `Bool`, `Text2Changed`, `ShortText2`) VALUES ('323e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 323, 1234567890, 3234567890123, '2003-12-15 08:33:03', '2009-06-29', '11:01:43', '2010-09-02 05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, true, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "SELECT `sys_forestnet_testddl`.`ShortText`, MIN(`sys_forestnet_testddl`.`SmallInt`), `sys_forestnet_testddl`.`LocalDate` AS 'Spalte C', `sys_forestnet_testddl`.`Int`, `sys_forestnet_testddl2`.`Id` FROM `sys_forestnet_testddl` INNER JOIN `sys_forestnet_testddl2` ON (`sys_forestnet_testddl2`.`Id` = `sys_forestnet_testddl`.`Id` AND `sys_forestnet_testddl2`.`DoubleCol` <= `sys_forestnet_testddl`.`DoubleCol`) WHERE `sys_forestnet_testddl`.`ShortText` <> 'Wert' OR `sys_forestnet_testddl2`.`Id` >= 123 AND `sys_forestnet_testddl`.`SmallInt` > 25353 GROUP BY `sys_forestnet_testddl`.`ShortText`, `sys_forestnet_testddl`.`LocalDate`, `sys_forestnet_testddl`.`Int`, `sys_forestnet_testddl2`.`Id` HAVING (`sys_forestnet_testddl`.`Int` <= 456.0 AND `sys_forestnet_testddl`.`ShortText` = 'Trew' AND `sys_forestnet_testddl`.`LocalDate` <> '2018-11-16') ORDER BY `sys_forestnet_testddl2`.`Id` ASC, `sys_forestnet_testddl`.`ShortText` DESC LIMIT 0, 10",
                        "UPDATE `sys_forestnet_testddl` SET `ShortText` = 'Wert', `Int` = 1337, `DoubleCol` = 35.67, `DateTime` = '2003-12-15 08:33:03' WHERE `ShortText` <> 'Wert' OR `SmallInt` >= 123 AND `DateTime` >= '2003-12-15 08:33:03'",
                        "SELECT * FROM `sys_forestnet_testddl` WHERE `sys_forestnet_testddl`.`DateTime` <> '2003-12-15 08:33:03' OR `sys_forestnet_testddl`.`Date` >= '2009-06-29' AND `sys_forestnet_testddl`.`Time` > '11:01:43'",
                        "SELECT * FROM `sys_forestnet_testddl` WHERE `sys_forestnet_testddl`.`LocalDateTime` <> '2010-09-02 05:55:13' OR `sys_forestnet_testddl`.`LocalDate` >= '2018-11-16' AND `sys_forestnet_testddl`.`LocalTime` > '17:42:23'",
                        "SELECT * FROM `sys_forestnet_testddl`",
                        "DELETE FROM `sys_forestnet_testddl` WHERE `sys_forestnet_testddl`.`ShortText` <> 'Wert' OR `sys_forestnet_testddl`.`SmallInt` >= 32.45 AND `sys_forestnet_testddl`.`DateTime` > '2003-12-15 08:33:03'",
                        "CREATE TABLE `REPLACE_RANDOM_sys_forestnet_testddl` (`Id` integer NOT NULL PRIMARY KEY AUTOINCREMENT, `UUID` varchar(36) NOT NULL UNIQUE, `ShortText` varchar(255) NULL, `Text` text NULL, `SmallInt` smallint NULL, `Int` integer NULL, `BigInt` bigint NULL, `DateTime` datetime NULL DEFAULT CURRENT_TIMESTAMP, `Date` datetime NOT NULL DEFAULT '2020-04-06 08:10:12', `Time` time NULL, `LocalDateTime` datetime NULL DEFAULT CURRENT_TIMESTAMP, `LocalDate` datetime NULL, `LocalTime` time DEFAULT '12:24:46', `DoubleCol` double NULL, `Decimal` decimal(38,9) NULL, `Bool` bit(1) NULL, `Text2Changed` varchar(36) NULL DEFAULT 'Das ist das Haus vom Nikolaus')::forestnetSQLQuerySeparator::INSERT INTO `REPLACE_RANDOM_sys_forestnet_testddl` (`Id`,`UUID`,`ShortText`,`Text`,`SmallInt`,`Int`,`BigInt`,`DateTime`,`Date`,`Time`,`LocalDateTime`,`LocalDate`,`LocalTime`,`DoubleCol`,`Decimal`,`Bool`,`Text2Changed`) SELECT `Id`,`UUID`,`ShortText`,`Text`,`SmallInt`,`Int`,`BigInt`,`DateTime`,`Date`,`Time`,`LocalDateTime`,`LocalDate`,`LocalTime`,`DoubleCol`,`Decimal`,`Bool`,`Text2Changed` FROM `sys_forestnet_testddl`::forestnetSQLQuerySeparator::DROP TABLE `sys_forestnet_testddl`::forestnetSQLQuerySeparator::ALTER TABLE `REPLACE_RANDOM_sys_forestnet_testddl` RENAME TO `sys_forestnet_testddl`::forestnetSQLQuerySeparator::CREATE UNIQUE INDEX `new_index_BigInt_Bool` ON `sys_forestnet_testddl` (`BigInt`, `Bool`)::forestnetSQLQuerySeparator::",
                        "CREATE TABLE `REPLACE_RANDOM_sys_forestnet_testddl` (`Id` integer NOT NULL PRIMARY KEY AUTOINCREMENT, `UUID` varchar(36) NOT NULL UNIQUE, `ShortText` varchar(255) NULL, `Text` text NULL, `SmallInt` smallint NULL, `DateTime` datetime NULL DEFAULT CURRENT_TIMESTAMP, `Date` datetime NOT NULL DEFAULT '2020-04-06 08:10:12', `Time` time NULL, `LocalDateTime` datetime NULL DEFAULT CURRENT_TIMESTAMP, `LocalDate` datetime NULL, `LocalTime` time DEFAULT '12:24:46', `DoubleCol` double NULL, `Decimal` decimal(38,9) NULL, `Bool` bit(1) NULL, `Text2Changed` varchar(36) NULL DEFAULT 'Das ist das Haus vom Nikolaus')::forestnetSQLQuerySeparator::INSERT INTO `REPLACE_RANDOM_sys_forestnet_testddl` (`Id`,`UUID`,`ShortText`,`Text`,`SmallInt`,`DateTime`,`Date`,`Time`,`LocalDateTime`,`LocalDate`,`LocalTime`,`DoubleCol`,`Decimal`,`Bool`,`Text2Changed`) SELECT `Id`,`UUID`,`ShortText`,`Text`,`SmallInt`,`DateTime`,`Date`,`Time`,`LocalDateTime`,`LocalDate`,`LocalTime`,`DoubleCol`,`Decimal`,`Bool`,`Text2Changed` FROM `sys_forestnet_testddl`::forestnetSQLQuerySeparator::DROP TABLE `sys_forestnet_testddl`::forestnetSQLQuerySeparator::ALTER TABLE `REPLACE_RANDOM_sys_forestnet_testddl` RENAME TO `sys_forestnet_testddl`::forestnetSQLQuerySeparator::CREATE UNIQUE INDEX `new_index_SmallInt_Bool` ON `sys_forestnet_testddl` (`SmallInt`, `Bool`)::forestnetSQLQuerySeparator::",
                        "DROP INDEX `new_index_SmallInt_Bool`",
                        "DELETE FROM `sys_forestnet_testddl`::forestnetSQLQuerySeparator::VACUUM",
                        "DROP TABLE `sys_forestnet_testddl`",
                        "DROP TABLE `sys_forestnet_testddl2`"
                    }, 
				    /* MSSQL */ new string[] {
                        "CREATE TABLE [sys_forestnet_testddl] ([Id] int NOT NULL PRIMARY KEY IDENTITY(1,1), [UUID] nvarchar(36) NOT NULL UNIQUE, [ShortText] nvarchar(255) NULL, [Text] text NULL, [SmallInt] smallint NULL, [Int] int NULL, [BigInt] bigint NULL, [DateTime] datetime NULL DEFAULT CURRENT_TIMESTAMP, [Date] datetime NOT NULL DEFAULT '2020-04-06T08:10:12', [Time] time NULL, [LocalDateTime] datetime NULL DEFAULT CURRENT_TIMESTAMP, [LocalDate] datetime NULL, [LocalTime] time DEFAULT '12:24:46', [DoubleCol] float NULL, [Decimal] decimal(38,9) NULL, [Bool] bit NULL)",
                        "CREATE TABLE [sys_forestnet_testddl2] ([Id] int NOT NULL PRIMARY KEY IDENTITY(1,1), [DoubleCol] float NULL)",
                        "ALTER TABLE [sys_forestnet_testddl] ADD [Text2] nvarchar(36) NULL, [ShortText2] nvarchar(255) NULL",
                        "CREATE UNIQUE INDEX [new_index_Int] ON [sys_forestnet_testddl] ([Int])",
                        "DROP INDEX [new_index_Int] ON [sys_forestnet_testddl]::forestnetSQLQuerySeparator::CREATE UNIQUE INDEX [new_index_SmallInt_Bool] ON [sys_forestnet_testddl] ([SmallInt], [Bool])",
                        "CREATE INDEX [new_index_Text2] ON [sys_forestnet_testddl] ([Text2])",
                        "DROP INDEX [new_index_Text2] ON [sys_forestnet_testddl]",
                        "EXEC sp_rename \"[sys_forestnet_testddl].[Text2]\", \"Text2Changed\", \"COLUMN\"::forestnetSQLQuerySeparator::ALTER TABLE [sys_forestnet_testddl] ALTER COLUMN [Text2Changed] nvarchar(255) NOT NULL",
                        "INSERT INTO [sys_forestnet_testddl] ([sys_forestnet_testddl].[UUID], [sys_forestnet_testddl].[ShortText], [sys_forestnet_testddl].[Text], [sys_forestnet_testddl].[SmallInt], [sys_forestnet_testddl].[Int], [sys_forestnet_testddl].[BigInt], [sys_forestnet_testddl].[DateTime], [sys_forestnet_testddl].[Date], [sys_forestnet_testddl].[Time], [sys_forestnet_testddl].[LocalDateTime], [sys_forestnet_testddl].[LocalDate], [sys_forestnet_testddl].[LocalTime], [sys_forestnet_testddl].[DoubleCol], [sys_forestnet_testddl].[Decimal], [sys_forestnet_testddl].[Bool], [sys_forestnet_testddl].[Text2Changed], [sys_forestnet_testddl].[ShortText2]) VALUES ('123e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 123, 1234567890, 1234567890123, '2003-12-15T08:33:03', '2009-06-29', '11:01:43', '2010-09-02T05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, 1, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "INSERT INTO [sys_forestnet_testddl] ([sys_forestnet_testddl].[UUID], [sys_forestnet_testddl].[ShortText], [sys_forestnet_testddl].[Text], [sys_forestnet_testddl].[SmallInt], [sys_forestnet_testddl].[Int], [sys_forestnet_testddl].[BigInt], [sys_forestnet_testddl].[DateTime], [sys_forestnet_testddl].[Date], [sys_forestnet_testddl].[Time], [sys_forestnet_testddl].[LocalDateTime], [sys_forestnet_testddl].[LocalDate], [sys_forestnet_testddl].[LocalTime], [sys_forestnet_testddl].[DoubleCol], [sys_forestnet_testddl].[Decimal], [sys_forestnet_testddl].[Bool], [sys_forestnet_testddl].[Text2Changed], [sys_forestnet_testddl].[ShortText2]) VALUES ('223e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 223, 1234567890, 2234567890123, '2003-12-15T08:33:03', '2009-06-29', '11:01:43', '2010-09-02T05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, 0, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "INSERT INTO [sys_forestnet_testddl] ([sys_forestnet_testddl].[UUID], [sys_forestnet_testddl].[ShortText], [sys_forestnet_testddl].[Text], [sys_forestnet_testddl].[SmallInt], [sys_forestnet_testddl].[Int], [sys_forestnet_testddl].[BigInt], [sys_forestnet_testddl].[DateTime], [sys_forestnet_testddl].[Date], [sys_forestnet_testddl].[Time], [sys_forestnet_testddl].[LocalDateTime], [sys_forestnet_testddl].[LocalDate], [sys_forestnet_testddl].[LocalTime], [sys_forestnet_testddl].[DoubleCol], [sys_forestnet_testddl].[Decimal], [sys_forestnet_testddl].[Bool], [sys_forestnet_testddl].[Text2Changed], [sys_forestnet_testddl].[ShortText2]) VALUES ('323e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 323, 1234567890, 3234567890123, '2003-12-15T08:33:03', '2009-06-29', '11:01:43', '2010-09-02T05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, 1, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "SELECT [sys_forestnet_testddl].[ShortText], MIN([sys_forestnet_testddl].[SmallInt]), [sys_forestnet_testddl].[LocalDate] AS 'Spalte C', [sys_forestnet_testddl].[Int], [sys_forestnet_testddl2].[Id] FROM [sys_forestnet_testddl] INNER JOIN [sys_forestnet_testddl2] ON ([sys_forestnet_testddl2].[Id] = [sys_forestnet_testddl].[Id] AND [sys_forestnet_testddl2].[DoubleCol] <= [sys_forestnet_testddl].[DoubleCol]) WHERE [sys_forestnet_testddl].[ShortText] <> 'Wert' OR [sys_forestnet_testddl2].[Id] >= 123 AND [sys_forestnet_testddl].[SmallInt] > 25353 GROUP BY [sys_forestnet_testddl].[ShortText], [sys_forestnet_testddl].[LocalDate], [sys_forestnet_testddl].[Int], [sys_forestnet_testddl2].[Id] HAVING ([sys_forestnet_testddl].[Int] <= 456.0 AND [sys_forestnet_testddl].[ShortText] = 'Trew' AND [sys_forestnet_testddl].[LocalDate] <> '2018-11-16') ORDER BY [sys_forestnet_testddl2].[Id] ASC, [sys_forestnet_testddl].[ShortText] DESC OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY",
                        "UPDATE [sys_forestnet_testddl] SET [sys_forestnet_testddl].[ShortText] = 'Wert', [sys_forestnet_testddl].[Int] = 1337, [sys_forestnet_testddl].[DoubleCol] = 35.67, [sys_forestnet_testddl].[DateTime] = '2003-12-15T08:33:03' WHERE [sys_forestnet_testddl].[ShortText] <> 'Wert' OR [sys_forestnet_testddl].[SmallInt] >= 123 AND [sys_forestnet_testddl].[DateTime] >= '2003-12-15T08:33:03'",
                        "SELECT * FROM [sys_forestnet_testddl] WHERE [sys_forestnet_testddl].[DateTime] <> '2003-12-15T08:33:03' OR [sys_forestnet_testddl].[Date] >= '2009-06-29' AND [sys_forestnet_testddl].[Time] > '11:01:43'",
                        "SELECT * FROM [sys_forestnet_testddl] WHERE [sys_forestnet_testddl].[LocalDateTime] <> '2010-09-02T05:55:13' OR [sys_forestnet_testddl].[LocalDate] >= '2018-11-16' AND [sys_forestnet_testddl].[LocalTime] > '17:42:23'",
                        "SELECT * FROM [sys_forestnet_testddl]",
                        "DELETE FROM [sys_forestnet_testddl] WHERE [sys_forestnet_testddl].[ShortText] <> 'Wert' OR [sys_forestnet_testddl].[SmallInt] >= 32.45 AND [sys_forestnet_testddl].[DateTime] > '2003-12-15T08:33:03'",
                        "ALTER TABLE [sys_forestnet_testddl] DROP COLUMN [ShortText2]",
                        "ALTER TABLE [sys_forestnet_testddl] DROP COLUMN [BigInt], COLUMN [Int]",
                        "DROP INDEX [new_index_SmallInt_Bool] ON [sys_forestnet_testddl]",
                        "TRUNCATE TABLE [sys_forestnet_testddl]",
                        "DROP TABLE [sys_forestnet_testddl]",
                        "DROP TABLE [sys_forestnet_testddl2]"
                    },
				    /* PGSQL */ new string[] {
                        "CREATE TABLE \"sys_forestnet_testddl\" (\"Id\" serial PRIMARY KEY, \"UUID\" varchar(36) NOT NULL UNIQUE, \"ShortText\" varchar(255) NULL, \"Text\" text NULL, \"SmallInt\" smallint NULL, \"Int\" integer NULL, \"BigInt\" bigint NULL, \"DateTime\" timestamp NULL DEFAULT CURRENT_TIMESTAMP, \"Date\" timestamp NOT NULL DEFAULT '2020-04-06 08:10:12', \"Time\" time NULL, \"LocalDateTime\" timestamp NULL DEFAULT CURRENT_TIMESTAMP, \"LocalDate\" timestamp NULL, \"LocalTime\" time DEFAULT '12:24:46', \"DoubleCol\" double precision NULL, \"Decimal\" decimal(38,9) NULL, \"Bool\" boolean NULL)",
                        "CREATE TABLE \"sys_forestnet_testddl2\" (\"Id\" serial PRIMARY KEY, \"DoubleCol\" double precision NULL)",
                        "ALTER TABLE \"sys_forestnet_testddl\" ADD \"Text2\" varchar(36) NULL, ADD \"ShortText2\" varchar(255) NULL",
                        "ALTER TABLE \"sys_forestnet_testddl\" ADD CONSTRAINT \"new_index_Int\" UNIQUE (\"Int\")",
                        "ALTER TABLE \"sys_forestnet_testddl\" ADD CONSTRAINT \"new_index_SmallInt_Bool\" UNIQUE (\"SmallInt\", \"Bool\"), DROP CONSTRAINT \"new_index_Int\"",
                        "CREATE INDEX \"new_index_Text2\" ON \"sys_forestnet_testddl\" (\"Text2\")",
                        "DROP INDEX \"new_index_Text2\"",
                        "ALTER TABLE \"sys_forestnet_testddl\" RENAME COLUMN \"Text2\" TO \"Text2Changed\"::forestnetSQLQuerySeparator::ALTER TABLE \"sys_forestnet_testddl\" ALTER COLUMN \"Text2Changed\" TYPE varchar(255), ALTER COLUMN \"Text2Changed\" SET NOT NULL, ALTER COLUMN \"Text2Changed\" SET DEFAULT  'Das ist das Haus vom Nikolaus'",
                        "INSERT INTO \"sys_forestnet_testddl\" (\"UUID\", \"ShortText\", \"Text\", \"SmallInt\", \"Int\", \"BigInt\", \"DateTime\", \"Date\", \"Time\", \"LocalDateTime\", \"LocalDate\", \"LocalTime\", \"DoubleCol\", \"Decimal\", \"Bool\", \"Text2Changed\", \"ShortText2\") VALUES ('123e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 123, 1234567890, 1234567890123, '2003-12-15 08:33:03', '2009-06-29', '11:01:43', '2010-09-02 05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, true, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "INSERT INTO \"sys_forestnet_testddl\" (\"UUID\", \"ShortText\", \"Text\", \"SmallInt\", \"Int\", \"BigInt\", \"DateTime\", \"Date\", \"Time\", \"LocalDateTime\", \"LocalDate\", \"LocalTime\", \"DoubleCol\", \"Decimal\", \"Bool\", \"Text2Changed\", \"ShortText2\") VALUES ('223e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 223, 1234567890, 2234567890123, '2003-12-15 08:33:03', '2009-06-29', '11:01:43', '2010-09-02 05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, false, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "INSERT INTO \"sys_forestnet_testddl\" (\"UUID\", \"ShortText\", \"Text\", \"SmallInt\", \"Int\", \"BigInt\", \"DateTime\", \"Date\", \"Time\", \"LocalDateTime\", \"LocalDate\", \"LocalTime\", \"DoubleCol\", \"Decimal\", \"Bool\", \"Text2Changed\", \"ShortText2\") VALUES ('323e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 323, 1234567890, 3234567890123, '2003-12-15 08:33:03', '2009-06-29', '11:01:43', '2010-09-02 05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, true, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "SELECT \"sys_forestnet_testddl\".\"ShortText\", MIN(\"sys_forestnet_testddl\".\"SmallInt\"), \"sys_forestnet_testddl\".\"LocalDate\" AS \"Spalte C\", \"sys_forestnet_testddl\".\"Int\", \"sys_forestnet_testddl2\".\"Id\" FROM \"sys_forestnet_testddl\" INNER JOIN \"sys_forestnet_testddl2\" ON (\"sys_forestnet_testddl2\".\"Id\" = \"sys_forestnet_testddl\".\"Id\" AND \"sys_forestnet_testddl2\".\"DoubleCol\" <= \"sys_forestnet_testddl\".\"DoubleCol\") WHERE \"sys_forestnet_testddl\".\"ShortText\" <> 'Wert' OR \"sys_forestnet_testddl2\".\"Id\" >= 123 AND \"sys_forestnet_testddl\".\"SmallInt\" > 25353 GROUP BY \"sys_forestnet_testddl\".\"ShortText\", \"sys_forestnet_testddl\".\"LocalDate\", \"sys_forestnet_testddl\".\"Int\", \"sys_forestnet_testddl2\".\"Id\" HAVING (\"sys_forestnet_testddl\".\"Int\" <= 456.0 AND \"sys_forestnet_testddl\".\"ShortText\" = 'Trew' AND \"sys_forestnet_testddl\".\"LocalDate\" <> '2018-11-16') ORDER BY \"sys_forestnet_testddl2\".\"Id\" ASC, \"sys_forestnet_testddl\".\"ShortText\" DESC LIMIT 10 OFFSET 0",
                        "UPDATE \"sys_forestnet_testddl\" SET \"ShortText\" = 'Wert', \"Int\" = 1337, \"DoubleCol\" = 35.67, \"DateTime\" = '2003-12-15 08:33:03' WHERE \"ShortText\" <> 'Wert' OR \"SmallInt\" >= 123 AND \"DateTime\" >= '2003-12-15 08:33:03'",
                        "SELECT * FROM \"sys_forestnet_testddl\" WHERE \"sys_forestnet_testddl\".\"DateTime\" <> '2003-12-15 08:33:03' OR \"sys_forestnet_testddl\".\"Date\" >= '2009-06-29' AND \"sys_forestnet_testddl\".\"Time\" > '11:01:43'",
                        "SELECT * FROM \"sys_forestnet_testddl\" WHERE \"sys_forestnet_testddl\".\"LocalDateTime\" <> '2010-09-02 05:55:13' OR \"sys_forestnet_testddl\".\"LocalDate\" >= '2018-11-16' AND \"sys_forestnet_testddl\".\"LocalTime\" > '17:42:23'",
                        "SELECT * FROM \"sys_forestnet_testddl\"",
                        "DELETE FROM \"sys_forestnet_testddl\" WHERE \"sys_forestnet_testddl\".\"ShortText\" <> 'Wert' OR \"sys_forestnet_testddl\".\"SmallInt\" >= 32.45 AND \"sys_forestnet_testddl\".\"DateTime\" > '2003-12-15 08:33:03'",
                        "ALTER TABLE \"sys_forestnet_testddl\" DROP \"ShortText2\"",
                        "ALTER TABLE \"sys_forestnet_testddl\" DROP \"BigInt\", DROP \"Int\"",
                        "ALTER TABLE \"sys_forestnet_testddl\" DROP CONSTRAINT \"new_index_SmallInt_Bool\"",
                        "TRUNCATE TABLE \"sys_forestnet_testddl\"",
                        "DROP TABLE \"sys_forestnet_testddl\"",
                        "DROP TABLE \"sys_forestnet_testddl2\""
                    },
				    /* ORACLE */ new string[] {
                        "CREATE TABLE \"sys_forestnet_testddl\" (\"Id\" NUMBER GENERATED by default on null as IDENTITY PRIMARY KEY, \"UUID\" VARCHAR2(36) NOT NULL UNIQUE, \"ShortText\" VARCHAR2(255) NULL, \"Text\" CLOB NULL, \"SmallInt\" NUMBER(5) NULL, \"Int\" NUMBER(10) NULL, \"BigInt\" LONG NULL, \"DateTime\" TIMESTAMP DEFAULT CURRENT_TIMESTAMP NULL, \"Date\" TIMESTAMP DEFAULT timestamp '2020-04-06 08:10:12' NOT NULL, \"Time\" INTERVAL DAY(0) TO SECOND(0) NULL, \"LocalDateTime\" TIMESTAMP DEFAULT CURRENT_TIMESTAMP NULL, \"LocalDate\" TIMESTAMP NULL, \"LocalTime\" INTERVAL DAY(0) TO SECOND(0) DEFAULT '0 12:24:46', \"DoubleCol\" BINARY_DOUBLE NULL, \"Decimal\" NUMBER(38,9) NULL, \"Bool\" CHAR(1) NULL)",
                        "CREATE TABLE \"sys_forestnet_testddl2\" (\"Id\" NUMBER GENERATED by default on null as IDENTITY PRIMARY KEY, \"DoubleCol\" BINARY_DOUBLE NULL)",
                        "ALTER TABLE \"sys_forestnet_testddl\" ADD (\"Text2\" VARCHAR2(36) NULL, \"ShortText2\" VARCHAR2(255) NULL)",
                        "ALTER TABLE \"sys_forestnet_testddl\" ADD CONSTRAINT \"new_index_Int\" UNIQUE (\"Int\")",
                        "ALTER TABLE \"sys_forestnet_testddl\" DROP CONSTRAINT \"new_index_Int\"::forestnetSQLQuerySeparator::ALTER TABLE \"sys_forestnet_testddl\" ADD CONSTRAINT \"new_index_SmallInt_Bool\" UNIQUE (\"SmallInt\", \"Bool\")",
                        "CREATE INDEX \"new_index_Text2\" ON \"sys_forestnet_testddl\" (\"Text2\")",
                        "DROP INDEX \"new_index_Text2\"",
                        "ALTER TABLE \"sys_forestnet_testddl\" RENAME COLUMN \"Text2\" TO \"Text2Changed\"::forestnetSQLQuerySeparator::ALTER TABLE \"sys_forestnet_testddl\" MODIFY (\"Text2Changed\" VARCHAR2(255) DEFAULT 'Das ist das Haus vom Nikolaus' NOT NULL)",
                        "INSERT INTO \"sys_forestnet_testddl\" (\"UUID\", \"ShortText\", \"Text\", \"SmallInt\", \"Int\", \"BigInt\", \"DateTime\", \"Date\", \"Time\", \"LocalDateTime\", \"LocalDate\", \"LocalTime\", \"DoubleCol\", \"Decimal\", \"Bool\", \"Text2Changed\", \"ShortText2\") VALUES ('123e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 123, 1234567890, 1234567890123, TO_DATE('2003-12-15 08:33:03', 'yyyy-mm-dd hh24:mi:ss'), TO_DATE('2009-06-29', 'yyyy-mm-dd'), TO_DSINTERVAL('+0 11:01:43'), TO_DATE('2010-09-02 05:55:13', 'yyyy-mm-dd hh24:mi:ss'), TO_DATE('2018-11-16', 'yyyy-mm-dd'), TO_DSINTERVAL('+0 17:42:23'), 3.141592, 2.718281828, '1', 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "INSERT INTO \"sys_forestnet_testddl\" (\"UUID\", \"ShortText\", \"Text\", \"SmallInt\", \"Int\", \"BigInt\", \"DateTime\", \"Date\", \"Time\", \"LocalDateTime\", \"LocalDate\", \"LocalTime\", \"DoubleCol\", \"Decimal\", \"Bool\", \"Text2Changed\", \"ShortText2\") VALUES ('223e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 223, 1234567890, 2234567890123, TO_DATE('2003-12-15 08:33:03', 'yyyy-mm-dd hh24:mi:ss'), TO_DATE('2009-06-29', 'yyyy-mm-dd'), TO_DSINTERVAL('+0 11:01:43'), TO_DATE('2010-09-02 05:55:13', 'yyyy-mm-dd hh24:mi:ss'), TO_DATE('2018-11-16', 'yyyy-mm-dd'), TO_DSINTERVAL('+0 17:42:23'), 3.141592, 2.718281828, '0', 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "INSERT INTO \"sys_forestnet_testddl\" (\"UUID\", \"ShortText\", \"Text\", \"SmallInt\", \"Int\", \"BigInt\", \"DateTime\", \"Date\", \"Time\", \"LocalDateTime\", \"LocalDate\", \"LocalTime\", \"DoubleCol\", \"Decimal\", \"Bool\", \"Text2Changed\", \"ShortText2\") VALUES ('323e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 323, 1234567890, 3234567890123, TO_DATE('2003-12-15 08:33:03', 'yyyy-mm-dd hh24:mi:ss'), TO_DATE('2009-06-29', 'yyyy-mm-dd'), TO_DSINTERVAL('+0 11:01:43'), TO_DATE('2010-09-02 05:55:13', 'yyyy-mm-dd hh24:mi:ss'), TO_DATE('2018-11-16', 'yyyy-mm-dd'), TO_DSINTERVAL('+0 17:42:23'), 3.141592, 2.718281828, '1', 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "SELECT \"sys_forestnet_testddl\".\"ShortText\", MIN(\"sys_forestnet_testddl\".\"SmallInt\"), \"sys_forestnet_testddl\".\"LocalDate\" AS \"Spalte C\", \"sys_forestnet_testddl\".\"Int\", \"sys_forestnet_testddl2\".\"Id\" FROM \"sys_forestnet_testddl\" INNER JOIN \"sys_forestnet_testddl2\" ON (\"sys_forestnet_testddl2\".\"Id\" = \"sys_forestnet_testddl\".\"Id\" AND \"sys_forestnet_testddl2\".\"DoubleCol\" <= \"sys_forestnet_testddl\".\"DoubleCol\") WHERE \"sys_forestnet_testddl\".\"ShortText\" <> 'Wert' OR \"sys_forestnet_testddl2\".\"Id\" >= 123 AND \"sys_forestnet_testddl\".\"SmallInt\" > 25353 GROUP BY \"sys_forestnet_testddl\".\"ShortText\", \"sys_forestnet_testddl\".\"LocalDate\", \"sys_forestnet_testddl\".\"Int\", \"sys_forestnet_testddl2\".\"Id\" HAVING (\"sys_forestnet_testddl\".\"Int\" <= 456.0 AND \"sys_forestnet_testddl\".\"ShortText\" = 'Trew' AND \"sys_forestnet_testddl\".\"LocalDate\" <> TO_DATE('2018-11-16', 'yyyy-mm-dd')) ORDER BY \"sys_forestnet_testddl2\".\"Id\" ASC, \"sys_forestnet_testddl\".\"ShortText\" DESC OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY",
                        "UPDATE \"sys_forestnet_testddl\" SET \"ShortText\" = 'Wert', \"Int\" = 1337, \"DoubleCol\" = 35.67, \"DateTime\" = TO_DATE('2003-12-15 08:33:03', 'yyyy-mm-dd hh24:mi:ss') WHERE \"ShortText\" <> 'Wert' OR \"SmallInt\" >= 123 AND \"DateTime\" >= TO_DATE('2003-12-15 08:33:03', 'yyyy-mm-dd hh24:mi:ss')",
                        "SELECT * FROM \"sys_forestnet_testddl\" WHERE \"sys_forestnet_testddl\".\"DateTime\" <> TO_DATE('2003-12-15 08:33:03', 'yyyy-mm-dd hh24:mi:ss') OR \"sys_forestnet_testddl\".\"Date\" >= TO_DATE('2009-06-29', 'yyyy-mm-dd') AND \"sys_forestnet_testddl\".\"Time\" > TO_DSINTERVAL('+0 11:01:43')",
                        "SELECT * FROM \"sys_forestnet_testddl\" WHERE \"sys_forestnet_testddl\".\"LocalDateTime\" <> TO_DATE('2010-09-02 05:55:13', 'yyyy-mm-dd hh24:mi:ss') OR \"sys_forestnet_testddl\".\"LocalDate\" >= TO_DATE('2018-11-16', 'yyyy-mm-dd') AND \"sys_forestnet_testddl\".\"LocalTime\" > TO_DSINTERVAL('+0 17:42:23')",
                        "SELECT * FROM \"sys_forestnet_testddl\"",
                        "DELETE FROM \"sys_forestnet_testddl\" WHERE \"sys_forestnet_testddl\".\"ShortText\" <> 'Wert' OR \"sys_forestnet_testddl\".\"SmallInt\" >= 32.45 AND \"sys_forestnet_testddl\".\"DateTime\" > TO_DATE('2003-12-15 08:33:03', 'yyyy-mm-dd hh24:mi:ss')",
                        "ALTER TABLE \"sys_forestnet_testddl\" DROP (\"ShortText2\")",
                        "ALTER TABLE \"sys_forestnet_testddl\" DROP (\"BigInt\", \"Int\")",
                        "ALTER TABLE \"sys_forestnet_testddl\" DROP CONSTRAINT \"new_index_SmallInt_Bool\"",
                        "TRUNCATE TABLE \"sys_forestnet_testddl\"",
                        "DROP TABLE \"sys_forestnet_testddl\"",
                        "DROP TABLE \"sys_forestnet_testddl2\""
                    },
				    /* NOSQLMDB */ new string[] {
                        "CREATE TABLE `sys_forestnet_testddl` (`Id` INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT, `UUID` VARCHAR NOT NULL UNIQUE, `ShortText` VARCHAR NULL, `Text` TEXT NULL, `SmallInt` SMALLINT NULL, `Int` INTEGER NULL, `BigInt` BIGINT NULL, `DateTime` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP, `Date` TIMESTAMP NOT NULL DEFAULT '2020-04-06 08:10:12', `Time` TIME NULL, `LocalDateTime` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP, `LocalDate` TIMESTAMP NULL, `LocalTime` TIME DEFAULT '12:24:46', `DoubleCol` DOUBLE NULL, `Decimal` DECIMAL NULL, `Bool` BOOL NULL)",
                        "CREATE TABLE `sys_forestnet_testddl2` (`Id` INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT, `DoubleCol` DOUBLE NULL)",
                        "ALTER TABLE `sys_forestnet_testddl` ADD `ShortText3` VARCHAR NULL, ADD `Text3` VARCHAR NULL",
                        "ALTER TABLE `sys_forestnet_testddl` ADD UNIQUE `new_index_Int` (`Int`)",
                        "ALTER TABLE `sys_forestnet_testddl` ADD UNIQUE `new_index_SmallInt_Bool` (`SmallInt`, `Bool`), DROP INDEX `new_index_Int`",
                        "ALTER TABLE `sys_forestnet_testddl` ADD INDEX `new_index_Text2` (`Text2`)",
                        "ALTER TABLE `sys_forestnet_testddl` DROP INDEX `new_index_Text2`",
                        "ALTER TABLE `sys_forestnet_testddl` CHANGE `Text2Changed` `Text2` VARCHAR NOT NULL DEFAULT 'Das ist das Haus vom Nikolaus'",
                        "INSERT INTO `sys_forestnet_testddl` (`sys_forestnet_testddl`.`UUID`, `sys_forestnet_testddl`.`ShortText`, `sys_forestnet_testddl`.`Text`, `sys_forestnet_testddl`.`SmallInt`, `sys_forestnet_testddl`.`Int`, `sys_forestnet_testddl`.`BigInt`, `sys_forestnet_testddl`.`DateTime`, `sys_forestnet_testddl`.`Date`, `sys_forestnet_testddl`.`Time`, `sys_forestnet_testddl`.`LocalDateTime`, `sys_forestnet_testddl`.`LocalDate`, `sys_forestnet_testddl`.`LocalTime`, `sys_forestnet_testddl`.`DoubleCol`, `sys_forestnet_testddl`.`Decimal`, `sys_forestnet_testddl`.`Bool`, `sys_forestnet_testddl`.`Text2Changed`, `sys_forestnet_testddl`.`ShortText2`) VALUES ('123e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 123, 1234567890, 1234567890123, '2003-12-15 08:33:03', '2009-06-29', '11:01:43', '2010-09-02 05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, true, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "INSERT INTO `sys_forestnet_testddl` (`sys_forestnet_testddl`.`UUID`, `sys_forestnet_testddl`.`ShortText`, `sys_forestnet_testddl`.`Text`, `sys_forestnet_testddl`.`SmallInt`, `sys_forestnet_testddl`.`Int`, `sys_forestnet_testddl`.`BigInt`, `sys_forestnet_testddl`.`DateTime`, `sys_forestnet_testddl`.`Date`, `sys_forestnet_testddl`.`Time`, `sys_forestnet_testddl`.`LocalDateTime`, `sys_forestnet_testddl`.`LocalDate`, `sys_forestnet_testddl`.`LocalTime`, `sys_forestnet_testddl`.`DoubleCol`, `sys_forestnet_testddl`.`Decimal`, `sys_forestnet_testddl`.`Bool`, `sys_forestnet_testddl`.`Text2Changed`, `sys_forestnet_testddl`.`ShortText2`) VALUES ('223e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 223, 1234567890, 2234567890123, '2003-12-15 08:33:03', '2009-06-29', '11:01:43', '2010-09-02 05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, false, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "INSERT INTO `sys_forestnet_testddl` (`sys_forestnet_testddl`.`UUID`, `sys_forestnet_testddl`.`ShortText`, `sys_forestnet_testddl`.`Text`, `sys_forestnet_testddl`.`SmallInt`, `sys_forestnet_testddl`.`Int`, `sys_forestnet_testddl`.`BigInt`, `sys_forestnet_testddl`.`DateTime`, `sys_forestnet_testddl`.`Date`, `sys_forestnet_testddl`.`Time`, `sys_forestnet_testddl`.`LocalDateTime`, `sys_forestnet_testddl`.`LocalDate`, `sys_forestnet_testddl`.`LocalTime`, `sys_forestnet_testddl`.`DoubleCol`, `sys_forestnet_testddl`.`Decimal`, `sys_forestnet_testddl`.`Bool`, `sys_forestnet_testddl`.`Text2Changed`, `sys_forestnet_testddl`.`ShortText2`) VALUES ('323e4567-e89b-42d3-a456-556642440000', 'a short text', 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.', 323, 1234567890, 3234567890123, '2003-12-15 08:33:03', '2009-06-29', '11:01:43', '2010-09-02 05:55:13', '2018-11-16', '17:42:23', 3.141592, 2.718281828, true, 'At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.', 'another short text')",
                        "SELECT `sys_forestnet_testddl`.`ShortText`, `sys_forestnet_testddl`.`LocalDate` AS 'Spalte C', `sys_forestnet_testddl`.`Int` FROM `sys_forestnet_testddl` WHERE `sys_forestnet_testddl`.`ShortText` <> 'Wert' OR `sys_forestnet_testddl`.`Id` >= 123 AND `sys_forestnet_testddl`.`SmallInt` > 25353 ORDER BY `sys_forestnet_testddl`.`Id` ASC, `sys_forestnet_testddl`.`ShortText` DESC LIMIT 0, 10",
                        "UPDATE `sys_forestnet_testddl` SET `sys_forestnet_testddl`.`ShortText` = 'Wert', `sys_forestnet_testddl`.`Int` = 1337, `sys_forestnet_testddl`.`DoubleCol` = 35.67, `sys_forestnet_testddl`.`DateTime` = '2003-12-15 08:33:03' WHERE `sys_forestnet_testddl`.`ShortText` <> 'Wert' OR `sys_forestnet_testddl`.`SmallInt` >= 123 AND `sys_forestnet_testddl`.`DateTime` >= '2003-12-15 08:33:03'",
                        "SELECT * FROM `sys_forestnet_testddl` WHERE `sys_forestnet_testddl`.`DateTime` <> '2003-12-15 08:33:03' OR `sys_forestnet_testddl`.`Date` >= '2009-06-29' AND `sys_forestnet_testddl`.`Time` > '11:01:43'",
                        "SELECT * FROM `sys_forestnet_testddl` WHERE `sys_forestnet_testddl`.`LocalDateTime` <> '2010-09-02 05:55:13' OR `sys_forestnet_testddl`.`LocalDate` >= '2018-11-16' AND `sys_forestnet_testddl`.`LocalTime` > '17:42:23'",
                        "SELECT * FROM `sys_forestnet_testddl`",
                        "DELETE FROM `sys_forestnet_testddl` WHERE `sys_forestnet_testddl`.`ShortText` <> 'Wert' OR `sys_forestnet_testddl`.`SmallInt` >= 32.45 AND `sys_forestnet_testddl`.`DateTime` > '2003-12-15 08:33:03'",
                        "ALTER TABLE `sys_forestnet_testddl` DROP `ShortText2`",
                        "ALTER TABLE `sys_forestnet_testddl` DROP `BigInt`, DROP `Int`",
                        "ALTER TABLE `sys_forestnet_testddl` DROP INDEX `new_index_SmallInt_Bool`",
                        "TRUNCATE TABLE `sys_forestnet_testddl`",
                        "DROP TABLE `sys_forestnet_testddl`",
                        "DROP TABLE `sys_forestnet_testddl2`",
                        "SELECT `sys_forestnet_products`.`SupplierID`, `sys_forestnet_products`.`CategoryID`, `sys_forestnet_categories`.`CategoryName`, `sys_forestnet_categories`.`Description`, `sys_forestnet_products`.`ProductID`, `sys_forestnet_products`.`ProductName`, `sys_forestnet_products`.`Unit`, `sys_forestnet_products`.`Price` FROM `sys_forestnet_products` INNER JOIN `sys_forestnet_categories` ON `sys_forestnet_products`.`CategoryID` = `sys_forestnet_categories`.`CategoryID` WHERE `sys_forestnet_products`.`ProductID` > 50 AND `sys_forestnet_categories`.`CategoryID` > 3 ORDER BY `sys_forestnet_products`.`SupplierID` ASC, `sys_forestnet_categories`.`CategoryName` ASC LIMIT 0, 50",
                        "SELECT `sys_forestnet_products`.`SupplierID`, COUNT(`sys_forestnet_products`.`ProductID`), `sys_forestnet_products`.`ProductName`, `sys_forestnet_products`.`Unit`, MAX(`sys_forestnet_products`.`Price`) FROM `sys_forestnet_products` WHERE `sys_forestnet_products`.`SupplierID` < 100 GROUP BY `sys_forestnet_products`.`SupplierID` HAVING MAX(`sys_forestnet_products`.`Price`) > 50.0 ORDER BY COUNT(`sys_forestnet_products`.`ProductID`) ASC, MAX(`sys_forestnet_products`.`Price`) ASC LIMIT 0, 50",
                        "SELECT `sys_forestnet_products`.`SupplierID`, `sys_forestnet_products`.`CategoryID`, `sys_forestnet_categories`.`CategoryName`, `sys_forestnet_categories`.`Description`, COUNT(`sys_forestnet_products`.`ProductID`), `sys_forestnet_products`.`ProductName`, `sys_forestnet_products`.`Unit`, MIN(`sys_forestnet_products`.`Price`) FROM `sys_forestnet_products` INNER JOIN `sys_forestnet_categories` ON `sys_forestnet_products`.`CategoryID` = `sys_forestnet_categories`.`CategoryID` WHERE `sys_forestnet_products`.`SupplierID` < 50 GROUP BY `sys_forestnet_products`.`SupplierID` HAVING MIN(`sys_forestnet_products`.`Price`) > 20.0 AND COUNT(`sys_forestnet_products`.`ProductID`) > 1 ORDER BY COUNT(`sys_forestnet_products`.`ProductID`) DESC, `sys_forestnet_products`.`SupplierID` ASC LIMIT 0, 50"
                    }
                };

                ForestNET.Lib.Global o_glob = ForestNET.Lib.Global.Instance;

                int i_amountQueries = 23;

                foreach (KeyValuePair<string, int> o_entry in a_baseGateways)
                {
                    o_glob.BaseGateway = Enum.Parse<BaseGateway>(o_entry.Key);

                    if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                    {
                        i_amountQueries = 26;
                    }
                    else
                    {
                        i_amountQueries = 23;
                    }

                    if (a_expectedQueries[a_baseGateways[o_entry.Key]].Length != i_amountQueries)
                    {
                        Assert.Fail("Amount of expected queries is not valid for basegateway '" + o_glob.BaseGateway + "': " + a_expectedQueries[a_baseGateways[o_entry.Key]].Length + " != " + i_amountQueries);
                    }

                    for (int i = 1; i <= i_amountQueries; i++)
                    {
                        if (ForestNET.Lib.Helper.IsStringEmpty(a_expectedQueries[a_baseGateways[o_entry.Key]][(i - 1)]))
                        {
                            continue;
                        }

                        List<KeyValuePair<string, Object>> a_values = [];
                        /* get query from generator method */
                        string s_testQuery = (TestQueryGenerator(i)?.ToString()) ?? "no query available";

                        /* extract values from sql statement */
                        s_testQuery = Query<Column>.ConvertToPreparedStatementQuery(o_glob.BaseGateway, s_testQuery, a_values, true);

                        /* create normal sql statement with values inserted */
                        s_testQuery = Query<Column>.ConvertPreparedStatementSqlQueryToStandard(s_testQuery, a_values);

                        /* get expected query */
                        string s_expectedQuery = a_expectedQueries[a_baseGateways[o_entry.Key]][(i - 1)];

                        /* within sqlite query generation, we use random string value for creating temp. tables for changing columns or constraints */
                        if ((o_glob.BaseGateway == BaseGateway.SQLITE) && ((i == 8) || (i == 18) || (i == 19)))
                        {
                            string s_random = s_testQuery.Substring(14, 30 - 14);
                            s_expectedQuery = s_expectedQuery.Replace("REPLACE_RANDOM", s_random);
                        }

                        Assert.That(
                            s_testQuery,
                            Is.EqualTo(s_expectedQuery),
                            "[" + o_glob.BaseGateway + "] - Query #" + (i) + " does not match expectation"
                        );
                    }
                }
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        public static IQuery? TestQueryGenerator(int p_i_queryNumber)
        {
            ForestNET.Lib.Global o_glob = ForestNET.Lib.Global.Instance;

            IQuery? o_queryReturn = null;

            DateTime o_dateTime = new(2003, 12, 15, 8, 33, 3);
            DateTime o_date = new(2009, 6, 29);
            TimeSpan o_time = new(11, 1, 43);

            DateTime o_localDateTime = new(2010, 9, 2, 5, 55, 13);
            DateTime o_localDate = new(2018, 11, 16);
            TimeSpan o_localTime = new(17, 42, 23);

            int i_number = 1;

            if (p_i_queryNumber == i_number++) // 1
            {
                /* #### ######  ############################################################################ */
                /* #### CREATE  ############################################################################ */
                /* #### ######  ############################################################################ */

                Query<Create> o_queryCreate = new(o_glob.BaseGateway, SqlType.CREATE, "sys_forestnet_testddl");

                /* #### Columns ############################################################################ */
                List<Dictionary<string, string>> a_columnsDefinition = TestGetColumnDefinitions(0);

                /* #### Query ############################################################################ */

                foreach (Dictionary<string, string> o_columnDefinition in a_columnsDefinition)
                {
                    ColumnStructure o_column = new(o_queryCreate);
                    o_column.ColumnTypeAllocation(o_columnDefinition["columnType"]);
                    o_column.Name = o_columnDefinition["name"];
                    o_column.AlterOperation = "ADD";

                    if (o_columnDefinition.TryGetValue("constraints", out string? s_value))
                    {
                        string[] a_constraints = s_value.Split(";");

                        for (int i = 0; i < a_constraints.Length; i++)
                        {
                            o_column.AddConstraint(o_queryCreate.ConstraintTypeAllocation(a_constraints[i]));

                            if ((a_constraints[i].CompareTo("DEFAULT") == 0) && (o_columnDefinition.TryGetValue("constraintDefaultValue", out string? s_bar)))
                            {
                                o_column.ConstraintDefaultValue = (Object)s_bar;
                            }
                        }
                    }

                    o_queryCreate.GetQuery<Create>()?.Columns.Add(o_column);
                }

                o_queryReturn = (IQuery?)o_queryCreate;
            }
            else if (p_i_queryNumber == i_number++) // 2
            {
                /* #### ######  ############################################################################ */
                /* #### CREATE  ############################################################################ */
                /* #### ######  ############################################################################ */

                Query<Create> o_queryCreate = new(o_glob.BaseGateway, SqlType.CREATE, "sys_forestnet_testddl2");

                /* #### Columns ############################################################################ */
                List<Dictionary<string, string>> a_columnsDefinition = [];

                Dictionary<string, string> o_columnDef = new()
                {
                    { "name", "Id" },
                    { "columnType", "integer [int]" },
                    { "constraints", "NOT NULL;PRIMARY KEY;AUTO_INCREMENT" }
                };
                a_columnsDefinition.Add(o_columnDef);

                o_columnDef = new()
                {
                    { "name", "DoubleCol" },
                    { "columnType", "double" },
                    { "constraints", "NULL" }
                };
                a_columnsDefinition.Add(o_columnDef);

                /* #### Query ############################################################################ */

                foreach (Dictionary<string, string> o_columnDefinition in a_columnsDefinition)
                {
                    ColumnStructure o_column = new(o_queryCreate);
                    o_column.ColumnTypeAllocation(o_columnDefinition["columnType"]);
                    o_column.Name = o_columnDefinition["name"];
                    o_column.AlterOperation = "ADD";

                    if (o_columnDefinition.TryGetValue("constraints", out string? s_value))
                    {
                        string[] a_constraints = s_value.Split(";");

                        for (int i = 0; i < a_constraints.Length; i++)
                        {
                            o_column.AddConstraint(o_queryCreate.ConstraintTypeAllocation(a_constraints[i]));

                            if ((a_constraints[i].CompareTo("DEFAULT") == 0) && (o_columnDefinition.TryGetValue("constraintDefaultValue", out string? s_bar)))
                            {
                                o_column.ConstraintDefaultValue = (Object)s_bar;
                            }
                        }
                    }

                    o_queryCreate.GetQuery<Create>()?.Columns.Add(o_column);
                }

                o_queryReturn = (IQuery?)o_queryCreate;
            }
            else if (p_i_queryNumber == i_number++) // 3
            {
                /* #### ####### ############################################################################ */
                /* #### ALTER 1 ############################################################################ */
                /* #### ####### ############################################################################ */

                Query<Alter> o_queryAlter = new(o_glob.BaseGateway, SqlType.ALTER, "sys_forestnet_testddl");

                /* #### Columns ############################################################################ */
                List<Dictionary<string, string>> a_columnsDefinition = [];

                Dictionary<string, string> o_columnDef = new()
                {
                    { "name", "Text2" },
                    { "columnType", "text [36]" },
                    { "constraints", "NULL" }
                };

                /* other query for nosqlmdb */
                if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                {
                    o_columnDef["name"] = "ShortText3";
                }

                a_columnsDefinition.Add(o_columnDef);

                o_columnDef = new()
                {
                    { "name", "ShortText2" },
                    { "columnType", "text [255]" },
                    { "constraints", "NULL" }
                };

                /* other query for nosqlmdb */
                if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                {
                    o_columnDef["name"] = "Text3";
                }

                a_columnsDefinition.Add(o_columnDef);

                /* #### Query ############################################################################ */

                foreach (Dictionary<string, string> o_columnDefinition in a_columnsDefinition)
                {
                    ColumnStructure o_column = new(o_queryAlter);
                    o_column.ColumnTypeAllocation(o_columnDefinition["columnType"]);
                    o_column.Name = o_columnDefinition["name"];
                    o_column.AlterOperation = "ADD";

                    if (o_columnDefinition.TryGetValue("constraints", out string? s_value))
                    {
                        string[] a_constraints = s_value.Split(";");

                        for (int i = 0; i < a_constraints.Length; i++)
                        {
                            o_column.AddConstraint(o_queryAlter.ConstraintTypeAllocation(a_constraints[i]));

                            if ((a_constraints[i].CompareTo("DEFAULT") == 0) && (o_columnDefinition.TryGetValue("constraintDefaultValue", out string? s_bar)))
                            {
                                o_column.ConstraintDefaultValue = (Object)s_bar;
                            }
                        }
                    }

                    o_queryAlter.GetQuery<Alter>()?.Columns.Add(o_column);
                }

                o_queryReturn = (IQuery?)o_queryAlter;
            }
            else if (p_i_queryNumber == i_number++) // 4
            {
                /* #### ####### ############################################################################ */
                /* #### ALTER 2 ############################################################################ */
                /* #### ####### ############################################################################ */

                Query<Alter> o_queryAlter = new(o_glob.BaseGateway, SqlType.ALTER, "sys_forestnet_testddl");

                /* #### Constraints ############################################################################ */
                Constraint o_constraint = new(o_queryAlter, "UNIQUE", "new_index_Int", "", "ADD");
                o_constraint.Columns.Add("Int");

                o_queryAlter.GetQuery<Alter>()?.Constraints.Add(o_constraint);

                o_queryReturn = (IQuery?)o_queryAlter;
            }
            else if (p_i_queryNumber == i_number++) // 5
            {
                /* #### ####### ############################################################################ */
                /* #### ALTER 3 ############################################################################ */
                /* #### ####### ############################################################################ */

                Query<Alter> o_queryAlter = new(o_glob.BaseGateway, SqlType.ALTER, "sys_forestnet_testddl");

                /* #### Constraints ############################################################################ */
                Constraint o_constraint = new(o_queryAlter, "UNIQUE", "new_index_Int", "new_index_SmallInt_Bool", "CHANGE");
                o_constraint.Columns.Add("SmallInt");
                o_constraint.Columns.Add("Bool");

                o_queryAlter.GetQuery<Alter>()?.Constraints.Add(o_constraint);

                o_queryReturn = (IQuery?)o_queryAlter;
            }
            else if (p_i_queryNumber == i_number++) // 6
            {
                /* #### ####### ############################################################################ */
                /* #### ALTER 4 ############################################################################ */
                /* #### ####### ############################################################################ */

                Query<Alter> o_queryAlter = new(o_glob.BaseGateway, SqlType.ALTER, "sys_forestnet_testddl");

                /* #### Constraints ############################################################################ */
                Constraint o_constraint = new(o_queryAlter, "INDEX", "new_index_Text2", "", "ADD");
                o_constraint.Columns.Add("Text2");

                o_queryAlter.GetQuery<Alter>()?.Constraints.Add(o_constraint);

                o_queryReturn = (IQuery?)o_queryAlter;
            }
            else if (p_i_queryNumber == i_number++) // 7
            {
                /* #### ####### ############################################################################ */
                /* #### ALTER 5 ############################################################################ */
                /* #### ####### ############################################################################ */

                Query<Alter> o_queryAlter = new(o_glob.BaseGateway, SqlType.ALTER, "sys_forestnet_testddl");

                /* #### Constraints ############################################################################ */
                Constraint o_constraint = new(o_queryAlter, "INDEX", "new_index_Text2", "", "DROP");

                o_queryAlter.GetQuery<Alter>()?.Constraints.Add(o_constraint);

                o_queryReturn = (IQuery?)o_queryAlter;
            }
            else if (p_i_queryNumber == i_number++) // 8
            {
                /* #### ####### ############################################################################ */
                /* #### ALTER 6 ############################################################################ */
                /* #### ####### ############################################################################ */

                Query<Alter> o_queryAlter = new(o_glob.BaseGateway, SqlType.ALTER, "sys_forestnet_testddl");

                /* #### Columns To Change ############################################################################ */
                List<Dictionary<string, string>> a_columnsDefinition = [];

                Dictionary<string, string> o_columnDef = new()
                {
                    { "name", "Text2" },
                    { "columnType", "text [255]" },
                    { "constraints", "NOT NULL;DEFAULT" },
                    { "constraintDefaultValue", "Das ist das Haus vom Nikolaus" },
                    { "newName", "Text2Changed" }
                };

                /* other query for nosqlmdb */
                if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                {
                    o_columnDef["name"] = "Text2Changed";
                    o_columnDef["newName"] = "Text2";
                }

                a_columnsDefinition.Add(o_columnDef);

                /* #### Columns ############################################################################ */
                List<Dictionary<string, string>> a_columnsDefinitionSqLite = TestGetColumnDefinitions(1);

                /* #### Query ############################################################################ */

                foreach (Dictionary<string, string> o_columnDefinition in a_columnsDefinition)
                {
                    ColumnStructure o_column = new(o_queryAlter);
                    o_column.ColumnTypeAllocation(o_columnDefinition["columnType"]);
                    o_column.Name = o_columnDefinition["name"];
                    o_column.AlterOperation = "CHANGE";

                    if (o_columnDefinition.TryGetValue("newName", out string? s_baz))
                    {
                        o_column.NewName = s_baz;
                    }

                    if (o_columnDefinition.TryGetValue("constraints", out string? s_value))
                    {
                        string[] a_constraints = s_value.Split(";");

                        for (int i = 0; i < a_constraints.Length; i++)
                        {
                            o_column.AddConstraint(o_queryAlter.ConstraintTypeAllocation(a_constraints[i]));

                            if ((a_constraints[i].CompareTo("DEFAULT") == 0) && (o_columnDefinition.TryGetValue("constraintDefaultValue", out string? s_bar)))
                            {
                                o_column.ConstraintDefaultValue = (Object)s_bar;
                            }
                        }
                    }

                    o_queryAlter.GetQuery<Alter>()?.Columns.Add(o_column);
                }

                /* only for sqlite */
                (o_queryAlter.GetQuery<Alter>() ?? throw new NullReferenceException("alter sql query is null")).SQLiteColumnsDefinition = a_columnsDefinitionSqLite;

                o_queryReturn = (IQuery?)o_queryAlter;
            }
            else if (p_i_queryNumber == i_number++) // 9
            {
                /* #### ######  ############################################################################ */
                /* #### INSERT  ############################################################################ */
                /* #### ######  ############################################################################ */

                Query<Insert> o_queryInsert = new(o_glob.BaseGateway, SqlType.INSERT, "sys_forestnet_testddl");
                /* #### Columns ############################################################################ */

                (o_queryInsert.GetQuery<Insert>() ?? throw new NullReferenceException("insert query is null")).NoSQLMDBColumnAutoIncrement = new Column(o_queryInsert, "Id");

                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "UUID"), "123e4567-e89b-42d3-a456-556642440000"));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "ShortText"), "a short text"));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Text"), "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua."));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "SmallInt"), (short)123));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Int"), 1_234_567_890));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "BigInt"), 1234567890123L));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "DateTime"), o_dateTime));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Date"), o_date));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Time"), o_time));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "LocalDateTime"), o_localDateTime));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "LocalDate"), o_localDate));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "LocalTime"), o_localTime));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "DoubleCol"), 3.141592d));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Decimal"), 2.718281828m));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Bool"), true));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Text2Changed"), "At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet."));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "ShortText2"), "another short text"));

                o_queryReturn = (IQuery?)o_queryInsert;
            }
            else if (p_i_queryNumber == i_number++) // 10
            {
                /* #### ######  ############################################################################ */
                /* #### INSERT  ############################################################################ */
                /* #### ######  ############################################################################ */

                Query<Insert> o_queryInsert = new(o_glob.BaseGateway, SqlType.INSERT, "sys_forestnet_testddl");
                /* #### Columns ############################################################################ */

                (o_queryInsert.GetQuery<Insert>() ?? throw new NullReferenceException("insert query is null")).NoSQLMDBColumnAutoIncrement = new Column(o_queryInsert, "Id");

                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "UUID"), "223e4567-e89b-42d3-a456-556642440000"));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "ShortText"), "a short text"));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Text"), "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua."));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "SmallInt"), (short)223));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Int"), 1_234_567_890));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "BigInt"), 2234567890123L));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "DateTime"), o_dateTime));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Date"), o_date));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Time"), o_time));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "LocalDateTime"), o_localDateTime));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "LocalDate"), o_localDate));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "LocalTime"), o_localTime));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "DoubleCol"), 3.141592d));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Decimal"), 2.718281828m));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Bool"), false));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Text2Changed"), "At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet."));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "ShortText2"), "another short text"));

                o_queryReturn = (IQuery?)o_queryInsert;
            }
            else if (p_i_queryNumber == i_number++) // 11
            {
                /* #### ######  ############################################################################ */
                /* #### INSERT  ############################################################################ */
                /* #### ######  ############################################################################ */

                Query<Insert> o_queryInsert = new(o_glob.BaseGateway, SqlType.INSERT, "sys_forestnet_testddl");
                /* #### Columns ############################################################################ */

                (o_queryInsert.GetQuery<Insert>() ?? throw new NullReferenceException("insert query is null")).NoSQLMDBColumnAutoIncrement = new Column(o_queryInsert, "Id");

                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "UUID"), "323e4567-e89b-42d3-a456-556642440000"));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "ShortText"), "a short text"));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Text"), "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua."));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "SmallInt"), (short)323));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Int"), 1_234_567_890));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "BigInt"), 3234567890123L));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "DateTime"), o_dateTime));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Date"), o_date));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Time"), o_time));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "LocalDateTime"), o_localDateTime));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "LocalDate"), o_localDate));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "LocalTime"), o_localTime));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "DoubleCol"), 3.141592d));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Decimal"), 2.718281828m));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Bool"), true));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "Text2Changed"), "At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet."));
                o_queryInsert.GetQuery<Insert>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryInsert, "ShortText2"), "another short text"));

                o_queryReturn = (IQuery?)o_queryInsert;
            }
            else if (p_i_queryNumber == i_number++) // 12
            {
                /* #### ######  ############################################################################ */
                /* #### SELECT  ############################################################################ */
                /* #### ######  ############################################################################ */

                Query<Select> o_querySelect = new(o_glob.BaseGateway, SqlType.SELECT, "sys_forestnet_testddl");

                if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                {
                    /* #### Columns ############################################################################ */
                    Column column_A = new(o_querySelect, "ShortText");
                    Column column_B = new(o_querySelect, "LocalDate", "Spalte C");
                    Column column_C = new(o_querySelect, "Int");
                    Column column_D = new(o_querySelect, "Id");

                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_A);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_B);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_C);
                    /* ##### Where ########################################################################### */
                    o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_A, "Wert", "<>"));
                    o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_D, 123, ">=", "OR"));
                    o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, new Column(o_querySelect, "SmallInt"), (short)25353, ">", "AND"));
                    /* #### OrderBy ############################################################################ */
                    (o_querySelect.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).OrderBy = new(o_querySelect, [column_D, column_A], [true, false]);
                    /* #### Limit ############################################################################ */
                    (o_querySelect.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).Limit = new Limit(o_querySelect, 0, 10);
                }
                else
                {
                    /* #### Columns ############################################################################ */
                    Column column_A = new(o_querySelect, "ShortText");
                    Column column_B = new(o_querySelect, "SmallInt", "", "MIN");
                    Column column_C = new(o_querySelect, "LocalDate", "Spalte C");
                    Column column_D = new(o_querySelect, "Int");
                    Column column_E = new(o_querySelect, "Id")
                    {
                        Table = "sys_forestnet_testddl2"
                    };

                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_A);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_B);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_C);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_D);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_E);
                    /* #### Joins ############################################################################ */
                    Join join_A = new(o_querySelect, "INNER JOIN")
                    {
                        Table = "sys_forestnet_testddl2"
                    };

                    Column column_F = new(o_querySelect, "Id")
                    {
                        Table = join_A.Table
                    };

                    Column column_G = new(o_querySelect, "DoubleCol")
                    {
                        Table = join_A.Table
                    };

                    join_A.Relations.Add(new Relation(o_querySelect, column_F, new Column(o_querySelect, "Id"), "=", "", true));
                    join_A.Relations.Add(new Relation(o_querySelect, column_G, new Column(o_querySelect, "DoubleCol"), "<=", "AND", false, true));

                    o_querySelect.GetQuery<Select>()?.Joins.Add(join_A);
                    /* ##### Where ########################################################################### */

                    o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_A, "Wert", "<>"));
                    o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_F, 123, ">=", "OR"));
                    o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, new Column(o_querySelect, "SmallInt"), (short)25353, ">", "AND"));
                    /* #### GroupBy ############################################################################ */
                    o_querySelect.GetQuery<Select>()?.GroupBy.Add(column_A);
                    o_querySelect.GetQuery<Select>()?.GroupBy.Add(column_C);
                    o_querySelect.GetQuery<Select>()?.GroupBy.Add(column_D);
                    o_querySelect.GetQuery<Select>()?.GroupBy.Add(column_E);
                    /* #### Having ############################################################################ */
                    o_querySelect.GetQuery<Select>()?.Having.Add(new Where(o_querySelect, column_D, 456.0f, "<=", "", true));
                    o_querySelect.GetQuery<Select>()?.Having.Add(new Where(o_querySelect, column_A, "Trew", "=", "AND"));
                    o_querySelect.GetQuery<Select>()?.Having.Add(new Where(o_querySelect, column_C, o_localDate, "<>", "AND", false, true));
                    /* #### OrderBy ############################################################################ */
                    (o_querySelect.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).OrderBy = new OrderBy(o_querySelect, [column_F, column_A], [true, false]);
                    /* #### Limit ############################################################################ */
                    (o_querySelect.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).Limit = new Limit(o_querySelect, 0, 10);
                }

                o_queryReturn = (IQuery?)o_querySelect;
            }
            else if (p_i_queryNumber == i_number++) // 13
            {
                /* #### ######  ############################################################################ */
                /* #### UPDATE  ############################################################################ */
                /* #### ######  ############################################################################ */

                Query<Update> o_queryUpdate = new(o_glob.BaseGateway, SqlType.UPDATE, "sys_forestnet_testddl");

                /* #### Columns ############################################################################ */
                o_queryUpdate.GetQuery<Update>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryUpdate, "ShortText"), "Wert"));
                o_queryUpdate.GetQuery<Update>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryUpdate, "Int"), 1337));
                o_queryUpdate.GetQuery<Update>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryUpdate, "DoubleCol"), 35.67f));
                o_queryUpdate.GetQuery<Update>()?.ColumnValues.Add(new ColumnValue(new Column(o_queryUpdate, "DateTime"), o_dateTime));

                /* ##### Where ########################################################################### */
                o_queryUpdate.GetQuery<Update>()?.Where.Add(new Where(o_queryUpdate, new Column(o_queryUpdate, "ShortText"), "Wert", "<>"));
                o_queryUpdate.GetQuery<Update>()?.Where.Add(new Where(o_queryUpdate, new Column(o_queryUpdate, "SmallInt"), 123, ">=", "OR"));
                o_queryUpdate.GetQuery<Update>()?.Where.Add(new Where(o_queryUpdate, new Column(o_queryUpdate, "DateTime"), o_dateTime, ">=", "AND"));

                o_queryReturn = (IQuery?)o_queryUpdate;
            }
            else if (p_i_queryNumber == i_number++) // 14
            {
                /* #### ######  ############################################################################ */
                /* #### SELECT  ############################################################################ */
                /* #### ######  ############################################################################ */

                Query<Select> o_querySelect = new(o_glob.BaseGateway, SqlType.SELECT, "sys_forestnet_testddl");

                /* #### Columns ############################################################################ */
                o_querySelect.GetQuery<Select>()?.Columns.Add(new Column(o_querySelect, "*"));

                /* ##### Where ########################################################################### */
                Column column_A = new(o_querySelect, "DateTime");
                Column column_B = new(o_querySelect, "Date");
                Column column_C = new(o_querySelect, "Time");

                o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_A, o_dateTime, "<>"));
                o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_B, o_date, ">=", "OR"));
                o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_C, o_time, ">", "AND"));

                o_queryReturn = (IQuery?)o_querySelect;
            }
            else if (p_i_queryNumber == i_number++) // 15
            {
                /* #### ######  ############################################################################ */
                /* #### SELECT  ############################################################################ */
                /* #### ######  ############################################################################ */

                Query<Select> o_querySelect = new(o_glob.BaseGateway, SqlType.SELECT, "sys_forestnet_testddl");

                /* #### Columns ############################################################################ */
                o_querySelect.GetQuery<Select>()?.Columns.Add(new Column(o_querySelect, "*"));

                /* ##### Where ########################################################################### */
                Column column_A = new(o_querySelect, "LocalDateTime");
                Column column_B = new(o_querySelect, "LocalDate");
                Column column_C = new(o_querySelect, "LocalTime");

                o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_A, o_localDateTime, "<>"));
                o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_B, o_localDate, ">=", "OR"));
                o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_C, o_localTime, ">", "AND"));

                o_queryReturn = (IQuery?)o_querySelect;
            }
            else if (p_i_queryNumber == i_number++) // 16
            {
                /* #### ######  ############################################################################ */
                /* #### SELECT  ############################################################################ */
                /* #### ######  ############################################################################ */

                Query<Select> o_querySelect = new(o_glob.BaseGateway, SqlType.SELECT, "sys_forestnet_testddl");

                /* #### Columns ############################################################################ */
                o_querySelect.GetQuery<Select>()?.Columns.Add(new Column(o_querySelect, "*"));

                o_queryReturn = (IQuery?)o_querySelect;
            }
            else if (p_i_queryNumber == i_number++) // 17
            {
                /* #### ######  ############################################################################ */
                /* #### DELETE  ############################################################################ */
                /* #### ######  ############################################################################ */

                Query<Delete> o_queryDelete = new(o_glob.BaseGateway, SqlType.DELETE, "sys_forestnet_testddl");

                /* ##### Where ########################################################################### */
                o_queryDelete.GetQuery<Delete>()?.Where.Add(new Where(o_queryDelete, new Column(o_queryDelete, "ShortText"), "Wert", "<>"));
                o_queryDelete.GetQuery<Delete>()?.Where.Add(new Where(o_queryDelete, new Column(o_queryDelete, "SmallInt"), 32.45f, ">=", "OR"));
                o_queryDelete.GetQuery<Delete>()?.Where.Add(new Where(o_queryDelete, new Column(o_queryDelete, "DateTime"), o_dateTime, ">", "AND"));

                o_queryReturn = (IQuery?)o_queryDelete;
            }
            else if (p_i_queryNumber == i_number++) // 18
            {
                /* #### ####### ############################################################################ */
                /* #### ALTER 7 ############################################################################ */
                /* #### ####### ############################################################################ */

                Query<Alter> o_queryAlter = new(o_glob.BaseGateway, SqlType.ALTER, "sys_forestnet_testddl");

                /* #### Query ############################################################################ */
                ColumnStructure o_column = new(o_queryAlter);
                o_column.ColumnTypeAllocation("text [255]");
                o_column.Name = "ShortText2";
                o_column.AlterOperation = "DROP";

                o_queryAlter.GetQuery<Alter>()?.Columns.Add(o_column);

                /* #### Columns ############################################################################ */
                List<Dictionary<string, string>> a_columnsDefinitionSqLite = TestGetColumnDefinitions(2);

                /* only for sqlite */
                (o_queryAlter.GetQuery<Alter>() ?? throw new NullReferenceException("alter query is null")).SQLiteColumnsDefinition = a_columnsDefinitionSqLite;

                /* #### Indexes ############################################################################ */
                List<Dictionary<string, string>> a_indexesDefinitionSqLite = [];

                Dictionary<string, string> o_columnDef = new()
                {
                    { "name", "new_index_BigInt_Bool" },
                    { "columns", "BigInt;Bool" },
                    { "unique", "1" }
                };
                a_indexesDefinitionSqLite.Add(o_columnDef);

                /* only for sqlite */
                (o_queryAlter.GetQuery<Alter>() ?? throw new NullReferenceException("alter query is null")).SQLiteIndexes = a_indexesDefinitionSqLite;

                o_queryReturn = (IQuery?)o_queryAlter;
            }
            else if (p_i_queryNumber == i_number++) // 19
            {
                /* #### ####### ############################################################################ */
                /* #### ALTER 8 ############################################################################ */
                /* #### ####### ############################################################################ */

                Query<Alter> o_queryAlter = new(o_glob.BaseGateway, SqlType.ALTER, "sys_forestnet_testddl");

                /* #### Query ############################################################################ */
                ColumnStructure o_column = new(o_queryAlter);
                o_column.ColumnTypeAllocation("integer [big]");
                o_column.Name = "BigInt";
                o_column.AlterOperation = "DROP";

                o_queryAlter.GetQuery<Alter>()?.Columns.Add(o_column);

                o_column = new(o_queryAlter);
                o_column.ColumnTypeAllocation("integer [int]");
                o_column.Name = "Int";
                o_column.AlterOperation = "DROP";

                o_queryAlter.GetQuery<Alter>()?.Columns.Add(o_column);

                /* #### Columns ############################################################################ */
                List<Dictionary<string, string>> a_columnsDefinitionSqLite = TestGetColumnDefinitions(3);

                /* only for sqlite */
                (o_queryAlter.GetQuery<Alter>() ?? throw new NullReferenceException("alter query is null")).SQLiteColumnsDefinition = a_columnsDefinitionSqLite;

                /* #### Indexes ############################################################################ */
                List<Dictionary<string, string>> a_indexesDefinitionSqLite = [];

                Dictionary<string, string> o_columnDef = new()
                {
                    { "name", "new_index_SmallInt_Bool" },
                    { "columns", "SmallInt;Bool" },
                    { "unique", "1" }
                };
                a_indexesDefinitionSqLite.Add(o_columnDef);

                /* only for sqlite */
                (o_queryAlter.GetQuery<Alter>() ?? throw new NullReferenceException("alter query is null")).SQLiteIndexes = a_indexesDefinitionSqLite;

                o_queryReturn = (IQuery?)o_queryAlter;
            }
            else if (p_i_queryNumber == i_number++) // 20
            {
                /* #### ####### ############################################################################ */
                /* #### ALTER 9 ############################################################################ */
                /* #### ####### ############################################################################ */

                Query<Alter> o_queryAlter = new(o_glob.BaseGateway, SqlType.ALTER, "sys_forestnet_testddl");

                /* #### Constraints ############################################################################ */
                Constraint o_constraint = new(o_queryAlter, "UNIQUE", "new_index_SmallInt_Bool", "", "DROP");

                o_queryAlter.GetQuery<Alter>()?.Constraints.Add(o_constraint);

                o_queryReturn = (IQuery?)o_queryAlter;
            }
            else if (p_i_queryNumber == i_number++) // 21
            {
                /* #### ######## ############################################################################ */
                /* #### TRUNCATE ############################################################################ */
                /* #### ######## ############################################################################ */

                Query<Truncate> o_queryTruncate = new(o_glob.BaseGateway, SqlType.TRUNCATE, "sys_forestnet_testddl");
                o_queryReturn = (IQuery?)o_queryTruncate;
            }
            else if (p_i_queryNumber == i_number++) // 22
            {
                /* #### #### ############################################################################ */
                /* #### DROP ############################################################################ */
                /* #### #### ############################################################################ */

                Query<Drop> o_queryDrop = new(o_glob.BaseGateway, SqlType.DROP, "sys_forestnet_testddl");
                o_queryReturn = (IQuery?)o_queryDrop;
            }
            else if (p_i_queryNumber == i_number++) // 23
            {
                /* #### #### ############################################################################ */
                /* #### DROP ############################################################################ */
                /* #### #### ############################################################################ */

                Query<Drop> o_queryDrop = new(o_glob.BaseGateway, SqlType.DROP, "sys_forestnet_testddl2");
                o_queryReturn = (IQuery?)o_queryDrop;
            }
            else if (p_i_queryNumber == i_number++) // 24
            {
                if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                {
                    /* #### ######  ############################################################################ */
                    /* #### SELECT  ############################################################################ */
                    /* #### ######  ############################################################################ */

                    Query<Select> o_querySelect = new(o_glob.BaseGateway, SqlType.SELECT, "sys_forestnet_products");

                    /* #### Columns ############################################################################ */
                    Column column_A = new(o_querySelect, "SupplierID");
                    Column column_B = new(o_querySelect, "CategoryID");
                    Column column_C = new(o_querySelect, "CategoryName")
                    {
                        Table = "sys_forestnet_categories"
                    };
                    Column column_D = new(o_querySelect, "Description")
                    {
                        Table = "sys_forestnet_categories"
                    };
                    Column column_E = new(o_querySelect, "ProductID");
                    Column column_F = new(o_querySelect, "ProductName");
                    Column column_G = new(o_querySelect, "Unit");
                    Column column_H = new(o_querySelect, "Price");

                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_A);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_B);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_C);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_D);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_E);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_F);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_G);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_H);
                    /* #### Joins ############################################################################ */
                    Join join_A = new(o_querySelect, "INNER JOIN")
                    {
                        Table = "sys_forestnet_categories"
                    };

                    Column column_I = new(o_querySelect, "CategoryID")
                    {
                        Table = join_A.Table
                    };

                    join_A.Relations.Add(new Relation(o_querySelect, column_B, column_I, "="));

                    o_querySelect.GetQuery<Select>()?.Joins.Add(join_A);
                    /* ##### Where ########################################################################### */
                    o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_E, 50, ">"));
                    o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_I, 3, ">", "AND"));
                    /* #### OrderBy ############################################################################ */
                    (o_querySelect.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).OrderBy = new OrderBy(o_querySelect, [column_A, column_C], [true, true]);
                    /* #### Limit ############################################################################ */
                    (o_querySelect.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).Limit = new Limit(o_querySelect, 0, 50);

                    o_queryReturn = (IQuery?)o_querySelect;
                }
            }
            else if (p_i_queryNumber == i_number++) // 25
            {
                if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                {
                    /* #### ######  ############################################################################ */
                    /* #### SELECT  ############################################################################ */
                    /* #### ######  ############################################################################ */

                    Query<Select> o_querySelect = new(o_glob.BaseGateway, SqlType.SELECT, "sys_forestnet_products");

                    /* #### Columns ############################################################################ */
                    Column column_A = new(o_querySelect, "SupplierID");
                    Column column_B = new(o_querySelect, "ProductID", "", "COUNT");
                    Column column_C = new(o_querySelect, "ProductName");
                    Column column_D = new(o_querySelect, "Unit");
                    Column column_E = new(o_querySelect, "Price", "", "MAX");

                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_A);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_B);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_C);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_D);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_E);
                    /* ##### Where ########################################################################### */
                    o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_A, 100, "<"));
                    /* #### GroupBy ############################################################################ */
                    o_querySelect.GetQuery<Select>()?.GroupBy.Add(column_A);
                    /* #### Having ############################################################################ */
                    o_querySelect.GetQuery<Select>()?.Having.Add(new Where(o_querySelect, column_E, 50.0d, ">"));
                    /* #### OrderBy ############################################################################ */
                    (o_querySelect.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).OrderBy = new OrderBy(o_querySelect, [column_B, column_E], [true, true]);
                    /* #### Limit ############################################################################ */
                    (o_querySelect.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).Limit = new Limit(o_querySelect, 0, 50);

                    o_queryReturn = (IQuery?)o_querySelect;
                }
            }
            else if (p_i_queryNumber == i_number++) // 26
            {
                if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
                {
                    /* #### ######  ############################################################################ */
                    /* #### SELECT  ############################################################################ */
                    /* #### ######  ############################################################################ */

                    Query<Select> o_querySelect = new(o_glob.BaseGateway, SqlType.SELECT, "sys_forestnet_products");

                    /* #### Columns ############################################################################ */
                    Column column_A = new(o_querySelect, "SupplierID");
                    Column column_B = new(o_querySelect, "CategoryID");
                    Column column_C = new(o_querySelect, "CategoryName")
                    {
                        Table = "sys_forestnet_categories"
                    };
                    Column column_D = new(o_querySelect, "Description")
                    {
                        Table = "sys_forestnet_categories"
                    };
                    Column column_E = new(o_querySelect, "ProductID", "", "COUNT");
                    Column column_F = new(o_querySelect, "ProductName");
                    Column column_G = new(o_querySelect, "Unit");
                    Column column_H = new(o_querySelect, "Price", "", "MIN");

                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_A);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_B);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_C);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_D);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_E);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_F);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_G);
                    o_querySelect.GetQuery<Select>()?.Columns.Add(column_H);
                    /* #### Joins ############################################################################ */
                    Join join_A = new(o_querySelect, "INNER JOIN")
                    {
                        Table = "sys_forestnet_categories"
                    };

                    Column column_I = new(o_querySelect, "CategoryID")
                    {
                        Table = join_A.Table
                    };

                    join_A.Relations.Add(new Relation(o_querySelect, column_B, column_I, "="));

                    o_querySelect.GetQuery<Select>()?.Joins.Add(join_A);
                    /* ##### Where ########################################################################### */
                    o_querySelect.GetQuery<Select>()?.Where.Add(new Where(o_querySelect, column_A, 50, "<"));
                    /* #### GroupBy ############################################################################ */
                    o_querySelect.GetQuery<Select>()?.GroupBy.Add(column_A);
                    /* #### Having ############################################################################ */
                    o_querySelect.GetQuery<Select>()?.Having.Add(new Where(o_querySelect, column_H, 20.0d, ">"));
                    o_querySelect.GetQuery<Select>()?.Having.Add(new Where(o_querySelect, column_E, 1, ">", "AND"));
                    /* #### OrderBy ############################################################################ */
                    (o_querySelect.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).OrderBy = new OrderBy(o_querySelect, [column_E, column_A], [false, true]);
                    /* #### Limit ############################################################################ */
                    (o_querySelect.GetQuery<Select>() ?? throw new NullReferenceException("select query is null")).Limit = new Limit(o_querySelect, 0, 50);

                    o_queryReturn = (IQuery?)o_querySelect;
                }
            }

            return o_queryReturn;
        }

        private static List<Dictionary<string, string>> TestGetColumnDefinitions(int p_i_columnDefinitionsNumber)
        {
            List<Dictionary<string, string>> a_columnsDefinition = [];

            Dictionary<string, string> o_properties = new()
            {
                { "name", "Id" },
                { "columnType", "integer [int]" },
                { "constraints", "NOT NULL;PRIMARY KEY;AUTO_INCREMENT" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "UUID" },
                { "columnType", "text [36]" },
                { "constraints", "NOT NULL;UNIQUE" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "ShortText" },
                { "columnType", "text [255]" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Text" },
                { "columnType", "text" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "SmallInt" },
                { "columnType", "integer [small]" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Int" },
                { "columnType", "integer [int]" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "BigInt" },
                { "columnType", "integer [big]" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "DateTime" },
                { "columnType", "datetime" },
                { "constraints", "NULL;DEFAULT" },
                { "constraintDefaultValue", "CURRENT_TIMESTAMP" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Date" },
                { "columnType", "datetime" },
                { "constraints", "NOT NULL;DEFAULT" },
                { "constraintDefaultValue", "2020-04-06 08:10:12" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Time" },
                { "columnType", "time" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "LocalDateTime" },
                { "columnType", "datetime" },
                { "constraints", "NULL;DEFAULT" },
                { "constraintDefaultValue", "CURRENT_TIMESTAMP" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "LocalDate" },
                { "columnType", "datetime" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "LocalTime" },
                { "columnType", "time" },
                { "constraints", "DEFAULT" },
                { "constraintDefaultValue", "12:24:46" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "DoubleCol" },
                { "columnType", "double" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Decimal" },
                { "columnType", "decimal" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            o_properties = new()
            {
                { "name", "Bool" },
                { "columnType", "bool" },
                { "constraints", "NULL" }
            };
            a_columnsDefinition.Add(o_properties);

            if (p_i_columnDefinitionsNumber == 1)
            {
                o_properties = new()
                {
                    { "name", "Text2" },
                    { "columnType", "text [36]" },
                    { "constraints", "NULL;DEFAULT" },
                    { "constraintDefaultValue", "Das ist das Haus vom Nikolaus" }
                };
                a_columnsDefinition.Add(o_properties);

                o_properties = new()
                {
                    { "name", "ShortText2" },
                    { "columnType", "text [255]" },
                    { "constraints", "NULL" }
                };
                a_columnsDefinition.Add(o_properties);
            }
            else if (p_i_columnDefinitionsNumber == 2)
            {
                o_properties = new()
                {
                    { "name", "Text2Changed" },
                    { "columnType", "text [36]" },
                    { "constraints", "NULL;DEFAULT" },
                    { "constraintDefaultValue", "Das ist das Haus vom Nikolaus" }
                };
                a_columnsDefinition.Add(o_properties);

                o_properties = new()
                {
                    { "name", "ShortText2" },
                    { "columnType", "text [255]" },
                    { "constraints", "NULL" }
                };
                a_columnsDefinition.Add(o_properties);
            }
            else if (p_i_columnDefinitionsNumber == 3)
            {
                o_properties = new()
                {
                    { "name", "Text2Changed" },
                    { "columnType", "text [36]" },
                    { "constraints", "NULL;DEFAULT" },
                    { "constraintDefaultValue", "Das ist das Haus vom Nikolaus" }
                };
                a_columnsDefinition.Add(o_properties);
            }

            return a_columnsDefinition;
        }
    }
}
