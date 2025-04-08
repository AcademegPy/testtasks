-- Дана таблица:
-- Dates - клиенты
-- (
--         	Id bigint,
--         	Dt date
-- );
-- 1.	Написать запрос, который возвращает интервалы для одинаковых Id. Например, есть такой набор данных:
-- Id	Dt
-- 1	01.01.2021
-- 1	10.01.2021
-- 1	30.01.2021
-- 2	15.01.2021
-- 2	30.01.2021
DROP TABLE IF EXISTS DATES;

CREATE TABLE DATES (ID BIGINT, DT DATE NOT NULL);

INSERT INTO
	DATES
VALUES
	(1, '2021-01-01'),
	(1, '2021-01-10'),
	(1, '2021-01-30'),
	(2, '2021-01-15'),
	(2, '2021-01-30');

SELECT
	*
FROM
	(
		SELECT
			ID,
			DT AS SD,
			LEAD(DT) OVER (
				PARTITION BY
					ID
				ORDER BY
					DT
			) AS ED
		FROM
			DATES
	)
WHERE
	ED IS NOT NULL