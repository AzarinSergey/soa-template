create table if not exists Wzd.Wizards(
	Id int generated always as identity primary key,
	UserUuid uuid not null,
	FirstName varchar(32) not null,
	LastName varchar(32) not null,
	MiddleName varchar(32) not null,
	PhoneNumber varchar(64) not null,
	Email varchar(64) not null
);

create table if not exists Wzd.Welcomes(
	Id int generated always as identity primary key,
	WizardId int not null, FOREIGN KEY (WizardId) REFERENCES Wzd.Wizards (Id),
	Title varchar(256) not null,
	DateTime timestamp null
);

create table if not exists Wzd.ReaderOrders(
	Id int generated always as identity primary key,
	InvitedToWelcomId int null, FOREIGN KEY (InvitedToWelcomId) REFERENCES Wzd.Welcomes (Id),
	PrimaryContactType smallint not null, FOREIGN KEY (PrimaryContactType) REFERENCES Wzd.PrimaryContactType (Id),
	FirstName varchar(32),
	LastName varchar(32),
	MiddleName varchar(32),
	PhoneNumber varchar(64) not null,
	City varchar(64) not null,
	WelcomeVisited bit not null,
	FirstPaymentReceived bit not null
);

create table if not exists Wzd.Programs(
	Id int generated always as identity primary key,
	Title varchar(1024) null,
	Decsription text null
);

create table if not exists Wzd.ProgramBooks(
	Id int generated always as identity primary key,
	ProgramId int not null, FOREIGN KEY (ProgramId) REFERENCES Wzd.Programs (Id),
	BookId int not null
);

create table if not exists Wzd.Cources(
	Id int generated always as identity primary key,
	ProgramId int not null, FOREIGN KEY (ProgramId) REFERENCES Wzd.Programs (Id),
	WizardId int not null, FOREIGN KEY (WizardId) REFERENCES Wzd.Wizards (Id),
	BookId int not null,
	Title varchar(128) not null,
	DayOfWeek smallint not null,
	StartTime time not null,
	Price decimal(18,2) not null
);
