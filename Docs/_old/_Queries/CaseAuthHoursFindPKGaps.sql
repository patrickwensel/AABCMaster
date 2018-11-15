-- Report all ID numbers missing from CaseAuthHours
DECLARE @i INT;

SELECT @i = MAX(ID) FROM dbo.CaseAuthHours;

WITH tmp (gapId) AS (
   SELECT DISTINCT a.ID + 1
   FROM dbo.CaseAuthHours a
   WHERE NOT EXISTS( SELECT * FROM dbo.CaseAuthHours b
        WHERE b.ID  = a.ID + 1)
   AND a.ID < @i

   UNION ALL

   SELECT a.gapId + 1
   FROM tmp a
   WHERE NOT EXISTS( SELECT * FROM dbo.CaseAuthHours b
        WHERE b.ID  = a.gapId + 1)
   AND a.gapId < @i
)
SELECT gapId
FROM tmp
ORDER BY gapId;