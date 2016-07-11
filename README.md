# FBIUCRDashboard

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)


Input parameters:

1) workspaceCollectionName - The PowerBI Embedded workspace collection name.
   - Rules: It can be 3 to 64 characters in length and made up of lowercase letters 'a'-'z', the numbers 0-9 and the hyphen. The hyphen may not lead or trail in the name. Note: Having a hyphen is mandatory.

2) storageAccountName - The storage account name.
   - Rules: It can be 3 to 64 characters in length and made up of lowercase letters 'a'-'z' and the numbers 0-9.

3) siteName - The website name.
   - Rules: It can be 2 to 60 characters in length and made up of letters 'a'-'z'/ 'A'-'Z', the numbers 0-9 and the hyphen. The hyphen may not lead or trail in the name.

4) sqlAdminLogin - The admin user of the SQL Server.
   - Rules: It should be a SQL Identifier, and not a typical system name (like admin, administrator, sa, root, dbmanager, loginmanager, etc.), or a built-in database user or role (like dbo, guest, public, etc.). It must not contain whitespaces, unicode characters, or nonalphabetic characters, and that it doesn't begin with numbers or symbols.

5) sqlAdminPassword - The password of the admin user of the SQL Server.
   - Rules: It must be at least 8 characters in length.
