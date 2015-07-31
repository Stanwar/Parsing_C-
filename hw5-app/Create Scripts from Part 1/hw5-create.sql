--
-- Sharad Tanwar
-- U of Illinois at Chicago 
-- CS 480, Databases
-- Hw5 - Create Script
--
--
-- DROP TABLE Stops_Intermediate;
-- DROP TABLE Ridership_Intermediate;
-- DROP TABLE Stop_Details;
-- DROP TABLE Riderships;
-- DROP TABLE Stops;
-- DROP TABLE Lines_Info;
-- DROP TABLE Station_Info;


CREATE TABLE Stops_Intermediate(
Stop_ID			VARCHAR(240),
Direction_ID	VARCHAR(240),
STOP_NAME		VARCHAR(240),
Station_Name	VARCHAR(240),
Station_Descriptive_Name VARCHAR(240),
Map_ID			VARCHAR(240),
ADA				VARCHAR(240), 
RED				VARCHAR(240),
BLUE			VARCHAR(240),
G				VARCHAR(240),
BRN				VARCHAR(240),
P				VARCHAR(240),
Pexp			VARCHAR(240),
Y				VARCHAR(240), 
Pnk				VARCHAR(240),
O				VARCHAR(240), 
Location		VARCHAR(240)
);

CREATE TABLE Ridership_Intermediate(
	station_id	VARCHAR(240),
	stationname VARCHAR(240),
	date		VARCHAR(240),
	daytype		VARCHAR(240),
	rides		VARCHAR(240)
);

CREATE TABLE Station_Info(
	Station_id					INT PRIMARY KEY,
	Station_Name				VARCHAR(240) NOT NULL,
	Station_Descriptive_Name	VARCHAR(240)
);

CREATE TABLE Lines_Info (
	Line_id		INT IDENTITY(1,1) PRIMARY KEY,
	Line_Name	VARCHAR(24) NOT NULL
);

CREATE TABLE Stops(
	Stop_id				INT PRIMARY KEY,
	Station_id			INT FOREIGN KEY REFERENCES Station_Info(Station_id) NOT NULL,
	Stop_Name			VARCHAR(240) NOT NULL ,
	Direction			VARCHAR(240) NOT NULL,
	Handicap_Accessible VARCHAR(240),
	Location			VARCHAR(240) NOT NULL
);

CREATE TABLE Stop_Details(
	Record_ID	INT IDENTITY(1,1) PRIMARY KEY,
	Stop_id		INT FOREIGN KEY REFERENCES Stops(Stop_id) NOT NULL,
	Line_id		INT FOREIGN KEY REFERENCES Lines_Info(line_id) NOT NULL 
);

CREATE TABLE Riderships(
	Record_ID	INT IDENTITY(1,1) PRIMARY KEY,
	station_id	INT FOREIGN KEY REFERENCES Station_Info(Station_id) NOT NULL ,
	ride_date	date NOT NULL ,
	daytype		NVARCHAR (240) NOT NULL,
	rides		INT NOT NULL
);

CREATE INDEX StationNAME ON Station_Info(Station_Name);
CREATE INDEX StopName ON Stops(Stop_Name);
CREATE INDEX Stop_id ON Stop_Details(Stop_id,Line_id);
CREATE INDEX Station_id ON Riderships(Station_id,ride_date,rides);
CREATE INDEX daytype ON Riderships(daytype);
