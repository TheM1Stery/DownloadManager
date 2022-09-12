CREATE TABLE Downloads
(
    Id INTEGER NOT NULL CONSTRAINT PK_Downloads PRIMARY KEY,
    Name TEXT NOT NULL,
    Size INTEGER,
    InstalledPath TEXT NOT NULL,
    LinkToDownload TEXT NOT NULL,
    IsFinished INT CONSTRAINT DF_Downloads_IsFinished DEFAULT 0
);

CREATE TABLE Tags
(
    Id INTEGER NOT NULL CONSTRAINT PK_Downloads PRIMARY KEY,
    Name TEXT NOT NULL,
    DownloadId INTEGER NOT NULL,
    CONSTRAINT FK_Tags_Downloads FOREIGN KEY(Id) REFERENCES Downloads(ID) ON DELETE CASCADE
);