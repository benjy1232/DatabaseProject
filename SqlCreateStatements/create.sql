CREATE DATABASE IF NOT EXISTS musiclibrary;
USE musiclibrary;

CREATE TABLE Artist (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    artist_name VARCHAR(100) NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE Album (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    album_name VARCHAR(100) NOT NULL,
    release_year YEAR NOT NULL,
    artist_id INT UNSIGNED NOT NULL,
    PRIMARY KEY (id),
    CONSTRAINT fk_artist_id FOREIGN KEY (artist_id) REFERENCES Artist (id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Genre (
                       id SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
                       name VARCHAR(50) NOT NULL,
                       PRIMARY KEY (id)
);

# Separate artist_id for song than album because song can have a featured artist that the album does not
CREATE TABLE Song (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    song_title VARCHAR(100) NOT NULL,
    artist_id INT UNSIGNED NOT NULL,
    album_id INT UNSIGNED NOT NULL,
    genre_id SMALLINT UNSIGNED,
    PRIMARY KEY (id),
    CONSTRAINT fk_song_album FOREIGN KEY (album_id) REFERENCES Album (id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_song_artist FOREIGN KEY (artist_id) REFERENCES Artist (id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_song_genre FOREIGN KEY (genre_id) REFERENCES Genre (id) ON DELETE CASCADE ON UPDATE CASCADE
);