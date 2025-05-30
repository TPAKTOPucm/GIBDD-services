CREATE SCHEMA IF NOT EXISTS camera;

CREATE TABLE IF NOT EXISTS camera.cars (
    id SERIAL PRIMARY KEY,
    base_number VARCHAR(6) NOT NULL,
    region INTEGER NOT NULL,
    registration_date TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT unique_plate UNIQUE (base_number, region)
);

CREATE INDEX IF NOT EXISTS idx_cars_plate ON camera.cars (base_number, region);