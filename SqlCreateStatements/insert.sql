USE musiclibrary;

INSERT INTO Artist VALUES
(1, 'FKJ'),
(2, 'MAMAMOO'),
(3, 'Snails House'),
(4, 'Keshi'),
(5, 'Polyphia'),
(6, 'Chon'),
(7, 'Red Velvet'),
(8, 'YOASOBI');
COMMIT;

INSERT INTO Album VALUES
(1, 'French Kiwi Juice', 2017, 1),
(2, 'Ylang Ylang', 2019, 1),
(3, 'V I N C E N T', 2022, 1),
(4, 'Reality in Black', 2019, 2),
(5, 'Ordinary Songs 2', 2016, 3),
(6, 'Pixel Galaxy', 2017, 3),
(7, 'GABRIEL', 2022, 4),
(8, 'New Levels New Devils', 2018, 5),
(9, 'The Worst', 2017, 5),
(10, 'CHON', 2019, 6),
(11, 'RBB', 2018, 7),
(12, 'Rookie', 2017, 7),
(13, 'THE BOOK', 2021, 8),
(14, 'THE BOOK 2', 2021 , 8);
COMMIT;

INSERT INTO Genre VALUES
(1, 'Jazz'),
(2, 'Alternative'),
(3, 'Math Rock');
COMMIT;

-- More songs could still be added but this is a good start
INSERT INTO Song VALUES
(1, 'Different Masks for Different Days', 1, 3, 1),
(2, 'UNDERSTAND', 4, 7, 2),
(3, 'Canggu', 1, 1, NULL),
(4, 'G.O.A.T.', 5, 8, 3);
COMMIT;