# FindTheBug API - Grafana Dashboard Setup

## Dashboard Overview

I've created a comprehensive Grafana dashboard (`grafana-dashboard.json`) that monitors your FindTheBug API with the following panels:

### Metrics Displayed

1. **Request Rate Gauge** - Real-time requests per second
2. **HTTP Requests Over Time** - Time series of all HTTP requests
3. **Request Duration (Percentiles)** - p50, p90, p99 latency metrics
4. **HTTP Status Codes Distribution** - Pie chart showing response codes
5. **Memory Usage** - Working set and private memory consumption
6. **CPU Usage** - Processor utilization percentage
7. **Requests by Endpoint** - Bar chart showing traffic per route

## How to Import the Dashboard

### Step 1: Access Grafana
1. Open your browser and navigate to: http://localhost:3000
2. Login with credentials: `admin` / `admin`
3. (Optional) Change the password when prompted

### Step 2: Add Prometheus Data Source
1. Click on **Configuration** (gear icon) → **Data Sources**
2. Click **Add data source**
3. Select **Prometheus**
4. Configure:
   - **Name**: `prometheus`
   - **URL**: `http://prometheus:9090`
5. Click **Save & Test**

### Step 3: Import Dashboard
1. Click on **Dashboards** (four squares icon) → **Import**
2. Click **Upload JSON file**
3. Select the `grafana-dashboard.json` file from the project root
4. Click **Import**

Alternatively, you can copy-paste the JSON content directly into the import dialog.

## Dashboard Features

- **Auto-refresh**: Updates every 5 seconds
- **Time range**: Last 15 minutes (adjustable)
- **Dark theme**: Professional appearance
- **Interactive**: Click on legends to filter metrics
- **Responsive**: Adapts to different screen sizes

## Available Prometheus Metrics

The dashboard uses these metrics from your API:

- `http_requests_received_total` - Total HTTP requests
- `http_request_duration_seconds` - Request latency histogram
- `process_working_set_bytes` - Memory usage
- `process_private_memory_bytes` - Private memory
- `process_cpu_seconds_total` - CPU time

## Customization

You can customize the dashboard by:
1. Clicking the panel title → **Edit**
2. Modifying queries, thresholds, or visualization settings
3. Adding new panels with additional metrics
4. Saving your changes

## Troubleshooting

**No data showing?**
- Ensure docker-compose services are running
- Verify Prometheus is scraping: http://localhost:9090/targets
- Check API is exposing metrics: http://localhost:9909/metrics

**Connection errors?**
- Verify Prometheus data source URL is `http://prometheus:9090` (not localhost)
- Ensure all containers are on the same Docker network
