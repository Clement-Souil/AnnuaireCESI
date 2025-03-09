# 🗂️ Annuaire d'Entreprise - C# WPF MVVM

## 📌 Description

Ce projet est une **application WPF** d'annuaire d'entreprise, développée en **C# avec l’architecture MVVM**. Il permet de gérer les utilisateurs et administrateurs via une **authentification sécurisée** et repose sur une base de données **MySQL**.

## 🚀 Fonctionnalités

- 🔐 **Authentification des utilisateurs** (email & mot de passe)
- 🎭 **Gestion des rôles** (Administrateur / Utilisateur)
- 📇 **Affichage et recherche des employés**
- ✏️ **Ajout, modification et suppression** de contacts (réservé aux admins)
- 🗃️ **Architecture MVVM**
- 🏗️ **Application modulaire et évolutive**

## 🛠️ Technologies utilisées

- **Langage** : C#
- **Framework** : WPF (Windows Presentation Foundation)
- **Architecture** : MVVM
- **Base de données** : MySQL
- **ORM** : Entity Framework Core
- **Version de .NET** : .NET 8
- **Versioning** : Git + GitHub

## 📋 Prérequis

Avant d’installer et d’exécuter ce projet, assure-toi d’avoir :

- **.NET 8** installé ([Télécharger ici](https://dotnet.microsoft.com/en-us/download))
- **MySQL** installé et configuré
- **Visual Studio 2022+** avec les extensions WPF et C#
- **Git** installé pour cloner le projet

## 🏗️ Installation

1. **Cloner le dépôt**
   ```sh
   git clone https://github.com/Clement-Souil/AnnuaireCESI.git
   cd AnnuaireCESI
   ```

2. **Configurer la base de données**
   - Modifier `appsettings.json` avec tes identifiants MySQL
   - Exécuter les migrations Entity Framework Core :
     ```sh
     dotnet ef database update
     ```
   - Utiliser le **Seeder** inclus pour générer des données de test

3. **Lancer l’application**
   - Dans **Visual Studio**, exécuter `AnnuaireApp` en mode Debug

## 🏛️ Architecture du projet

📂 **AnnuaireCESI**  
├── 📁 **AnnuaireAPI** *(Backend - API ASP.NET Core)*  
│   ├── **Controllers** *(Gestion des requêtes API)*  
│   ├── **appsettings.json** *(Configuration de l’API)*  
│   ├── **Program.cs** *(Point d’entrée de l’API)*  
│  
├── 📁 **AnnuaireApp** *(Frontend - Application WPF)*  
│   ├── **Views** *(Interfaces utilisateur en XAML)*  
│   ├── **ViewModels** *(Gestion de la logique MVVM)*  
│   ├── **Models, Services, Helpers** *(Données, logique métier et utilitaires)*  
│   ├── **App.xaml** *(Configuration globale de l’application WPF)*  
│  
├── 📁 **AnnuaireLibrary** *(Bibliothèque commune - Gestion des données)*  
│   ├── **DAO, DbContext** *(Accès et gestion de la base de données MySQL)*  
│   ├── **DTO, Factories** *(Structures de données et création d’objets)*  
│   ├── **Migrations** *(Gestion des mises à jour de la base de données)*  
│  
├── 📁 **Seeder** *(Remplissage automatique de la base de données)*  

## 🛡️ Sécurité & Authentification

- **Stockage sécurisé des mots de passe** avec un **hachage Bcrypt**
- **Gestion des rôles** basée sur `idRole` en base de données
- **Protection des accès administratifs** via des vérifications côté serveur

## ✅ Améliorations prévues

- 📌 Implémentation d’un **système de permissions avancé**
- 🌐 Déploiement d’une **API REST** pour une version web
- 📊 Ajout de **statistiques et rapports** sur l’utilisation de l’annuaire

