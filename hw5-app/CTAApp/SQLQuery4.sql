/*
DECLARE @bulk_cmd VARCHAR(1000)
SET @bulk_cmd = '
                                        BULK INSERT dbo.Riderships
                                      FROM "C:\Users\Sharad\Desktop\CTA-L-Ridership-Daily.csv"
                                      WITH
                                      (
									  FIRSTROW =2,
                                      FIELDTERMINATOR ='','',
                                      ROWTERMINATOR ='''+CHAR(10)+'''
                                      )'

									  EXEC(@bulk_cmd)


									  INSERT INTO Riderships_XX(station_id,date,daytype,rides)
    SELECT station_id,date,daytype,rides FROM Riderships;
	*/

SELECT * FROM Ridership_Intermediate;

DELETE FROM Ridership_Intermediate;

SELECT * FROM Stops_Intermediate;
select * from Station_Info;

DELETE FROM Stops;
DELETE FROM Station_info;
DELETE FROM Stops

INSERT 
INTO Station_Info(station_id,station_name,Station_Descriptive_Name)  
SELECT 
DISTINCT 
	Map_ID,
	(SELECT TOP 1 Station_Name FROM Stops_Intermediate S
			WHERE S.Map_ID = S1.Map_ID ) Station_Name,
	(SELECT TOP 1 Station_Descriptive_Name FROM Stops_Intermediate S
			WHERE S.Map_ID = S1.Map_ID ) Station_Descriptive_Name
	FROM Stops_Intermediate S1
	ORDER BY Map_ID ASC;

INSERT INTO Stops(Stop_id,
				  station_id, 
			      Stop_Name,
				  Direction,
				  Handicap_Accessible,
				  Location)
SELECT DISTINCT 
	   Stop_ID,
	   Map_id,
	   Stop_Name,
	   (CASE  
	   WHEN Direction_ID = 'E'
		THEN 'East'
	   WHEN Direction_ID = 'W'
		THEN 'West'
		WHEN Direction_ID = 'S'
		THEN 'South'
		WHEN Direction_ID = 'N'
		Then 'North'
		Else 'N/A'
		END ) Direction,
	   ADA,
	   Location 
  FROM Stops_Intermediate
  ;

  INSERT INTO Lines_Info(Line_Name)
  SELECT																      
		DISTINCT MAX(Line_ID) + 1, 
			 CASE 
			  WHEN Pink
    FROM Stops_Intermediate;

INSERT INTO Lines_Info(Line_Name) VALUES('Pink');

INSERT INTO Lines_Info(Line_Name) VALUES('Orange');
INSERT INTO Lines_Info(Line_Name) VALUES('Red');
INSERT INTO Lines_Info(Line_Name) VALUES('Blue');
INSERT INTO Lines_Info(Line_Name) VALUES('Brown');
INSERT INTO Lines_Info(Line_Name) VALUES('Green');
INSERT INTO Lines_Info(Line_Name) VALUES('Purple');
INSERT INTO Lines_Info(Line_Name) VALUES('Purple-Express');
INSERT INTO Lines_Info(Line_Name) VALUES('Yellow');

  INSERT INTO Stop_Details(Stop_id,Line_id)
  SELECT Stop_id, 
		(SELECT Line_id
		  FROM Lines_Info
		 WHERE Line_Name = 'Pink'
		)  Line_id
    FROM Stops_Intermediate
   WHERE UPPER(Pnk) = UPPER('True');

     INSERT INTO Stop_Details(Stop_id,Line_id)
  SELECT Stop_id, 
		(SELECT Line_id
		  FROM Lines_Info
		 WHERE Line_Name = 'Red'
		)  Line_id
    FROM Stops_Intermediate
   WHERE UPPER(RED) = UPPER('True');

        INSERT INTO Stop_Details(Stop_id,Line_id)
  SELECT Stop_id, 
		(SELECT Line_id
		  FROM Lines_Info
		 WHERE Line_Name = 'Blue'
		)  Line_id
    FROM Stops_Intermediate
   WHERE UPPER(BLUE) = UPPER('True');

           INSERT INTO Stop_Details(Stop_id,Line_id)
  SELECT Stop_id, 
		(SELECT Line_id
		  FROM Lines_Info
		 WHERE Line_Name = 'Green'
		)  Line_id
    FROM Stops_Intermediate
   WHERE UPPER(G) = UPPER('True');

              INSERT INTO Stop_Details(Stop_id,Line_id)
  SELECT Stop_id, 
		(SELECT Line_id
		  FROM Lines_Info
		 WHERE Line_Name = 'Brown'
		)  Line_id
    FROM Stops_Intermediate
   WHERE UPPER(BRN) = UPPER('True');

 INSERT INTO Stop_Details(Stop_id,Line_id)
  SELECT Stop_id, 
		(SELECT Line_id
		  FROM Lines_Info
		 WHERE Line_Name = 'Purple'
		)  Line_id
    FROM Stops_Intermediate
   WHERE UPPER(P) = UPPER('True');

                 INSERT INTO Stop_Details(Stop_id,Line_id)
  SELECT Stop_id, 
		(SELECT Line_id
		  FROM Lines_Info
		 WHERE Line_Name = 'Purple-Express'
		)  Line_id
    FROM Stops_Intermediate
   WHERE UPPER(Pexp) = UPPER('True');

 INSERT INTO Stop_Details(Stop_id,Line_id)
  SELECT Stop_id, 
		(SELECT Line_id
		  FROM Lines_Info
		 WHERE Line_Name = 'Yellow'
		)  Line_id
    FROM Stops_Intermediate
   WHERE UPPER(Y) = UPPER('True');

 INSERT INTO Stop_Details(Stop_id,Line_id)
  SELECT Stop_id, 
		(SELECT Line_id
		  FROM Lines_Info
		 WHERE Line_Name = 'Orange'
		)  Line_id
    FROM Stops_Intermediate
   WHERE UPPER(O) = UPPER('True');

   SELECT COUNT(1) FROM Stop_Details;