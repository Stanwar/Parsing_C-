# Transactions_In_SQLSERVER

This project shows how transactions are implemented in a database and in this case - SQL SERVER. The language used is C#. The project is workng on real CTA data. 

Challenges tackled : 
1. The first challenge was to insert the huge amount of CTA into the system. There were multiple choices : LINQ Queries, BULK INSERT or INSERT through a query. The approach taken by me was BULK INSERT as I achieved insertion in less than 10 seconds when comparative methods took almost 5 minutes to do that. 
2. The GUI is created in Visual Studio and the language used is C#. 

Steps/Features of the App.
1. Load the database using the load data button 
2. Include the two file paths in the given textboxes. 
3. Data can be purged using purge database button. 
4. Once Data is loaded, use Show Details Button to view Stop information. 
5. Any Duplicate Data or Errors and counts while loading are provided in the Load Status Listbox.
6. BULK INSERT Error File is also generated at the same path as the loading files. Please delete the file if it exists after a specific run. 
