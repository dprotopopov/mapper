DROP TABLE IF EXISTS Temp_Node;
DROP TABLE IF EXISTS Temp_Way;
DROP TABLE IF EXISTS Temp_Relation;

CREATE TABLE Temp_Node (
	Id BIGINT, 
	Version INTEGER, 
	Latitude DOUBLE PRECISION, 
	Longitude DOUBLE PRECISION,
	ChangeSetId BIGINT, 
	TimeStamp TIMESTAMP,
	UserId INT, 
	UserName VARCHAR(255), 
	Visible BOOLEAN, 
	Tags hstore
);

CREATE TABLE Temp_Way (
	Id BIGINT, 
	Version INTEGER, 
	ChangeSetId BIGINT, 
	TimeStamp TIMESTAMP,
	UserId INTEGER, 
	UserName VARCHAR(255), 
	Visible BOOLEAN, 
	Tags hstore,
	Nodes BIGINT[]
);

CREATE TABLE Temp_Relation (
	Id BIGINT, 
	Version INTEGER, 
	ChangeSetId BIGINT, 
	TimeStamp TIMESTAMP,
	UserId INTEGER, 
	UserName VARCHAR(255), 
	Visible BOOLEAN, 
	Tags hstore,
	Members RelationMember[]
);

