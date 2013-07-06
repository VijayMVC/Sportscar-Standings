CREATE TABLE [CMS_WorkflowStepRoles] (
		[WorkflowStepRoleID]      [int] IDENTITY(1, 1) NOT NULL,
		[StepID]                  [int] NOT NULL,
		[RoleID]                  [int] NOT NULL,
		[StepSourcePointGUID]     [uniqueidentifier] NULL
) 
ALTER TABLE [CMS_WorkflowStepRoles]
	ADD
	CONSTRAINT [PK_CMS_WorkflowStepRoles]
	PRIMARY KEY
	NONCLUSTERED
	([WorkflowStepRoleID])
	
CREATE UNIQUE CLUSTERED INDEX [IX_CMS_WorkflowStepRoles_StepID_StepSourcePointGUID_RoleID]
	ON [CMS_WorkflowStepRoles] ([StepID], [StepSourcePointGUID], [RoleID])
	
ALTER TABLE [CMS_WorkflowStepRoles]
	WITH CHECK
	ADD CONSTRAINT [FK_CMS_WorkflowStepRoles_RoleID_CMS_Role]
	FOREIGN KEY ([RoleID]) REFERENCES [CMS_Role] ([RoleID])
ALTER TABLE [CMS_WorkflowStepRoles]
	CHECK CONSTRAINT [FK_CMS_WorkflowStepRoles_RoleID_CMS_Role]
ALTER TABLE [CMS_WorkflowStepRoles]
	WITH CHECK
	ADD CONSTRAINT [FK_CMS_WorkflowStepRoles_StepID_CMS_WorkflowStep]
	FOREIGN KEY ([StepID]) REFERENCES [CMS_WorkflowStep] ([StepID])
ALTER TABLE [CMS_WorkflowStepRoles]
	CHECK CONSTRAINT [FK_CMS_WorkflowStepRoles_StepID_CMS_WorkflowStep]
