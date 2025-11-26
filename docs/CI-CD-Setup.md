# CI/CD Setup Guide

## GitHub Actions

This project uses GitHub Actions for continuous integration and automated testing.

## Architecture Tests Workflow

### Overview

The architecture tests workflow runs automatically on every push to ensure the codebase maintains Clean Architecture principles.

**File:** `.github/workflows/architecture-tests.yml`

### Triggers

The workflow runs on:
- **Push** to `main`, `master`, or `develop` branches
- **Pull requests** targeting `main`, `master`, or `develop` branches

### Workflow Steps

1. **Checkout code** - Retrieves the latest code from the repository
2. **Setup .NET** - Installs .NET 8.0 SDK
3. **Restore dependencies** - Restores NuGet packages
4. **Build solution** - Compiles the entire solution in Release mode
5. **Run Architecture Tests** - Executes all architecture tests
6. **Publish Test Results** - Publishes test results for visibility

### Badge

The README includes a status badge showing the current state of architecture tests:

```markdown
[![Architecture Tests](https://github.com/YOUR_USERNAME/FindTheBug/actions/workflows/architecture-tests.yml/badge.svg)](https://github.com/YOUR_USERNAME/FindTheBug/actions/workflows/architecture-tests.yml)
```

**Note:** Replace `YOUR_USERNAME` with your actual GitHub username.

### Test Results

Test results are published automatically and can be viewed in:
- GitHub Actions workflow run summary
- Pull request checks
- Commit status checks

### Current Test Status

The architecture tests verify:
- ✅ Layer dependencies (Application → Domain, Infrastructure → Application)
- ✅ Naming conventions (Commands, Queries, Handlers)
- ✅ Handler inheritance (ICommandHandler, IQueryHandler)
- ✅ Repository patterns
- ✅ Entity immutability

**Note:** Some tests may fail due to NetArchTest.Rules reflection issues with generic types in .NET 10. See [Architecture Tests Status](Architecture-Tests-Status.md) for details.

## Setup Instructions

### 1. Enable GitHub Actions

GitHub Actions is enabled by default for public repositories. For private repositories:

1. Go to repository **Settings**
2. Navigate to **Actions** → **General**
3. Enable **Allow all actions and reusable workflows**

### 2. Update Badge URL

In `README.md`, replace `YOUR_USERNAME` with your GitHub username:

```markdown
[![Architecture Tests](https://github.com/YOUR_USERNAME/FindTheBug/actions/workflows/architecture-tests.yml/badge.svg)](https://github.com/YOUR_USERNAME/FindTheBug/actions/workflows/architecture-tests.yml)
```

### 3. First Run

The workflow will run automatically on the next push to `main`, `master`, or `develop`.

To trigger manually:
1. Go to **Actions** tab in GitHub
2. Select **Architecture Tests** workflow
3. Click **Run workflow**

## Viewing Results

### In GitHub Actions

1. Navigate to the **Actions** tab
2. Click on the latest workflow run
3. View the **Architecture Test Results** check
4. Expand test steps to see detailed output

### In Pull Requests

Architecture test results appear automatically in PR checks:
- ✅ Green check = All tests passed
- ❌ Red X = Some tests failed
- 🟡 Yellow dot = Tests running

## Troubleshooting

### Workflow Not Running

**Problem:** Workflow doesn't trigger on push

**Solutions:**
- Ensure you're pushing to `main`, `master`, or `develop`
- Check that `.github/workflows/architecture-tests.yml` exists
- Verify GitHub Actions is enabled in repository settings

### Tests Failing

**Problem:** Architecture tests fail in CI but pass locally

**Solutions:**
- Ensure all dependencies are restored: `dotnet restore`
- Check .NET version matches (8.0.x)
- Review test output in GitHub Actions logs
- See [Architecture Tests Status](Architecture-Tests-Status.md) for known issues

### Badge Not Showing

**Problem:** Badge shows "unknown" or doesn't load

**Solutions:**
- Verify workflow has run at least once
- Check that workflow file name matches badge URL
- Ensure repository is public or you're logged into GitHub
- Replace `YOUR_USERNAME` with actual GitHub username

## Future Enhancements

### Recommended Additions

1. **Code Coverage**
   - Add code coverage reporting
   - Generate coverage badges

2. **Build Workflow**
   - Separate build and test workflows
   - Add deployment steps

3. **Unit Tests**
   - Add unit test workflow
   - Run on every push and PR

4. **Integration Tests**
   - Add integration test workflow
   - Run on schedule or manual trigger

5. **Release Workflow**
   - Automate version tagging
   - Generate release notes
   - Deploy to staging/production

## Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET GitHub Actions](https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net)
- [Architecture Testing Guide](Architecture-Testing-Guide.md)
- [Architecture Tests Status](Architecture-Tests-Status.md)
