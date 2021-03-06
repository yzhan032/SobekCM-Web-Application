
-- Just double check these columns were added
if ( NOT EXISTS (select * from sys.columns where Name = N'Redirect' and Object_ID = Object_ID(N'SobekCM_WebContent')))
BEGIN
	ALTER TABLE [dbo].SobekCM_WebContent add Redirect nvarchar(500) null;
END;
GO

-- Ensure the SobekCM_WebContent_Add stored procedure exists
IF object_id('SobekCM_WebContent_Add') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Add as select 1;');
GO

-- Add a new web content page
ALTER PROCEDURE [dbo].[SobekCM_WebContent_Add]
	@Level1 varchar(100),
	@Level2 varchar(100),
	@Level3 varchar(100),
	@Level4 varchar(100),
	@Level5 varchar(100),
	@Level6 varchar(100),
	@Level7 varchar(100),
	@Level8 varchar(100),
	@UserName nvarchar(100),
	@Title nvarchar(255),
	@Summary nvarchar(1000),
	@Redirect nvarchar(500),
	@WebContentID int output
AS
BEGIN	
	-- Is there a match already for this?
	if ( EXISTS ( select 1 from SobekCM_WebContent 
	              where ( Level1=@Level1 )
	                and ((Level2 is null and @Level2 is null ) or ( Level2=@Level2)) 
					and ((Level3 is null and @Level3 is null ) or ( Level3=@Level3))
					and ((Level4 is null and @Level4 is null ) or ( Level4=@Level4))
					and ((Level5 is null and @Level5 is null ) or ( Level5=@Level5))
					and ((Level6 is null and @Level6 is null ) or ( Level6=@Level6))
					and ((Level7 is null and @Level7 is null ) or ( Level7=@Level7))
					and ((Level8 is null and @Level8 is null ) or ( Level8=@Level8))))
	begin
		-- Get the web content id
		set @WebContentID = (   select top 1 WebContentID 
								from SobekCM_WebContent 
								where ( Level1=@Level1 )
								  and ((Level2 is null and @Level2 is null ) or ( Level2=@Level2)) 
								  and ((Level3 is null and @Level3 is null ) or ( Level3=@Level3))
								  and ((Level4 is null and @Level4 is null ) or ( Level4=@Level4))
								  and ((Level5 is null and @Level5 is null ) or ( Level5=@Level5))
								  and ((Level6 is null and @Level6 is null ) or ( Level6=@Level6))
								  and ((Level7 is null and @Level7 is null ) or ( Level7=@Level7))
								  and ((Level8 is null and @Level8 is null ) or ( Level8=@Level8)));

		-- Ensure the title and summary are correct
		update SobekCM_WebContent set Title=@Title, Summary=@Summary, Redirect=@Redirect where WebContentID=@WebContentID;
		
		-- Was this previously deleted?
		if ( EXISTS ( select 1 from SobekCM_WebContent where Deleted='true' and WebContentID=@WebContentID ))
		begin
			-- Undelete this 
			update SobekCM_WebContent
			set Deleted='false'
			where WebContentID = @WebContentID;

			-- Mark this in the milestones then
			insert into SobekCM_WebContent_Milestones ( WebContentID, Milestone, MilestoneDate, MilestoneUser )
			values ( @WebContentID, 'Restored previously deleted page', getdate(), @UserName );
		end;
	end
	else
	begin
		-- Add the new web content then
		insert into SobekCM_WebContent ( Level1, Level2, Level3, Level4, Level5, Level6, Level7, Level8, Title, Summary, Deleted, Redirect )
		values ( @Level1, @Level2, @Level3, @Level4, @Level5, @Level6, @Level7, @Level8, @Title, @Summary, 'false', @Redirect );

		-- Get the new ID for this
		set @WebContentID = SCOPE_IDENTITY();

		-- Now, add this to the milestones table
		insert into SobekCM_WebContent_Milestones ( WebContentID, Milestone, MilestoneDate, MilestoneUser )
		values ( @WebContentID, 'Add new page', getdate(), @UserName );
	end;
END;
GO


