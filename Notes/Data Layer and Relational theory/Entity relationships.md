In a database, models are used to describe the structure of the data stored in the database and 
and they also provide a way to query data across multiple tables.

1. **One-to-One (1:1)** - A one-to-one relationship means that a record in one table is associated with only one record in another table and vice versa. Foreign key in one of the tables

2. **One-to-Many (1:N)** - A one-to-many relationship means that a record in one table is associated with many records in another table, but each record in the second table is associated with only one record in the first table. Foreign key in the table that has N cardinality  
3. **Many-to-Many (N:M)** - A many-to-many relationship means that a record in one table can be associated with many records in another table and vice versa. A third table, known as a **join table**, is used to manage this relationship. Foreign keys to both tables in the join table

  
