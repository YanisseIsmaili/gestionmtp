# Yprotect - Gestionnaire de mots de passe

## Description
Application de gestion de mots de passe développée en C# avec Avalonia UI. Interface cyberpunk avec thèmes multiples, base SQLite et import CSV/dictionnaire.

## Prérequis
- **.NET 8.0** ou supérieur
- **Visual Studio 2022** ou **VS Code** avec extension C#

## Installation

### 1. Vérifier .NET
```bash
dotnet --version
```
Si manquant : [Télécharger .NET 8](https://dotnet.microsoft.com/download)

### 2. Cloner le projet
```bash
git clone [URL_DU_REPO]
cd gestionmtp
```

### 3. Restaurer les packages
```bash
dotnet restore
```

## Lancement

### Build et Run
```bash
dotnet build Yprotect.csproj
dotnet run --project Yprotect.csproj
```

### Ou depuis dossier projet
```bash
dotnet build
dotnet run
```

## Première utilisation

### Connexion par défaut
- **Email** : `admin@yprotect.com`
- **Mot de passe** : `AdminPassword123!`

### Base de données
- Créée automatiquement au premier lancement
- SQLite : `yprotect.db` dans le dossier bin

## Fonctionnalités

### Interface principale
- **CRUD** mots de passe
- **Recherche** temps réel
- **Import CSV** (format : Site,Username,Password)
- **Thèmes** : System/Dark/Light/Neon
- **Paramètres** : Import dictionnaire

### Formats supportés
- **CSV mots de passe** : `site,username,password`
- **Dictionnaire** : `.txt` (un mot par ligne) ou `.csv`

## Structure technique

### Architecture
- **MVVM** : Model-View-ViewModel
- **Base** : SQLite avec Entity Framework Core
- **UI** : Avalonia (cross-platform)

### Tables
- `Utilisateurs` : Comptes (admin par défaut)
- `Passwords` : Mots de passe chiffrés
- `MotsDictionnaire` : Dictionnaire importé

## Dépendances principales
```xml
<PackageReference Include="Avalonia" Version="11.0.6" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.6" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.6" />
```

## Dépannage

### Erreur compilation
```bash
dotnet clean
dotnet restore
dotnet build
```

### Base corrompue
```bash
dotnet ef database drop
dotnet ef database update
```

### Logs
Fichiers dans `bin/Debug/net8.0/yprotect_[date].log`

## Contact
[Yanisse-ISMAILI] - [Yanisse.ismaili@ynov.com] - [B3-DEV]