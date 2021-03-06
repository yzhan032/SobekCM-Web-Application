
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



alter table mySobek_DefaultMetadata add UserID int null;
GO

alter table mySobek_DefaultMetadata add [Description] varchar(255) not null default('');
GO

update mySobek_DefaultMetadata set [Description] = MetadataName;
GO


-- Procedure to delete a default metadata set
-- were linked to this web skin
CREATE PROCEDURE [dbo].[mySobek_Delete_DefaultMetadata]
	@MetadataCode varchar(20)
AS
BEGIN

	if ( @MetadataCode != 'NONE' )
	begin
		delete from mySobek_DefaultMetadata where MetadataCode=@MetadataCode;
	end;

END;
GO

GRANT EXECUTE ON [dbo].[mySobek_Delete_DefaultMetadata] to sobek_user;
GO


if ( not exists ( select * from mySobek_DefaultMetadata where MetadataCode='NONE' ))
begin
	insert into mySobek_DefaultMetadata ( MetadataCode, [MetadataName], [Description] )
	values ( 'NONE', 'No default values', 'Default metadata set which represents NO default metadata' );
end;
GO

drop table mySobek_User_Project_Link;
GO

drop table mySobek_User_Group_Project_Link;
GO

drop table mySobek_Project;
GO

drop procedure mySobek_Delete_Project;
GO


-- Add a new default metadata set to this database
ALTER PROCEDURE [dbo].[mySobek_Save_DefaultMetadata]
	@metadata_code varchar(20),
	@metadata_name varchar(100),
	@description varchar(255),
	@userid int
AS
BEGIN
	
	-- Does this project already exist?
	if (( select count(*) from mySobek_DefaultMetadata where MetadataCode=@metadata_code ) > 0 )
	begin
		-- Update the existing default metadata
		update mySobek_DefaultMetadata
		set [Description] = @description, [MetadataName] = @metadata_name
		where MetadataCode = @metadata_code;
	end
	else
	begin
		-- Add a new set
		insert into mySobek_DefaultMetadata ( [Description], MetadataCode, UserID, MetadataName )
		values ( @description, @metadata_code, @userid, @metadata_name );
	end;
END;
GO

alter table mySobek_Template add [Description] varchar(255) not null default('');
GO

update mySobek_Template set [Description] = TemplateName;
GO


-- Add a new template to this database
ALTER PROCEDURE [dbo].[mySobek_Save_Template]
	@template_code varchar(20),
	@template_name varchar(100),
	@description varchar(255)
AS
BEGIN
	
	-- Does this template already exist?
	if (( select count(*) from mySobek_Template where TemplateCode=@template_code ) > 0 )
	begin
		-- Update the existing template
		update mySobek_Template
		set TemplateName = @template_name, [Description]=@description
		where TemplateCode = @template_code
	end
	else
	begin
		-- Add a new template
		insert into mySobek_Template ( TemplateName, TemplateCode, [Description] )
		values ( @template_name, @template_code, @description )
	end
END
GO


-- Get the list of all templates and default metadata sets 
ALTER PROCEDURE [dbo].[mySobek_Get_All_Template_DefaultMetadatas]
AS
BEGIN
	
	select MetadataCode, MetadataName, [Description], UserID
	from mySobek_DefaultMetadata
	order by MetadataCode;

	select TemplateCode, TemplateName, [Description]
	from mySobek_Template
	order by TemplateCode;

END;
GO

alter table SobekCM_IP_Restriction_Range add Deleted bit not null default('false');
GO

ALTER PROCEDURE [dbo].[SobekCM_Get_All_IP_Restrictions]
AS
BEGIN

	-- No need to perform any locks here
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- Get all the IP information
	select R.Title, R.IP_RangeID, R.Not_Valid_Statement, isnull(S.StartIP,'') as StartIP, isnull(S.EndIP,'') as EndIP, coalesce(R.Notes,'') as Notes
	from SobekCM_IP_Restriction_Range AS R LEFT JOIN 
	     SobekCM_IP_Restriction_Single AS S ON R.IP_RangeID = S.IP_RangeID
	where R.Deleted = 'false'
	order by IP_RangeID ASC;

END;
GO

CREATE PROCEDURE [dbo].[SobekCM_Delete_IP_Range]
	@rangeid int
AS
BEGIN
	UPDATE SobekCM_IP_Restriction_Range set Deleted='TRUE' where IP_RangeID=@rangeid;
END;
GO

GRANT EXECUTE ON dbo.SobekCM_Delete_IP_Range to sobek_user;
GO

