CREATE TABLE [COM_OptionCategory] (
		[CategoryID]                  [int] IDENTITY(1, 1) NOT NULL,
		[CategoryDisplayName]         [nvarchar](200) NULL,
		[CategoryName]                [nvarchar](200) NOT NULL,
		[CategorySelectionType]       [nvarchar](200) NOT NULL,
		[CategoryDefaultOptions]      [nvarchar](200) NULL,
		[CategoryDescription]         [nvarchar](max) NULL,
		[CategoryDefaultRecord]       [nvarchar](200) NOT NULL,
		[CategoryEnabled]             [bit] NOT NULL,
		[CategoryGUID]                [uniqueidentifier] NOT NULL,
		[CategoryLastModified]        [datetime] NOT NULL,
		[CategoryDisplayPrice]        [bit] NULL,
		[CategorySiteID]              [int] NULL,
		[CategoryTextMaxLength]       [int] NULL,
		[CategoryFormControlName]     [nvarchar](200) NULL,
		[CategorySKUID]               [int] NULL,
		[CategoryType]                [nvarchar](20) NULL
)  
ALTER TABLE [COM_OptionCategory]
	ADD
	CONSTRAINT [PK_COM_OptionCategory]
	PRIMARY KEY
	NONCLUSTERED
	([CategoryID])
	WITH FILLFACTOR=80
	
ALTER TABLE [COM_OptionCategory]
	ADD
	CONSTRAINT [DEFAULT_COM_OptionCategory_CategoryDisplayPrice]
	DEFAULT ((1)) FOR [CategoryDisplayPrice]
ALTER TABLE [COM_OptionCategory]
	ADD
	CONSTRAINT [DEFAULT_COM_OptionCategory_CategoryEnabled]
	DEFAULT ((0)) FOR [CategoryEnabled]
CREATE CLUSTERED INDEX [IX_COM_OptionCategory_CategoryDisplayName_CategoryEnabled]
	ON [COM_OptionCategory] ([CategoryDisplayName], [CategoryEnabled])
	
ALTER TABLE [COM_OptionCategory]
	WITH CHECK
	ADD CONSTRAINT [FK_COM_OptionCategory_CategorySiteID_CMS_Site]
	FOREIGN KEY ([CategorySiteID]) REFERENCES [CMS_Site] ([SiteID])
ALTER TABLE [COM_OptionCategory]
	CHECK CONSTRAINT [FK_COM_OptionCategory_CategorySiteID_CMS_Site]
