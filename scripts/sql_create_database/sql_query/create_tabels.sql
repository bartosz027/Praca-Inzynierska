USE PI_DB;

CREATE TABLE Account (
    ID                  INT                 NOT NULL        PRIMARY KEY IDENTITY,
	Username            VARCHAR(64)         NOT NULL,
	
    Email               VARCHAR(128)        NOT NULL,
    Password            VARCHAR(1024)       NOT NULL,
	
	Verified			BIT					NOT NULL,
	AccessToken			VARCHAR(32)			NULL,
	
	-- NP:Friendships  -> remove_property()
	-- NP:Friendships1 -> rename_to("Friends")
	
	-- NP:Messages  -> remove_property()
	-- NP:Messages1 -> remove_property()
	
	-- NP:FriendInvitations  -> remove_property()
	-- NP:FriendInvitations1 -> remove_property()
);

CREATE TABLE Friendship (
	ID 					INT					NOT NULL		PRIMARY KEY IDENTITY,

    UserID              INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
    FriendID            INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
	
	-- NP:Account1 -> rename_to("User")
	-- NP:Account  -> rename_to("Friend")
);

CREATE TABLE Message (
    ID                  INT                 NOT NULL        PRIMARY KEY IDENTITY,
	
    Content             VARCHAR(5000)       NOT NULL,
	SendDate			DATETIME			NOT NULL,

    SenderID            INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
    ReceiverID          INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
	
	-- NP:Account1 -> rename_to("Sender")
	-- NP:Account  -> rename_to("Receiver")
);

CREATE TABLE FriendInvitation (
	ID 					INT					NOT NULL		PRIMARY KEY IDENTITY,

    SenderID            INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
    ReceiverID          INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
	
	-- NP:Account1 -> rename_to("Sender")
	-- NP:Account  -> rename_to("Receiver")
);

CREATE TABLE Verification (
	ID                  INT                 NOT NULL        PRIMARY KEY IDENTITY,
	
	Email				VARCHAR(128)		NOT NULL,
	Code				VARCHAR(6)			NOT NULL,
	
	ExpireDate			DATETIME			NOT NULL,
);