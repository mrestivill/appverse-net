#appverse-net

[![Build Status](https://travis-ci.org/glarfs/appverse-net.png?branch=master)](https://travis-ci.org/glarfs/appverse-net)

This repository holds our initiative to extend Appverse philosophy to Microsoft's own flavour



## Steps to compile and execute the solution for the first time


1-. Right click over Solution 'AppverseMVC’ node and select “Enable NuGet Package Restore”.

2-. Rebuild solution: 0 errors.

3-. Check database connetions Appverse\web.Config file:

    <add name="ShowcaseConnection" connectionString="Data Source=.\sqlexpress;Initial Catalog=AppverseMVC_dev;Integrated Security=True" providerName="System.Data.SqlClient" />
    
    <add name="MembershipConnection" connectionString="Data Source=.\sqlexpress;Initial Catalog=Appverse_users_dev;Integrated Security=True" providerName="System.Data.SqlClient" />

This configuration is using integrated security and a local SQL Server Express. Check if this configuration is ok for you. 

4-. Create an empty database for the "ShowcaseConnection" with its name. In this case “AppverseMVC_dev”.

5-. Open the “Package Manager Console” in “View -> Other Windows -> Package Manager Console”.

6-. Run the 'Update-Database' command. This will create the "MembershipConnection" database.

7-. Execute the project.



## License

    Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

     This Source  Code Form  is subject to the  terms of  the Appverse Public License 
     Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
     file, You can obtain one at <http://appverse.org/legal/appverse-license/>.

     Redistribution and use in  source and binary forms, with or without modification, 
     are permitted provided that the  conditions  of the  AppVerse Public License v2.0 
     are met.

     THIS SOFTWARE IS PROVIDED BY THE  COPYRIGHT HOLDERS  AND CONTRIBUTORS "AS IS" AND
     ANY EXPRESS  OR IMPLIED WARRANTIES, INCLUDING, BUT  NOT LIMITED TO,   THE IMPLIED
     WARRANTIES   OF  MERCHANTABILITY   AND   FITNESS   FOR A PARTICULAR  PURPOSE  ARE
     DISCLAIMED. EXCEPT IN CASE OF WILLFUL MISCONDUCT OR GROSS NEGLIGENCE, IN NO EVENT
     SHALL THE  COPYRIGHT OWNER  OR  CONTRIBUTORS  BE LIABLE FOR ANY DIRECT, INDIRECT,
     INCIDENTAL,  SPECIAL,   EXEMPLARY,  OR CONSEQUENTIAL DAMAGES  (INCLUDING, BUT NOT
     LIMITED TO,  PROCUREMENT OF SUBSTITUTE  GOODS OR SERVICES;  LOSS OF USE, DATA, OR
     PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
     WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR OTHERWISE) 
     ARISING  IN  ANY WAY OUT  OF THE USE  OF THIS  SOFTWARE,  EVEN  IF ADVISED OF THE 
     POSSIBILITY OF SUCH DAMAGE.
