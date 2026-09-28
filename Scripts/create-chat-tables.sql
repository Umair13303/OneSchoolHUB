IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChatConversations')
BEGIN
    CREATE TABLE ChatConversations (
        ChatConversationId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(255) NULL,
        ConversationType NVARCHAR(50) NOT NULL DEFAULT 'direct',
        ClassId INT NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedBy INT NULL,
        UpdatedBy INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        InstituteId INT NULL,
        CampusId INT NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChatConversationMembers')
BEGIN
    CREATE TABLE ChatConversationMembers (
        MemberId INT IDENTITY(1,1) PRIMARY KEY,
        ConversationId INT NOT NULL,
        UserId INT NOT NULL,
        IsAdmin BIT NOT NULL DEFAULT 0,
        JoinedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_ChatConversationMembers_ChatConversations FOREIGN KEY (ConversationId) REFERENCES ChatConversations(ChatConversationId) ON DELETE CASCADE,
        CONSTRAINT FK_ChatConversationMembers_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChatMessages')
BEGIN
    CREATE TABLE ChatMessages (
        ChatMessageId INT IDENTITY(1,1) PRIMARY KEY,
        ConversationId INT NOT NULL,
        SenderId INT NOT NULL,
        Content NVARCHAR(MAX) NOT NULL DEFAULT '',
        SentAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AttachmentUrl NVARCHAR(MAX) NULL,
        AttachmentName NVARCHAR(255) NULL,
        AttachmentType NVARCHAR(50) NULL,
        AttachmentSize BIGINT NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedBy INT NULL,
        UpdatedBy INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        InstituteId INT NULL,
        CampusId INT NULL,
        CONSTRAINT FK_ChatMessages_ChatConversations FOREIGN KEY (ConversationId) REFERENCES ChatConversations(ChatConversationId) ON DELETE CASCADE,
        CONSTRAINT FK_ChatMessages_Users FOREIGN KEY (SenderId) REFERENCES Users(UserId)
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChatMessageReads')
BEGIN
    CREATE TABLE ChatMessageReads (
        ReadId INT IDENTITY(1,1) PRIMARY KEY,
        MessageId INT NOT NULL,
        UserId INT NOT NULL,
        ReadAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_ChatMessageReads_ChatMessages FOREIGN KEY (MessageId) REFERENCES ChatMessages(ChatMessageId) ON DELETE CASCADE,
        CONSTRAINT FK_ChatMessageReads_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END;
