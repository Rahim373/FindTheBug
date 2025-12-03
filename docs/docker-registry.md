# Docker Image Publishing to GitHub Container Registry

## Overview

The FindTheBug.WebAPI project is automatically built and published as a Docker image to GitHub Container Registry (ghcr.io) using GitHub Actions.

## Workflow Triggers

The Docker build and publish workflow is triggered on:

- **Push to branches**: `main`, `master`, or `develop`
- **Pull requests**: targeting `main`, `master`, or `develop` (builds but doesn't push)
- **Version tags**: Any tag matching `v*.*.*` (e.g., `v1.0.0`, `v2.1.3`)
- **Manual trigger**: Via the GitHub Actions UI (workflow_dispatch)

## Image Tags

The workflow automatically generates multiple tags for each build:

| Tag Type | Example | When Applied |
|----------|---------|--------------|
| Branch name | `ghcr.io/rahim373/findthebug:main` | On push to any branch |
| PR number | `ghcr.io/rahim373/findthebug:pr-123` | On pull requests |
| Semantic version | `ghcr.io/rahim373/findthebug:1.0.0` | On version tags |
| Major.Minor | `ghcr.io/rahim373/findthebug:1.0` | On version tags |
| Major | `ghcr.io/rahim373/findthebug:1` | On version tags |
| SHA | `ghcr.io/rahim373/findthebug:main-abc1234` | On all commits |
| Latest | `ghcr.io/rahim373/findthebug:latest` | On default branch only |

## Features

### Multi-Platform Support
Images are built for both `linux/amd64` and `linux/arm64` architectures, ensuring compatibility with:
- x86_64 servers and cloud instances
- ARM-based systems (e.g., AWS Graviton, Apple Silicon)

### Build Caching
The workflow uses GitHub Actions cache to speed up builds by caching Docker layers between runs.

### Security
- **Artifact Attestation**: Each published image includes build provenance attestation for supply chain security
- **Minimal Permissions**: The workflow uses the minimum required permissions
- **No Secrets Required**: Uses the built-in `GITHUB_TOKEN` for authentication

## Pulling Images

### Public Access
If your repository is public, anyone can pull the image:

```bash
docker pull ghcr.io/rahim373/findthebug:latest
```

### Private Repository Access
For private repositories, authenticate first:

```bash
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin
docker pull ghcr.io/rahim373/findthebug:latest
```

## Running the Image

### Basic Usage
```bash
docker run -p 8080:8080 ghcr.io/rahim373/findthebug:latest
```

### With Environment Variables
```bash
docker run -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="your-connection-string" \
  ghcr.io/rahim373/findthebug:latest
```

### Using Docker Compose
The existing `docker-compose.yml` can be updated to use the published image instead of building locally:

```yaml
services:
  api:
    image: ghcr.io/rahim373/findthebug:latest
    # Remove the 'build' section
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      # ... other environment variables
```

## Creating Releases

To publish a versioned release:

1. Create and push a version tag:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

2. The workflow will automatically:
   - Build the Docker image
   - Tag it with multiple versions (`1.0.0`, `1.0`, `1`)
   - Push to GitHub Container Registry
   - Generate build attestation

## Viewing Published Images

Published images can be viewed at:
- Repository packages page: `https://github.com/Rahim373/FindTheBug/pkgs/container/findthebug`
- Or navigate to: Repository → Packages

## Troubleshooting

### Build Failures
Check the GitHub Actions logs at: Repository → Actions → Docker Build and Publish

### Permission Issues
Ensure the repository has the following settings:
- Settings → Actions → General → Workflow permissions: "Read and write permissions"
- Settings → Packages → Package creation: Enabled

### Image Not Found
- Verify the image name matches your repository (case-sensitive)
- For private repos, ensure you're authenticated with proper permissions

## Workflow File Location

The workflow is defined in: `.github/workflows/docker-publish.yml`
