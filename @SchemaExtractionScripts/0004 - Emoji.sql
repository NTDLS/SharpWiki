SET NOCOUNT ON;

DECLARE @Text TABLE (Id Int NOT NULL IDENTITY(1,1), [Text] NVARCHAR(MAX))
DECLARE @EmojiId INT
DECLARE @Name as nvarchar(128)
DECLARE @Path as nvarchar(256)
DECLARE @Category as nvarchar(256)
DECLARE @Categories as nvarchar(2000) = ''

DECLARE Pages CURSOR FAST_FORWARD FOR
	SELECT
		[Id],
		[Name],
		[Path]
	FROM
		[Emoji]
OPEN Pages
FETCH FROM Pages INTO @EmojiId, @Name, @Path

WHILE(@@FETCH_STATUS = 0)
BEGIN--WHILE
	--------------------------------------------------------------------------------------------------

	SET @Categories = ''

	DECLARE Categories CURSOR FAST_FORWARD FOR
		SELECT
			[Category]
		FROM
			[EmojiCategory]
		WHERE
			EmojiId = @EmojiId
	OPEN Categories
	FETCH FROM Categories INTO @Category

	WHILE(@@FETCH_STATUS = 0)
	BEGIN--WHILE
		--------------------------------------------------------------------------------------------------

		SET @Categories = @Categories + @Category

		--------------------------------------------------------------------------------------------------
		FETCH FROM Categories INTO @Category

		IF(@@FETCH_STATUS = 0)
		BEGIN--IF
			SET @Categories = @Categories + ',' 
		END--IF

	END--WHILE

	CLOSE Categories DEALLOCATE Categories

	INSERT INTO @Text SELECT 'EXEC SaveEmoji @Name=''' + @Name + ''', @Path=''' + @Path + ''', @Categories= ''' + @Categories + ''''
	INSERT INTO @Text SELECT 'GO'

	--------------------------------------------------------------------------------------------------
	FETCH FROM Pages INTO @EmojiId, @Name, @Path
END--WHILE

CLOSE Pages DEALLOCATE Pages

SELECT [Text] FROM @Text ORDER BY Id
