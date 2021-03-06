--Create the Project Table
IF NOT EXISTS
(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SobekCM_Project]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.SobekCM_Project
	(
	ProjectID int NOT NULL IDENTITY (1, 1),
	ProjectCode nvarchar(20) NULL,
	ProjectName nvarchar(100) NOT NULL,
	ProjectManager nvarchar(100) NULL,
	GrantID nvarchar(20) NULL,
	GrantName nvarchar(250) NULL,
	StartDate date NULL,
	EndDate date NULL,
	isActive bit NULL,
	Description nvarchar(1000) NULL,
	Specifications nvarchar(1000) NULL,
	Priority nvarchar(100) NULL,
	QC_Profile nvarchar(100) NULL,
	TargetItemCount int NULL,
	TargetPageCount int NULL,
	Comments nvarchar(1000) NULL,
	CopyrightPermissions nvarchar(1000) NULL
	)  ON [PRIMARY]
END
GO
ALTER TABLE dbo.SobekCM_Project ADD CONSTRAINT
	PK_SobekCM_Project PRIMARY KEY CLUSTERED 
	(
	ProjectID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.SobekCM_Project SET (LOCK_ESCALATION = TABLE)
GO

GRANT EXECUTE ON [dbo].[SobekCM_Project] to sobek_user;


--Create the Project-Aggregation Link table
IF NOT EXISTS
(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SobekCM_Project_Aggregation_Link]') AND type in (N'U'))
BEGIN

CREATE TABLE dbo.SobekCM_Project_Aggregation_Link
	(
	ProjectID int NOT NULL,
	AggregationID int NOT NULL
	)  ON [PRIMARY]
END
GO
ALTER TABLE dbo.SobekCM_Project_Aggregation_Link ADD CONSTRAINT
	PK_SobekCM_Project_Aggregation_Link PRIMARY KEY CLUSTERED 
	(
	ProjectID,
	AggregationID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.SobekCM_Project_Aggregation_Link SET (LOCK_ESCALATION = TABLE)
GO

--Add the foreign keys
ALTER TABLE dbo.SobekCM_Project_Aggregation_Link ADD CONSTRAINT FK_Project_Aggregation
FOREIGN KEY(ProjectID) REFERENCES SobekCM_Project(ProjectID)
ON DELETE CASCADE
ON UPDATE CASCADE
GO

ALTER TABLE dbo.SobekCM_Project_Aggregation_Link ADD CONSTRAINT FK_Aggregation
FOREIGN KEY(AggregationID) REFERENCES mySobek_Project(ProjectID)
ON DELETE CASCADE
ON UPDATE CASCADE
GO

GRANT EXECUTE ON [dbo].[SobekCM_Project_Aggregation_Link] to sobek_user;

--Create the Project - Default Metadata link table

IF NOT EXISTS
(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SobekCM_Project_DefaultMetadata_Link]') AND type in (N'U'))
BEGIN

CREATE TABLE dbo.SobekCM_Project_DefaultMetadata_Link
	(
	ProjectID int NOT NULL,
	DefaultMetadataID int NOT NULL
	)  ON [PRIMARY]

END
ALTER TABLE dbo.SobekCM_Project_DefaultMetadata_Link ADD CONSTRAINT
	PK_SobekCM_Project_DefaultMetadata_Link PRIMARY KEY CLUSTERED 
	(
	ProjectID,
	DefaultMetadataID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

--Add the foreign key constrants
ALTER TABLE dbo.SobekCM_Project_DefaultMetadata_Link SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE dbo.SobekCM_Project_DefaultMetadata_Link ADD CONSTRAINT FK_Project
FOREIGN KEY(ProjectID) REFERENCES SobekCM_Project(ProjectID)
ON DELETE CASCADE
ON UPDATE CASCADE
GO

ALTER TABLE dbo.SobekCM_Project_DefaultMetadata_Link ADD CONSTRAINT FK_DefaultMetadata
FOREIGN KEY(DefaultMetadataID) REFERENCES mySobek_DefaultMetadata(DefaultMetadataID)
ON DELETE CASCADE
ON UPDATE CASCADE
GO

GRANT EXECUTE ON [dbo].[SobekCM_Project_DefaultMetadata_Link] to sobek_user;

--Create the Project-Template Link table
IF NOT EXISTS
(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SobekCM_Project_Template_Link]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.SobekCM_Project_Template_Link
	(
	ProjectID int NOT NULL,
	TemplateID int NOT NULL
	)  ON [PRIMARY]
END

ALTER TABLE dbo.SobekCM_Project_Template_Link ADD CONSTRAINT
	PK_SobekCM_Project_Template_Link PRIMARY KEY CLUSTERED 
	(
	ProjectID,
	TemplateID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.SobekCM_Project_Template_Link SET (LOCK_ESCALATION = TABLE)
GO

--Add the foreign key constraints
ALTER TABLE dbo.SobekCM_Project_Template_Link ADD CONSTRAINT FK_Project_1
FOREIGN KEY(ProjectID) REFERENCES SobekCM_Project(ProjectID)
ON DELETE CASCADE
ON UPDATE CASCADE
GO

ALTER TABLE dbo.SobekCM_Project_Template_Link ADD CONSTRAINT FK_Template
FOREIGN KEY(TemplateID) REFERENCES mySobek_Template(TemplateID)
ON DELETE CASCADE
ON UPDATE CASCADE
GO

GRANT EXECUTE ON [dbo].[SobekCM_Project_Template_Link] to sobek_user;

--Create the Project_Item Link Table
IF NOT EXISTS
(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SobekCM_Project_Template_Link]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.SobekCM_Project_Item_Link
	(
	ProjectID int NOT NULL,
	ItemID int NOT NULL
	)  ON [PRIMARY]
END

ALTER TABLE dbo.SobekCM_Project_Item_Link ADD CONSTRAINT
	PK_SobekCM_Project_Item_Link PRIMARY KEY CLUSTERED 
	(
	ProjectID,
	ItemID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.SobekCM_Project_Item_Link SET (LOCK_ESCALATION = TABLE)
GO

--Create the foreign key constraints
ALTER TABLE dbo.SobekCM_Project_Item_Link ADD CONSTRAINT FK_Project_Item_ProjectID
FOREIGN KEY(ProjectID) REFERENCES SobekCM_Project(ProjectID)
ON DELETE CASCADE
ON UPDATE CASCADE
GO

ALTER TABLE dbo.SobekCM_Project_Item_Link ADD CONSTRAINT FK_Project_Item_ItemID
FOREIGN KEY(ItemID) REFERENCES SobekCM_Item(ItemID)
ON DELETE CASCADE
ON UPDATE CASCADE
GO

GRANT EXECUTE ON [dbo].[SobekCM_Project_Item_Link] to sobek_user;

--Create the stored procedure to save/edit a Project

-- Ensure the stored procedure exists
IF object_id('SobekCM_Save_Project') IS NULL EXEC ('create procedure dbo.SobekCM_Save_Project as select 1;');
GO



ALTER PROCEDURE [dbo].[SobekCM_Save_Project]
	@ProjectID int,
	@ProjectCode nvarchar(20),
	@ProjectName nvarchar(100),
	@ProjectManager nvarchar(100),
	@GrantID nvarchar(250),
	@GrantName bigint,
	@StartDate date,
	@EndDate date,
	@isActive bit,
	@Description nvarchar(MAX),
	@Specifications nvarchar(MAX),
	@Priority nvarchar(100),
	@QC_Profile nvarchar(100),
	@TargetItemCount int,
	@TargetPageCount int,
	@Comments nvarchar(MAX),
	@CopyrightPermissions nvarchar(1000),
	@New_ProjectID int output
	
AS
  Begin transaction

	-- Set the return ProjectID value first
	set @New_ProjectID = @ProjectID;
	

	-- If this project does not exist (ProjectID) insert, else update
	if (( select count(*) from SobekCM_Project  where ( ProjectID = @ProjectID ))  < 1 )
	   begin	
	    	-- begin insert
		    insert into SobekCM_Project (ProjectCode, ProjectName, ProjectManager, GrantID, GrantName, StartDate, EndDate, isActive, [Description], Specifications, [Priority],QC_Profile, TargetItemCount, TargetPageCount, Comments, CopyrightPermissions)
		    values (@ProjectCode, @ProjectName, @ProjectManager, @GrantID, @GrantName, @StartDate, @EndDate, @isActive, @Description, @Specifications, @Priority, @QC_Profile, @TargetItemCount, @TargetPageCount, @Comments, @CopyrightPermissions);
     	--Get the new ProjectID for this row
     	set @New_ProjectID = @@IDENTITY;
     	end
	else
	    begin
	    --update the corresponding row in the SobekCM_Project table
	    update SobekCM_Project
	    set ProjectCode=@ProjectCode, ProjectName=@ProjectName, ProjectManager=@ProjectManager, GrantID=@GrantID, GrantName=@GrantName, StartDate=@StartDate, EndDate=@EndDate, isActive=@isActive, [Description]=@Description, Specifications=@Specifications, [Priority]=@Priority, QC_Profile=@QC_Profile, TargetItemCount=@TargetItemCount, TargetPageCount=@TargetPageCount, Comments=@Comments, CopyrightPermissions=@CopyrightPermissions
	    where ProjectID=@ProjectID;
	    end	
		
commit transaction;		
	

--Stored procedure for creating a Project_Aggregation Link
-- Ensure the stored procedure exists
IF object_id('SobekCM_Save_Project_Aggregation_Link') IS NULL EXEC ('create procedure dbo.SobekCM_Save_Project_Aggregation_Link as select 1;');
GO



ALTER PROCEDURE [dbo].[SobekCM_Save_Project_Aggregation_Link]
	@ProjectID int,
	@AggregationID int
	
AS
Begin
  --If this link does not already exist, insert it
  if((select count(*) from SobekCM_Project_Aggregation_Link  where ( ProjectID = @ProjectID and AggregationID=@AggregationID ))  < 1 )
    insert into SobekCM_Project_Aggregation_Link(ProjectID, AggregationID)
    values(@ProjectID, @AggregationID);
End


--Stored procedure to insert a new Project-DefaultMetadata Link
-- Ensure the stored procedure exists
IF object_id('SobekCM_Save_Project_DefaultMetadata_Link') IS NULL EXEC ('create procedure dbo.SobekCM_Save_Project_DefaultMetadata_Link as select 1;');
GO



ALTER PROCEDURE [dbo].[SobekCM_Save_Project_DefaultMetadata_Link]
	@ProjectID int,
	@DefaultMetadataID int
	
AS
Begin
  --If this link does not already exist, insert it
  if((select count(*) from SobekCM_Project_DefaultMetadata_Link  where ( ProjectID = @ProjectID and DefaultMetadataID=@DefaultMetadataID ))  < 1 )
    insert into SobekCM_Project_DefaultMetadata_Link(ProjectID, DefaultMetadataID)
    values(@ProjectID, @DefaultMetadataID);
End


--Stored procedure to create Project - Template Link
-- Ensure the stored procedure exists
IF object_id('SobekCM_Save_Project_Template_Link') IS NULL EXEC ('create procedure dbo.SobekCM_Save_Project_Template_Link as select 1;');
GO



ALTER PROCEDURE [dbo].[SobekCM_Save_Project_Template_Link]
	@ProjectID int,
	@TemplateID int
	
AS
Begin
  --If this link does not already exist, insert it
  if((select count(*) from SobekCM_Project_Template_Link  where ( ProjectID = @ProjectID and TemplateID=@TemplateID ))  < 1 )
    insert into SobekCM_Project_Template_Link(ProjectID, TemplateID)
    values(@ProjectID, @TemplateID);
End


--Stored procedure for creating a new Project-Item link
-- Ensure the stored procedure exists
IF object_id('SobekCM_Save_Project_Item_Link') IS NULL EXEC ('create procedure dbo.SobekCM_Save_Project_Item_Link as select 1;');
GO



ALTER PROCEDURE [dbo].[SobekCM_Save_Project_Item_Link]
	@ProjectID int,
	@ItemID int
	
AS
Begin
  --If this link does not already exist, insert it
  if((select count(*) from SobekCM_Project_Item_Link  where ( ProjectID = @ProjectID and ItemID=@ItemID ))  < 1 )
    insert into SobekCM_Project_Item_Link(ProjectID, ItemID)
    values(@ProjectID, @ItemID);
End


--Delete a Project-Item link
-- Ensure the stored procedure exists
IF object_id('SobekCM_Delete_Project_Item_Link') IS NULL EXEC ('create procedure dbo.SobekCM_Delete_Project_Item_Link as select 1;');
GO



ALTER PROCEDURE [dbo].[SobekCM_Delete_Project_Item_Link]
	@ProjectID int,
	@ItemID int
	
AS
Begin
  --If this link exists, delete it
  if((select count(*) from SobekCM_Project_Item_Link  where ( ProjectID = @ProjectID and ItemID=@ItemID ))  = 1 )
    delete from SobekCM_Project_Item_Link
    where (ProjectID=@ProjectID and ItemID=@ItemID);
End

--Delete a Project-DefaultMetadata Link
-- Ensure the stored procedure exists
IF object_id('SobekCM_Delete_Project_DefaultMetadata_Link') IS NULL EXEC ('create procedure dbo.SobekCM_Delete_Project_DefaultMetadata_Link as select 1;');
GO



ALTER PROCEDURE [dbo].[SobekCM_Delete_Project_DefaultMetadata_Link]
	@ProjectID int,
	@DefaultMetadataID int
	
AS
Begin
  --If this link exists, delete it
  if((select count(*) from SobekCM_Project_DefaultMetadata_Link  where ( ProjectID = @ProjectID and DefaultMetadataID=@DefaultMetadataID ))  = 1 )
    delete from SobekCM_Project_DefaultMetadata_Link
    where (ProjectID=@ProjectID and DefaultMetadataID=@DefaultMetadataID);
End


--Delete Project-Input Template link

-- Ensure the stored procedure exists
IF object_id('SobekCM_Delete_Project_Template_Link') IS NULL EXEC ('create procedure dbo.SobekCM_Delete_Project_Template_Link as select 1;');
GO



ALTER PROCEDURE [dbo].[SobekCM_Delete_Project_Template_Link]
	@ProjectID int,
	@TemplateID int
	
AS
Begin
  --If this link exists, delete it
  if((select count(*) from SobekCM_Project_Template_Link  where ( ProjectID = @ProjectID and TemplateID=@TemplateID ))  = 1 )
    delete from SobekCM_Project_Template_Link
    where (ProjectID=@ProjectID and TemplateID=@TemplateID);
End



--Delete the Project-Aggregation Link

-- Ensure the stored procedure exists
IF object_id('SobekCM_Delete_Project_Aggregation_Link') IS NULL EXEC ('create procedure dbo.SobekCM_Delete_Project_Aggregation_Link as select 1;');
GO



ALTER PROCEDURE [dbo].[SobekCM_Delete_Project_Aggregation_Link]
	@ProjectID int,
	@AggregationID int
	
AS
Begin
  --If this link exists, delete it
  if((select count(*) from SobekCM_Project_Aggregation_Link  where ( ProjectID = @ProjectID and AggregationID=@AggregationID ))  = 1 )
    delete from SobekCM_Project_Aggregation_Link
    where (ProjectID=@ProjectID and AggregationID=@AggregationID);
End


--Get the aggregations by ProjectID
-- Ensure the stored procedure exists
IF object_id('SobekCM_Get_Aggregations_By_ProjectID') IS NULL EXEC ('create procedure dbo.SobekCM_Get_Aggregations_By_ProjectID as select 1;');
GO


ALTER PROCEDURE [dbo].[SobekCM_Get_Aggregations_By_ProjectID]
	@ProjectID int
		
AS
Begin
  
  select AggregationID from SobekCM_Project_Aggregation_Link
  where ProjectID=@ProjectID;
 End
 
 
 --Get Items by ProjectID
 -- Ensure the stored procedure exists
IF object_id('SobekCM_Get_Items_By_ProjectID') IS NULL EXEC ('create procedure dbo.SobekCM_Get_Items_By_ProjectID as select 1;');
GO


ALTER PROCEDURE [dbo].[SobekCM_Get_Items_By_ProjectID]
	@ProjectID int
		
AS
Begin
  
  select ItemID from SobekCM_Project_Item_Link
  where ProjectID=@ProjectID;
 End
 
--Get the default metadata by Project ID
-- Ensure the stored procedure exists
IF object_id('SobekCM_Get_DefaultMetadata_By_ProjectID') IS NULL EXEC ('create procedure dbo.SobekCM_Get_DefaultMetadata_By_ProjectID as select 1;');
GO


ALTER PROCEDURE [dbo].[SobekCM_Get_DefaultMetadata_By_ProjectID]
	@ProjectID int
		
AS
Begin
  
  select DefaultMetadataID from SobekCM_Project_DefaultMetadata_Link
  where ProjectID=@ProjectID;
 End
 
 --Get the templates by ProjectID
 -- Ensure the stored procedure exists
IF object_id('SobekCM_Get_Templates_By_ProjectID') IS NULL EXEC ('create procedure dbo.SobekCM_Get_Templates_By_ProjectID as select 1;');
GO


ALTER PROCEDURE [dbo].[SobekCM_Get_Templates_By_ProjectID]
	@ProjectID int
		
AS
Begin
  
  select TemplateID from SobekCM_Project_Template_Link
  where ProjectID=@ProjectID;
 End
 
 