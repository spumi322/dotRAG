
User authentication verifies the identity of a user, while authorization determines the user's access rights within the database.

- **User Authentication:**
    
    - **Process:** Users provide credentials (e.g., username and password) for verification.
    - **Best Practice:** Use strong, hashed passwords and employ multi-factor authentication when possible.
- **User Authorization:**
    
    - **Process:** Assign specific privileges (e.g., SELECT, INSERT, UPDATE, DELETE) to users or roles.
    - **Best Practice:** Follow the principle of least privilege, granting only necessary permissions for each user or role.

**What Is Row-Level Security?**

- RLS allows you to control access to individual rows (records) in database tables based on user permissions, through security policies.
- Prevents unauthorized users from viewing sensitive data.

### SQL Injection Prevention:

SQL injection is a type of cyber attack where malicious SQL statements are inserted into input fields, leading to unauthorized access or data manipulation.

- Parameterized Queries
- Input Validation
- Stored Procedures and ORM