-- EXECUTED at every program start


CREATE TABLE if not exists "version"(
	"id" INTEGER,
	"db_version" INTEGER DEFAULT 1,
	"api_version" INTEGER DEFAULT 1,
	"api_valid_until" INTEGER,
	PRIMARY KEY("id" AUTOINCREMENT)
);


CREATE TABLE if not exists "trackdatas" (
	"id"	INTEGER,
	"name"	TEXT,
	"local_file"	TEXT,
	"uri"	TEXT,
	"image_uri"	TEXT,
	"description"	TEXT,
	"author"	TEXT,
	"album"	TEXT,
	"genre"	TEXT,
	"duration"	INTEGER,
	PRIMARY KEY("id" AUTOINCREMENT)
);


CREATE TABLE if not exists "buttons" (
	"id"	INTEGER,
	"position"	INTEGER NOT NULL,
	"nick_name"	TEXT,
	"is_earrape"	INTEGER DEFAULT 0,
	"is_loop"	INTEGER DEFAULT 0,
	"track_id"	INTEGER,
	PRIMARY KEY("id" AUTOINCREMENT),
	FOREIGN KEY("track_id") REFERENCES "trackdatas"("id") ON UPDATE CASCADE ON DELETE SET NULL
);


CREATE TABLE if not exists "config" (
	"id"	INTEGER,
	"bot_token"	TEXT,
	"bot_volume"	REAL DEFAULT 1,
	"bot_owner_id"	INTEGER DEFAULT 0,
	"button_width"	INTEGER,
	"button_height"	INTEGER,
	PRIMARY KEY("id" AUTOINCREMENT)
);





CREATE TRIGGER if not EXISTS version_no_insert
BEFORE INSERT ON version
WHEN (SELECT COUNT(*) FROM version) >= 1   -- limit here
BEGIN
    SELECT RAISE(FAIL, 'only one row for version');
END;




INSERT INTO version (id, db_version, api_version) SELECT 1, 1, 1 WHERE NOT EXISTS (SELECT * FROM version);
INSERT INTO config (id) SELECT 1 WHERE NOT EXISTS (SELECT * FROM config);