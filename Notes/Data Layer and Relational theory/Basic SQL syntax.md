### *Basics of SQL syntax and the principle of dividing commands into 5 groups*

SQL (Structured Query Language) is a standard language for managing and manipulating databases. 

_1. DDL (Data Definition Language):_ These commands are used to define or alter the structure of the database objects.
**CREATE:** to create databases and database objects.
**ALTER:** to alter existing database objects.
**DROP:** to delete databases and database objects.

_2. DML (Data Manipulation Language):_ These commands are used to manipulate the data within objects.
**SELECT:** to retrieve data from a database.
**INSERT:** to insert data into a table.
**UPDATE:** to update existing data within a table.
**DELETE:** to delete records from a database table.

_3. DQL (Data Query Language):_ This is used to fetch the data from the database.
**SELECT:** to retrieve data from a database.
  
_4. DCL (Data Control Language):_ These commands are used to control the access to data stored in a database (Authorization).
**GRANT:** to grant specific user to perform specific task.
**REVOKE**: to cancel previously granted or denied permissions.

_5. TCL (Transaction Control Language):_ These commands are used to manage transactions in the database.
**COMMIT:** to save the work done in transactions.
**ROLLBACK:** to undo the changes made by the current transaction.
**SAVEPOINT:** to divide the transaction into smaller parts.
**SET TRANSACTION:** to specify characteristics for the transaction.
