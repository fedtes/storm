INSERT INTO Users (ID, FirstName, LastName)
VALUES (1, 'Mario', 'Rossi'),
(2,'Chiara', 'Bianchi'),
(3,'Luca', 'Neri');


INSERT INTO UserAddresses (UserID, City, Street, Number)
VALUES (1, 'Roma', 'Via Cavour', 11),
(2,'Milano', 'Via Garibalti', 44),
(3,'Torino', 'Piazza Monterosa', 9);


INSERT INTO UserContacts (UserID, Email, Mobile, Phone)
VALUES (1, 'mario.rossi@test.it', '33333133131', '042166564'),
(2,'chiara.bianchi@test.it', '333332323121', '0421665448'),
(3,'luca.neri@test.it', '3326564522', '0427744866');

INSERT INTO Tickets (ID, UserID, Problem, Solution)
VALUES 
	(1, 1, 'Lorem ipsum', 'not found'),
	(2, 1, 'asd', 'found'),
	(3, 2, 'test', 'ok'),
	(4, 2, 'test', ''),
	(5, 2, 'qweqwe', ''),
	(6, 2, 'Lorem ipsum', 'ko'),
	(7, 2, 'Lorem ipsum', 'not found'),
	(8, 2, 'qwe', 'not found'),
	(9, 1, 'Lorem ipsum', 'ok'),
	(10, 1, 'Lorem ipsum', 'ok'),
	(11, 3, 'Lorem ipsum', ''),
	(12, 3, 'teset', 'ok ko'),
	(13, 3, 'test2', 'ok ko'),
	(14, 3, 'Lorem ipsum', ''),
	(15, 1, 'ipsum', 'not'),
	(16, 1, 'ipsum', 'solved'),
	(17, 2, 'ipsum', 'solved'),
	(18, 2, 'ipsum', 'solved'),
	(19, 1, 'ipsum', ''),
	(20, 1, 'asdipsum', ''),
	(21, 1, 'lora ipsum', '');