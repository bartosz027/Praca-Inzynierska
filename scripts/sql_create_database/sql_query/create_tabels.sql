USE PI_DB;

CREATE TABLE Account (
    ID                  INT                 NOT NULL        PRIMARY KEY IDENTITY,
    Username            VARCHAR(64)         NOT NULL,

    Email               VARCHAR(128)        NOT NULL,
    Password            VARCHAR(1024)       NOT NULL,
	
	Verified			BIT					NOT NULL,
	AccessToken			VARCHAR(32)			NULL,
	
	-- NP:Friendships  -> rename_to("Friends")
	-- NP:Friendships1 -> remove_property()
	
	-- NP:Messages  -> rename_to("MessagesReceived")
	-- NP:Messages1 -> rename_to("MessagesSent")
);

CREATE TABLE Friendship (
	ID 					INT					NOT NULL		PRIMARY KEY IDENTITY,

    UserID              INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
    FriendID            INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
	
	-- NP:Account  -> rename_to("Friend")
	-- NP:Account1 -> rename_to("User")
);

CREATE TABLE Message (
    ID                  INT                 NOT NULL        PRIMARY KEY IDENTITY,
    Content             VARCHAR(5000)       NOT NULL,

    SenderID            INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
    ReceiverID          INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
	
	-- NP:Account  -> rename_to("Receiver")
	-- NP:Account1 -> rename_to("Sender")
);

CREATE TABLE Verification (
	ID                  INT                 NOT NULL        PRIMARY KEY IDENTITY,
	
	Email				VARCHAR(128)		NOT NULL,
	Code				VARCHAR(6)			NOT NULL,
	
	ExpireDate			DATETIME			NOT NULL,
);