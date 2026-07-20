# HA-Magiline

Intégration locale et expérimentale de **piscines Magiline équipées d'un boîtier iMAGI-X** dans **Home Assistant**.

Le projet vise à piloter et superviser la piscine sans dépendance au cloud pour les opérations courantes, en s'appuyant sur l'API HTTP locale découverte sur le boîtier.

> [!WARNING]
> Le projet est en reverse engineering actif. Les commandes publiées comme « confirmées » ont été testées sur une installation réelle, mais elles peuvent varier selon les versions de boîtier, de firmware ou de configuration. Toute commande doit être testée avec prudence.

## État actuel

### Confirmé expérimentalement

- API HTTP locale accessible sur le port `11000` ;
- lecture de l'état global de la piscine ;
- commande locale du projecteur ;
- commande locale du mode de filtration ;
- aucune authentification locale observée sur l'installation testée.

### Déduit de l'APK

L'application Android est basée sur **React Native / Expo** et utilise **Hermes**. La logique métier se trouve principalement dans `assets/index.android.bundle`.

Les éléments suivants ont été identifiés dans le bundle :

- `SpotlightService` ;
- `FiltrationService` ;
- `WinteringService` ;
- `sendOrders` ;
- `handleLocalConnection` ;
- `sendPoolConfiguration`.

L'analyse de ces services a permis de reconstruire les premières commandes locales fonctionnelles.

## API locale connue

Base URL :

```text
http://<IP_DU_BOITIER>:11000/api/v1/pool/local
```

| Fonction | Méthode et endpoint | Statut |
|---|---|---|
| Informations générales | `GET /api/v1/pool/info` | Confirmé |
| État local complet | `GET /api/v1/pool/local` | Confirmé |
| Projecteur | `POST /api/v1/pool/local/spotlight` | Confirmé expérimentalement |
| Filtration | `POST /api/v1/pool/local/configure-filtration` | Confirmé expérimentalement |

### Projecteur

Allumage :

```json
{
  "mode": {
    "wanted": 2
  }
}
```

Extinction :

```json
{
  "mode": {
    "wanted": 1
  }
}
```

### Filtration

```json
{
  "mode": {
    "wanted": 0
  }
}
```

| `wanted` | Comportement observé |
|---:|---|
| `0` | Mode automatique |
| `1` | Marche forcée permanente |
| `2` | Arrêt |

Sur l'installation étudiée, `wanted = 0` correspond au mode automatique personnalisé / expert configuré dans le boîtier.

## Méthodologie de reverse engineering

1. Identifier le service fonctionnel dans le bundle Hermes de l'APK.
2. Reconstruire l'endpoint HTTP local et son payload JSON.
3. Tester la commande avec un script PowerShell indépendant.
4. Vérifier physiquement le comportement du boîtier.
5. Documenter la preuve, le niveau de confiance et les éventuelles limites.
6. Intégrer ensuite la fonction dans Home Assistant.

Cette méthode a déjà permis de valider le projecteur et la filtration.

## Architecture cible

```text
Home Assistant
  └── custom_components/magiline
        ├── config_flow.py
        ├── api.py
        ├── coordinator.py
        ├── sensor.py
        ├── switch.py
        ├── light.py
        └── diagnostics.py
              │
              └── API HTTP locale iMAGI-X :11000
```

Le composant utilisera un `DataUpdateCoordinator` et interrogera périodiquement `GET /api/v1/pool/local`, avec un intervalle initial envisagé de 15 à 30 secondes.

## Structure du dépôt

```text
HA-Magiline/
├── README.md
├── docs/
│   ├── architecture.md
│   ├── local-api.md
│   ├── reverse-engineering.md
│   ├── apk-analysis.md
│   ├── design-decisions.md
│   ├── developer-guide.md
│   └── roadmap.md
├── experiments/
│   └── powershell/
├── custom_components/
│   └── magiline/
└── tests/
```

## Niveaux de confiance

- **Confirmé expérimentalement** : commande exécutée et effet vérifié sur le matériel.
- **Déduit de l'APK** : information retrouvée dans le bundle, mais pas encore validée sur le matériel.
- **Hypothèse** : piste technique restant à démontrer.
- **À vérifier** : comportement observé partiellement ou dépendant de la configuration.

## Roadmap

- **v0.1** : lecture d'état, projecteur, filtration ;
- **v0.2** : chauffage, volet et accessoires ;
- **v0.3** : configuration, programmes et hivernage ;
- **v1.0** : intégration stabilisée, documentation complète et publication HACS.

## Périmètre éthique et sécurité

Le projet n'a pas pour objectif de casser la sécurité du produit, de contourner une authentification, de modifier le firmware ou d'intercepter des communications cloud privées. Il documente et utilise uniquement les interfaces locales accessibles par le propriétaire de l'équipement sur son propre réseau.

Le pilotage d'une piscine peut avoir des conséquences matérielles et sanitaires. Conservez toujours un moyen de retour au fonctionnement manuel ou automatique et ne publiez jamais d'identifiants, jetons, adresses privées ou captures non anonymisées.

## Licence et affiliation

Aucune licence n'est encore définie. Tant qu'un fichier `LICENSE` n'est pas ajouté, le contenu reste protégé par le droit d'auteur par défaut.

Ce projet est indépendant et non affilié à Magiline. Les noms et marques cités appartiennent à leurs propriétaires respectifs.
