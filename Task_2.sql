-- Clients - клиенты
-- (
-- Id bigint, -- Id клиента
-- ClientName nvarchar(200) -- Наименование клиента
-- );
-- ClientContacts - контакты клиентов
-- (
-- Id bigint, -- Id контакта
-- ClientId bigint, -- Id клиента
-- ContactType nvarchar(255), -- тип контакта
-- ContactValue nvarchar(255) --  значение контакта
-- );
-- 1.	Написать запрос, который возвращает наименование клиентов и кол-во контактов клиентов
-- 2.	Написать запрос, который возвращает список клиентов, у которых есть более 2 контактов
DROP TABLE IF EXISTS CLIENTS,
CLIENT_CONTACTS;

CREATE TABLE CLIENTS (ID BIGSERIAL PRIMARY KEY, CLIENT_NAME CHAR(200));

CREATE TABLE CLIENT_CONTACTS (
	ID BIGSERIAL PRIMARY KEY,
	CLIENT_ID BIGINT REFERENCES CLIENTS (ID) ON DELETE CASCADE,
	CONTACT_TYPE CHAR(255) NOT NULL,
	CONTACT_VALUE CHAR(255) NOT NULL
);

INSERT INTO
	CLIENTS (CLIENT_NAME)
VALUES
	('Anton'),
	('Vova'),
	('Vasya'),
	('Kirill'),
	('Maksim'),
	('Aleksandr');

INSERT INTO
	CLIENT_CONTACTS (CLIENT_ID, CONTACT_TYPE, CONTACT_VALUE)
VALUES
	(1, 'home', '324585'),
	(1, 'mobile', '79552456754'),
	(1, 'mobile', '79596346712'),
	(2, 'home', '324595'),
	(2, 'mobile', '79595346715'),
	(3, 'mobile', '79144759596'),
	(4, 'home', '230139'),
	(4, 'mobile', '79265962535'),
	(5, 'home', '445986'),
	(6, 'home', '324585'),
	(6, 'home', '324595'),
	(6, 'mobile', '79267777777'),
	(6, 'mobile', '79265555555');

-- 1.
SELECT
	CLIENT_NAME,
	COUNT(*) AS COUNT
FROM
	CLIENTS
	INNER JOIN CLIENT_CONTACTS ON CLIENTS.ID = CLIENT_CONTACTS.CLIENT_ID
GROUP BY
	CLIENT_NAME
ORDER BY
	COUNT DESC;

-- 2.
SELECT
	CLIENT_NAME,
	COUNT(*) AS "COUNT"
FROM
	CLIENTS
	INNER JOIN CLIENT_CONTACTS ON CLIENTS.ID = CLIENT_CONTACTS.CLIENT_ID
GROUP BY
	CLIENT_NAME
HAVING
	COUNT(*) > 2
ORDER BY
	"COUNT" DESC;