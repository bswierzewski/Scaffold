# src/Backend

This directory contains the backend for the `Scaffold` application.

The backend structure is split into:
- `Host` - the API entry point.
- `Modules` - isolated business modules.
- `Shared` - libraries shared by modules and the host.

Architectural assumption: business modules should remain as independent as possible and communicate only through explicit contracts.