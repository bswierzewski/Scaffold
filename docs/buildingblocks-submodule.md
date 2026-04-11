# BuildingBlocks Submodule

`BuildingBlocks` is tracked as a Git submodule at `src/backend/BuildingBlocks`.
The submodule is configured to follow the `main` branch of `https://github.com/bswierzewski/building_blocks.git`.

## Update To The Latest `main`

1. Make sure you are at the root of the `Scaffold` repository.
2. Update the submodule to the latest commit from `origin/main`:

```bash
git submodule update --remote --merge src/backend/BuildingBlocks
```

3. Verify which commit was pulled:

```bash
git submodule status
git -C src/backend/BuildingBlocks log -1 --oneline --decorate
```

4. Validate the solution slice that depends on `BuildingBlocks`:

```bash
dotnet build src/backend/Bootstrappers/Scaffold.Api/Scaffold.Api.csproj
dotnet build src/aspire/Scaffold.AppHost/Scaffold.AppHost.csproj
```

5. Commit the updated submodule pointer in the parent repository:

```bash
git add src/backend/BuildingBlocks
git commit -m "Update BuildingBlocks submodule"
git push
```

## Work On `BuildingBlocks` And Push To `main`

1. Enter the submodule:

```bash
cd src/backend/BuildingBlocks
```

2. Confirm you are on `main` and up to date:

```bash
git checkout main
git pull origin main
```

3. Make your changes, then commit and push them inside the submodule:

```bash
git add .
git commit -m "Describe the BuildingBlocks change"
git push origin main
```

4. Go back to the parent repository and record the new submodule commit:

```bash
cd ../../..
git add src/backend/BuildingBlocks
git commit -m "Update BuildingBlocks submodule pointer"
git push
```

## Clone With Submodule

If the repository is cloned fresh, initialize the submodule with:

```bash
git submodule update --init --recursive
```

## Current Project Configuration

- Submodule path: `src/backend/BuildingBlocks`
- Tracked branch: `main`
- Parent repository stores only the submodule commit pointer, not the full branch state