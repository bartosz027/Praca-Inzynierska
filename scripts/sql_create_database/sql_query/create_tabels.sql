USE PI_DB;

CREATE TABLE Account (
    ID                  INT                 NOT NULL        PRIMARY KEY IDENTITY,
	Username            VARCHAR(128)        NOT NULL,
	
    Email               VARCHAR(128)        NOT NULL,
    Password            VARCHAR(64)         NOT NULL,
	
	Status				BIT					NOT NULL		DEFAULT 1,
	Verified			BIT					NOT NULL		DEFAULT 0,
	
	UserImage			VARCHAR(512)		NOT NULL,
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
	
	IsRead				BIT					NOT NULL		DEFAULT 0,
	
	-- NP:Account1 -> rename_to("Sender")
	-- NP:Account  -> rename_to("Receiver")
);

CREATE TABLE Image (
    ID                  INT                 NOT NULL        PRIMARY KEY IDENTITY,
	MessageID           INT                 NOT NULL        FOREIGN KEY REFERENCES Message(ID),
	Filename            VARCHAR(5000)       NOT NULL,
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
	
	Email				VARCHAR(128) 		NOT NULL,
	Code				VARCHAR(6)			NOT NULL,
	
	ExpireDate			DATETIME			NOT NULL,
);