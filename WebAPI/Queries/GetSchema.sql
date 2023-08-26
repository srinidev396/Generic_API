DECLARE @IsCounter INT;

SELECT @IsCounter = COUNT(*)
FROM Tables 
WHERE TableName = @tablename 
    AND CounterFieldName IS NOT NULL 
    AND CounterFieldName <> '';

SELECT
    c.TABLE_NAME,
    c.COLUMN_NAME,
    c.DATA_TYPE,
    c.CHARACTER_MAXIMUM_LENGTH,
    c.IS_NULLABLE,
    CASE WHEN tc.CONSTRAINT_TYPE = 'PRIMARY KEY' THEN 'Yes' ELSE 'No' END AS IS_PRIMARY_KEY,
    CASE 
        WHEN tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND @IsCounter = 1 THEN 'Yes/no' 
        WHEN tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND col.is_identity = 1 THEN 'Yes' 
		WHEN tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND c.DATA_TYPE = 'nvarchar' THEN 'no'
		WHEN tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND c.DATA_TYPE = 'int' AND col.is_identity = 0 THEN 'no' 
		ELSE '-'
       
    END AS IS_AUTO_INCREMENT,
    CASE 
        WHEN tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND @IsCounter = 1 THEN 'Fusion Counter' 
        WHEN tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND c.DATA_TYPE = 'nvarchar' THEN 'Text' 
        WHEN tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND col.is_identity = 1 THEN 'Automatic Counter' 
        WHEN tc.CONSTRAINT_TYPE = 'PRIMARY KEY' THEN 'long integer'
        ELSE '-'
    END AS PRIMARY_KEY_TYPE
FROM 
    INFORMATION_SCHEMA.COLUMNS c
    LEFT JOIN (
        SELECT
            ku.TABLE_SCHEMA,
            ku.TABLE_NAME,
            ku.COLUMN_NAME,
            tc.CONSTRAINT_TYPE
        FROM
            INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
            JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc ON ku.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
            AND ku.TABLE_SCHEMA = tc.TABLE_SCHEMA
            AND ku.TABLE_NAME = tc.TABLE_NAME
            AND ku.CONSTRAINT_SCHEMA = tc.CONSTRAINT_SCHEMA
        WHERE
            tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
    ) tc ON c.TABLE_SCHEMA = tc.TABLE_SCHEMA
    AND c.TABLE_NAME = tc.TABLE_NAME
    AND c.COLUMN_NAME = tc.COLUMN_NAME
JOIN sys.columns col ON OBJECT_ID(c.TABLE_SCHEMA + '.' + c.TABLE_NAME) = col.object_id
    AND c.COLUMN_NAME = col.name
WHERE
    c.TABLE_NAME = @tablename
ORDER BY
    c.TABLE_SCHEMA,
    c.TABLE_NAME,
    c.ORDINAL_POSITION;

