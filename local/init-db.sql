CREATE DATABASE public;
       
USE public;
    
CREATE TABLE teachers(
    teacherId VARCHAR(64) PRIMARY KEY,
    name VARCHAR(64) NOT NULL,
    email VARCHAR(64) NOT NULL
);

INSERT INTO teachers (teacherId, name, email) VALUES ('b75n49c', 'Max Brian', 'max.brian@karma.com')