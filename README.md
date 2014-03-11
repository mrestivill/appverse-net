appverse-net
============

This repository holds our initiative to extend Appverse philosophy to Microsoft's own flavour



Steps to compile and execute the solution for the first time:
------------------------------------------------------------

1. Right click over Solution 'AppverseMVC’ node and select “Enable NuGet Package Restore”.
2. Rebuild solution: 0 errors
3. Check database connetions Appverse\web.Config file:
    <add name="ShowcaseConnection" connectionString="Data Source=.\sqlexpress;Initial Catalog=AppverseMVC_dev;Integrated Security=True" providerName="System.Data.SqlClient" />
    <add name="MembershipConnection" connectionString="Data Source=.\sqlexpress;Initial Catalog=Appverse_users_dev;Integrated Security=True" providerName="System.Data.SqlClient" />

This configuration is using integrated security and a local SQL Server Express. Check if this configuration is ok for you. 
4. Create an empty database for the "ShowcaseConnection" with its name. In this case “AppverseMVC_dev”
5. Open the “Package Manager Console” in “View -> Other Windows -> Package Manager Console”
6. Run the 'Update-Database' command. This will create the "MembershipConnection" database
7. Execute the project.

