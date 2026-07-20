# API locale Magiline

## Endpoints confirmés

- GET /api/v1/pool/info
- GET /api/v1/pool/local
- POST /api/v1/pool/local/spotlight
- POST /api/v1/pool/local/configure-filtration

## Filtration

| wanted | Mode |
|---|---|
|0|Auto|
|1|Marche forcée|
|2|Arrêt|

## Spotlight

ON:
```json
{"mode":{"wanted":2}}
```

OFF:
```json
{"mode":{"wanted":1}}
```

Statut : confirmé expérimentalement.