-- Ensure the SobekCM_WebContent_Get_Page stored procedure exists
IF object_id('SobekCM_WebContent_Get_Page') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Get_Page as select 1;');
GO

-- Get basic details about an existing web content page
ALTER PROCEDURE [dbo].[SobekCM_WebContent_Get_Page]
	@Level1 varchar(100),
	@Level2 varchar(100),
	@Level3 varchar(100),
	@Level4 varchar(100),
	@Level5 varchar(100),
	@Level6 varchar(100),
	@Level7 varchar(100),
	@Level8 varchar(100)
AS
BEGIN	
	-- Return the couple of requested pieces of information
	select top 1 W.WebContentID, W.Title, W.Summary, W.Deleted, M.MilestoneDate, M.MilestoneUser, W.Redirect, W.Level1, W.Level2, W.Level3, W.Level4, W.Level5, W.Level6, W.Level7, W.Level8
	from SobekCM_WebContent W left outer join
	     SobekCM_WebContent_Milestones M on W.WebContentID=M.WebContentID
	where ( Level1=@Level1 )
	  and ((Level2 is null and @Level2 is null ) or ( Level2=@Level2)) 
	  and ((Level3 is null and @Level3 is null ) or ( Level3=@Level3))
	  and ((Level4 is null and @Level4 is null ) or ( Level4=@Level4))
	  and ((Level5 is null and @Level5 is null ) or ( Level5=@Level5))
	  and ((Level6 is null and @Level6 is null ) or ( Level6=@Level6))
	  and ((Level7 is null and @Level7 is null ) or ( Level7=@Level7))
	  and ((Level8 is null and @Level8 is null ) or ( Level8=@Level8))
	order by M.MilestoneDate DESC;
END;
GO

-- Ensure the SobekCM_WebContent_Get_Page stored procedure exists
IF object_id('SobekCM_WebContent_Get_Page_ID') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Get_Page_ID as select 1;');
GO

-- Get basic details about an existing web content page
ALTER PROCEDURE [dbo].[SobekCM_WebContent_Get_Page_ID]
	@WebContentID int
AS
BEGIN	
	-- Return the couple of requested pieces of information
	select top 1 W.WebContentID, W.Title, W.Summary, W.Deleted, M.MilestoneDate, M.MilestoneUser, W.Redirect, W.Level1, W.Level2, W.Level3, W.Level4, W.Level5, W.Level6, W.Level7, W.Level8
	from SobekCM_WebContent W left outer join
	     SobekCM_WebContent_Milestones M on W.WebContentID=M.WebContentID
	where W.WebContentID = @WebContentID
	order by M.MilestoneDate DESC;
END;
GO

-- Ensure the SobekCM_WebContent_All stored procedure exists
IF object_id('SobekCM_WebContent_All') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_All as select 1;');
GO

-- Return all the web content pages, regardless of whether they are redirects or an actual content page
ALTER PROCEDURE [dbo].[SobekCM_WebContent_All]
AS
BEGIN

	-- Get the pages, with the time last updated
	with webcontent_last_update as
	(
		select WebContentID, Max(WebContentMilestoneID) as MaxMilestoneID
		from SobekCM_WebContent_Milestones
		group by WebContentID
	)
	select W.WebContentID, W.Level1, W.Level2, W.Level3, W.Level4, W.Level5, W.Level6, W.Level7, W.Level8, W.Title, W.Summary, W.Deleted, W.Redirect, M.MilestoneDate, M.MilestoneUser
	from SobekCM_WebContent W left outer join
		 webcontent_last_update L on L.WebContentID=W.WebContentID left outer join
	     SobekCM_WebContent_Milestones M on M.WebContentMilestoneID=L.MaxMilestoneID
	order by W.Level1, W.Level2, W.Level3, W.Level4, W.Level5, W.Level6, W.Level7, W.Level8;

END;
GO

-- Ensure the SobekCM_WebContent_All_Pages stored procedure exists
IF object_id('SobekCM_WebContent_All_Pages') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_All_Pages as select 1;');
GO

