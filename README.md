# HA-Magiline

Projet expérimental visant à piloter et superviser un équipement de piscine **Magiline** depuis **Home Assistant**.

L’objectif est de comprendre le protocole utilisé par l’équipement et son application mobile, puis de construire une intégration locale, documentée et réutilisable.

> [!WARNING]
> Ce dépôt est à un stade exploratoire. Il ne contient pas encore une intégration Home Assistant prête pour la production.

## Objectifs

- identifier les échanges entre l’application Magiline et l’équipement de piscine ;
- documenter les commandes, états et paramètres disponibles ;
- permettre le pilotage local de la filtration ;
- exposer les informations utiles dans Home Assistant ;
- éviter, autant que possible, une dépendance permanente au cloud constructeur ;
- conserver une séparation nette entre les essais techniques et une future version stable.

## Premières observations

Les valeurs suivantes ont été observées expérimentalement pour le pilotage de la filtration :

| Code | Mode observé |
|---:|---|
| `0` | Mode automatique |
| `1` | Marche continue |
| `2` | Arrêt |

Le mode automatique semble pouvoir prendre plusieurs formes selon la configuration de l’installation. Sur l’équipement étudié, le profil utilisé est un mode automatique personnalisé / expert.

Ces correspondances devront être confirmées et complétées au fur et à mesure des tests.

## Périmètre envisagé

### Commandes

- passage en mode automatique ;
- marche forcée de la filtration ;
- arrêt de la filtration ;
- récupération et modification éventuelle des paramètres de programmation.

### Supervision

- état courant de la filtration ;
- mode actif ;
- horaires ou règles de fonctionnement ;
- disponibilité de l’équipement ;
- autres mesures accessibles selon le matériel installé.

## Architecture cible

L’architecture définitive reste à confirmer, mais le projet pourra comprendre :

```text
Home Assistant
      │
      ├── Intégration personnalisée HA
      │
      ├── Client Python du protocole Magiline
      │
      └── Équipement Magiline sur le réseau local
```

Une première phase pourra utiliser des scripts ou outils de test indépendants avant leur intégration dans Home Assistant.

## Structure prévisionnelle du dépôt

```text
HA-Magiline/
├── README.md
├── docs/                       # Notes de recherche et documentation du protocole
├── experiments/                # Scripts et essais non destinés à la production
├── custom_components/
│   └── magiline/               # Future intégration Home Assistant
└── tests/                      # Tests automatisés
```

## Méthode de travail

1. Capturer ou observer les échanges réseau autorisés entre l’application et l’équipement.
2. Identifier les points d’accès, formats de messages et mécanismes d’authentification.
3. Reproduire les lectures sans modifier l’état de l’installation.
4. Tester ensuite les commandes dans un environnement contrôlé.
5. Documenter chaque découverte dans `docs/`.
6. Encapsuler le protocole dans une bibliothèque testable.
7. Créer l’intégration Home Assistant.

## Sécurité

Le pilotage d’une filtration de piscine peut avoir des conséquences matérielles et sanitaires.

- Ne jamais publier d’identifiants, jetons, clés API ou adresses privées.
- Tester les commandes avec prudence.
- Prévoir un retour à un fonctionnement manuel ou automatique sûr.
- Ne pas contourner les sécurités intégrées au matériel.
- Vérifier le comportement réel de l’équipement après chaque commande.

## Statut

🚧 **Exploration initiale**

Le dépôt sert pour le moment à capitaliser les recherches, les essais et la documentation avant le développement d’une intégration stable.

## Contribution

Les retours d’expérience sur les équipements Magiline, les variantes de contrôleurs et les protocoles observés sont bienvenus.

Lors du partage d’informations, veillez à supprimer toutes les données sensibles ou propres à votre installation.

## Licence

Aucune licence n’est encore définie. Tant qu’un fichier `LICENSE` n’est pas ajouté, le contenu du dépôt reste protégé par le droit d’auteur par défaut.

## Avertissement

Ce projet est indépendant et non affilié à Magiline. Les noms et marques cités appartiennent à leurs propriétaires respectifs.
