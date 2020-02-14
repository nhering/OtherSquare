DECLARE @Delete TINYINT = 0

IF @Delete = 1
BEGIN
	DELETE FROM [ndh_otsq].[dbo].[UserSetting] 
	DELETE FROM [ndh_otsq].[lms].[Category] 
	DELETE FROM [ndh_otsq].[lms].[FlashCard] 
	DELETE FROM [ndh_otsq].[lms].[FlashCardAnswer] 
	DELETE FROM [ndh_otsq].[lms].[Subject] 
END

SELECT * FROM [ndh_otsq].[dbo].[UserSetting]
SELECT * FROM [ndh_otsq].[lms].[Category]
SELECT * FROM [ndh_otsq].[lms].[FlashCard]
SELECT * FROM [ndh_otsq].[lms].[FlashCardAnswer]
SELECT * FROM [ndh_otsq].[lms].[Subject]