-- Return all the web content pages that are not set as redirects
ALTER PROCEDURE [dbo].[SobekCM_WebContent_All_Pages]
AS
BEGIN

	-- Get the pages, with the time last updated
	with webcontent_last_update as
	(
		select WebContentID, Max(WebContentMilestoneID) as MaxMilestoneID
		from SobekCM_WebContent_Milestones
		group by WebContentID
	)
	select W.WebContentID, W.Level1, W.Level2, W.Level3, W.Level4, W.Level5, W.Level6, W.Level7, W.Level8, W.Title, W.Summary, W.Deleted, W.Redirect, M.MilestoneDate, M.MilestoneUser
	from SobekCM_WebContent W left outer join
		 webcontent_last_update L on L.WebContentID=W.WebContentID left outer join
	     SobekCM_WebContent_Milestones M on M.WebContentMilestoneID=L.MaxMilestoneID
	where ( len(coalesce(W.Redirect,'')) = 0 ) and ( Deleted = 'false' )
	order by W.Level1, W.Level2, W.Level3, W.Level4, W.Level5, W.Level6, W.Level7, W.Level8;

	-- Get the distinct top level pages
	select distinct(W.Level1)
	from SobekCM_WebContent W
	where ( len(coalesce(W.Redirect,'')) = 0 ) and ( Deleted = 'false' )
	order by W.Level1;

	-- Get the distinct top TWO level pages
	select W.Level1, W.Level2
	from SobekCM_WebContent W
	where ( len(coalesce(W.Redirect,'')) = 0 )
	  and ( W.Level2 is not null )
	  and ( Deleted = 'false' )
	group by W.Level1, W.Level2
	order by W.Level1, W.Level2;

END;
GO

-- Ensure the SobekCM_WebContent_All_Redirects stored procedure exists
IF object_id('SobekCM_WebContent_All_Redirects') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_All_Redirects as select 1;');
GO

-- Return all the web content pages that are set as redirects
ALTER PROCEDURE [dbo].[SobekCM_WebContent_All_Redirects]
AS
BEGIN

	-- Get the pages, with the time last updated
	with webcontent_last_update as
	(
		select WebContentID, Max(WebContentMilestoneID) as MaxMilestoneID
		from SobekCM_WebContent_Milestones
		group by WebContentID
	)
	select W.WebContentID, W.Level1, W.Level2, W.Level3, W.Level4, W.Level5, W.Level6, W.Level7, W.Level8, W.Title, W.Summary, W.Deleted, W.Redirect, M.MilestoneDate, M.MilestoneUser
	from SobekCM_WebContent W left outer join
		 webcontent_last_update L on L.WebContentID=W.WebContentID left outer join
	     SobekCM_WebContent_Milestones M on M.WebContentMilestoneID=L.MaxMilestoneID
	where ( len(coalesce(W.Redirect,'')) > 0 ) and ( Deleted = 'false' )
	order by W.Level1, W.Level2, W.Level3, W.Level4, W.Level5, W.Level6, W.Level7, W.Level8;

	-- Get the distinct top level pages
	select distinct(W.Level1)
	from SobekCM_WebContent W
	where ( len(coalesce(W.Redirect,'')) > 0 ) and ( Deleted = 'false' )
	order by W.Level1;

	-- Get the distinct top TWO level pages
	select W.Level1, W.Level2
	from SobekCM_WebContent W
	where ( len(coalesce(W.Redirect,'')) > 0 )
	  and ( W.Level2 is not null )
	  and ( Deleted = 'false' )
	group by W.Level1, W.Level2
	order by W.Level1, W.Level2;

END;
GO

-- Ensure the SobekCM_WebContent_All_Redirects stored procedure exists
IF object_id('SobekCM_WebContent_Get_Recent_Changes') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Get_Recent_Changes as select 1;');
GO

