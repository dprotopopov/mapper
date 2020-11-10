INSERT INTO Node(
	Id,
	Version,
	Latitude,
	Longitude,
	ChangeSetId,
	TimeStamp,
	UserId,
	UserName,
	Visible,
	Tags
)
SELECT 
	Id,
	Version,
	Latitude,
	Longitude,
	ChangeSetId,
	TimeStamp,
	UserId,
	UserName,
	Visible,
	Tags
FROM Temp_Node
ON CONFLICT (Id) DO UPDATE SET
	Id=EXCLUDED.Id,
	Version=EXCLUDED.Version,
	Latitude=EXCLUDED.Latitude,
	Longitude=EXCLUDED.Longitude,
	ChangeSetId=EXCLUDED.ChangeSetId,
	TimeStamp=EXCLUDED.TimeStamp,
	UserId=EXCLUDED.UserId,
	UserName=EXCLUDED.UserName,
	Visible=EXCLUDED.Visible,
	Tags=EXCLUDED.Tags;
INSERT INTO Way(
	Id,
	Version,
	ChangeSetId,
	TimeStamp,
	UserId,
	UserName,
	Visible,
	Tags,
	Nodes
)
SELECT 
	Id,
	Version,
	ChangeSetId,
	TimeStamp,
	UserId,
	UserName,
	Visible,
	Tags,
	Nodes
FROM Temp_Way
ON CONFLICT (Id) DO UPDATE SET
	Id=EXCLUDED.Id,
	Version=EXCLUDED.Version,
	ChangeSetId=EXCLUDED.ChangeSetId,
	TimeStamp=EXCLUDED.TimeStamp,
	UserId=EXCLUDED.UserId,
	UserName=EXCLUDED.UserName,
	Visible=EXCLUDED.Visible,
	Tags=EXCLUDED.Tags,
	Nodes=EXCLUDED.Nodes;
INSERT INTO Relation(
	Id,
	Version,
	ChangeSetId,
	TimeStamp,
	UserId,
	UserName,
	Visible,
	Tags,
	Members
)
SELECT 
	Id,
	Version,
	ChangeSetId,
	TimeStamp,
	UserId,
	UserName,
	Visible,
	Tags,
	Members
FROM Temp_Relation
ON CONFLICT (Id) DO UPDATE SET
	Id=EXCLUDED.Id,
	Version=EXCLUDED.Version,
	ChangeSetId=EXCLUDED.ChangeSetId,
	TimeStamp=EXCLUDED.TimeStamp,
	UserId=EXCLUDED.UserId,
	UserName=EXCLUDED.UserName,
	Visible=EXCLUDED.Visible,
	Tags=EXCLUDED.Tags,
	Members=EXCLUDED.Members;

DROP TABLE Temp_Node;
DROP TABLE Temp_Way;
DROP TABLE Temp_Relation;
