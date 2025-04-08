CREATE TABLE "CodeValues"
(
    "Id" SERIAL NOT NULL, -- порядковый номер + первичный ключ
    "Code" integer NOT NULL, -- поле code
    "Value" text NOT NULL, -- поле value
    CONSTRAINT "PK_CodeValues" PRIMARY KEY ("Id")
)