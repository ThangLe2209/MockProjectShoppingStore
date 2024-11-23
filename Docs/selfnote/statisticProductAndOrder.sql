Select * from Orders
Select * from Products
Select * from OrderDetails
-- que1

Select *
From Orders
Where Orders.CreatedDate >= '20241030 00:00:00.000' AND Orders.CreatedDate <= '20241101 00:00:00.000'

Select Count(*) as TotalOrder
From Orders
Where Orders.CreatedDate >= '20241030 00:00:00.000' AND Orders.CreatedDate <= '20241101 00:00:00.000'

Select P.Id, P.Price, P.Name, x.[SumQuantity by Order], (P.Price * x.[SumQuantity by Order]) As Revenue
From Products P
Join (
Select ProductId, Sum(quantity) as [SumQuantity by Order]
From OrderDetails
Where OrderDetails.CreatedDate >= '20241030 00:00:00.000' AND OrderDetails.CreatedDate <= '20241101 00:00:00.000'
Group by ProductId
) x
On P.Id = x.ProductId

Select *
From OrderDetails
Where OrderDetails.CreatedDate >= '20241030 00:00:00.000' AND OrderDetails.CreatedDate <= '20241101 00:00:00.000'



-- que3
select distinct ProductId, Sum(quantity) OVER(PARTITION BY ProductId) as [SumQuantity]
From OrderDetails 

--Test
DECLARE @i1 int = 90
	IF @i1 = 7
		SET @i1 = @i1 + 23
	ELSE IF @i1 = 30
		SET @i1 = @i1 + 60
	ELSE IF @i1 = 90
		SET @i1 = @i1 + 275
PRINT @i1


SELECT CAST(CAST(GETDATE() as date) as datetime)
DECLARE @curDate1 Datetime = CAST(CAST(GETDATE() as date) as datetime)
SET @curDate1 = DATEADD(DAY,-365,@curDate1)
PRINT @curDate1


select * From Products
SELECT TOP 1 Products.BrandId
From Products


--final result 
CREATE TABLE TestStatisticTable (
    ID int Primary key Identity(1,1),
    ProductID uniqueidentifier,
    ProductName nvarchar(MAX),
    ProductPrice decimal(8, 2),
	ProductQuantitySold int,
    RevenueFromProduct decimal(8, 2),
	TotalOrderFromProductInDateRange int,
	TotalOrderFromAllProductInDateRange int,
	DateRange nvarchar(MAX),
	TotalRevenueFromAllProductInDateRange decimal(8, 2)
);

--EXEC sp_RENAME 'TableName.OldColumnName' , 'NewColumnName', 'COLUMN'
--EXEC sp_RENAME 'TestStatisticTable.OrdersFromProduct' , 'ProductQuantitySold', 'COLUMN'

-- After create table run on here
DECLARE @i int = 0
DECLARE @curDateAsEndDate Datetime = GETDATE()
--PRINT @curDateAsEndDate
DECLARE @curDate Datetime = CAST(CAST(GETDATE() as date) as datetime)

--SET @curDate = DATEADD(DAY,-2,@curDate)
--PRINT @curDate
	--Select Count(*) as TotalOrder
	--From Orders
	--Where Orders.CreatedDate >= @curDate AND Orders.CreatedDate <= @curDateAsEndDate
	--WHere Orders.CreatedDate Between @curDate And @curDateAsEndDate
	--Select * From Orders
