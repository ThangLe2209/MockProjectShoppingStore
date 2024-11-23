BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Brands" (
	"Id"	TEXT NOT NULL,
	"Name"	TEXT NOT NULL,
	"Description"	TEXT,
	"Slug"	TEXT,
	"Status"	INTEGER NOT NULL,
	CONSTRAINT "PK_Brands" PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "Categories" (
	"Id"	TEXT NOT NULL,
	"Name"	TEXT NOT NULL,
	"Description"	TEXT,
	"Slug"	TEXT,
	"Status"	INTEGER NOT NULL,
	CONSTRAINT "PK_Categories" PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "OrderDetails" (
	"Id"	TEXT NOT NULL,
	"OrderCode"	TEXT NOT NULL,
	"UserName"	TEXT NOT NULL,
	"ProductId"	TEXT NOT NULL,
	"Price"	TEXT NOT NULL,
	"Quantity"	INTEGER NOT NULL,
	CONSTRAINT "PK_OrderDetails" PRIMARY KEY("Id"),
	CONSTRAINT "FK_OrderDetails_Products_ProductId" FOREIGN KEY("ProductId") REFERENCES "Products"("Id") ON DELETE RESTRICT
);
CREATE TABLE IF NOT EXISTS "Orders" (
	"Id"	TEXT NOT NULL,
	"OrderCode"	TEXT NOT NULL,
	"UserName"	TEXT NOT NULL,
	"CreatedDate"	TEXT NOT NULL,
	"Status"	INTEGER NOT NULL,
	CONSTRAINT "PK_Orders" PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "Products" (
	"Id"	TEXT NOT NULL,
	"Name"	TEXT NOT NULL,
	"Slug"	TEXT,
	"Description"	TEXT,
	"Price"	decimal(8, 2) NOT NULL,
	"BrandId"	TEXT NOT NULL,
	"CategoryId"	TEXT NOT NULL,
	"Image"	TEXT NOT NULL,
	CONSTRAINT "PK_Products" PRIMARY KEY("Id"),
	CONSTRAINT "FK_Products_Brands_BrandId" FOREIGN KEY("BrandId") REFERENCES "Brands"("Id") ON DELETE RESTRICT,
	CONSTRAINT "FK_Products_Categories_CategoryId" FOREIGN KEY("CategoryId") REFERENCES "Categories"("Id") ON DELETE RESTRICT
);
CREATE TABLE IF NOT EXISTS "Ratings" (
	"Id"	TEXT NOT NULL,
	"ProductId"	TEXT NOT NULL,
	"Comment"	TEXT NOT NULL,
	"Name"	TEXT NOT NULL,
	"Email"	TEXT NOT NULL,
	"Star"	TEXT NOT NULL,
	CONSTRAINT "PK_Ratings" PRIMARY KEY("Id"),
	CONSTRAINT "FK_Ratings_Products_ProductId" FOREIGN KEY("ProductId") REFERENCES "Products"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
	"MigrationId"	TEXT NOT NULL,
	"ProductVersion"	TEXT NOT NULL,
	CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY("MigrationId")
);
INSERT INTO "Brands" VALUES ('692C55B7-5A6B-473E-958B-F5374958B499','Apple','Apple is large brand in the world','apple',1);
INSERT INTO "Brands" VALUES ('A0B926B9-9240-4C4C-AA67-B84874991E41','Samsung','Samsung is large brand in the world','samsung',1);
INSERT INTO "Categories" VALUES ('2A556306-6499-45FF-ADB0-0432481A1BF2','Pc','Pc is large Product in the world','pc',1);
INSERT INTO "Categories" VALUES ('39A3369B-7109-40BE-A656-792D735CAD0F','Macbook','Macbook is large Product in the world','macbook',1);
INSERT INTO "OrderDetails" VALUES ('3365A851-653F-4527-B57A-99D028E96C78','e68b42a4-7c0e-4989-998b-99cd2eef0274','thang@gmail.com','08EDA072-EABA-4505-B2E1-20FF3956763B','1233.0',3);
INSERT INTO "OrderDetails" VALUES ('760F27F2-0C78-4454-8A73-9197B4B6FA56','e68b42a4-7c0e-4989-998b-99cd2eef0274','thang@gmail.com','F6367A8B-297D-42A4-AD32-AA31081E74C5','1233.0',2);
INSERT INTO "Orders" VALUES ('6A27019B-92A5-4DDB-80D2-A14EA3B57032','e68b42a4-7c0e-4989-998b-99cd2eef0274','thang@gmail.com','2024-10-18 20:21:01.224818',0);
INSERT INTO "Products" VALUES ('08EDA072-EABA-4505-B2E1-20FF3956763B','Macbook','macbook','Macbook is the Best',1233,'692C55B7-5A6B-473E-958B-F5374958B499','39A3369B-7109-40BE-A656-792D735CAD0F','1.jpg');
INSERT INTO "Products" VALUES ('F6367A8B-297D-42A4-AD32-AA31081E74C5','Pc','pc','Pc is the Best',1233,'A0B926B9-9240-4C4C-AA67-B84874991E41','2A556306-6499-45FF-ADB0-0432481A1BF2','1.jpg');
INSERT INTO "__EFMigrationsHistory" VALUES ('20241004043704_InitialShoppingStore','8.0.0');
INSERT INTO "__EFMigrationsHistory" VALUES ('20241016152300_AddOrderAndOrderDetailModel','8.0.0');
CREATE INDEX IF NOT EXISTS "IX_OrderDetails_ProductId" ON "OrderDetails" (
	"ProductId"
);
CREATE INDEX IF NOT EXISTS "IX_Products_BrandId" ON "Products" (
	"BrandId"
);
CREATE INDEX IF NOT EXISTS "IX_Products_CategoryId" ON "Products" (
	"CategoryId"
);
CREATE INDEX IF NOT EXISTS "IX_Ratings_ProductId" ON "Ratings" (
	"ProductId"
);
COMMIT;