-- Get the list of recent changes to all web content pages
ALTER PROCEDURE [dbo].[SobekCM_WebContent_Get_Recent_Changes]
AS
BEGIN

	-- Get all milestones
	select W.WebContentID, W.Level1, W.Level2, W.Level3, W.Level4, W.Level5, W.Level6, W.Level7, W.Level8, MilestoneDate, MilestoneUser, Milestone, W.Title
	from SobekCM_WebContent_Milestones M, SobekCM_WebContent W
	where M.WebContentID=W.WebContentID
	order by MilestoneDate DESC;

	-- Get the distinct list of users that made changes
	select MilestoneUser
	from SobekCM_WebContent_Milestones
	group by MilestoneUser
	order by MilestoneUser;

	-- Return the distinct first level
	select Level1 
	from SobekCM_WebContent_Milestones M, SobekCM_WebContent W
	where M.WebContentID=W.WebContentID
	group by Level1
	order by Level1;
	
	-- Return the distinct first TWO level					
	select Level1, Level2
	from SobekCM_WebContent_Milestones M, SobekCM_WebContent W
	where M.WebContentID=W.WebContentID
	group by Level1, Level2
	order by Level1, Level2;


END;
GO

-- Ensure the SobekCM_WebContent_All_Redirects stored procedure exists
IF object_id('SobekCM_WebContent_Edit') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Edit as select 1;');
GO

-- Edit basic information on an existing web content page
ALTER PROCEDURE [dbo].[SobekCM_WebContent_Edit]
	@WebContentID int,
	@UserName nvarchar(100),
	@Title nvarchar(255),
	@Summary nvarchar(1000),
	@Redirect varchar(500),
	@MilestoneText varchar(max)
AS
BEGIN	
	-- Make the change
	update SobekCM_WebContent
	set Title=@Title, Summary=@Summary, Redirect=@Redirect
	where WebContentID=@WebContentID;

	-- Now, add a milestone
	if ( len(coalesce(@MilestoneText,'')) > 0 )
	begin
		insert into SobekCM_WebContent_Milestones (WebContentID, Milestone, MilestoneDate, MilestoneUser )
		values ( @WebContentID, @MilestoneText, getdate(), @UserName );
	end
	else
	begin
		insert into SobekCM_WebContent_Milestones (WebContentID, Milestone, MilestoneDate, MilestoneUser )
		values ( @WebContentID, 'Edited', getdate(), @UserName );
	end;

END;
GO

-- Ensure the SobekCM_WebContent_All_Redirects stored procedure exists
IF object_id('SobekCM_WebContent_Usage_Report') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Usage_Report as select 1;');
GO

-- Pull the usage for all top-level web content pages between two dates
ALTER PROCEDURE [dbo].[SobekCM_WebContent_Usage_Report]
	@year1 smallint,
	@month1 smallint,
	@year2 smallint,
	@month2 smallint
AS
BEGIN	

	with stats_compiled as
	(	
		select Level1, Level2, Level3, Level4, Level5, Level6, Level7, Level8, sum(Hits) as Hits, sum(Hits_Complete) as HitsHierarchical
		from SobekCM_WebContent_Statistics
		where ((( [Month] >= @month1 ) and ( [Year] = @year1 )) or ([Year] > @year1 ))
		  and ((( [Month] <= @month2 ) and ( [Year] = @year2 )) or ([Year] < @year2 ))
		group by Level1, Level2, Level3, Level4, Level5, Level6, Level7, Level8
	)
	select coalesce(W.Level1, S.Level1) as Level1, coalesce(W.Level2, S.Level2) as Level2, coalesce(W.Level3, S.Level3) as Level3,
	       coalesce(W.Level4, S.Level4) as Level4, coalesce(W.Level5, S.Level5) as Level5, coalesce(W.Level6, S.Level6) as Level6,
		   coalesce(W.Level7, S.Level7) as Level7, coalesce(W.Level8, S.Level8) as Level8, W.Deleted, coalesce(W.Title,'(no title)') as Title, S.Hits, S.HitsHierarchical
	into #TEMP1
	from stats_compiled S left outer join
	     SobekCM_WebContent W on     ( W.Level1=S.Level1 ) 
		                         and ( coalesce(W.Level2,'')=coalesce(S.Level2,''))
								 and ( coalesce(W.Level3,'')=coalesce(S.Level3,''))
								 and ( coalesce(W.Level4,'')=coalesce(S.Level4,''))
								 and ( coalesce(W.Level5,'')=coalesce(S.Level5,''))
								 and ( coalesce(W.Level6,'')=coalesce(S.Level6,''))
								 and ( coalesce(W.Level7,'')=coalesce(S.Level7,''))
								 and ( coalesce(W.Level8,'')=coalesce(S.Level8,''))
	order by Level1, Level2, Level3, Level4, Level5, Level6, Level7, Level8;	
	
	-- Return the full stats
	select * from #TEMP1;
	
	-- Return the distinct first level
	select Level1 
	from #TEMP1
	group by Level1
	order by Level1;
	
	-- Return the distinct first TWO level					
	select Level1, Level2
	from #TEMP1
	group by Level1, Level2
	order by Level1, Level2;

