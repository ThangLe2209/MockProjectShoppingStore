 select * from OrderDetails

SELECT * FROM Brands
SELECT * FROM Products

Select t2.*, t1.Price From 
(
	Select * From Products
	Where Id In (
	 select temp2.ProductId
	 From (
		select temp1.ProductId, ROW_NUMBER() OVER (ORDER BY temp1.TotalQuantity desc) as TotalQuantityRankBySum 
		From (
			select distinct ProductId, SUM(Quantity) OVER (PARTITION BY ProductId) as TotalQuantity
			From OrderDetails 
		) temp1
	 ) temp2
	 Where temp2.TotalQuantityRankBySum < 6
	)
) t1
INNER JOIN Brands t2
ON t1.BrandId = t2.Id
	

 Go;

INSERT INTO "OrderDetails" VALUES ('3365A851-653F-4527-B57A-99D028E96C80','e68b42a4-7c0e-4989-998b-99cd2eef0278','thang4@gmail.com','08EDA072-EABA-4505-B2E1-20FF3956763B','1233.0',03);
INSERT INTO "OrderDetails" VALUES ('760F27F2-0C78-4454-8A73-9197B4B6FA58','e68b42a4-7c0e-4989-998b-99cd2eef0278','thang4@gmail.com','F6367A8B-297D-42A4-AD32-AA31081E74C5','1233.0',40);


-- /////////////////////////////////////////////////////////////////
-- Get calculation by orders which already paid
Use tesstSqlite123
select * from Orders

SELECT * FROM Brands
SELECT * FROM Products

Select t2.*, t1.Id, t1.Price From 
(
	Select * From Products
	Where Id In (
	 select temp2.ProductId
	 From (
		select temp1.ProductId, ROW_NUMBER() OVER (ORDER BY temp1.TotalQuantity desc) as TotalQuantityRankBySum 
		From (
			select distinct ProductId, SUM(Quantity) OVER (PARTITION BY ProductId) as TotalQuantity
			From OrderDetails 
			Join Orders On OrderDetails.OrderCode = Orders.OrderCode
			Where Status = 1
		) temp1
	 ) temp2
	 Where temp2.TotalQuantityRankBySum < 6
	)
) t1
INNER JOIN Brands t2
ON t1.BrandId = t2.Id
	

 Go;

Select * From Brands
Select * From Products
Select * From OrderDetails
Select * From Orders
INSERT INTO "Orders" VALUES ('6A27019B-92A5-4DDB-80D2-A14EA3B57034','e68b42a4-7c0e-4989-998b-99cd2eef0278','thang@gmail.com','2024-10-18 20:21:01.224818',1);
INSERT INTO "OrderDetails" VALUES ('3365A851-653F-4527-B57A-99D028E96C80','e68b42a4-7c0e-4989-998b-99cd2eef0278','thang4@gmail.com','08EDA072-EABA-4505-B2E1-20FF3956763B','1233.0',03);
INSERT INTO "OrderDetails" VALUES ('760F27F2-0C78-4454-8A73-9197B4B6FA58','e68b42a4-7c0e-4989-998b-99cd2eef0278','thang4@gmail.com','F6367A8B-297D-42A4-AD32-AA31081E74C5','1233.0',40);
INSERT INTO "Brands" VALUES ('692C55B7-5A6B-473E-958B-F5374958B500','Apple123','Apple123 is large brand in the world','apple123',1);


-- Another approach

			Select Brands.*, (CASE WHEN totalBrandCountTable.SumBrandQuantity IS NULL THEN 0 ELSE totalBrandCountTable.SumBrandQuantity END) AS SumBrandQuantity
			From Brands
			Left Outer Join (
				--Select Products.BrandId, totalProductCountTable.*
				select Products.BrandId , Sum(totalProductCountTable.SumProductQuantity) as SumBrandQuantity
				From Products 
				Join (
					select distinct ProductId, SUM(Quantity) OVER (PARTITION BY ProductId) as SumProductQuantity
					From OrderDetails 
					Join Orders On OrderDetails.OrderCode = Orders.OrderCode
					Where Status = 1
				) totalProductCountTable
				ON Products.Id = totalProductCountTable.ProductId
				Group by Products.BrandId
			) totalBrandCountTable
			On Brands.Id = totalBrandCountTable.BrandId
			Order by SumBrandQuantity desc
			OFFSET 0 ROWS
			FETCH NEXT 10 ROWS ONLY
			

