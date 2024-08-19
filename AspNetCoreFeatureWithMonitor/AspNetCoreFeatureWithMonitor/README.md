## 安裝Grafana on Docker
[Grafana Docker](https://grafana.com/docs/grafana/latest/setup-grafana/installation/docker/)
[Prometheus Docker](https://prometheus.io/docs/prometheus/latest/installation/#using-docker)
[Loki](https://grafana.com/docs/loki/latest/setup/install/docker/)


### 使用docker-compose建立 Prometheus、Grafana、Loki

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
  local-sql-server:
      hostname: local-host-sql
      container_name: local-sql-server
      restart: always
      image: mcr.microsoft.com/mssql/server:2022-latest
      ports:
        - '1433:1433'
      environment:
        - ACCEPT_EULA=Y
        - SA_PASSWORD=Aa123456
  loki:
    image: grafana/loki:3.0.0
    hostname: loki
    container_name: loki
    ports:
      - "3100:3100"
    volumes:
      - loki:/mnt/config
    command:
      - "-config.file=/mnt/config/loki-config.yaml"
volumes:
  grafana:
  prometheus:
  loki:
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

ex: \docker-desktop-data\data\docker\volumes\grafana-stack_prometheus\_data

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

    static_configs:
      - targets: ["localhost:9090"]
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


## loki容器設定

1. 將專案內的loki-config.yml檔案放置docker volumn掛載出來的位置
ex: {host}\docker-desktop-data\data\docker\volumes\grafana-stack_loki\_data)

2. 放置完後需要啟動loki container

## 專案中設定Log寫入至loki(使用Serilog)
1. [安裝Serilog to loki](https://github.com/serilog-contrib/serilog-sinks-grafana-loki)
2. 加入Serilog WriteTo的設定

```
...
{
  "Name": "GrafanaLoki",
  "Args": {
    "uri": "http://localhost:3100",
    "labels": [
      {
        "key": "app",
        "value": "AspNetCoreFeatureWithMonitor"
      }
    ],
    "propertiesAsLabels": [
      "app"
    ]
  }
}
...
```






