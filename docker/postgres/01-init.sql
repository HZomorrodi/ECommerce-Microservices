-- Create the Users table
CREATE TABLE IF NOT EXISTS public."Users"
(
    "UserId" uuid NOT NULL,           -- Changed from UserID to UserId
    "PersonName" character varying(50) NOT NULL,
    "Email" character varying(50) NOT NULL,
    "Password" character varying(50) NOT NULL,
    "Gender" character varying(15),
    CONSTRAINT "Users_pkey" PRIMARY KEY ("UserId")
);