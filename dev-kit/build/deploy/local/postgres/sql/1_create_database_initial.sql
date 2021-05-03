create schema if not exists Wzd;

create schema if not exists Rdr;

create schema if not exists Sts;

create schema if not exists Bsp;

create table Bsp.BookCategoryType(
	Id smallint generated always as identity primary key,
	TypeName varchar(100)		not null
);

create table Wzd.PrimaryContactType(
	Id smallint generated always as identity primary key,
	TypeName varchar(100)		not null
);

create table Rdr.ReaderGroupStatusType(
	Id smallint generated always as identity primary key,
	TypeName varchar(100)		not null
);

create table Rdr.CourceOrderStatusType(
	Id smallint generated always as identity primary key,
	TypeName varchar(100)		not null
);