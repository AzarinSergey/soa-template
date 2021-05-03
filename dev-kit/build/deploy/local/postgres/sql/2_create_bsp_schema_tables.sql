create table if not exists Bsp.BookCategory(
	Id int generated always as identity primary key,
	ParentId int null, FOREIGN KEY (ParentId) REFERENCES Bsp.BookCategory (Id),
	Title varchar(32) not null,
	BookCategoryType smallint not null, FOREIGN KEY (BookCategoryType) REFERENCES Bsp.BookCategoryType (Id)
);

create table if not exists Bsp.Books(
	Id int generated always as identity primary key,
	Title varchar(64) not null,
	AnnotationHtml text null
);

create table if not exists Bsp.BooksBookCategoryRelation(
	Id int generated always as identity primary key,
	BookId int not null, FOREIGN KEY (BookId) REFERENCES Bsp.Books (Id),
	BookCategoryId int not null, FOREIGN KEY (BookCategoryId) REFERENCES Bsp.BookCategory (Id)
);

create table if not exists Bsp.Authors(
	Id int generated always as identity primary key,
	FirstName varchar(64),
	LastName varchar(64),
	MiddleName varchar(64),
	BirthDate date null,
	DiedOn date null,
	Title varchar(1024) null,
	AboutHtml text null
);

create table if not exists Bsp.BookAuthorRelations(
	Id int generated always as identity primary key,
	BookId int not null, FOREIGN KEY (BookId) REFERENCES Bsp.Books (Id),
	AuthorId int not null, FOREIGN KEY (AuthorId) REFERENCES Bsp.Authors (Id)
);