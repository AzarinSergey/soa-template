/*
Seed flags
*/
insert into Bsp.BookCategoryType (TypeName) values ('root');
insert into Bsp.BookCategoryType (TypeName) values ('year1');
insert into Bsp.BookCategoryType (TypeName) values ('year2');
insert into Bsp.BookCategoryType (TypeName) values ('year3');
insert into Bsp.BookCategoryType (TypeName) values ('year4');

insert into Wzd.PrimaryContactType (TypeName) values ('undefined');
insert into Wzd.PrimaryContactType (TypeName) values ('phone_call');
insert into Wzd.PrimaryContactType (TypeName) values ('telegramm');
insert into Wzd.PrimaryContactType (TypeName) values ('whatsup');

insert into Rdr.ReaderGroupStatusType (TypeName) values ('undefined');
insert into Rdr.ReaderGroupStatusType (TypeName) values ('selection');
insert into Rdr.ReaderGroupStatusType (TypeName) values ('in_progress');
insert into Rdr.ReaderGroupStatusType (TypeName) values ('dismiss');

insert into Rdr.CourceOrderStatusType(TypeName) values ('undefined');
insert into Rdr.CourceOrderStatusType(TypeName) values ('created');
insert into Rdr.CourceOrderStatusType(TypeName) values ('payed');
insert into Rdr.CourceOrderStatusType(TypeName) values ('canceled');
insert into Rdr.CourceOrderStatusType(TypeName) values ('outdated');
insert into Rdr.CourceOrderStatusType(TypeName) values ('error');