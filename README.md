LeSheet - Back-End (Compte Caro & Moi)

Ce projet est une API REST développée avec ASP.NET Core 10 permettant de gérer les dépenses partagées et les remboursements entre deux utilisateurs (Mathieu et Caroline). Elle calcule automatiquement l'équilibre des comptes et suggère qui doit rembourser qui.
🚀 Technologies utilisées

    .NET 10 (C# 14)

    Entity Framework Core 10 (SQL Server)

    Minimal APIs pour une structure légère et performante

    Scalar pour la documentation interactive de l'API (OpenAPI 3.1)

📋 Fonctionnalités

    Initialisation simplifiée : Création automatique des profils utilisateurs via un point de terminaison dédié.

    Gestion des dépenses : Enregistrement des frais avec description, montant et auteur de la dépense.

    Suivi des remboursements : Enregistrement des paiements directs entre utilisateurs pour ajuster la balance.

    Calcul de balance intelligent : Un algorithme calcule le total dû par chaque personne en prenant en compte les dépenses communes (divisées par 2) et les remboursements déjà effectués.

🛠️ Installation et Lancement
Prérequis

    .NET 10 SDK

    SQL Server (LocalDB ou instance complète)

Configuration

    Clonez le dépôt :
    Bash

    git clone https://github.com/Peemers/LeSheet-Compte-Caro-et-Moi-BackEnd.git

    Configurez votre chaîne de connexion dans le fichier appsettings.json (ou via les secrets d'utilisateur).

    Appliquez les migrations à la base de données :
    Bash

    dotnet ef database update

Exécution

Lancez le projet avec la commande suivante :
Bash

dotnet run --project LeSheet

L'API sera accessible sur http://localhost:5120. La documentation interactive Scalar est disponible à l'adresse : http://localhost:5120/scalar.
📡 Points de terminaison (Endpoints)
Méthode	Route	Description
POST	/api/setup	Initialise les utilisateurs par défaut.
GET	/api/depenses	Récupère l'historique complet des dépenses.
POST	/api/depenses	Ajoute une nouvelle dépense partagée.
POST	/api/remboursement	Enregistre un remboursement entre utilisateurs.
GET	/api/Balance	Affiche l'état des comptes et qui doit combien.
📐 Structure du Projet

    Data/ : Contient le AppDataContext pour la gestion de la base de données avec EF Core.

    Models/ : Contient les entités du domaine (User, Depense, Remboursement) et les DTOs.

    Program.cs : Point d'entrée de l'application contenant la configuration des services et la définition des routes Minimal API.

Projet développé dans un but pédagogique pour l'apprentissage de l'écosystème .NET moderne.
