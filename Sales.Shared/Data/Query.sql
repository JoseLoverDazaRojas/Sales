USE [Sales2023]

SELECT * FROM [dbo].[Countries]
SELECT * FROM  [dbo].[States]
SELECT * FROM [dbo].[Cities] 
SELECT * FROM  [dbo].[Categories]
SELECT * FROM [dbo].[AspNetUsers]

SELECT * FROM [dbo].[Countries] AS C INNER JOIN
[dbo].[States] AS S ON C.Id = S.CountryId INNER JOIN
[dbo].[Cities] AS Ci ON S.Id = Ci.StateId
WHERE C.Id = 37 AND S.Id = 619 AND Ci.Id = 20624
