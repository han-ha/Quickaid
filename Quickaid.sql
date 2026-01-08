USE master;
GO

ALTER DATABASE QuickAid SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE QuickAid;
GO

CREATE DATABASE QuickAid;
GO

USE QuickAid;
GO

CREATE TABLE [users] (
    [id] int PRIMARY KEY IDENTITY(1, 1),
    [username] varchar(100) UNIQUE NOT NULL,
    [email] varchar(255) UNIQUE NOT NULL,
    [role] varchar(20) NOT NULL,
    [created_at] datetime DEFAULT (GETDATE())
);
GO

CREATE TABLE [passwords] (
    [id] int PRIMARY KEY IDENTITY(1, 1),
    [user_id] int NOT NULL,
    [hashed_password] varchar(255) NOT NULL,
    [salt] varchar(255),
    [last_changed] datetime DEFAULT (GETDATE())
);
GO

CREATE TABLE [aed_points] (
    [id] int PRIMARY KEY IDENTITY(1, 1),
    [latitude] decimal(9,6),
    [longitude] decimal(9,6),
    [description] varchar(max),
    [added_by] int,
    [verified] bit DEFAULT (0),
    [updated_at] datetime DEFAULT (GETDATE())
);
GO

CREATE TABLE [quizzes] (
    [id] int PRIMARY KEY IDENTITY(1, 1),
    [title] varchar(150) NOT NULL,
    [description] varchar(max),
    [created_at] datetime DEFAULT (GETDATE()),
    [number_of_questions] int,
    [max_score] int
);
GO

CREATE TABLE [questions] (
    [id] int PRIMARY KEY IDENTITY(1, 1),
    [question_text] varchar(max) NOT NULL,
    [created_at] datetime DEFAULT (GETDATE()),
    [number_of_answers] int
);
GO

CREATE TABLE [quiz_questions] (
    [quiz_id] int NOT NULL,
    [question_id] int NOT NULL,
    CONSTRAINT PK_quiz_questions PRIMARY KEY (quiz_id, question_id)
);
GO

CREATE TABLE [answers] (
    [id] int PRIMARY KEY IDENTITY(1, 1),
    [question_id] int NOT NULL,
    [answer_text] varchar(255),
    [is_correct] bit DEFAULT (0)
);
GO

CREATE TABLE [user_quiz_results] (
    [id] int PRIMARY KEY IDENTITY(1, 1),
    [user_id] int NOT NULL,
    [quiz_id] int NOT NULL,
    [score] int,
    [completed_at] datetime DEFAULT (GETDATE())
);
GO

CREATE TABLE [articles] (
    [id] int PRIMARY KEY IDENTITY(1, 1),
    [title] varchar(200),
    [content] varchar(max),
    [created_by] int,
    [created_at] datetime DEFAULT (GETDATE()),
    [updated_at] datetime DEFAULT (GETDATE())
);
GO

EXEC sp_addextendedproperty
@name = N'Column_Description',
@value = 'SQL Server: IDENTITY(1,1), id=0 = systemowy użytkownik',
@level0type = N'Schema', @level0name = 'dbo',
@level1type = N'Table',  @level1name = 'users',
@level2type = N'Column', @level2name = 'id';
GO

EXEC sp_addextendedproperty
@name = N'Column_Description',
@value = 'user / admin / system?',
@level0type = N'Schema', @level0name = 'dbo',
@level1type = N'Table',  @level1name = 'users',
@level2type = N'Column', @level2name = 'role';
GO

EXEC sp_addextendedproperty
@name = N'Column_Description',
@value = 'np. przy wejsciu do apteki',
@level0type = N'Schema', @level0name = 'dbo',
@level1type = N'Table',  @level1name = 'aed_points',
@level2type = N'Column', @level2name = 'description';
GO

EXEC sp_addextendedproperty
@name = N'Column_Description',
@value = '0 = systemowy AED',
@level0type = N'Schema', @level0name = 'dbo',
@level1type = N'Table',  @level1name = 'aed_points',
@level2type = N'Column', @level2name = 'added_by';
GO

EXEC sp_addextendedproperty
@name = N'Column_Description',
@value = '> 0',
@level0type = N'Schema', @level0name = 'dbo',
@level1type = N'Table',  @level1name = 'quizzes',
@level2type = N'Column', @level2name = 'number_of_questions';
GO

EXEC sp_addextendedproperty
@name = N'Column_Description',
@value = '> 0, <= 4',
@level0type = N'Schema', @level0name = 'dbo',
@level1type = N'Table',  @level1name = 'questions',
@level2type = N'Column', @level2name = 'number_of_answers';
GO

ALTER TABLE [passwords] 
ADD CONSTRAINT FK_passwords_users
FOREIGN KEY ([user_id]) REFERENCES [users]([id])
ON DELETE CASCADE;
GO

ALTER TABLE [aed_points] 
ADD CONSTRAINT FK_aed_points_users
FOREIGN KEY ([added_by]) REFERENCES [users]([id])
ON DELETE SET NULL;
GO

ALTER TABLE [quiz_questions] 
ADD CONSTRAINT FK_quiz_questions_quizzes
FOREIGN KEY ([quiz_id]) REFERENCES [quizzes]([id])
ON DELETE CASCADE;
GO

ALTER TABLE [quiz_questions] 
ADD CONSTRAINT FK_quiz_questions_questions
FOREIGN KEY ([question_id]) REFERENCES [questions]([id])
ON DELETE CASCADE;
GO

ALTER TABLE [answers] 
ADD CONSTRAINT FK_answers_questions
FOREIGN KEY ([question_id]) REFERENCES [questions]([id])
ON DELETE CASCADE;
GO

ALTER TABLE [user_quiz_results] 
ADD CONSTRAINT FK_user_quiz_results_users
FOREIGN KEY ([user_id]) REFERENCES [users]([id])
ON DELETE CASCADE;
GO

ALTER TABLE [user_quiz_results] 
ADD CONSTRAINT FK_user_quiz_results_quizzes
FOREIGN KEY ([quiz_id]) REFERENCES [quizzes]([id])
ON DELETE CASCADE;
GO

ALTER TABLE [articles] 
ADD CONSTRAINT FK_articles_users
FOREIGN KEY ([created_by]) REFERENCES [users]([id])
ON DELETE SET NULL;
GO
