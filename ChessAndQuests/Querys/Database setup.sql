CREATE TABLE  [tbl_player]
(
	[pl_id] INT NOT NULL PRIMARY KEY,
	[pl_username] NVARCHAR(100) NOT NULL,
	[pl_password] NVARCHAR NOT NULL,
);

CREATE TABLE [tbl_game]
(
	[gm_id] INT NOT NULL PRIMARY KEY,
	[pl_wh_id] INT NOT NULL FOREIGN KEY REFERENCES [tbl_player](pl_id),
	[pl_bl_id] INT NOT NULL FOREIGN KEY REFERENCES [tbl_player](pl_id),
	[gm_key] NVARCHAR(100) NOT NULL,
	[gm_current_fen] NVARCHAR(1000) NOT NULL,
	[gm_status] INT NOT NULL,
	[gm_turn] INT NOT NULL
);

