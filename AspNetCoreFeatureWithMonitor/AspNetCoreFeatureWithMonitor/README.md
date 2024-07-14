## 安裝Grafana on Docker
[Grafana Docker](https://grafana.com/docs/grafana/latest/setup-grafana/installation/docker/)
[Prometheus Docker](https://prometheus.io/docs/prometheus/latest/installation/#using-docker)

### 使用docker-compose建立Prometheus和Grafana
```
name: grafana-stack

version: "3.8"
services:
  grafana:
    hostname: grafana
    image: grafana/grafana-enterprise
    container_name: grafana
    restart: always
    volumes:
      - grafana:/var/lib/grafana
    ports:
      - '3000:3000'
  prometheus:
    hostname: prometheus
    image: prom/prometheus
    container_name: prometheus
    restart: always
    environment:
      - ENABLE_FEATURE=otlp-write-receiver
    ports:
      - '9090:9090'
    volumes:
      - prometheus:/etc/prometheus

volumes:
  grafana:
  prometheus: 
```

## 專案安裝Exporter到Prometheus

[ASP.NET CORE與Kestrel遙測](https://learn.microsoft.com/zh-tw/dotnet/core/diagnostics/observability-with-otel#net-implementation-of-opentelemetry)

## 安裝套件
- OpenTelemetry.Exporter.OpenTelemetryProtocol
- OpenTelemetry.Exporter.Prometheus.AspNetCore
- OpenTelemetry.Extensions.Hosting
- OpenTelemetry.Instrumentation.AspNetCore

## 程式碼加入設定

### 遙測資料拋送Prometheus
```
service.AddOpenTelemetry().WithMetrics(opt =>
        {
            opt.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("AspNetCoreFeatureWithMonitor"))
                .AddMeter("AspNetCoreFeatureWithMonitor")
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter()
                .AddPrometheusExporter();
        });
```

#### 啟用端點(要給Prometheus收資料的 預測會是/metrics)
```
app.MapPrometheusScrapingEndpoint();
```

## Prometheus 增加收資料的設定

- 打開prometheus.yml (先前的docker compose有掛載整個資料夾，在資料夾中找到這個檔案)

預設的內容
```
# my global config
global:
  scrape_interval: 15s # Set the scrape interval to every 15 seconds. Default is every 1 minute.
  evaluation_interval: 15s # Evaluate rules every 15 seconds. The default is every 1 minute.
  # scrape_timeout is set to the global default (10s).

# Alertmanager configuration
alerting:
  alertmanagers:
    - static_configs:
        - targets:
          # - alertmanager:9093

# Load rules once and periodically evaluate them according to the global 'evaluation_interval'.
rule_files:
  # - "first_rules.yml"
  # - "second_rules.yml"

# A scrape configuration containing exactly one endpoint to scrape:
# Here it's Prometheus itself.
scrape_configs:
  # The job name is added as a label `job=<job_name>` to any timeseries scraped from this config.
  - job_name: "prometheus"
  
    # metrics_path defaults to '/metrics'
    # scheme defaults to 'http'.

    scrape_interval: 1s # poll very quickly for a more responsive demo
    static_configs:
      - targets: ["host.docker.internal:5288"]
```

在 `scrape_configs` 設定中加入Job的區段
```
- job_name: "prometheus" 
  
    # metrics_path defaults to '/metrics'
    # scheme defaults to 'http'.

    scrape_interval: 1s # poll very quickly for a more responsive demo
    static_configs:
      - targets: ["host.docker.internal:5288"] #這邊Prometheus是在docker內，連到我本地的應用程式，所以使用host.docker.internal指向到本機
```

- 加入設定後再重啟container 去Status => Target確認新加入的Job有成功收集資料