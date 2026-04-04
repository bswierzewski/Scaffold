# src/Backend/Modules

This directory groups isolated business modules for the backend.

Each module should have its own directory, its own `classlib` project, and its own elements such as:
- endpoints,
- domain services,
- models,
- validation,
- an optional `DbContext` or persistence configuration,
- contracts required for integration with the host.

Modules should not depend on each other's internal implementation details.