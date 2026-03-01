#### 1. **SUM:**

- **Purpose:** Calculates the sum of a numeric column.
    


    `SELECT SUM(Amount) AS TotalAmount FROM Orders;`
    

#### 2. **AVG:**

- **Purpose:** Calculates the average (mean) of a numeric column.
    
  
	
	`SELECT AVG(Amount) AS AverageAmount FROM Orders;`
    

#### 3. **COUNT:**

- **Purpose:** Counts the number of rows in a result set.
    
 
	
	`SELECT COUNT(*) AS OrderCount FROM Orders;`
    

#### 4. **MIN:**

- **Purpose:** Retrieves the minimum value in a numeric column.
    
 
	
	`SELECT MIN(Amount) AS MinAmount FROM Orders;`
    

#### 5. **MAX:**

- **Purpose:** Retrieves the maximum value in a numeric column.
    

	
	`SELECT MAX(Amount) AS MaxAmount FROM Orders;`