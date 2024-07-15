SELECT
	P.Id,
	P.[Name],
	P.Navigation,
	P.[Description],
	P.Revision,
	P.CreatedByUserId,
	P.CreatedDate,
	P.ModifiedByUserId,
	P.ModifiedDate,
	Createduser.AccountName as CreatedByUserName,
	ModifiedUser.AccountName as ModifiedByUserName,
	(SELECT COUNT(0) FROM deletedpagerevisions_db.[PageRevision] WHERE PageId = P.Id) as DeletedRevisionCount,
	@PageSize as PaginationPageSize,
	(
		SELECT
			CAST((Count(0) + (@PageSize - 1.0)) / @PageSize AS INTEGER)
		FROM
			[Page] as P
		WHERE
			P.Id IN (SELECT PID.Value FROM TempPageIds as PID)
	) as PaginationPageCount
FROM
	[Page] as P
INNER JOIN users_db.Profile as ModifiedUser
	ON ModifiedUser.UserId = P.ModifiedByUserId
INNER JOIN users_db.Profile as Createduser
	ON Createduser.UserId = P.CreatedByUserId
WHERE
	P.Id IN (SELECT PID.Value FROM TempPageIds as PID)
ORDER BY
	P.[Name],
	P.Id
LIMIT @PageSize
OFFSET (@PageNumber - 1) * @PageSize
