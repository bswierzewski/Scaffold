# infra/traefik

This directory is intended for Traefik configuration used by the local `Scaffold` environment.

The following files are expected to live here eventually:
- `traefik.yml` - the main Traefik configuration.
- `dynamic.yml` - router configuration, for example `app.localhost` and `api.localhost`.
- `certs.pem` and `key.pem` - local development certificates used for HTTPS.

Notes:
- certificate files should be ignored by `.gitignore`,
- the configuration should support routing for both the frontend and the backend.