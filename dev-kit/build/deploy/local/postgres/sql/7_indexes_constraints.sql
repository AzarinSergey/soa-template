--BSP
--имена типов категорий уникальны
create unique index idx_BookCategoryTypeNameUniq
on Bsp.BookCategoryType (TypeName);

--заголовки категорий уникальны
create unique index idx_BookCategoryTitleUniq
on Bsp.BookCategory (Title);

--получить все книги автора
create index idx_BookAuthorRelationsAuthorFilter
on Bsp.BookAuthorRelations (AuthorId);

--получить все книги лежащие в категории
create index idx_BooksBookCategoryRelationBookCategoryFilter
on Bsp.BooksBookCategoryRelation(BookCategoryId);

--т.к. категория иерархична - 
--получить все книги в категории 
--тоже самое что и получать все книги в субкатегориях
create index idx_BookCategoryParentFilter
on Bsp.BookCategory(ParentId);

--если категория типа 'root' нельзя добавить ссылку в 'ParentId'
alter table Bsp.BookCategory
add column CategoryTypeName varchar(100);

update Bsp.BookCategory
set CategoryTypeName = TypeName
from Bsp.BookCategory bc
inner join Bsp.BookCategoryType bct on bc.BookCategoryType = bct.Id 
where bct.TypeName = 'root';

alter table Bsp.BookCategory
add constraint ParentOfRootShouldBeNull  CHECK (CategoryTypeName = 'root' and ParentId = null);

--WZD
create unique index idx_PrimaryContactTypeNameUniq
on Wzd.PrimaryContactType (TypeName);

--RDR

create unique index idx_CourceOrderStatusTypeNameUniq
on Rdr.CourceOrderStatusType (TypeName);

create unique index idx_ReaderGroupStatusTypeNameUniq
on Rdr.ReaderGroupStatusType (TypeName);