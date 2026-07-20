# Analyse de l'APK iMAGI-X

## Statut des informations

Les informations de ce document sont **déduites de l'APK**. Elles ne deviennent « confirmées expérimentalement » que lorsqu'un test sur le boîtier reproduit le comportement attendu.

## Structure générale

L'application Android utilise :

- React Native ;
- Expo ;
- Hermes ;
- un bundle applicatif principal dans `assets/index.android.bundle`.

Le bundle observé est en bytecode Hermes version 96. La faible quantité de logique métier visible dans les classes Java/Kotlin explique pourquoi l'analyse native seule fournit peu d'informations utiles.

## Outils et approches

- capture réseau avec Fiddler ;
- inspection de l'APK ;
- navigation dans les classes avec JADX ;
- extraction et recherche de chaînes dans le bundle ;
- analyse du bytecode Hermes ;
- rapprochement entre noms de services, endpoints et payloads ;
- validation finale avec PowerShell.

## Services identifiés

### SpotlightService

Service associé au projecteur. Son analyse a conduit à la commande locale `POST /api/v1/pool/local/spotlight`, ensuite validée expérimentalement.

### FiltrationService

Service associé aux modes de filtration. Son analyse a conduit à `POST /api/v1/pool/local/configure-filtration`, ensuite validé expérimentalement.

### WinteringService

Service vraisemblablement associé à l'hivernage. L'endpoint et les payloads n'ont pas encore été validés.

### sendOrders

Fonction ou mécanisme générique d'envoi d'ordres. À analyser pour comprendre la construction commune des commandes et identifier de nouvelles fonctions.

### handleLocalConnection

Élément central pour comprendre la décision entre connexion locale et distante, ainsi que la construction de l'URL locale.

### sendPoolConfiguration

Mécanisme probablement utilisé pour envoyer une configuration plus large de la piscine. À traiter avec prudence : une configuration globale est plus risquée qu'une commande unitaire.

## Stratégie d'analyse recommandée

1. Rechercher le nom métier de la fonction dans le bundle.
2. Repérer les constantes d'endpoint proches.
3. Identifier la forme de l'objet transmis à `sendOrders` ou au mécanisme équivalent.
4. Comparer la structure avec l'état retourné par `/pool/local`.
5. Construire un test PowerShell minimal.
6. Tester une seule transition réversible.
7. Relire l'état local et vérifier physiquement l'équipement.

## Limites

- le bundle peut changer entre versions de l'application ;
- les noms minifiés ou transformés peuvent compliquer la recherche ;
- un endpoint présent dans l'APK peut ne pas être pris en charge par tous les firmwares ;
- la présence d'une fonction ne constitue pas une preuve de son comportement ;
- aucune archive APK ou donnée propriétaire ne doit être publiée dans le dépôt sans vérification juridique.
