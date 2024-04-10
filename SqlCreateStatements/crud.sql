USE musiclibrary;
-- CREATE
INSERT INTO Artist VALUES
(1, 'FKJ');

INSERT INTO Album VALUES
(1, 'French Kiwi Juice', 2017, 1);

INSERT INTO Song VALUES
(1, 'Go Back Home', 1, 1);
-- READ
SELECT * FROM Artist WHERE artist_id = 1;

SELECT release_year FROM Album WHERE album_id = 1;

SELECT song_title FROM Song WHERE album_id = 1;

-- UPDATE
UPDATE Artist
SET artist_name = 'French Kiwi Juice'
WHERE artist_id = 1;

UPDATE Album
SET Release_Year = 2018
WHERE artist_id = 1;

UPDATE Song
SET song_title = 'Lying Together'
WHERE album_id = 1;

-- DELETE
DELETE FROM Song WHERE song_title = 'Lying Together';
DELETE FROM Album WHERE Release_Year = 2018;
DELETE FROM Artist WHERE artist_name = 'French Kiwi Juice';