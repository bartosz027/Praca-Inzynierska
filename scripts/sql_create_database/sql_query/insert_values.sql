USE PI_DB;

-- Account
SET IDENTITY_INSERT [dbo].[Account] ON
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password], [Verified]) VALUES (1, N'pudzian028', N'test1@gmail.com', N'okon1', 1)
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password], [Verified]) VALUES (2, N'karmelek17', N'test2@gmail.com', N'okon2', 1)
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password], [Verified]) VALUES (3, N'pawelek216', N'test3@gmail.com', N'okon3', 1)
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password], [Verified]) VALUES (4, N'marian2000', N'test4@gmail.com', N'okon4', 1)
SET IDENTITY_INSERT [dbo].[Account] OFF

-- Friendship
SET IDENTITY_INSERT [dbo].[Friendship] ON
	INSERT INTO [dbo].[Friendship] ([ID], [UserID], [FriendID]) VALUES (1, 1, 2)
	INSERT INTO [dbo].[Friendship] ([ID], [UserID], [FriendID]) VALUES (2, 2, 1)

	INSERT INTO [dbo].[Friendship] ([ID], [UserID], [FriendID]) VALUES (3, 1, 3)
	INSERT INTO [dbo].[Friendship] ([ID], [UserID], [FriendID]) VALUES (4, 3, 1)

	INSERT INTO [dbo].[Friendship] ([ID], [UserID], [FriendID]) VALUES (5, 2, 3)
	INSERT INTO [dbo].[Friendship] ([ID], [UserID], [FriendID]) VALUES (6, 3, 2)
SET IDENTITY_INSERT [dbo].[Friendship] OFF

-- Message
SET IDENTITY_INSERT [dbo].[Message] ON
	INSERT INTO [dbo].[Message] ([ID], [Content],[SendDate], [SenderID], [ReceiverID]) VALUES (1, N'Message sent by pudzian028', N'2022-06-09 21:37:00', 1, 2)
	INSERT INTO [dbo].[Message] ([ID], [Content],[SendDate], [SenderID], [ReceiverID]) VALUES (2, N'Message sent by karmelek17', N'2022-06-09 21:39:00', 2, 1)

	INSERT INTO [dbo].[Message] ([ID], [Content],[SendDate], [SenderID], [ReceiverID]) VALUES (3, N'Message sent by pawelek216', N'2022-06-09 10:39:00', 3, 1)
	INSERT INTO [dbo].[Message] ([ID], [Content],[SendDate], [SenderID], [ReceiverID]) VALUES (4, N'Message sent by pudzian028', N'2022-06-09 12:37:00', 1, 3)

	INSERT INTO [dbo].[Message] ([ID], [Content],[SendDate], [SenderID], [ReceiverID]) VALUES (5, N'Message sent by karmelek17', N'2022-06-09 15:03:00', 2, 3)
	INSERT INTO [dbo].[Message] ([ID], [Content],[SendDate], [SenderID], [ReceiverID]) VALUES (6, N'Message sent by pawelek216', N'2022-06-09 03:15:00', 3, 2)
SET IDENTITY_INSERT [dbo].[Message] OFF