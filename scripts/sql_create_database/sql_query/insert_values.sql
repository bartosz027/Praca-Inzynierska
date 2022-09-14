USE PI_DB;

-- Account
SET IDENTITY_INSERT [dbo].[Account] ON
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password], [Verified]) VALUES (1, N'pudzian028', N'test1@gmail.com', N'0i3K10Z2H2oW73LrX5Iue+nQejTFtN/z7okKE3CETnzwTJnv5Hv/MZ9h14/pA+iK', 1)
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password], [Verified]) VALUES (2, N'karmelek17', N'test2@gmail.com', N'RsB/nj4y4VzaB2jyv1e0h+StcErraDKiv4X++w7v7m91vlEIll4MGYl3yO7r39e+', 1)
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password], [Verified]) VALUES (3, N'pawelek216', N'test3@gmail.com', N'8pI1L1HWIJhcjVF5OoMbQ72BuUtiIe3wj5RxyAwZiTJ4epSB4SdkOyePIu9EVjsw', 1)
	
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password], [Verified]) VALUES (4, N'andrzejek6', N'test4@gmail.com', N'0i3K10Z2H2oW73LrX5Iue+nQejTFtN/z7okKE3CETnzwTJnv5Hv/MZ9h14/pA+iK', 1)
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password], [Verified]) VALUES (5, N'mariuszek9', N'test5@gmail.com', N'RsB/nj4y4VzaB2jyv1e0h+StcErraDKiv4X++w7v7m91vlEIll4MGYl3yO7r39e+', 1)
	INSERT INTO [dbo].[Account] ([ID], [Username], [Email], [Password], [Verified]) VALUES (6, N'kasztan997', N'test6@gmail.com', N'8pI1L1HWIJhcjVF5OoMbQ72BuUtiIe3wj5RxyAwZiTJ4epSB4SdkOyePIu9EVjsw', 1)
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
	INSERT INTO [dbo].[Message] ([ID], [Content], [SendDate], [SenderID], [ReceiverID]) VALUES (1, N'Message sent by pudzian028', N'2022-06-09 21:37:00', 1, 2)
	INSERT INTO [dbo].[Message] ([ID], [Content], [SendDate], [SenderID], [ReceiverID]) VALUES (2, N'Message sent by karmelek17', N'2022-06-09 21:37:10', 2, 1)
	
	INSERT INTO [dbo].[Message] ([ID], [Content], [SendDate], [SenderID], [ReceiverID]) VALUES (3, N'Message sent by pudzian028', N'2022-06-09 21:37:20', 1, 3)
	INSERT INTO [dbo].[Message] ([ID], [Content], [SendDate], [SenderID], [ReceiverID]) VALUES (4, N'Message sent by pawelek216', N'2022-06-09 21:37:30', 3, 1)
	
	INSERT INTO [dbo].[Message] ([ID], [Content], [SendDate], [SenderID], [ReceiverID]) VALUES (5, N'Message sent by karmelek17', N'2022-06-09 21:37:40', 2, 3)
	INSERT INTO [dbo].[Message] ([ID], [Content], [SendDate], [SenderID], [ReceiverID]) VALUES (6, N'Message sent by pawelek216', N'2022-06-09 21:37:50', 3, 2)
SET IDENTITY_INSERT [dbo].[Message] OFF