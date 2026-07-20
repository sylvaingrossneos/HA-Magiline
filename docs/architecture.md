# Architecture du projet

## Vision

HA-Magiline doit fournir une intégration Home Assistant locale pour les piscines Magiline équipées d'un boîtier iMAGI-X.

Les objectifs structurants sont :

- fonctionnement local pour les usages courants ;
- aucune dépendance obligatoire au cloud Magiline ;
- architecture Home Assistant moderne ;
- séparation entre reverse engineering, client API et plateformes Home Assistant ;
- traçabilité du niveau de confiance de chaque découverte.

## Périmètre

### Inclus

- lecture de l'état du bassin et des équipements exposés par l'API locale ;
- pilotage du projecteur ;
- pilotage des modes de filtration ;
- extension progressive au chauffage, volet, balnéo, accessoires et hivernage ;
- diagnostics anonymisés ;
- installation future via HACS.

### Exclus

- modification du firmware ;
- contournement d'authentification ;
- interception de données cloud privées ;
- dépendance fonctionnelle au cloud ;
- publication de secrets ou de données propres à une installation.

## Architecture logique

```text
Home Assistant
  │
  ├── Config Flow
  │     └── adresse IP / nom d'hôte du boîtier
  │
  ├── Magiline API Client
  │     ├── GET /api/v1/pool/info
  │     ├── GET /api/v1/pool/local
  │     └── commandes POST locales
  │
  ├── DataUpdateCoordinator
  │     └── polling toutes les 15 à 30 secondes
  │
  └── Entités
        ├── sensors
        ├── switches
        ├── light
        └── futures entités cover / climate / select
```

## Composants envisagés

| Fichier | Responsabilité |
|---|---|
| `api.py` | Client HTTP local, sérialisation, erreurs et commandes |
| `coordinator.py` | Rafraîchissement périodique et état partagé |
| `config_flow.py` | Découverte/configuration du boîtier et test de connexion |
| `const.py` | Domaine, endpoints, valeurs et paramètres communs |
| `sensor.py` | Températures, états, informations techniques |
| `switch.py` | Commandes binaires et modes simples |
| `light.py` | Projecteur de piscine |
| `diagnostics.py` | Export de diagnostic nettoyé des données sensibles |

## Modèle de données

Le JSON complet retourné par `/api/v1/pool/local` reste à échantillonner et anonymiser avant de figer les modèles d'entités. Le composant doit donc commencer par conserver la réponse brute dans le coordinator, puis introduire des accesseurs robustes lorsque les structures sont confirmées.

## Résilience

Le client devra distinguer :

- boîtier inaccessible ;
- délai dépassé ;
- réponse HTTP en erreur ;
- JSON invalide ou inattendu ;
- commande acceptée sans changement d'état ;
- version de firmware ou structure non reconnue.

Après une commande, un rafraîchissement anticipé du coordinator permettra de confirmer l'état effectivement appliqué.

## Sécurité opérationnelle

Les modes de filtration peuvent affecter le traitement de l'eau. L'intégration ne doit pas masquer le comportement automatique du boîtier ni multiplier les écritures. Les commandes doivent être explicites, journalisées sans données sensibles et suivies d'une relecture d'état.
