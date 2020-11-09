CREATE EXTENSION IF NOT EXISTS hstore;

DROP TABLE IF EXISTS Node;
DROP TABLE IF EXISTS Way;
DROP TABLE IF EXISTS Relation;
DROP TYPE IF EXISTS RelationMember;

CREATE TABLE Node (
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

CREATE TABLE Way (
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

CREATE TYPE RelationMember AS (
	Id BIGINT, 
    Role VARCHAR(255),
    Type INTEGER
);

CREATE TABLE Relation (
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

