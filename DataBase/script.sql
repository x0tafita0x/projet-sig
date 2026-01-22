-- Active: 1703146120333@@127.0.0.1@5432@ecole
create extension postgis;

CREATE TABLE ecole(
   id_ecole SERIAL,
   nom_ecole VARCHAR(100)  NOT NULL,
   type_etablissement VARCHAR(50)  NOT NULL,
   emplacement GEOMETRY,
   PRIMARY KEY(id_ecole),
   UNIQUE(nom_ecole)
);

CREATE TABLE categorie(
   id_categorie SERIAL,
   nom_categorie VARCHAR(50) ,
   PRIMARY KEY(id_categorie)
);

CREATE TABLE categorie_ecole(
   id_ecole INTEGER,
   id_categorie INTEGER,
   PRIMARY KEY(id_ecole, id_categorie),
   FOREIGN KEY(id_ecole) REFERENCES ecole(id_ecole) on delete cascade,
   FOREIGN KEY(id_categorie) REFERENCES categorie(id_categorie) on delete cascade
);

create or replace view v_detailEcole as 
select nom_ecole,type_etablissement,emplacement,nom_categorie,e.id_ecole,c.id_categorie
from ecole e
join categorie_ecole ce on ce.id_ecole=e.id_ecole
join categorie c on c.id_categorie=ce.id_categorie;







insert into ecole(nom_ecole, type_etablissement, emplacement) values 
('Lycee Andoharanofotsy', 'Public', ST_SetSRID(ST_MakePoint(-18.99261376411983, 47.5356905241325), 4326)),
('Andrian School Andoharanofotsy', 'Privé', ST_SetSRID(ST_MakePoint(-18.977730442085914, 47.53156972363868), 4326)),
('EPP Andoharanofotsy', 'Public', ST_SetSRID(ST_MakePoint(-18.98770357133361, 47.536490310307045), 4326)),
('Mororano Andoharanofotsy', 'Privé', ST_SetSRID(ST_MakePoint(-18.983327715782867, 47.52752987394125), 4326)),
('La Fontaine', 'Privé', ST_SetSRID(ST_MakePoint(-18.971909117994386, 47.532911765494795), 4326)),
('Les papillons roses', 'Privé', ST_SetSRID(ST_MakePoint(-18.981602185369333, 47.537175057568376), 4326)),
('Mon trésor', 'Privé', ST_SetSRID(ST_MakePoint(-18.973344042374716, 47.53535029309838), 4326)),
('Au P''tit Pré', 'Privé', ST_SetSRID(ST_MakePoint(-18.990988563416664, 47.53477252523486), 4326)),
('Sweet First Year', 'Privé', ST_SetSRID(ST_MakePoint(-18.985052408017626, 47.53389422353079), 4326)),
('Parc des princes', 'Privé', ST_SetSRID(ST_MakePoint(-18.98167282816322, 47.53025960452915), 4326)),
('Platoni Academy', 'Privé', ST_SetSRID(ST_MakePoint(-18.979466729359256, 47.53029647457383), 4326)),
('EPP Ambohimanala', 'Public', ST_SetSRID(ST_MakePoint(-18.977163095723597, 47.520862436522286), 4326)),
('La Source Claire', 'Privé', ST_SetSRID(ST_MakePoint(-18.979598681795093, 47.53465349420369), 4326)),
('Sissi', 'Privé', ST_SetSRID(ST_MakePoint(-18.98060946189626, 47.53524701900559), 4326)),
('Les Lapereaux', 'Privé', ST_SetSRID(ST_MakePoint(-18.977724712555272, 47.53336618148112), 4326)),
('Savoir', 'Privé', ST_SetSRID(ST_MakePoint(-18.97488283160561, 47.53251793832131), 4326)),
('La Flèche', 'Privé', ST_SetSRID(ST_MakePoint(-18.97446743164199, 47.53389330401835), 4326)),
('Ny Amboara', 'Privé', ST_SetSRID(ST_MakePoint(-18.975134935896012, 47.53499904956889), 4326)),
('Saint Pierre Malaza', 'Privé', ST_SetSRID(ST_MakePoint(-18.97228441618455, 47.53007620978074), 4326)),
('La Pimprenelle', 'Privé', ST_SetSRID(ST_MakePoint(-18.97164554930371, 47.52978538355075), 4326)),
('L''espoir', 'Privé', ST_SetSRID(ST_MakePoint(-18.972699395476468, 47.53643674158842), 4326)),
('ISTS - Institut Superieur de Travail Social', 'Privé', ST_SetSRID(ST_MakePoint(-18.993070781276444, 47.533485795861935), 4326)),
('IT University', 'Privé', ST_SetSRID(ST_MakePoint(-18.985946867651666, 47.532794257259475), 4326)),
('Miabo', 'Privé', ST_SetSRID(ST_MakePoint(-18.982096179228833, 47.52512188411459), 4326)),
('EPP Volotara', 'Public', ST_SetSRID(ST_MakePoint(-18.981359467510007, 47.52560581443318), 4326)),
('Noah''s Arc', 'Privé', ST_SetSRID(ST_MakePoint(-18.98091729259148, 47.52637128306711), 4326)),
('La cigogne', 'Privé', ST_SetSRID(ST_MakePoint(-18.975907510766802, 47.52171276355542), 4326));

insert into categorie(nom_categorie) values 
('Primaire'),
('College'),
('Lycee'),
('Universite');

insert into categorie_ecole(id_ecole,id_categorie) values 
(1,2),
(1,3),
(2,1),
(3,1),
(4,2),
(5,1),
(5,2),
(6,1),
(7,1),
(8,1),
(8,2),
(8,3),
(9,1),
(10,1),
(10,2),
(10,3),
(11,1),
(11,2),
(11,3),
(12,1),
(13,3),
(14,3),
(15,2),
(16,2),
(17,3),
(18,1),
(19,3),
(20,1),
(20,2),
(20,3),
(21,2),
(22,4),
(23,4),
(24,2),
(25,1),
(26,1),
(26,3),
(27,2);