END;
GO


-- Ensure the SobekCM_WebContent_Has_Usage stored procedure exists
IF object_id('SobekCM_WebContent_Has_Usage') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_Has_Usage as select 1;');
GO

-- Pull the flag indicating if this instance has any web content usage logged
ALTER PROCEDURE [dbo].SobekCM_WebContent_Has_Usage
	@value bit output
AS
BEGIN	

	if ( exists ( select 1 from SobekCM_WebContent_Statistics ))
		set @value = 'true';
	else
		set @value = 'false';
	
END;
GO

-- Ensure the SobekCM_WebContent_All_Brief stored procedure exists
IF object_id('SobekCM_WebContent_All_Brief') IS NULL EXEC ('create procedure dbo.SobekCM_WebContent_All_Brief as select 1;');
GO

-- Return a brief account of all the web content pages, regardless of whether they are redirects or an actual content page
ALTER PROCEDURE [dbo].[SobekCM_WebContent_All_Brief]
AS
BEGIN

	-- Get the complete list of all active web content pages, with segment level names, primary key, and redirect URL
	select W.WebContentID, W.Level1, W.Level2, W.Level3, W.Level4, W.Level5, W.Level6, W.Level7, W.Level8, W.Redirect
	from SobekCM_WebContent W 
	where Deleted = 'false'
	order by W.Level1, W.Level2, W.Level3, W.Level4, W.Level5, W.Level6, W.Level7, W.Level8;

END;
GO


GRANT EXECUTE ON [dbo].[SobekCM_WebContent_Get_Page] TO sobek_user;
GRANT EXECUTE ON [dbo].[SobekCM_WebContent_Get_Page_ID] TO sobek_user;
GRANT EXECUTE ON [dbo].[SobekCM_WebContent_All] TO sobek_user;
GRANT EXECUTE ON [dbo].[SobekCM_WebContent_All_Pages] TO sobek_user;
GRANT EXECUTE ON [dbo].[SobekCM_WebContent_All_Pages] TO sobek_builder;
GRANT EXECUTE ON [dbo].[SobekCM_WebContent_All_Redirects] TO sobek_user;
GRANT EXECUTE ON [dbo].[SobekCM_WebContent_Edit] TO sobek_user;
GRANT EXECUTE ON [dbo].[SobekCM_WebContent_Edit] TO sobek_builder;
GRANT EXECUTE ON [dbo].[SobekCM_WebContent_Usage_Report] to sobek_user;
GRANT EXECUTE ON [dbo].[SobekCM_WebContent_Has_Usage] to sobek_user;
GRANT EXECUTE ON [dbo].[SobekCM_WebContent_All_Brief] to sobek_user;
GO

-- Drop index, if it exists 
if ( EXISTS ( select 1 from sys.indexes WHERE name='IX_SobekCM_WebContent_Milestones_Date_ID' AND object_id = OBJECT_ID('SobekCM_WebContent_Milestones')))
	DROP INDEX IX_SobekCM_WebContent_Milestones_Date_ID ON [dbo].SobekCM_WebContent_Milestones
GO

alter table SobekCM_WebContent_Milestones 
alter column MilestoneDate datetime not null;
GO

/****** Object:  Index [IX_SobekCM_WebContent_Milestones_Date_ID]    Script Date: 6/4/2015 6:55:43 AM ******/
CREATE NONCLUSTERED INDEX [IX_SobekCM_WebContent_Milestones_Date_ID] ON [dbo].[SobekCM_WebContent_Milestones]
(
	[WebContentID] ASC,
	[MilestoneDate] ASC
)
INCLUDE ( 	[MilestoneUser]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

