CREATE TABLE ArioPlatform.dbo.Businesses (
	BusinessID bigint NOT NULL,
	BusinessName varchar(50),
	Address varchar(max),
	PhoneNumber varchar(50),
	Industry bigint,
	Website nvarchar(max),
	PRIMARY KEY (BusinessID)
)
;
CREATE TABLE ArioPlatform.dbo.BusinessRoles (
	RoleID bigint NOT NULL,
	RoleName varchar(25) NOT NULL,
	PRIMARY KEY (RoleID)
)
;
CREATE TABLE ArioPlatform.dbo.BusinessUserJoin (
	ID bigint NOT NULL,
	BusinessID bigint NOT NULL,
	UserID bigint NOT NULL,
	RoleID bigint NOT NULL,
	PRIMARY KEY (ID)
)
;
CREATE TABLE ArioPlatform.dbo.Labels (
	ID bigint NOT NULL,
	NodeComponentID bigint NOT NULL,
	Text nvarchar(max),
	Style nvarchar(50),
	PRIMARY KEY (ID)
)
;
CREATE TABLE ArioPlatform.dbo.NodeComponents (
	ID bigint NOT NULL,
	NodeID bigint NOT NULL,
	Type varchar(50) NOT NULL,
	PRIMARY KEY (ID)
)
;
CREATE TABLE ArioPlatform.dbo.Nodes (
	ID bigint NOT NULL,
	ParentID bigint,
	BusinessID bigint,
	NodeName varchar(50),
	XPosition decimal(8,6) NOT NULL,
	YPosition decimal(8,6) NOT NULL,
	ZPosition decimal(8,6) NOT NULL,
	PRIMARY KEY (ID)
)
;
CREATE TABLE ArioPlatform.dbo.NodeTeamJoin (
	ID bigint NOT NULL,
	NodeID bigint NOT NULL,
	TeamID bigint NOT NULL,
	PRIMARY KEY (ID)
)
;
CREATE TABLE ArioPlatform.dbo.QRAnchorComponents (
	ID bigint NOT NULL,
	NodeComponentID bigint NOT NULL,
	QRAnchorID bigint NOT NULL,
	PRIMARY KEY (ID)
)
;
CREATE TABLE ArioPlatform.dbo.TeamRoles (
	RoleID bigint NOT NULL,
	RoleName varchar(25) NOT NULL,
	PRIMARY KEY (RoleID)
)
;
CREATE TABLE ArioPlatform.dbo.Teams (
	TeamID bigint NOT NULL,
	BusinessID bigint NOT NULL,
	TeamCreatorID bigint NOT NULL,
	TeamName varchar(50),
	PRIMARY KEY (TeamID)
)
;
CREATE TABLE ArioPlatform.dbo.Users (
	UserID bigint NOT NULL,
	FirstName varchar(50),
	LastName varchar(50),
	Email nvarchar(max) NOT NULL,
	Company varchar(50),
	PhoneNumber varchar(50),
	Industry bigint,
	LastModified nvarchar(50),
	UserPhoto nvarchar(max),
	IsArio bigint NOT NULL,
	PRIMARY KEY (UserID)
)
;
CREATE TABLE ArioPlatform.dbo.UserTeamJoin (
	ID bigint NOT NULL,
	UserID bigint NOT NULL,
	TeamID bigint NOT NULL,
	RoleID bigint NOT NULL,
	PRIMARY KEY (ID)
)
;
INSERT INTO ArioPlatform.dbo.Businesses(BusinessID, BusinessName, Address, PhoneNumber, Industry, Website) VALUES (1, 'Ario', '54 Old Hampton Lane', '757-222-2222', 1, N'www.ario.com')
;
INSERT INTO ArioPlatform.dbo.Businesses(BusinessID, BusinessName, Address, PhoneNumber, Industry, Website) VALUES (2, 'JF Alliance Group', '54 Old Hampton Lane', '757-222-2222', 1, N'www.jfalliancegroup.com')
;
INSERT INTO ArioPlatform.dbo.Businesses(BusinessID, BusinessName, Address, PhoneNumber, Industry, Website) VALUES (3, 'Mon;com', '123 Granby Street, Norfolk, VA', '757-333-5656', 0, N'www.mon;.com')
;
INSERT INTO ArioPlatform.dbo.Businesses(BusinessID, BusinessName, Address, PhoneNumber, Industry, Website) VALUES (4, 'Marine Mikes', 'The sketchy part of Cleveland', '757-867-5309', 0, N'www.reddit.com/r/ripple')
;
INSERT INTO ArioPlatform.dbo.Businesses(BusinessID, BusinessName, Address, PhoneNumber, Industry, Website) VALUES (5, 'The Mr President', '915 Colley', 'Heckin ;od boi', 0, N'www.flubba.com')
;
INSERT INTO ArioPlatform.dbo.BusinessRoles(RoleID, RoleName) VALUES (1, 'ario')
;
INSERT INTO ArioPlatform.dbo.BusinessRoles(RoleID, RoleName) VALUES (2, 'owner')
;
INSERT INTO ArioPlatform.dbo.BusinessRoles(RoleID, RoleName) VALUES (3, 'admin')
;
INSERT INTO ArioPlatform.dbo.BusinessRoles(RoleID, RoleName) VALUES (4, 'user')
;
INSERT INTO ArioPlatform.dbo.BusinessRoles(RoleID, RoleName) VALUES (5, 'invited')
;
INSERT INTO ArioPlatform.dbo.BusinessUserJoin(ID, BusinessID, UserID, RoleID) VALUES (1, 1, 1, 1)
;
INSERT INTO ArioPlatform.dbo.BusinessUserJoin(ID, BusinessID, UserID, RoleID) VALUES (2, 2, 2, 2)
;
INSERT INTO ArioPlatform.dbo.BusinessUserJoin(ID, BusinessID, UserID, RoleID) VALUES (3, 1, 3, 1)
;
INSERT INTO ArioPlatform.dbo.BusinessUserJoin(ID, BusinessID, UserID, RoleID) VALUES (4, 2, 4, 1)
;
INSERT INTO ArioPlatform.dbo.BusinessUserJoin(ID, BusinessID, UserID, RoleID) VALUES (5, 2, 5, 3)
;
INSERT INTO ArioPlatform.dbo.BusinessUserJoin(ID, BusinessID, UserID, RoleID) VALUES (6, 2, 6, 3)
;
INSERT INTO ArioPlatform.dbo.BusinessUserJoin(ID, BusinessID, UserID, RoleID) VALUES (7, 2, 7, 5)
;
INSERT INTO ArioPlatform.dbo.Labels(ID, NodeComponentID, Text, Style) VALUES (1, 1, N'description', N'#FFFFFF')
;
INSERT INTO ArioPlatform.dbo.Labels(ID, NodeComponentID, Text, Style) VALUES (2, 2, N'description', N'#FFFFFF')
;
INSERT INTO ArioPlatform.dbo.Labels(ID, NodeComponentID, Text, Style) VALUES (3, 3, N'The computer that Andrew uses for work. Sometimes he takes it home to work out-of-office; today is not that day.', N'#FFD800')
;
INSERT INTO ArioPlatform.dbo.Labels(ID, NodeComponentID, Text, Style) VALUES (4, 4, N'description', N'#FFFFFF')
;
INSERT INTO ArioPlatform.dbo.Labels(ID, NodeComponentID, Text, Style) VALUES (5, 5, N'This is a parent node.', N'#F28747')
;
INSERT INTO ArioPlatform.dbo.Labels(ID, NodeComponentID, Text, Style) VALUES (6, 6, N'New node description', N'#4974D4')
;
INSERT INTO ArioPlatform.dbo.Labels(ID, NodeComponentID, Text, Style) VALUES (7, 7, N'This one isn’t really a mug, it’s more of a cup. Idk, I ;t it at a coffee shop on my way to the office :D', N'#4974D4')
;
INSERT INTO ArioPlatform.dbo.Labels(ID, NodeComponentID, Text, Style) VALUES (8, 8, N'Here’s a node. Nodes should auto-size the label. I think it worked!', N'#4EB37B')
;
INSERT INTO ArioPlatform.dbo.Labels(ID, NodeComponentID, Text, Style) VALUES (9, 9, N'This is the gun by the door', N'#F28747')
;
INSERT INTO ArioPlatform.dbo.Labels(ID, NodeComponentID, Text, Style) VALUES (10, 10, N'This is Jackson’s computer.', N'#E1E7ED')
;
INSERT INTO ArioPlatform.dbo.Labels(ID, NodeComponentID, Text, Style) VALUES (11, 11, N'Description', N'#E6BD29')
;
INSERT INTO ArioPlatform.dbo.NodeComponents(ID, NodeID, Type) VALUES (1, 3, 'label')
;
INSERT INTO ArioPlatform.dbo.NodeComponents(ID, NodeID, Type) VALUES (2, 4, 'label')
;
INSERT INTO ArioPlatform.dbo.NodeComponents(ID, NodeID, Type) VALUES (3, 5, 'label')
;
INSERT INTO ArioPlatform.dbo.NodeComponents(ID, NodeID, Type) VALUES (4, 6, 'label')
;
INSERT INTO ArioPlatform.dbo.NodeComponents(ID, NodeID, Type) VALUES (5, 7, 'label')
;
INSERT INTO ArioPlatform.dbo.NodeComponents(ID, NodeID, Type) VALUES (6, 8, 'label')
;
INSERT INTO ArioPlatform.dbo.NodeComponents(ID, NodeID, Type) VALUES (7, 9, 'label')
;
INSERT INTO ArioPlatform.dbo.NodeComponents(ID, NodeID, Type) VALUES (8, 10, 'label')
;
INSERT INTO ArioPlatform.dbo.NodeComponents(ID, NodeID, Type) VALUES (9, 11, 'label')
;
INSERT INTO ArioPlatform.dbo.NodeComponents(ID, NodeID, Type) VALUES (10, 12, 'label')
;
INSERT INTO ArioPlatform.dbo.NodeComponents(ID, NodeID, Type) VALUES (11, 13, 'label')
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (1, 1, 1, 'andrw', 0.000000, 0.000000, 0.000000)
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (2, 1, 1, 'node', 0.000000, 0.000000, 0.000000)
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (3, 1, 1, 'node', 0.000000, 0.000000, 0.000000)
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (4, 1, 1, 'node', 0.000000, 0.000000, 0.000000)
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (5, 0, 1, 'Andrew’s Computer', -0.162415, -0.268799, -0.047004)
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (6, 1, 1, 'node', 0.000000, 0.000000, 0.000000)
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (7, 0, 2, 'Test Parent', -1.408552, -0.547171, 0.791166)
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (8, 7, 2, 'New node', 0.635789, -0.015636, -0.110718)
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (9, 7, 2, 'coffee mug test', -0.818331, -0.790945, -0.499116)
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (10, 7, 2, 'Testing 2', 1.838927, -1.178556, 1.961782)
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (11, 7, 2, 'Rifle', 0.786163, -0.083191, 4.590433)
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (12, 0, 2, 'Jaxon’s Macbook', -0.733523, -0.750840, 0.670554)
;
INSERT INTO ArioPlatform.dbo.Nodes(ID, ParentID, BusinessID, NodeName, XPosition, YPosition, ZPosition) VALUES (13, 0, 2, 'New node 3', -0.224119, -0.169840, -0.010075)
;
INSERT INTO ArioPlatform.dbo.TeamRoles(RoleID, RoleName) VALUES (1, 'admin')
;
INSERT INTO ArioPlatform.dbo.TeamRoles(RoleID, RoleName) VALUES (2, 'author')
;
INSERT INTO ArioPlatform.dbo.TeamRoles(RoleID, RoleName) VALUES (3, 'readonly')
;
INSERT INTO ArioPlatform.dbo.TeamRoles(RoleID, RoleName) VALUES (4, 'inactive')
;
INSERT INTO ArioPlatform.dbo.Teams(TeamID, BusinessID, TeamCreatorID, TeamName) VALUES (1, 2, 2, 'Super Trooperss')
;
INSERT INTO ArioPlatform.dbo.Teams(TeamID, BusinessID, TeamCreatorID, TeamName) VALUES (2, 1, 1, 'Ario Customer Support')
;
INSERT INTO ArioPlatform.dbo.Teams(TeamID, BusinessID, TeamCreatorID, TeamName) VALUES (3, 2, 2, 'Something')
;
INSERT INTO ArioPlatform.dbo.Teams(TeamID, BusinessID, TeamCreatorID, TeamName) VALUES (4, 2, 2, 'Jaxons Jackal')
;
INSERT INTO ArioPlatform.dbo.Users(UserID, FirstName, LastName, Email, Company, PhoneNumber, Industry, LastModified, UserPhoto, IsArio) VALUES (1, 'Joseph', 'Weaver', N'jweaver@ario.com', 'Ario, Inc', '757-333-3333', 1, N'10/30/2017 14:45:22', null, 1)
;
INSERT INTO ArioPlatform.dbo.Users(UserID, FirstName, LastName, Email, Company, PhoneNumber, Industry, LastModified, UserPhoto, IsArio) VALUES (2, 'Falana', 'Dula-King', N'fdula-king@jfalliancegroup.com', 'JF Alliance Group', '757-222-2222', 1, N'11/16/2017 3:18:09 PM', null, 0)
;
INSERT INTO ArioPlatform.dbo.Users(UserID, FirstName, LastName, Email, Company, PhoneNumber, Industry, LastModified, UserPhoto, IsArio) VALUES (3, 'Nathan', 'Fender', N'nfender@ario.com', 'Ario', '757-111-1111', 1, N'10/30/2017 16:10:10', null, 1)
;
INSERT INTO ArioPlatform.dbo.Users(UserID, FirstName, LastName, Email, Company, PhoneNumber, Industry, LastModified, UserPhoto, IsArio) VALUES (4, 'Tomasz', 'Foster', N'tfoster@jfalliancegroup.com', 'JF Alliance Group', '757-201-0788', 1, N'11/16/2017 4:32:32 PM', null, 1)
;
INSERT INTO ArioPlatform.dbo.Users(UserID, FirstName, LastName, Email, Company, PhoneNumber, Industry, LastModified, UserPhoto, IsArio) VALUES (5, 'Andrew', ';tow', N'a;tow@jfalliancegroup.com', 'JF Alliance Group', '757-666-6666', 1, N'11/13/2017 12:27:00', null, 0)
;
INSERT INTO ArioPlatform.dbo.Users(UserID, FirstName, LastName, Email, Company, PhoneNumber, Industry, LastModified, UserPhoto, IsArio) VALUES (6, 'Jacob', 'Galito', N'jgalito@jfalliancegroup.com', 'JF Alliance Group', '757-508-9987', 1, N'11/16/2017 4:33:33 PM', null, 0)
;
INSERT INTO ArioPlatform.dbo.Users(UserID, FirstName, LastName, Email, Company, PhoneNumber, Industry, LastModified, UserPhoto, IsArio) VALUES (7, 'Jaxon', 'Cramer', N'jcramer@jfalliancegroup.com', 'JF Alliance Group', '5555555555', 1, N'11/15/2017 8:02:41 PM', null, 0)
;
INSERT INTO ArioPlatform.dbo.UserTeamJoin(ID, UserID, TeamID, RoleID) VALUES (2, 3, 2, 1)
;
INSERT INTO ArioPlatform.dbo.UserTeamJoin(ID, UserID, TeamID, RoleID) VALUES (3, 1, 2, 2)
;
INSERT INTO ArioPlatform.dbo.UserTeamJoin(ID, UserID, TeamID, RoleID) VALUES (5, 5, 1, 1)
;
INSERT INTO ArioPlatform.dbo.UserTeamJoin(ID, UserID, TeamID, RoleID) VALUES (7, 4, 3, 1)
;
INSERT INTO ArioPlatform.dbo.UserTeamJoin(ID, UserID, TeamID, RoleID) VALUES (8, 5, 3, 3)
;
INSERT INTO ArioPlatform.dbo.UserTeamJoin(ID, UserID, TeamID, RoleID) VALUES (9, 4, 4, 3)
;
INSERT INTO ArioPlatform.dbo.UserTeamJoin(ID, UserID, TeamID, RoleID) VALUES (10, 4, 1, 2)
;
INSERT INTO ArioPlatform.dbo.UserTeamJoin(ID, UserID, TeamID, RoleID) VALUES (11, 2, 1, 2)
;
INSERT INTO ArioPlatform.dbo.UserTeamJoin(ID, UserID, TeamID, RoleID) VALUES (12, 6, 1, 3)
;
INSERT INTO ArioPlatform.dbo.UserTeamJoin(ID, UserID, TeamID, RoleID) VALUES (13, 7, 1, 4)
;
ALTER TABLE ArioPlatform.dbo.BusinessUserJoin
	ADD FOREIGN KEY (BusinessID) 
	REFERENCES Businesses (BusinessID)
