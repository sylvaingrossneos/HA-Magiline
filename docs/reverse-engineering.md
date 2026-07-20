# Reverse engineering

## Chronologie

### 1. Recherche d'une interface locale

**Objectif** : déterminer si le boîtier expose une interface exploitable sans cloud.

**Méthode** : scan du réseau local et observation des ports ouverts.

**Résultat** : service HTTP découvert sur le port `11000`.

**Conclusion** : une API locale existe.

### 2. Découverte des endpoints de lecture

Les endpoints suivants ont été validés :

```text
GET /api/v1/pool/info
GET /api/v1/pool/local
```

`/pool/local` renvoie l'état global de la piscine, notamment des informations relatives à la température, la filtration, le chauffage, le projecteur, les accessoires et le volet. La structure JSON complète doit encore être anonymisée et documentée champ par champ.

### 3. Capture réseau avec Fiddler

**Objectif** : observer une commande envoyée par l'application Android lors d'un clic utilisateur.

**Résultat** : le polling régulier de `GET /pool/local` a été visible, mais les commandes locales n'ont pas été identifiées par cette seule méthode.

**Limite rencontrée** : l'application communique aussi avec `admin.imagi-x.fr` et `grpc.imagi-x.fr`. Le trafic HTTPS n'a pas été déchiffré, probablement en raison d'un mécanisme de certificate pinning ou d'une configuration équivalente.

**Conclusion** : la capture réseau seule était insuffisante pour reconstruire les écritures.

### 4. Analyse de l'APK

L'application a été identifiée comme une application React Native / Expo utilisant Hermes. Le code Java/Kotlin natif est réduit ; la logique métier principale se trouve dans :

```text
assets/index.android.bundle
```

Le bundle est en bytecode Hermes version 96.

### 5. Identification des services

Les symboles et services suivants ont été identifiés :

- `SpotlightService` ;
- `FiltrationService` ;
- `WinteringService` ;
- `sendOrders` ;
- `handleLocalConnection` ;
- `sendPoolConfiguration`.

Ces éléments ont confirmé que l'application disposait d'une voie de commande locale.

### 6. Validation du projecteur

L'analyse de `SpotlightService` a conduit à la reconstruction de :

```text
POST /api/v1/pool/local/spotlight
```

Le payload a été testé avec PowerShell. Le projecteur a réagi conformément à la commande.

### 7. Validation de la filtration

La même méthode appliquée à `FiltrationService` a permis de reconstruire :

```text
POST /api/v1/pool/local/configure-filtration
```

Les valeurs `wanted` suivantes ont été validées :

- `0` : mode automatique ;
- `1` : marche forcée permanente ;
- `2` : arrêt.

## Méthode reproductible

Pour chaque nouvelle fonction :

1. rechercher le service correspondant dans le bundle Hermes ;
2. identifier les fonctions de connexion locale et de construction de commande ;
3. reconstruire l'endpoint et le payload ;
4. tester d'abord avec un script isolé ;
5. observer physiquement l'effet ;
6. relire `/pool/local` pour confirmer l'état ;
7. documenter le résultat et son niveau de confiance ;
8. seulement ensuite intégrer la fonction dans Home Assistant.

## Pistes restant à explorer

### Priorité haute

- chauffage ;
- volet roulant ;
- balnéo ;
- accessoires.

### Priorité moyenne

- programmes horaires ;
- hivernage ;
- variantes du mode automatique / expert ;
- envoi de configuration complète.

### Priorité faible

- compréhension du cloud ;
- gRPC ;
- services distants non nécessaires au fonctionnement local.

## Pièges rencontrés

- supposer que les commandes seraient visibles comme de simples appels HTTP via Fiddler ;
- consacrer trop de temps au trafic HTTPS alors que la logique locale était embarquée dans l'APK ;
- chercher principalement dans le code natif Android alors que l'essentiel se trouvait dans le bundle Hermes ;
- confondre une chaîne trouvée dans l'APK avec une commande validée sur le matériel ;
- documenter un mode sans préciser s'il s'agit d'un fait, d'une déduction ou d'une hypothèse.
