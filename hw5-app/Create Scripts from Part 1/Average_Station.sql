 CREATE PROCEDURE Average_Station
	@daytype NVARCHAR(64)
   AS
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
   where daytype =@daytype
   group by station_id,daytype
   ORDER BY Max_avg DESC)T3
   ON T3.station_id = S.Station_id
   ;