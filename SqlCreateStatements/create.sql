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
    CONSTRAINT fk_artist_id FOREIGN KEY (artist_id) REFERENCES Artist (id) ON DELETE RESTRICT ON UPDATE CASCADE
);

# Separate artist_id for song than album because song can have a featured artist that the album does not
CREATE TABLE Song (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    song_title VARCHAR(100) NOT NULL,
    artist_id INT UNSIGNED NOT NULL,
    album_id INT UNSIGNED NOT NULL,
    song_price DOUBLE UNSIGNED NOT NULL,
    genre_id SMALLINT,
    PRIMARY KEY (id),
    CONSTRAINT fk_song_album FOREIGN KEY (album_id) REFERENCES Album (id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_song_artist FOREIGN KEY (artist_id) REFERENCES Artist (id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_song_genre FOREIGN KEY (genre_id) REFERENCES Genre (id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE Genre (
    id SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    name VARCHAR(50) NOT NULL,
    PRIMARY KEY (id)
);

# For User use Basic Authentication and unhashed passwords (Not good practice)
CREATE TABLE User (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT,
    user_name VARCHAR(100) NOT NULL,
    password VARCHAR(100) NOT NULL,
    PRIMARY KEY (id)
);

# Each users library is obtained from this large table connecting the user to the songs they own
CREATE TABLE UserSongs (
    user_id INT UNSIGNED NOT NULL,
    song_id INT UNSIGNED NOT NULL,
    CONSTRAINT fk_user_id FOREIGN KEY (user_id) REFERENCES User (id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_song_id FOREIGN KEY (song_id) REFERENCES Song (id) ON DELETE RESTRICT ON UPDATE CASCADE
);