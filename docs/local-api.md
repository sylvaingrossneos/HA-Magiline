# API locale Magiline

## Base URL

```text
http://<IP_DU_BOITIER>:11000
```

Aucune authentification locale n'a été observée sur l'installation testée. Cela reste à confirmer sur d'autres versions de firmware et d'autres configurations.

## Niveaux de confiance

- **Confirmé expérimentalement** : appel exécuté, effet vérifié physiquement et/ou dans l'état retourné.
- **Déduit de l'APK** : endpoint, service ou structure retrouvé dans le bundle, non encore validé sur le matériel.
- **Hypothèse** : piste à tester.

## Endpoints connus

| Fonction | Méthode | Endpoint | Statut |
|---|---|---|---|
| Informations générales | GET | `/api/v1/pool/info` | Confirmé |
| État local global | GET | `/api/v1/pool/local` | Confirmé |
| Projecteur | POST | `/api/v1/pool/local/spotlight` | Confirmé expérimentalement |
| Filtration | POST | `/api/v1/pool/local/configure-filtration` | Confirmé expérimentalement |

## GET `/api/v1/pool/info`

Retourne des informations générales sur la piscine ou le contrôleur. La réponse exacte doit encore être anonymisée et ajoutée au dépôt.

Exemple PowerShell :

```powershell
Invoke-RestMethod -Method Get -Uri "http://$PoolControllerIp`:11000/api/v1/pool/info"
```

## GET `/api/v1/pool/local`

Retourne l'état local global. Les explorations indiquent notamment la présence d'informations relatives à :

- température ;
- filtration ;
- chauffage ;
- projecteur ;
- accessoires ;
- volet ;
- autres fonctions selon la configuration.

La structure complète ne doit pas être figée dans le composant tant que plusieurs exemples anonymisés n'ont pas été comparés.

```powershell
Invoke-RestMethod -Method Get -Uri "http://$PoolControllerIp`:11000/api/v1/pool/local"
```

## POST `/api/v1/pool/local/spotlight`

### Allumage

```json
{
  "mode": {
    "wanted": 2
  }
}
```

### Extinction

```json
{
  "mode": {
    "wanted": 1
  }
}
```

> Le mapping `1 = éteint`, `2 = allumé` est propre au champ `spotlight.mode.wanted`. Il ne doit pas être généralisé automatiquement aux autres équipements.

## POST `/api/v1/pool/local/configure-filtration`

Payload minimal observé :

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

Sur l'installation testée, le mode automatique configuré est un mode personnalisé / expert. D'autres variantes de configuration peuvent exister derrière la même valeur `wanted = 0`.

## Validation recommandée d'une commande

1. Lire `/pool/local` avant l'appel.
2. Envoyer une seule commande réversible.
3. Vérifier physiquement l'équipement.
4. Relire `/pool/local`.
5. Conserver une réponse anonymisée dans les preuves du projet.
6. Revenir au mode automatique ou à l'état initial après le test.

## Endpoints à découvrir

Les fonctions suivantes sont visibles ou suggérées par l'APK, mais leurs endpoints et payloads ne sont pas encore confirmés :

- chauffage ;
- volet ;
- balnéo ;
- accessoires ;
- hivernage ;
- programmes horaires ;
- configuration globale.
