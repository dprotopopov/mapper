CREATE EXTENSION IF NOT EXISTS hstore;
CREATE SEQUENCE IF NOT EXISTS record_number_seq;

DROP TABLE IF EXISTS place;
DROP TABLE IF EXISTS node;
DROP TABLE IF EXISTS way;
DROP TABLE IF EXISTS relation;
DROP TABLE IF EXISTS temp_node;
DROP TABLE IF EXISTS temp_way;
DROP TABLE IF EXISTS temp_relation;
DROP TYPE IF EXISTS relation_member;

CREATE TABLE place (
	osm_id BIGINT, 
	osm_type INTEGER,
	tags hstore,
	record_number BIGINT DEFAULT nextval('record_number_seq')
);

CREATE TABLE node (
	id BIGINT, 
	version INTEGER, 
	latitude DOUBLE PRECISION, 
	longitude DOUBLE PRECISION,
	change_set_id BIGINT, 
	time_stamp TIMESTAMP,
	user_id INT, 
	user_name VARCHAR(255), 
	visible BOOLEAN, 
	tags hstore,
	record_number BIGINT DEFAULT nextval('record_number_seq')
);

CREATE TABLE way (
	id BIGINT, 
	version INTEGER, 
	change_set_id BIGINT, 
	time_stamp TIMESTAMP,
	user_id INTEGER, 
	user_name VARCHAR(255), 
	visible BOOLEAN, 
	tags hstore,
	nodes BIGINT[],
	record_number BIGINT DEFAULT nextval('record_number_seq')
);

CREATE TYPE relation_member AS (
	id BIGINT, 
    role VARCHAR(255),
    type INTEGER
);

CREATE TABLE relation (
	id BIGINT, 
	version INTEGER, 
	change_set_id BIGINT, 
	time_stamp TIMESTAMP,
	user_id INTEGER, 
	user_name VARCHAR(255), 
	visible BOOLEAN, 
	tags hstore,
	members relation_member[],
	record_number BIGINT DEFAULT nextval('record_number_seq')
);

