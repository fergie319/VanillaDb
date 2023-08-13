/*
{
  "GetAll": true,
  "Insert": true,
  "Update": true,
  "Delete": true,
  "TableAlias": "Book",
  "TemporalGetAll": true,
  "TemporalGetAsOf": true
}
*/
-- Some random comment
CREATE TABLE [dbo].[Book] -- Another comment
(
	[BookId] INT NOT NULL IDENTITY PRIMARY KEY, 
    [ISBN10] NCHAR(10), 
    [ISBN13] NCHAR(13) NULL,    -- More random comments
    [ASIN10] NCHAR(10) NULL,
    [UpdateDateUtc] DateTime NOT NULL,
    [ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.BookHistory)) -- Blarg!
GO;

CREATE UNIQUE NONCLUSTERED INDEX IDX_ISBN13_NOTNULL
ON [dbo].[Book](ISBN13) -- Comments in an index
WHERE ISBN13 IS NOT NULL
-- Comments before GO!
GO;

CREATE UNIQUE NONCLUSTERED INDEX IDX_ASIN_NOTNULL -- comments at the beginning of an index
ON [dbo].[Book]([ASIN10])
WHERE [ASIN10] IS NOT NULL
GO; -- Comments on GO!

-- Comments in between statements

CREATE NONCLUSTERED INDEX IDX_UpdateDateUtc_Asin
ON [dbo].[Book]([UpdateDateUtc], [ASIN10])
GO;
