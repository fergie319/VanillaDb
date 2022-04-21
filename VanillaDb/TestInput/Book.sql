/*
{
  "GetAll": true,
  "Insert": true,
  "Update": true,
  "Delete": true
}
*/
CREATE TABLE [dbo].[Book]
(
	[BookId] INT NOT NULL IDENTITY PRIMARY KEY, 
    [ISBN10] NCHAR(10), 
    [ISBN13] NCHAR(13) NULL, 
    [ASIN10] NCHAR(10) NULL,
    [UpdateDateUtc] DateTime NOT NULL,
    [ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.BookHistory))
GO;

CREATE UNIQUE NONCLUSTERED INDEX IDX_ISBN13_NOTNULL
ON [dbo].[Book](ISBN13)
WHERE ISBN13 IS NOT NULL
GO;

CREATE UNIQUE NONCLUSTERED INDEX IDX_ASIN_NOTNULL
ON [dbo].[Book]([ASIN10])
WHERE [ASIN10] IS NOT NULL
GO;

CREATE NONCLUSTERED INDEX IDX_UpdateDateUtc_Asin
ON [dbo].[Book]([UpdateDateUtc], [ASIN10])
GO;
