USE kinepalisse;

-- Genres
INSERT INTO Genre (nom_genre) VALUES
  ('Drame'), ('Action'), ('Comédie'), ('Science-fiction'), ('Animation');

-- Réalisateurs
INSERT INTO Realisateur (nom, prenom, date_naissance) VALUES
  ('Nolan', 'Christopher', '1970-07-30'),
  ('Bird',  'Brad',        '1957-09-11'),
  ('Villeneuve', 'Denis',  '1967-10-03');

-- Acteurs
INSERT INTO Acteur (nom, prenom, date_naissance) VALUES
  ('DiCaprio', 'Leonardo', '1974-11-11'),
  ('Page',     'Elliot',   '1987-02-21'),
  ('Chalamet', 'Timothée', '1995-12-27'),
  ('Ferguson', 'Rebecca',  '1983-10-19');

-- Films
INSERT INTO Film (titre, duree, date_sortie, synopsis, affiche_url, id_realisateur) VALUES
  ('Inception',    148, '2010-07-21', 'Un voleur extrait des secrets dans les rêves.', NULL, 1),
  ('Interstellar', 169, '2014-11-05', 'Une équipe traverse un trou de ver.',           NULL, 1),
  ('Ratatouille',  111, '2007-08-01', 'Un rat veut devenir grand chef.',               NULL, 2),
  ('Dune',         155, '2021-09-15', 'L''héritier d''une grande famille sur Arrakis.', NULL, 3),
  ('Dune Part 2',  166, '2024-02-28', 'La suite des aventures de Paul Atréides.',       NULL, 3);

-- Liaisons N-N (id_film, id_genre/id_acteur)
INSERT INTO Film_Genre (id_film, id_genre) VALUES
  (1, 4), (1, 2),    -- Inception : SF + Action
  (2, 4), (2, 1),    -- Interstellar : SF + Drame
  (3, 5), (3, 3),    -- Ratatouille : Animation + Comédie
  (4, 4), (4, 1),    -- Dune : SF + Drame
  (5, 4), (5, 1);    -- Dune Part 2 : SF + Drame

INSERT INTO Film_Acteur (id_film, id_acteur) VALUES
  (1, 1), (1, 2),
  (2, 1),
  (4, 3), (4, 4),
  (5, 3), (5, 4);
