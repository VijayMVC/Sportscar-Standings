CREATE PROCEDURE [Proc_CMS_Site_Select]
	@SiteID int
AS
BEGIN
  SELECT * FROM CMS_Site WHERE SiteID = @SiteID
END