INSERT INTO "Products" VALUES ('F6367A8B-297D-42A4-AD32-AA31081E74D7','Pc123','pc123','Pc123 is the Best',1234,'A0B926B9-9240-4C4C-AA67-B84874991E41','2A556306-6499-45FF-ADB0-0432481A1BF2','1.jpg');
INSERT INTO "Orders" VALUES ('6A27019B-92A5-4DDB-80D2-A14EA3B57035','e68b42a4-7c0e-4989-998b-99cd2eef0279','thang@gmail.com','2024-10-18 20:21:01.224818',1);
INSERT INTO "OrderDetails" VALUES ('3365A851-653F-4527-B57A-99D028E96C82','e68b42a4-7c0e-4989-998b-99cd2eef0278','thang4@gmail.com','F6367A8B-297D-42A4-AD32-AA31081E74D7','1234.0',10);
INSERT INTO "OrderDetails" VALUES ('3365A851-653F-4527-B57A-99D028E96C83','e68b42a4-7c0e-4989-998b-99cd2eef0279','thang@gmail.com','F6367A8B-297D-42A4-AD32-AA31081E74D7','1234.0',2);


-- final solution
Use ShoppingStoreSQLDemo
Select TOP(16) Brands.*, (CASE WHEN totalBrandCountTable.SumBrandQuantity IS NULL THEN 0 ELSE totalBrandCountTable.SumBrandQuantity END) AS SumBrandQuantity
                                From Brands
                                Left Outer Join (
                                        --Select Products.BrandId, totalProductCountTable.*
                                        select Products.BrandId , Sum(totalProductCountTable.SumProductQuantity) as SumBrandQuantity
                                        From Products
                                        Join (
                                                select distinct ProductId, SUM(Quantity) OVER (PARTITION BY ProductId) as SumProductQuantity
                                                From OrderDetails
                                                Join Orders On OrderDetails.OrderCode = Orders.OrderCode
                                                --Where Status = 0
                                        ) totalProductCountTable
                                        ON Products.Id = totalProductCountTable.ProductId
                                        Group by Products.BrandId
                                ) totalBrandCountTable
                                On Brands.Id = totalBrandCountTable.BrandId
                                Order by SumBrandQuantity desc

	  --\t\t\tSelect TOP(16) Brands.*, (CASE WHEN totalBrandCountTable.SumBrandQuantity IS NULL THEN 0 ELSE totalBrandCountTable.SumBrandQuantity END) AS SumBrandQuantity\r\n\t\t\tFrom Brands\r\n\t\t\tLeft Outer Join (\r\n\t\t\t\t--Select Products.BrandId, totalProductCountTable.*\r\n\t\t\t\tselect Products.BrandId , Sum(totalProductCountTable.SumProductQuantity) as SumBrandQuantity\r\n\t\t\t\tFrom Products \r\n\t\t\t\tJoin (\r\n\t\t\t\t\tselect distinct ProductId, SUM(Quantity) OVER (PARTITION BY ProductId) as SumProductQuantity\r\n\t\t\t\t\tFrom OrderDetails \r\n\t\t\t\t\tJoin Orders On OrderDetails.OrderCode = Orders.OrderCode\r\n\t\t\t\t\tWhere Status = 0\r\n\t\t\t\t) totalProductCountTable\r\n\t\t\t\tON Products.Id = totalProductCountTable.ProductId\r\n\t\t\t\tGroup by Products.BrandId\r\n\t\t\t) totalBrandCountTable\r\n\t\t\tOn Brands.Id = totalBrandCountTable.BrandId\r\n\t\t\tOrder by SumBrandQuantity desc