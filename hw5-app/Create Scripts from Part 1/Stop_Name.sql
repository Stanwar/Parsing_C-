 CREATE PROCEDURE Stop_Name
   AS
    SELECT DISTINCT CONCAT(Stop_id,' | ',Stop_Name) Stop_Name
	 FROM Stops;