;

ALTER TABLE ArioPlatform.dbo.BusinessUserJoin
	ADD FOREIGN KEY (RoleID) 
	REFERENCES BusinessRoles (RoleID)
;

ALTER TABLE ArioPlatform.dbo.BusinessUserJoin
	ADD FOREIGN KEY (UserID) 
	REFERENCES Users (UserID)
;


ALTER TABLE ArioPlatform.dbo.Labels
	ADD FOREIGN KEY (NodeComponentID) 
	REFERENCES NodeComponents (ID)
;


ALTER TABLE ArioPlatform.dbo.NodeComponents
	ADD FOREIGN KEY (NodeID) 
	REFERENCES Nodes (ID)
;


ALTER TABLE ArioPlatform.dbo.NodeTeamJoin
	ADD FOREIGN KEY (NodeID) 
	REFERENCES Nodes (ID)
;

ALTER TABLE ArioPlatform.dbo.NodeTeamJoin
	ADD FOREIGN KEY (TeamID) 
	REFERENCES Teams (TeamID)
;


ALTER TABLE ArioPlatform.dbo.QRAnchorComponents
	ADD FOREIGN KEY (NodeComponentID) 
	REFERENCES NodeComponents (ID)
;


ALTER TABLE ArioPlatform.dbo.Teams
	ADD FOREIGN KEY (BusinessID) 
	REFERENCES Businesses (BusinessID)
;

ALTER TABLE ArioPlatform.dbo.Teams
	ADD FOREIGN KEY (TeamCreatorID) 
	REFERENCES Users (UserID)
;


ALTER TABLE ArioPlatform.dbo.UserTeamJoin
	ADD FOREIGN KEY (RoleID) 
	REFERENCES TeamRoles (RoleID)
;

ALTER TABLE ArioPlatform.dbo.UserTeamJoin
	ADD FOREIGN KEY (TeamID) 
	REFERENCES Teams (TeamID)
;

ALTER TABLE ArioPlatform.dbo.UserTeamJoin
	ADD FOREIGN KEY (UserID) 
	REFERENCES Users (UserID)
;


