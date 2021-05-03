create table if not exists Rdr.ReaderGroups(
	Id int generated always as identity primary key,
	ReaderGroupStatusType smallint not null, FOREIGN KEY (ReaderGroupStatusType) REFERENCES Rdr.ReaderGroupStatusType (Id),
	WizardUuid uuid not null,
	CurrentCourceId int not null
);

create table if not exists Rdr.Readers(
	Id int generated always as identity primary key,
	ReaderGroupId int not null, FOREIGN KEY (ReaderGroupId) REFERENCES Rdr.ReaderGroups (Id),
	UserUuid uuid null,
	Email varchar(128) null,
	PhoneNumber varchar(64) not null,
	FirstName varchar(64),
	LastName varchar(64),
	MiddleName varchar(64),
	City varchar(64) not null,
	CurrentCourceId int not null,
	IsLeaver bit not null,
	LeaverCauseDescription varchar(4096) null
);

create table if not exists Rdr.Meetings(
	Id int generated always as identity primary key,
	CourceId int not null,
	DateTime timestamp not null,
	Title varchar(128) null,
	Annotation varchar(128) null
);

create table if not exists Rdr.Attendance(
	Id int generated always as identity primary key,
	ReaderId int not null, FOREIGN KEY (ReaderId) REFERENCES Rdr.Readers (Id),
	MeetingId int not null, FOREIGN KEY (MeetingId) REFERENCES Rdr.Meetings (Id),
	IsAttended bit not null,
	Comment varchar(1024) null
);

create table if not exists Rdr.CourceReports(
	Id int generated always as identity primary key,
	ReaderId int not null, FOREIGN KEY (ReaderId) REFERENCES Rdr.Readers (Id),
	CourceId int not null,
	Header varchar(64) not null,
	Title varchar(256) null,
	HtmlBody text not null
);

create table if not exists Rdr.CourceOrders(
	Id int generated always as identity primary key,
	ReaderId int not null, FOREIGN KEY (ReaderId) REFERENCES Rdr.Readers (Id),
	CourceOrderStatusType smallint not null, FOREIGN KEY (CourceOrderStatusType) REFERENCES Rdr.CourceOrderStatusType (Id),
	CourceId int not null,
	Cost decimal(18,2) not null
);