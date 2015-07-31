SELECT Direction,
		(CASE WHEN 
			Handicap_Accessible = 'false'
			THEN 'No'
		WHEN UPPER(Handicap_Accessible) = UPPER('True')
		   THEN 'Yes'
		ELSE 'N/A'
		END) Handicap,
		(SELECT Station_Name
		   FROM Station_Info
		   WHERE Station_id = S.Station_id
		) Station_Name,
		Stop_ID
  FROM Stops S
 WHERE Stop_Name = 'Austin (Forest Pk-bound)';

 --

 SELECT SUM(R.rides) SUM_STATION_RIDE, 
		AVG(R.rides) AVG_STATION_RIDE
   FROM Riderships R
  WHERE station_id = (SELECT DISTINCT S.station_id 
						FROM Station_Info S, 
							 Stops S1
					   WHERE Station_Name = '47th'
					     AND S.Station_id = S1.Station_id
						 AND S1.Stop_id = '30238')
   ;

   SELECT Line_Name 
     FROM Lines_Info
    WHERE line_id IN 
	(SELECT Line_id
	  FROM Stop_Details  
	  WHERE Stop_id = 30238);

	  
	  SELECT DISTINCT CONCAT(Stop_id,' | ',Stop_Name)
	 FROM Stops;

	 SELECT 
        DISTINCT 
	        Map_ID,
	        (SELECT Station_Name FROM Stops_Intermediate S
			        WHERE S.Map_ID = S1.Map_ID ) Station_Name,
	        (SELECT TOP 1 Station_Descriptive_Name FROM Stops_Intermediate S
			        WHERE S.Map_ID = S1.Map_ID ) Station_Descriptive_Name
	        FROM Stops_Intermediate S1
	        ORDER BY Map_ID ASC;

			Select S1.Station_Name, count(S1.Station_Name)
			  FROM Stops_Intermediate S1, Stops_Intermediate S2
			  WHERE S1.Map_ID = S2.MAP_ID
			GROUP BY S1.Station_Name 
			HAVING COUNT(S1.Station_Name) = 1;

			Select S1.Station_Name, count(S1.Station_Name)
			  FROM Stops_Intermediate S1
			GROUP BY S1.Station_Name 
			HAVING COUNT(S1.Station_Name) = 1;

			Select * from Station_Info where station_id = 40500;

			SELECT station_name, Max_avg
	 FROM Station_Info S
	 INNER JOIN
      (SELECT TOP 1 station_id, 
					daytype, 
				MAX(Average_Rides) Max_avg
   FROM 
	   (SELECT R.station_id,R.daytype, AVG(R.Rides) Average_Rides
		 FROM Riderships R 
		INNER JOIN (SELECT station_id 
			FROM Station_Info) T
	   ON T.Station_id = R.station_id
	   GROUP BY R.Station_id,R.daytype)T2
   where daytype = 'W'
   group by station_id,daytype
   ORDER BY Max_avg DESC)T3
   ON T3.station_id = S.Station_id