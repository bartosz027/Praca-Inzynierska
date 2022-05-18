USE PI_DB;

CREATE TABLE Account (
    ID                  INT                 NOT NULL        PRIMARY KEY IDENTITY,
    Username            VARCHAR(64)         NOT NULL,

    Email               VARCHAR(128)        NOT NULL,
    Password            VARCHAR(1024)       NOT NULL,
	
	-- NP:Friendships -> rename_to("Friends")
	-- NP:Friendships1 -> remove_property(true)
	
	-- NP:PrivateMessages -> rename_to("PrivateMessagesReceived")
	-- NP:PrivateMessages1 -> rename_to("PrivateMessagesSent")
);

CREATE TABLE Friendship (
	ID 					INT					NOT NULL		PRIMARY KEY IDENTITY,

    UserID              INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
    FriendID            INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
	
	-- NP:Account -> rename_to("Friend")
	-- NP:Account1 -> rename_to("User")
);

CREATE TABLE PrivateMessage (
    ID                  INT                 NOT NULL        PRIMARY KEY IDENTITY,
    Content             VARCHAR(5000)       NOT NULL,

    SenderID            INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
    ReceiverID          INT                 NOT NULL        FOREIGN KEY REFERENCES Account(ID),
	
	-- NP:Account -> rename_to("Receiver")
	-- NP:Account1 -> rename_to("Sender")
);