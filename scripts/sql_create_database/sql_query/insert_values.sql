USE PI_DB;

-- Account
SET IDENTITY_INSERT [dbo].[Account] ON
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password]) VALUES (1, N'pudzian028', N'test1@gmail.com', N'okon')
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password]) VALUES (2, N'karmelek17', N'test2@gmail.com', N'okon')
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password]) VALUES (3, N'pawelek216', N'test3@gmail.com', N'okon')
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password]) VALUES (4, N'marian2000', N'test4@gmail.com', N'okon')
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

-- PrivateMessage
SET IDENTITY_INSERT [dbo].[PrivateMessage] ON
	INSERT INTO [dbo].[PrivateMessage] ([ID], [Content], [SenderID], [ReceiverID]) VALUES (1, N'Message sent by pudzian028', 1, 2)
	INSERT INTO [dbo].[PrivateMessage] ([ID], [Content], [SenderID], [ReceiverID]) VALUES (2, N'Message sent by karmelek17', 2, 1)

	INSERT INTO [dbo].[PrivateMessage] ([ID], [Content], [SenderID], [ReceiverID]) VALUES (3, N'Message sent by pawelek216', 3, 1)
	INSERT INTO [dbo].[PrivateMessage] ([ID], [Content], [SenderID], [ReceiverID]) VALUES (4, N'Message sent by pudzian028', 1, 3)

	INSERT INTO [dbo].[PrivateMessage] ([ID], [Content], [SenderID], [ReceiverID]) VALUES (5, N'Message sent by karmelek17', 2, 3)
	INSERT INTO [dbo].[PrivateMessage] ([ID], [Content], [SenderID], [ReceiverID]) VALUES (6, N'Message sent by pawelek216', 3, 2)
SET IDENTITY_INSERT [dbo].[PrivateMessage] OFF