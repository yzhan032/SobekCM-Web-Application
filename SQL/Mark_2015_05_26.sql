

-- Esure the SobekCM_Save_Item_Behaviors_Minimal exists
IF object_id('SobekCM_Save_Item_Behaviors_Minimal') IS NULL EXEC ('create procedure dbo.SobekCM_Save_Item_Behaviors_Minimal as select 1;');
GO

-- Saves the behavior information about an item in this library
-- Written by Mark Sullivan 
ALTER PROCEDURE [dbo].[SobekCM_Save_Item_Behaviors_Minimal]
	@ItemID int,
	@TextSearchable bit,
	@Viewer1_TypeID int,
	@Viewer1_Label nvarchar(50),
	@Viewer1_Attribute nvarchar(250),
	@Viewer2_TypeID int,
	@Viewer2_Label nvarchar(50),
	@Viewer2_Attribute nvarchar(250),
	@Viewer3_TypeID int,
	@Viewer3_Label nvarchar(50),
	@Viewer3_Attribute nvarchar(250),
	@Viewer4_TypeID int,
	@Viewer4_Label nvarchar(50),
	@Viewer4_Attribute nvarchar(250),
	@Viewer5_TypeID int,
	@Viewer5_Label nvarchar(50),
	@Viewer5_Attribute nvarchar(250),
	@Viewer6_TypeID int,
	@Viewer6_Label nvarchar(50),
	@Viewer6_Attribute nvarchar(250)
AS
begin transaction

	--Update the main item
	update SobekCM_Item
	set TextSearchable = @TextSearchable
	where ( ItemID = @ItemID );
	
	-- Add the first viewer information
	if (( @Viewer1_TypeID > 0 ) and ( not exists ( select 1 from SobekCM_Item_Viewers where ItemID=@ItemID and ItemViewTypeID=@Viewer1_TypeID )))
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, @Viewer1_TypeID, @Viewer1_Attribute, @Viewer1_Label );
	end;
	
	-- Add the second viewer information
	if (( @Viewer2_TypeID > 0 ) and ( not exists ( select 1 from SobekCM_Item_Viewers where ItemID=@ItemID and ItemViewTypeID=@Viewer2_TypeID )))
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, @Viewer2_TypeID, @Viewer2_Attribute, @Viewer2_Label );
	end;
	
	-- Add the third viewer information
	if (( @Viewer3_TypeID > 0 ) and ( not exists ( select 1 from SobekCM_Item_Viewers where ItemID=@ItemID and ItemViewTypeID=@Viewer3_TypeID )))
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, @Viewer3_TypeID, @Viewer3_Attribute, @Viewer3_Label );
	end;
	
	-- Add the fourth viewer information
	if (( @Viewer4_TypeID > 0 ) and ( not exists ( select 1 from SobekCM_Item_Viewers where ItemID=@ItemID and ItemViewTypeID=@Viewer4_TypeID )))
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, @Viewer4_TypeID, @Viewer4_Attribute, @Viewer4_Label );
	end;
	
	-- Add the fifth viewer information
	if (( @Viewer5_TypeID > 0 ) and ( not exists ( select 1 from SobekCM_Item_Viewers where ItemID=@ItemID and ItemViewTypeID=@Viewer5_TypeID )))
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, @Viewer5_TypeID, @Viewer5_Attribute, @Viewer5_Label );
	end;
	
	-- Add the first viewer information
	if (( @Viewer6_TypeID > 0 ) and ( not exists ( select 1 from SobekCM_Item_Viewers where ItemID=@ItemID and ItemViewTypeID=@Viewer6_TypeID )))
	begin
		-- Insert this viewer information
		insert into SobekCM_Item_Viewers ( ItemID, ItemViewTypeID, Attribute, Label )
		values ( @ItemID, @Viewer6_TypeID, @Viewer6_Attribute, @Viewer6_Label );
	end;

commit transaction
GO

GRANT EXECUTE ON dbo.SobekCM_Save_Item_Behaviors_Minimal to sobek_user;
GRANT EXECUTE ON dbo.SobekCM_Save_Item_Behaviors_Minimal to sobek_builder;
GO