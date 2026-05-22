-- Force l'encodage utf8mb4 pour cette session cliente (accents, emojis)
SET NAMES utf8mb4;

USE kinepalisse;

-- Genres (id 1-5)
INSERT INTO Genre (nom_genre) VALUES
  ('Drame'), ('Action'), ('Comédie'), ('Science-fiction'), ('Animation');

-- Réalisateurs (id 1-7)
INSERT INTO Realisateur (nom, prenom, date_naissance) VALUES
  ('Nolan',      'Christopher', '1970-07-30'),
  ('Bird',       'Brad',        '1957-09-11'),
  ('Villeneuve', 'Denis',       '1967-10-03'),
  ('Wachowski',  'Lana',        '1965-06-21'),
  ('Tarantino',  'Quentin',     '1963-03-27'),
  ('Miyazaki',   'Hayao',       '1941-01-05'),
  ('Bong',       'Joon-ho',     '1969-09-14');

-- Acteurs (id 1-7)
INSERT INTO Acteur (nom, prenom, date_naissance) VALUES
  ('DiCaprio', 'Leonardo', '1974-11-11'),
  ('Page',     'Elliot',   '1987-02-21'),
  ('Chalamet', 'Timothée', '1995-12-27'),
  ('Ferguson', 'Rebecca',  '1983-10-19'),
  ('Reeves',   'Keanu',    '1964-09-02'),
  ('Gosling',  'Ryan',     '1980-11-12'),
  ('Murphy',   'Cillian',  '1976-05-25');

-- Films (id 1-15)
INSERT INTO Film (titre, duree, date_sortie, synopsis, affiche_url, id_realisateur) VALUES
  ('Inception',            148, '2010-07-21', 'Un voleur extrait des secrets dans les rêves.',            NULL, 1),
  ('Interstellar',         169, '2014-11-05', 'Une équipe traverse un trou de ver.',                      NULL, 1),
  ('Ratatouille',          111, '2007-08-01', 'Un rat veut devenir grand chef.',                          NULL, 2),
  ('Dune',                 155, '2021-09-15', 'L''héritier d''une grande famille sur Arrakis.',           NULL, 3),
  ('Dune Part 2',          166, '2024-02-28', 'La suite des aventures de Paul Atréides.',                 NULL, 3),
  ('The Dark Knight',      152, '2008-07-16', 'Batman affronte le Joker à Gotham.',                      NULL, 1),
  ('The Matrix',           136, '1999-03-31', 'Un programmeur découvre la vérité sur sa réalité.',        NULL, 4),
  ('Pulp Fiction',         154, '1994-05-12', 'Des histoires criminelles entrelacées à Los Angeles.',    NULL, 5),
  ('Le Voyage de Chihiro', 125, '2001-07-20', 'Une fillette se retrouve piégée dans le monde des esprits.', NULL, 6),
  ('Parasite',             132, '2019-05-30', 'Deux familles aux destins opposés se percutent.',          NULL, 7),
  ('Blade Runner 2049',    164, '2017-10-04', 'Un réplicant enquête sur un secret qui menace la société.', NULL, 3),
  ('Toy Story',             81, '1995-11-22', 'Des jouets prennent vie quand les humains ont le dos tourné.', NULL, NULL),
  ('Avengers: Endgame',    181, '2019-04-24', 'Les Avengers tentent d''annuler le claquement de Thanos.', NULL, NULL),
  ('Oppenheimer',          180, '2023-07-19', 'L''histoire du père de la bombe atomique.',               NULL, 1),
  ('La La Land',           128, '2016-11-30', 'Une actrice et un pianiste se rencontrent à Los Angeles.', NULL, NULL);

-- Liaisons Film_Genre
INSERT INTO Film_Genre (id_film, id_genre) VALUES
  (1,  4), (1,  2),   -- Inception : SF + Action
  (2,  4), (2,  1),   -- Interstellar : SF + Drame
  (3,  5), (3,  3),   -- Ratatouille : Animation + Comédie
  (4,  4), (4,  1),   -- Dune : SF + Drame
  (5,  4), (5,  1),   -- Dune Part 2 : SF + Drame
  (6,  2), (6,  1),   -- The Dark Knight : Action + Drame
  (7,  4), (7,  2),   -- The Matrix : SF + Action
  (8,  1), (8,  2),   -- Pulp Fiction : Drame + Action
  (9,  5),            -- Le Voyage de Chihiro : Animation
  (10, 1),            -- Parasite : Drame
  (11, 4), (11, 1),   -- Blade Runner 2049 : SF + Drame
  (12, 5), (12, 3),   -- Toy Story : Animation + Comédie
  (13, 2), (13, 4),   -- Avengers: Endgame : Action + SF
  (14, 1),            -- Oppenheimer : Drame
  (15, 3), (15, 1);   -- La La Land : Comédie + Drame

-- Liaisons Film_Acteur
INSERT INTO Film_Acteur (id_film, id_acteur) VALUES
  (1,  1), (1,  2),   -- Inception : DiCaprio, Page
  (2,  1),            -- Interstellar : DiCaprio
  (4,  3), (4,  4),   -- Dune : Chalamet, Ferguson
  (5,  3), (5,  4),   -- Dune Part 2 : Chalamet, Ferguson
  (6,  7),            -- The Dark Knight : Murphy
  (7,  5),            -- The Matrix : Reeves
  (11, 6),            -- Blade Runner 2049 : Gosling
  (14, 7),            -- Oppenheimer : Murphy
  (15, 6);            -- La La Land : Gosling

-- Salles
INSERT INTO Salle (nom_salle, capacite) VALUES
  ('Salle 1',  80),
  ('Salle 2', 120),
  ('Salle 3',  50);

-- Tarifs : modèle événementiel (pas de tarif catégoriel enfant/étudiant/senior)
INSERT INTO Tarif (type_tarif, prix) VALUES
  ('Normal',              10.00),
  ('Promotion -20%',       8.00),
  ('Fête du Cinéma -50%',  5.00);

-- Séances dans les prochains jours (id_film: 1=Inception, 2=Interstellar, 3=Ratatouille, 4=Dune)
INSERT INTO Seance (date_heure, id_film, id_salle, id_tarif) VALUES
  (DATE_ADD(UTC_TIMESTAMP(), INTERVAL 1 DAY),                        1, 1, 1),
  (DATE_ADD(UTC_TIMESTAMP(), INTERVAL 1 DAY) + INTERVAL 4 HOUR,     2, 1, 1),
  (DATE_ADD(UTC_TIMESTAMP(), INTERVAL 2 DAY),                        3, 2, 3),
  (DATE_ADD(UTC_TIMESTAMP(), INTERVAL 3 DAY),                        4, 3, 1);
