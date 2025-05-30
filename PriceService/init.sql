CREATE SCHEMA IF NOT EXISTS price_service;

CREATE TABLE IF NOT EXISTS price_service.fines (
    reason TEXT PRIMARY KEY,
    price NUMERIC(10, 2) NOT NULL
);

CREATE TABLE IF NOT EXISTS price_service.payment (
    id INTEGER PRIMARY KEY,
    bank_code INTEGER NOT NULL,
    account_code INTEGER NOT NULL
);

INSERT INTO price_service.payment VALUES(1, 543424, 220023553);

INSERT INTO price_service.fines
VALUES ('Speeding (20-40 km/h)', 750),
       ('Speeding (40-60 km/h)', 1500),
       ('Speeding (60+ km/h)', 5000),
       ('Running a red light', 3000),
       ('Parking in a no-parking zone', 5000),
       ('Seatbelt violation', 1500),
       ('Invalid parking', 2000),
       ('Expired registration or insurance', 800);