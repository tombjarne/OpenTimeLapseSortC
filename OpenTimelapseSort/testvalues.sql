BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Imports" (
	"id"	INTEGER NOT NULL,
	"timestamp"	TEXT NOT NULL,
	"importDate"	TEXT,
	"name"	TEXT,
	"length"	INTEGER NOT NULL,
	"fetch"	INTEGER NOT NULL,
	CONSTRAINT "PK_Imports" PRIMARY KEY("id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "Directories" (
	"id"	INTEGER NOT NULL,
	"timestamp"	TEXT NOT NULL,
	"target"	TEXT,
	"name"	TEXT,
	"Importid"	INTEGER,
	CONSTRAINT "FK_Directories_Imports_Importid" FOREIGN KEY("Importid") REFERENCES "Imports"("id") ON DELETE RESTRICT,
	CONSTRAINT "PK_Directories" PRIMARY KEY("id" AUTOINCREMENT)
);
CREATE INDEX IF NOT EXISTS "IX_Directories_Importid" ON "Directories" (
	"Importid"
);
COMMIT;
