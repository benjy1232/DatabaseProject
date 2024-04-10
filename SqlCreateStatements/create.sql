CREATE DATABASE musiclibrary;
USE musiclibrary;

CREATE TABLE Artist (
    artist_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    artist_name VARCHAR(100) NOT NULL,
    PRIMARY KEY (artist_id)
);

CREATE TABLE Album (
    album_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    album_name VARCHAR(100) NOT NULL,
    release_year YEAR NOT NULL,
    artist_id INT UNSIGNED NOT NULL,
    PRIMARY KEY (album_id),
    CONSTRAINT fk_artist_id FOREIGN KEY (artist_id) REFERENCES Artist (artist_id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE Song (
    song_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    song_title VARCHAR(100) NOT NULL,
    artist_id INT UNSIGNED NOT NULL,
    album_id INT UNSIGNED NOT NULL,
    PRIMARY KEY (song_id),
    CONSTRAINT fk_song_album FOREIGN KEY (album_id) REFERENCES Album (album_id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_song_artist FOREIGN KEY (artist_id) REFERENCES Artist (artist_id) ON DELETE RESTRICT ON UPDATE CASCADE
);