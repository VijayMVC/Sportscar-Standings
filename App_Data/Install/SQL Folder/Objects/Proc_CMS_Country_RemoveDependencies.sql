CREATE PROCEDURE [Proc_CMS_Country_RemoveDependencies]
	@ID int
AS
BEGIN
	BEGIN TRANSACTION;
	
	DELETE FROM COM_Address WHERE AddressCountryID = @ID
	
	DELETE FROM COM_Customer WHERE CustomerCountryID = @ID
	
	DELETE FROM COM_TaxClassCountry WHERE CountryID = @ID
	
	-- Remove state dependencies - E-commerce
	UPDATE COM_Address SET AddressStateID = NULL WHERE AddressStateID IN (SELECT StateID FROM CMS_State WHERE CountryID = @ID)
	UPDATE COM_Customer SET CustomerStateID = NULL WHERE CustomerStateID IN (SELECT StateID FROM CMS_State WHERE CountryID = @ID)
	DELETE FROM COM_TaxClassState WHERE TaxClassStateID IN (SELECT StateID FROM CMS_State WHERE CountryID = @ID)
	
	DELETE FROM CMS_State WHERE CountryID = @ID
	
	COMMIT TRANSACTION;
END
