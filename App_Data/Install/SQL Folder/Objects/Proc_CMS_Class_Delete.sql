CREATE PROCEDURE [Proc_CMS_Class_Delete]
	@ClassID int
AS
BEGIN
  DELETE FROM CMS_Class WHERE ClassID = @ClassID
END
