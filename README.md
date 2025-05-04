<p align="center">
  <a href="https://forestany.net/" target="_blank">
    <img alt="forestNET" src="https://forestany.net/pngs/fnet-logo.png" width="400">
  </a>
</p>

This custom-built [forestNET Framework](https://forestany.net/fnet.php) in C#, developed in Visual Studio, is designed to streamline the development of robust applications by providing a comprehensive suite of tools and interfaces.

It offers seamless integration with console applications, efficient file handling solutions, and database management capabilities. Additionally, the framework supports both FTP and SFTP protocols for secure file transfers, and includes advanced features for socket programming, enabling smooth communication across networks. By combining these functionalities, the framework simplifies complex tasks, threading, enhancing developer productivity and application performance.

Following **database systems** are supported by forestNET:

* MariaDB/MySQL
* SQLite3
* MSSQL
* OracleDB
* PostgreSQL
* MongoDB

This framework is designed to be in sync with the corresponding *forestJ framework* in Java, ensuring both platforms offer the same functionality and capabilities. This alignment enables seamless interoperability between .NET and Java applications, allowing for smooth integration and consistent performance across diverse environments.

forestNET framework will be released under the **GPLv3 license** and the **MIT license**. Thus it is freely possible to use forestNET in other projects - projects with free software or in commercial projects.

## Releases

### ForestNET.Lib.Net 1.0.3 (stable) + ForestNET.Lib.Net.SQL.Pool 1.0.0 (stable)
Implementation of a tiny HTTP 1.1 server and client with TLS support. Working modules for dynamic web application, SOAP service and REST service. Configuration class for tiny HTTP(S)/SOAP/REST server instance with SQL pool integration. *05/2025*

### ForestNET.Lib.Net 1.0.2 (stable)
Added advanced features for socket programming, like TCP/UDP socket tasks with TLS support, enabling smooth communication across networks. *05/2025*

### ForestNET.Lib.Net.Mail 1.0.0 (stable) + ForestNET.Lib.Net 1.0.1 (stable)
Implementation to use mail protocols(IMAP, POP3, SMTP). Added functionality for message boxes and network message marshalling. *04/2025*

### 1.0.14 (stable) + ForestNET.Lib.Net.FTP 1.0.0 (stable) + ForestNET.Lib.Net.SFTP 1.0.0 (stable)
Added support for both FTP(S) and SFTP protocols for secure file transfers. *04/2025*

### 1.0.13 (stable) + ForestNET.Lib.AI 1.0.0 (stable) + ForestNET.Lib.Net 1.0.0 (stable)
Added functionality to create and use neural networks for ai purposes. Added functionality for simple web requests over http(s). *04/2025*

### 1.0.12.1 (stable) + ForestNET.Lib.SQL 1.0.0 (stable) + ForestNET.Lib.SQL.Pool 1.0.0 (stable) + ForestNET.Lib.SQL.MariaDB 1.0.0 (stable) + ForestNET.Lib.SQL.MSSQL 1.0.0 (stable) + ForestNET.Lib.SQL.NOSQLMDB 1.0.0 (stable) + ForestNET.Lib.SQL.Oracle 1.0.0 (stable) + ForestNET.Lib.SQL.PGSQL 1.0.0 (stable) + ForestNET.Lib.SQL.SQLite 1.0.0 (stable)
Enabled integration of database management capabilities. *04/2025*

### 1.0.11 (stable)
Added XML file parser. *04/2025*

### 1.0.10 (stable)
Added JSON file parser. *03/2025*

### 1.0.9 (stable)
Added YAML file parser. *03/2025*

### 1.0.8 (stable)
Additional functionalities: ZIP compression, CSV file parser. *03/2025*

### 1.0.7 (stable)
Added support for flat files or fixed record length files. Automatically detecting records, group headers or footers as stacks of data. *02/2025*

### 1.0.6 (stable)
Additional core functionalities: Timer, State machine, File system watcher, Dijkstra shortest path algorithm. *01/2025*

### 1.0.5 (stable)
Implementation of console progress bar functionality and symmetric cryptography AES/GCM methods. *12/2024*

### 1.0.4 (stable)
Sorts class as collection of static methods to sort dynamic lists and dynamic key-value maps. Also possibility to get sort progress with delegate implementation. *12/2024*

### 1.0.3 (stable)
Added functionality for currency handling, date interval and memory observation. *12/2024*

### 1.0.2 (stable)
Added logging functionality within global singleton class of forestNET library. *11/2024*

### 1.0.1 (stable)
Added file handling library functions. *11/2024*

### 1.0.0 (stable)
First release of the forestNET Framework 1.0.0 (stable). Provision of foundation files(Helper) + console application library functions. *10/2024*

## Tests

* **Windows**
	* Microsoft Windows 11 Pro - OS Version: 10.0.26100 N/A Build 26100
	* Microsoft Visual Studio Community 2022 (64-bit) - Version 17.13.5
  * .NET 8.0

* **Database**

  * Linux - Linux version 6.1.0-33-amd64 (debian-kernel@lists.debian.org) (gcc-12 (Debian 12.2.0-14) 12.2.0, GNU ld (GNU Binutils for Debian) 2.40) #1 SMP PREEMPT_DYNAMIC Debian 6.1.133-1 (2025-04-10)
    * Mariadb
      * 10.11.11-MariaDB-0+deb12u1
    * MSSQL
      * Microsoft SQL Server 2022 (RTM-CU16) (KB5048033) - 16.0.4165.4 (X64)
    * Oracle
      * Oracle Database 23ai Free Release 23.0.0.0.0 - Version 23.7.0.25.01
    * PGSQL
      * PostgreSQL 15.12 (Debian 15.12-0+deb12u2) on x86_64-pc-linux-gnu, compiled by gcc (Debian 12.2.0-14) 12.2.0, 64-bit
    * MongoDB
      * 7.0.19
    * SQLite
      * 3.48.0
  * Windows - Microsoft Windows 11 Enterprise Evaluation - 10.0.22621 N/A Build 22621
    * Mariadb
      * 11.7.2-MariaDB
    * MSSQL
      * Microsoft SQL Server 2022 (RTM) - 16.0.1000.6 (X64)
    * Oracle
      * Oracle Database 21c Express Edition Release 21.0.0.0.0 - Version 21.3.0.0.0
    * PGSQL
      * PostgreSQL 17.4 on x86_64-windows, compiled by msvc-19.42.34436, 64-bit
    * MongoDB
      * 8.0.8
    * SQLite
      * 3.48.0

* **FTP/SFTP**

  * Linux - Linux version 6.1.0-33-amd64 (debian-kernel@lists.debian.org) (gcc-12 (Debian 12.2.0-14) 12.2.0, GNU ld (GNU Binutils for Debian) 2.40) #1 SMP PREEMPT_DYNAMIC Debian 6.1.133-1 (2025-04-10)
    * proftp
      * 1.3.8+dfsg-4+deb12u4

* **Mail**

  * Linux - Linux version 6.1.0-33-amd64 (debian-kernel@lists.debian.org) (gcc-12 (Debian 12.2.0-14) 12.2.0, GNU ld (GNU Binutils for Debian) 2.40) #1 SMP PREEMPT_DYNAMIC Debian 6.1.133-1 (2025-04-10)
    * iRedMail
      * 1.7.3