WHILE @i < 367
BEGIN
	IF @i = 0
		Begin
		SET @i = @i + 1
		SET @curDate = DATEADD(DAY,-1,@curDate)
		End
	ELSE IF @i = 1
		Begin
		SET @i = @i + 6
		SET @curDate = DATEADD(DAY,-7,@curDate)
		End
	ELSE IF @i = 7
		Begin
		SET @i = @i + 23
		SET @curDate = DATEADD(DAY,-30,@curDate)
		End
	ELSE IF @i = 30
		Begin
		SET @i = @i + 60
		SET @curDate = DATEADD(DAY,-90,@curDate)
		End
	ELSE IF @i = 90
		Begin
		SET @i = @i + 275
		SET @curDate = DATEADD(DAY,-365,@curDate)
		end
	ELSE 
		BREAK;

    /* your code*/
	Insert dbo.TestStatisticTable(ProductID,ProductName,ProductPrice,ProductQuantitySold,RevenueFromProduct,TotalOrderFromProductInDateRange,TotalOrderFromAllProductInDateRange,DateRange,TotalRevenueFromAllProductInDateRange)
	Select P.Id,P.Name, P.Price, x.[SumQuantity by OrderDetails], (P.Price * x.[SumQuantity by OrderDetails]) As Revenue, x.TotalOrderFromProduct, t.TotalOrderInDateRange, 
	(CASE WHEN @i = 1 THEN '1 day before' 
		  WHEN @i = 7 THEN '7 days before'
		  WHEN @i = 30 THEN '1 month before'
		  WHEN @i = 90 THEN '3 months before'
		  WHEN @i = 365 THEN '1 years before'
		  ELSE 'Not calculate' END) AS [DateRange] , SUM(P.Price * x.[SumQuantity by OrderDetails]) OVER ()  AS TotalRevenueInDateRange
	From Products P
	Join (
		Select ProductId, Count(OrderCode) AS [TotalOrderFromProduct], Sum(quantity) as [SumQuantity by OrderDetails]
		From OrderDetails
		Where OrderDetails.CreatedDate >= @curDate AND OrderDetails.CreatedDate <= @curDateAsEndDate
		--Where OrderDetails.CreatedDate >= '20241030 00:00:00.000' AND OrderDetails.CreatedDate <= '20241101 00:00:00.000'
		Group by ProductId
		) x
	On P.Id = x.ProductId
	Cross Join (
		Select Count(*) as TotalOrderInDateRange
		From Orders
		Where Orders.CreatedDate Between @curDate And @curDateAsEndDate
		--Where Orders.CreatedDate >= @curDate AND Orders.CreatedDate <= @curDateAsEndDate
		--Where Orders.CreatedDate >= '20241030 00:00:00.000' AND Orders.CreatedDate <= '20241101 00:00:00.000'
	) t
END

Select * From dbo.TestStatisticTable
TRUNCATE Table dbo.TestStatisticTable


-- Test
Select *
From Products as P
Inner Join (	
    --Select ProductId, Count(*) AS [ciao], Sum(Quantity) AS [humhum]
	Select ProductId, Count(OrderCode) AS [TotalOrderFromProduct], Sum(Quantity) AS [SoldQuantity]
	From OrderDetails
	Where OrderDetails.CreatedDate >= '20240920 00:00:00.000' AND OrderDetails.CreatedDate <= '20241102 00:00:00.000'
	Group by ProductId
) x
On P.Id = x.ProductId

Select * from OrderDetails
Where OrderDetails.CreatedDate >= '20240920 00:00:00.000' AND OrderDetails.CreatedDate <= '20241102 00:00:00.000'
AND ProductId = '1280759C-7F99-4CE0-BAA3-FB78BC2E6E58'

Select * from Orders
Where Orders.CreatedDate >= '20240920 00:00:00.000' AND Orders.CreatedDate <= '20241102 00:00:00.000'
--Cross Join (
--		Select Top 2 ProductId, Sum(quantity) as [SumQuantity by Order]
--		From OrderDetails
--		Where OrderDetails.CreatedDate >= '20241030 00:00:00.000' AND OrderDetails.CreatedDate <= '20241101 00:00:00.000'
--		Group by ProductId
--) x



--Test
Go
select ProductId, Max(x.SumQuantity) 
From (
select ProductId, Sum(quantity) OVER(PARTITION BY ProductId,CreatedDate) as [SumQuantity]
From OrderDetails 
) x
Group by ProductId
