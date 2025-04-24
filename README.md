# Passeport Nautique Estrie
Passeport Nautique Estrie est un projet "open source" qui comporte un gestionnaire web et une appplication mobile conçus afin d'étudier, et de prévenir La prolifération des espèces exotiques envahissantes.

### Client et responsable du projet
[Conseil régional de l’environnement de l’Estrie (CRE Estrie)](https://www.environnementestrie.ca/)

[Lien vers l'appel d'offre, pour plus de détails](passeport_embarcations.pdf)

## MISE EN CONTEXTE
La prolifération des espèces exotiques envahissantes cause d’importants impacts environnementaux, sociaux et économiques sur le territoire de l’Estrie. De nombreux organismes présents sur le terrain expriment donc différents besoins pour prévenir et lutter contre les espèces exotiques envahissantes.

Afin de prévenir les risques d’introduction de ces espèces, plusieurs solutions peuvent être mises en place. Entre-autre, le lavage des embarcations de plaisance avant tout déplacement entre plans d’eau est la méthode la plus efficace pour réduire les risques d’introductions d’espèces aquatiques envahissantes telle que la moule zébrée.

Le Passeport Nautique Estrie, est une application (mobile et gestionnaire web), permettant notamment de connaître et de suivre:
1. L’historique des déplacements d’une embarcation.
2. L’historique de lavages d’une embarcation.
3. Le type de lavage que l’embarcation a eu dans la passée.
4. Gestion des utilsateurs
5. Gestion des plans d'eaux et leurs gestionnaire, par les administrateurs du système
6. Gestion des employés, par les gestionnaire de plans d'eaux
7. Une plateforme permettant aux chercheurs d'étudier et analyser les déplacements d'embarcations de façon anonyme (aucune donnée sur les citoyens n'est partagé ou utilisé).
8. D'autres fonctionnalités sont à venir.

---
## Développement continu localement (localhost)
- Vous aurez besoin de fichiers d'environnement .env pour la solution et pour [docker-compose.yml](pne-docker/docker-compose.yml)

## Commandes utiles
- `dotnet ef migrations <command> -c ScaffoldMigrationContext`
- `dotnet ef database update -c ScaffoldMigrationContext`


### Structure sommaire du projet
| Dossier | Description |
| ----------- | ----------- |
| pne-docker/ | Matériel pour utilser l'image Docker avec une base de donnée PostgreSQL. |
| src/ | La solution du projet .NET C# (mvc + api) |

### Secret exclus du dépôt avec [.gitignore](.gitignore)
Fichiers pour les variables d'environnents secret et ignoré par .gitignore
Vous devez avoir les fichiers correspondant ci-dessous dans les dossiers indiqués dans l'entête du tableau.
Les fichiers env.txt sont de simple exemple sur les variables d'environnements requises.
| src/ | pne-docker/ | pne-docker/secrets |
| ----------- | ----------- | ----------- |
| [.env](src/env.txt) | [.env](pne-docker/env.txt) | secret_db_name.txt |
| |  | secret_db_user.txt |
| |  | secret_db_password.txt |
| |  | secret_google_id.txt |
| |  | secret_google_secret.txt |
| |  | secret_firebase_projectname.txt |
| |  | secret_firebase_apikey.txt |

### Command de "build Docker" à la racine de la solution (src/)
- `dotnet publish --os linux -c Release`
- `docker build -t <username>/pne-image -f Dockerfile .`

---
### LICENCE
Ce projet est sous la [Licence Libre du Québec – Réciprocité (LiLiQ-R)](LICENSE)
