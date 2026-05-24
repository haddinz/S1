SELECT * from "AspNetRoles"

SELECT table_name
from information_schema.tables
where
    table_schema = 'public'
ORDER BY table_name