create table Sts.Users(
	Id uuid not null DEFAULT UUID_GENERATE_V4(), PRIMARY KEY (Id),
	Login varchar(256) not null,
	PasswordHash	varchar(4096)
);