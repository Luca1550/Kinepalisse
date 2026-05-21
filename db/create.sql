-- =============================================================================
-- Kinepalisse — Script de création de la base de données MySQL
-- =============================================================================
-- À exécuter manuellement (MySQL Workbench, DBeaver ou ligne de commande) :
--   mysql -u root -p < docs/create.sql
--
-- Préalable (à faire une seule fois, en tant que root) :
--   CREATE DATABASE kinepalisse CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
--   CREATE USER 'kine_app'@'localhost' IDENTIFIED BY 'KineDev_2026!';
--   GRANT ALL PRIVILEGES ON kinepalisse.* TO 'kine_app'@'localhost';
--   FLUSH PRIVILEGES;
--
-- Puis :
--   USE kinepalisse;
--   SOURCE docs/create.sql;
-- =============================================================================

-- Sécurité : si on relance le script, on repart de zéro proprement.
-- (Désactive temporairement les contraintes pour pouvoir DROP dans n'importe quel ordre.)
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS Paiement;
DROP TABLE IF EXISTS Reservation;
DROP TABLE IF EXISTS Seance;
DROP TABLE IF EXISTS Film_Acteur;
DROP TABLE IF EXISTS Film_Genre;
DROP TABLE IF EXISTS Film;
DROP TABLE IF EXISTS Client;
DROP TABLE IF EXISTS Utilisateur;
DROP TABLE IF EXISTS Tarif;
DROP TABLE IF EXISTS Salle;
DROP TABLE IF EXISTS Realisateur;
DROP TABLE IF EXISTS Acteur;
DROP TABLE IF EXISTS Genre;

SET FOREIGN_KEY_CHECKS = 1;

-- -----------------------------------------------------------------------------
-- Tables "catalogue" (sans dépendance)
-- -----------------------------------------------------------------------------

CREATE TABLE Genre (
    id_genre   INT AUTO_INCREMENT PRIMARY KEY,
    nom_genre  VARCHAR(100) NOT NULL UNIQUE
) 

CREATE TABLE Acteur (
    id_acteur      INT AUTO_INCREMENT PRIMARY KEY,
    nom            VARCHAR(100) NOT NULL,
    prenom         VARCHAR(100) NOT NULL,
    date_naissance DATE NULL
) 

CREATE TABLE Realisateur (
    id_realisateur INT AUTO_INCREMENT PRIMARY KEY,
    nom            VARCHAR(100) NOT NULL,
    prenom         VARCHAR(100) NOT NULL,
    date_naissance DATE NULL
) 

CREATE TABLE Salle (
    id_salle   INT AUTO_INCREMENT PRIMARY KEY,
    nom_salle  VARCHAR(100) NOT NULL UNIQUE,
    capacite   INT NOT NULL,
    CHECK (capacite > 0)
) 

CREATE TABLE Tarif (
    id_tarif    INT AUTO_INCREMENT PRIMARY KEY,
    type_tarif  VARCHAR(50)    NOT NULL UNIQUE,
    prix        DECIMAL(10, 2) NOT NULL,
    CHECK (prix >= 0)
) 

-- -----------------------------------------------------------------------------
-- Utilisateur + Client
-- -----------------------------------------------------------------------------

CREATE TABLE Utilisateur (
    id_utilisateur     INT AUTO_INCREMENT PRIMARY KEY,
    email              VARCHAR(255) NOT NULL UNIQUE,
    mot_de_passe_hash  VARCHAR(255) NOT NULL,                       -- hash BCrypt
    role               VARCHAR(20)  NOT NULL,                       -- 'Client', 'Admin' ou 'Guichetier'
    date_creation      DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
) 

CREATE TABLE Client (
    id_client       INT AUTO_INCREMENT PRIMARY KEY,
    nom             VARCHAR(100) NOT NULL,
    prenom          VARCHAR(100) NOT NULL,
    email           VARCHAR(255) NULL,
    telephone       VARCHAR(30)  NULL,
    id_utilisateur  INT NULL,
    CONSTRAINT fk_client_user
        FOREIGN KEY (id_utilisateur) REFERENCES Utilisateur(id_utilisateur)
        ON DELETE CASCADE
) 

-- -----------------------------------------------------------------------------
-- Film + tables de liaison N-N
-- -----------------------------------------------------------------------------

CREATE TABLE Film (
    id_film         INT AUTO_INCREMENT PRIMARY KEY,
    titre           VARCHAR(200) NOT NULL,
    duree           INT          NOT NULL,                          -- en minutes
    date_sortie     DATE         NULL,
    synopsis        TEXT         NULL,
    affiche_url     VARCHAR(500) NULL,
    id_realisateur  INT          NULL,
    CHECK (duree > 0),
    CONSTRAINT fk_film_realisateur
        FOREIGN KEY (id_realisateur) REFERENCES Realisateur(id_realisateur)
        ON DELETE SET NULL
) 

CREATE TABLE Film_Genre (
    id_film   INT NOT NULL,
    id_genre  INT NOT NULL,
    PRIMARY KEY (id_film, id_genre),
    CONSTRAINT fk_fg_film  FOREIGN KEY (id_film)  REFERENCES Film(id_film)   ON DELETE CASCADE,
    CONSTRAINT fk_fg_genre FOREIGN KEY (id_genre) REFERENCES Genre(id_genre) ON DELETE CASCADE
) 

CREATE TABLE Film_Acteur (
    id_film    INT NOT NULL,
    id_acteur  INT NOT NULL,
    PRIMARY KEY (id_film, id_acteur),
    CONSTRAINT fk_fa_film   FOREIGN KEY (id_film)   REFERENCES Film(id_film)     ON DELETE CASCADE,
    CONSTRAINT fk_fa_acteur FOREIGN KEY (id_acteur) REFERENCES Acteur(id_acteur) ON DELETE CASCADE
) 

-- -----------------------------------------------------------------------------
-- Séance, Réservation, Paiement
-- -----------------------------------------------------------------------------

CREATE TABLE Seance (
    id_seance   INT AUTO_INCREMENT PRIMARY KEY,
    date_heure  DATETIME NOT NULL,
    id_film     INT NOT NULL,
    id_salle    INT NOT NULL,
    id_tarif    INT NOT NULL,
    CONSTRAINT fk_seance_film  FOREIGN KEY (id_film)  REFERENCES Film(id_film)   ON DELETE RESTRICT,
    CONSTRAINT fk_seance_salle FOREIGN KEY (id_salle) REFERENCES Salle(id_salle) ON DELETE RESTRICT,
    CONSTRAINT fk_seance_tarif FOREIGN KEY (id_tarif) REFERENCES Tarif(id_tarif) ON DELETE RESTRICT,
    -- Index utile pour la détection de conflits (recherche par salle + créneau)
    INDEX idx_seance_salle_date (id_salle, date_heure)
) 

CREATE TABLE Reservation (
    id_reservation    INT AUTO_INCREMENT PRIMARY KEY,
    id_client         INT NOT NULL,
    id_seance         INT NOT NULL,
    nb_places         INT NOT NULL,
    date_reservation  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    statut            VARCHAR(20) NOT NULL DEFAULT 'Confirmee',   -- 'Confirmee' ou 'Annulee'
    CHECK (nb_places > 0),
    CONSTRAINT fk_resa_client  FOREIGN KEY (id_client)  REFERENCES Client(id_client)   ON DELETE CASCADE,
    CONSTRAINT fk_resa_seance  FOREIGN KEY (id_seance)  REFERENCES Seance(id_seance)   ON DELETE CASCADE,
    INDEX idx_resa_seance_statut (id_seance, statut)              -- accélère SUM(nb_places) par séance
) 

CREATE TABLE Paiement (
    id_paiement     INT AUTO_INCREMENT PRIMARY KEY,
    id_reservation  INT NOT NULL,
    montant         DECIMAL(10, 2) NOT NULL,
    mode_paiement   VARCHAR(30)   NOT NULL,                       -- 'CarteEnLigne', 'Especes', 'Bancontact'
    statut          VARCHAR(20)   NOT NULL DEFAULT 'Paye',        -- 'Paye' ou 'Rembourse'
    date_paiement   DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CHECK (montant >= 0),
    CONSTRAINT fk_paiement_resa
        FOREIGN KEY (id_reservation) REFERENCES Reservation(id_reservation) ON DELETE CASCADE
) 

-- =============================================================================
-- Fin du schéma. Pour les données de démo, exécute ensuite db/seed.sql.
-- =============================================================================
