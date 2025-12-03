# Docker Image Publishing for Angular App

## Overview

The FindTheBug.App (Angular application) is automatically built and published as a Docker image to GitHub Container Registry (ghcr.io) using GitHub Actions.

## Workflow Triggers

The Docker build and publish workflow for the Angular app is triggered on:

- **Push to branches**: `main`, `master`, or `develop` (only when files in `src/FindTheBug.App/` change)
- **Pull requests**: targeting `main`, `master`, or `develop` (builds but doesn't push, only when app files change)
- **Version tags**: Any tag matching `app-v*.*.*` (e.g., `app-v1.0.0`, `app-v2.1.3`)
- **Manual trigger**: Via the GitHub Actions UI (workflow_dispatch)

## Path-Based Triggering

The workflow is optimized to only run when relevant files change:
- Changes in `src/FindTheBug.App/**`
- Changes to the workflow file itself

This prevents unnecessary builds when only the API or other parts of the project are modified.

## Image Tags

The workflow automatically generates multiple tags for each build:

| Tag Type | Example | When Applied |
|----------|---------|--------------|
| Branch name | `ghcr.io/rahim373/findthebug-app:main` | On push to any branch |
| PR number | `ghcr.io/rahim373/findthebug-app:pr-123` | On pull requests |
| Semantic version | `ghcr.io/rahim373/findthebug-app:app-1.0.0` | On version tags |
| Major.Minor | `ghcr.io/rahim373/findthebug-app:app-1.0` | On version tags |
| Major | `ghcr.io/rahim373/findthebug-app:app-1` | On version tags |
| SHA | `ghcr.io/rahim373/findthebug-app:main-abc1234` | On all commits |
| Latest | `ghcr.io/rahim373/findthebug-app:latest` | On default branch only |

## Docker Image Details

### Base Images
- **Build Stage**: `node:20-alpine` - Lightweight Node.js 20 for building the Angular app
- **Runtime Stage**: `nginx:alpine` - Lightweight Nginx for serving the static files

### Build Process
1. Install npm dependencies with `npm ci`
2. Build Angular app for production with `npm run build`
3. Copy built files to Nginx
4. Configure Nginx for Angular SPA routing

### Nginx Configuration
The image includes a custom Nginx configuration with:
- **SPA Routing**: All routes serve `index.html` for Angular routing
- **Gzip Compression**: Enabled for text and JavaScript files
- **Caching**: Static assets cached for 1 year, `index.html` never cached
- **Security Headers**: X-Frame-Options, X-Content-Type-Options, X-XSS-Protection
- **Health Check**: `/health` endpoint for container health monitoring

## Features

### Multi-Platform Support
Images are built for both `linux/amd64` and `linux/arm64` architectures.

### Build Caching
The workflow uses GitHub Actions cache to speed up builds by caching Docker layers between runs.

### Security
- **Artifact Attestation**: Each published image includes build provenance attestation
- **Minimal Permissions**: The workflow uses the minimum required permissions
- **No Secrets Required**: Uses the built-in `GITHUB_TOKEN` for authentication

## Pulling Images

### Public Access
If your repository is public, anyone can pull the image:

```bash
docker pull ghcr.io/rahim373/findthebug-app:latest
```

### Private Repository Access
For private repositories, authenticate first:

```bash
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin
docker pull ghcr.io/rahim373/findthebug-app:latest
```

## Running the Image

### Basic Usage
```bash
docker run -p 80:80 ghcr.io/rahim373/findthebug-app:latest
```

The Angular app will be available at `http://localhost`

### With Custom Port
```bash
docker run -p 8080:80 ghcr.io/rahim373/findthebug-app:latest
```

The Angular app will be available at `http://localhost:8080`

### With Environment-Specific Configuration
If you need to configure API endpoints or other runtime settings, you can mount a custom configuration:

```bash
docker run -p 80:80 \
  -v $(pwd)/config.json:/usr/share/nginx/html/assets/config.json \
  ghcr.io/rahim373/findthebug-app:latest
```

### Health Check
The container includes a health check endpoint:

```bash
curl http://localhost/health
# Returns: healthy
```

## Docker Compose Integration

You can add the Angular app to your `docker-compose.yml`:

```yaml
services:
  app:
    image: ghcr.io/rahim373/findthebug-app:latest
    ports:
      - "80:80"
    depends_on:
      - api
    networks:
      - findthebug-network

  api:
    image: ghcr.io/rahim373/findthebug:latest
    # ... API configuration
```

## Creating Releases

To publish a versioned release for the Angular app:

1. Create and push a version tag with `app-` prefix:
   ```bash
   git tag app-v1.0.0
   git push origin app-v1.0.0
   ```

2. The workflow will automatically:
   - Build the Docker image
   - Tag it with multiple versions (`app-1.0.0`, `app-1.0`, `app-1`)
   - Push to GitHub Container Registry
   - Generate build attestation

## Local Development

### Building Locally
```bash
cd src/FindTheBug.App
docker build -t findthebug-app:local .
```

### Running Locally Built Image
```bash
docker run -p 80:80 findthebug-app:local
```

## Viewing Published Images

Published images can be viewed at:
- Repository packages page: `https://github.com/Rahim373/FindTheBug/pkgs/container/findthebug-app`
- Or navigate to: Repository → Packages

## Troubleshooting

### Build Failures
Check the GitHub Actions logs at: Repository → Actions → Docker Build and Publish Angular App

### Permission Issues
Ensure the repository has the following settings:
- Settings → Actions → General → Workflow permissions: "Read and write permissions"
- Settings → Packages → Package creation: Enabled

### Image Not Found
- Verify the image name matches your repository (case-sensitive)
- For private repos, ensure you're authenticated with proper permissions

### Angular Routing Issues
If routes don't work properly:
- Ensure the nginx configuration is properly copied in the Dockerfile
- Check that the `try_files` directive is present in nginx.conf

## Workflow File Location

The workflow is defined in: `.github/workflows/docker-publish-app.yml`

## Related Files

- **Dockerfile**: `src/FindTheBug.App/Dockerfile`
- **Nginx Config**: `src/FindTheBug.App/nginx.conf`
- **Docker Ignore**: `src/FindTheBug.App/.dockerignore`
