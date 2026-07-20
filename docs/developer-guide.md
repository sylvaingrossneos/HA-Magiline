# Guide développeur

## Pré-requis

Pour les explorations locales :

- un poste connecté au même réseau que le boîtier iMAGI-X ;
- PowerShell 7 ou Windows PowerShell ;
- l'adresse IP locale du boîtier ;
- une possibilité de vérifier physiquement l'effet d'une commande ;
- idéalement une copie légitime de l'APK installée sur votre propre appareil pour l'analyse.

Pour le futur composant Home Assistant :

- Python compatible avec la version ciblée de Home Assistant ;
- une instance Home Assistant de développement ;
- les outils de validation Home Assistant/HACS lorsque le composant sera suffisamment avancé.

## Tester l'API locale

Définir l'adresse du boîtier :

```powershell
$PoolControllerIp = "192.168.1.100"
```

Lire l'état :

```powershell
./experiments/powershell/Get-MagilineState.ps1 -HostName $PoolControllerIp
```

Commander le projecteur :

```powershell
./experiments/powershell/Set-MagilineSpotlight.ps1 -HostName $PoolControllerIp -State On
./experiments/powershell/Set-MagilineSpotlight.ps1 -HostName $PoolControllerIp -State Off
```

Commander la filtration :

```powershell
./experiments/powershell/Set-MagilineFiltration.ps1 -HostName $PoolControllerIp -Mode Auto
./experiments/powershell/Set-MagilineFiltration.ps1 -HostName $PoolControllerIp -Mode ForceOn
./experiments/powershell/Set-MagilineFiltration.ps1 -HostName $PoolControllerIp -Mode Off
```

## Ajouter une découverte

Une nouvelle fonction ne doit pas être intégrée directement dans Home Assistant.

1. Identifier la piste dans l'APK.
2. Documenter le service ou les chaînes retrouvées.
3. Créer un script expérimental minimal.
4. Tester une commande réversible.
5. Capturer l'état avant et après, puis anonymiser les données.
6. Mettre à jour `docs/local-api.md` avec le niveau de confiance.
7. Ajouter seulement ensuite la fonction dans le client Python et les entités Home Assistant.

## Convention documentaire

Chaque fait doit être qualifié :

- **Confirmé expérimentalement** ;
- **Déduit de l'APK** ;
- **Hypothèse** ;
- **À vérifier**.

Une découverte doit idéalement comporter :

- version de l'application ;
- version de firmware si connue ;
- endpoint ;
- méthode HTTP ;
- payload ;
- réponse ;
- état avant/après ;
- résultat physique ;
- limites ou incertitudes.

## Données sensibles

Avant tout commit, retirer :

- adresses IP privées propres à l'installation ;
- identifiants du bassin ou du compte ;
- jetons et cookies ;
- numéros de série ;
- coordonnées personnelles ;
- captures HTTPS non anonymisées.
