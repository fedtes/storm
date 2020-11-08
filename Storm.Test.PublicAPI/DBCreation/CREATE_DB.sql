CREATE TABLE Users(
	ID INTEGER PRIMARY KEY,
	FirstName TEXT,
	LastName TEXT
);

CREATE TABLE UserAddresses(
	UserID INTEGER NOT NULL,
	City TEXT,
	Street TEXT,
	Number INT
);

CREATE TABLE UserContacts(
	UserID INTEGER NOT NULL,
	Email TEXT,
	Mobile TEXT,
	Phone TEXT
);


CREATE TABLE Tickets(
	ID INTEGER PRIMARY KEY,
	UserID INTEGER,
	Problem TEXT,
	Solution TEXT
);
