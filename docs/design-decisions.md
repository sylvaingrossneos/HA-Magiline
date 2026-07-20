# Décisions de conception

## ADR-001 — Utiliser l'API locale comme interface principale

**Décision** : toutes les fonctions courantes doivent utiliser l'API HTTP locale du boîtier.

**Motifs** :

- meilleure résilience en cas de coupure Internet ;
- latence réduite ;
- maîtrise des données ;
- cohérence avec la philosophie Home Assistant ;
- endpoints locaux déjà confirmés.

**Conséquence** : les fonctions exclusivement cloud ne seront pas prioritaires.

## ADR-002 — Utiliser REST/HTTP plutôt que MQTT

**Décision** : le premier client communiquera directement avec les endpoints HTTP exposés par le boîtier.

**Motifs** :

- interface réellement observée ;
- aucun broker MQTT ni protocole MQTT confirmé ;
- possibilité de reproduire les appels avec des outils standards ;
- faible complexité de déploiement.

MQTT ne sera envisagé que si une interface native est découverte ultérieurement.

## ADR-003 — Utiliser un DataUpdateCoordinator

**Décision** : l'état sera centralisé dans un `DataUpdateCoordinator` Home Assistant.

**Motifs** :

- `/pool/local` renvoie un état global réutilisable par plusieurs plateformes ;
- réduction des appels redondants ;
- gestion standardisée des indisponibilités et rafraîchissements ;
- rafraîchissement anticipé après commande.

## ADR-004 — Commencer par du polling

**Décision** : l'intégration interrogera périodiquement `/pool/local`, initialement toutes les 15 à 30 secondes.

**Motifs** :

- aucun mécanisme push local confirmé ;
- l'application officielle effectue elle-même des lectures régulières ;
- comportement simple et prévisible.

L'intervalle devra rester configurable ou ajustable si le boîtier supporte mal les lectures fréquentes.

## ADR-005 — Séparer expérimentation et production

**Décision** : les scripts de reconstruction des commandes restent dans `experiments/`, tandis que le code Home Assistant ne contient que des comportements suffisamment validés.

**Motifs** :

- éviter qu'une hypothèse devienne une fonctionnalité exposée ;
- conserver les preuves et outils de reproduction ;
- permettre des essais rapides sans dégrader le composant.

## ADR-006 — Qualifier chaque découverte

Chaque information doit porter un niveau explicite :

- confirmé expérimentalement ;
- déduit de l'APK ;
- hypothèse ;
- à vérifier.

Une chaîne ou un service présent dans l'APK n'est pas considéré comme une preuve suffisante d'un endpoint fonctionnel.

## ADR-007 — Ne pas envoyer de configuration globale sans nécessité

Les commandes unitaires et réversibles sont prioritaires. Les fonctions telles que `sendPoolConfiguration` ne seront testées qu'après compréhension complète du payload, car elles pourraient modifier plusieurs paramètres à la fois.
