CREATE TABLE "extension" (
	"identify"	TEXT NOT NULL UNIQUE,
	"name"	TEXT NOT NULL UNIQUE,
	"version"	TEXT NOT NULL,
	"arch"	TEXT,
	PRIMARY KEY("identify")
)
