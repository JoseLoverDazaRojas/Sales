USE [Sales2023]

-- USER

SELECT * FROM [dbo].[AspNetUsers]
SELECT * FROM [dbo].[AspNetRoles]

-- MASTER

SELECT * FROM [dbo].[Categories]
SELECT * FROM [dbo].[Countries]
SELECT * FROM  [dbo].[States]
SELECT * FROM [dbo].[Cities]
SELECT * FROM [dbo].[Orders]
SELECT * FROM [dbo].[OrderDetails]
SELECT * FROM [dbo].[Products]
SELECT * FROM [dbo].[ProductImages]
SELECT * FROM [dbo].[ProductCategories]
SELECT * FROM [dbo].[TemporalOrders]

-- QUERYS

SELECT * FROM [dbo].[Countries] AS C INNER JOIN
[dbo].[States] AS S ON C.Id = S.CountryId INNER JOIN
[dbo].[Cities] AS Ci ON S.Id = Ci.StateId
WHERE C.Id = 37 AND S.Id = 619 AND Ci.Id = 20624

SELECT * FROM [dbo].[Products] AS P INNER JOIN 
[dbo].[ProductImages] AS PRI ON PRI.ProductId = P.Id INNER JOIN 
[dbo].[ProductCategories] AS PC ON PC.ProductId = P.Id INNER JOIN 
[dbo].[Categories] AS C ON C.Id = PC.CategoryId
WHERE P.Id = 2
