1) Restore below database backups in SQL Server.

Database backup file path:
--------------------------
\DashBoard\DashBoard\App_Data\DBs\DashBoardNew.bak
\DashBoard\DashBoard\App_Data\DBs\ChartSample.bak




2) Update connectionstring details in web.config file with restored database details.

ConnectionString names to be updated
-------------------------------------
DefaultConnection
DashBoardDBEntities




3) Build application to download packages. Then run application

Note: In one machine I noticed dotnetcompile package issue. By doing a "clean and rebuild" the solution, issue get resolved. JFYI