# Projeto API de Cursos (.NET Core)

Este projeto é uma **API REST em .NET Core** para gerenciar cursos.
O ambiente utiliza **Docker Compose** para subir a aplicação junto com serviços de suporte.

## Serviços

* **API** — aplicação principal em .NET Core.
* **MySQL** — banco de dados relacional.
* **Redis** — cache em memória.
* **LocalStack** — simulação de serviços AWS (ex.: SQS, S3).
* **Prometheus** — métricas e monitoramento.
* **Grafana** — visualizador de dados - dashboards de monitoramento.

## Pré-requisitos

* [Docker](https://docs.docker.com/get-docker/)
* [Docker Compose](https://docs.docker.com/compose/install/)
### Opcional - para rodar localmente sem container
* [SDK .NET Core 8.0](https://dotnet.microsoft.com/download)
* renomear appsettings.example.json para appsettings.json

## Webhook ou Worker ?

Para usar webhook de pagamentos configure APP_USE_WEBHOOK=true no docker-compose.yml

Se APP_USE_WEBHOOK=false, serão usados:
 - SqsService -> simula fila (SQS) através do container localstack
 - PaymentWorker : BackgroundService -> consome as mensagens da fila

## Subindo o ambiente

```bash
docker compose up -d
```

Isso iniciará todos os containers necessários.

## Acesso aos serviços

* **Swagger**: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)
* **MySQL**: localhost:3306 (`user: root`, `password: root`)
* **Redis**: localhost:6379
* **LocalStack**: [http://localhost:4566](http://localhost:4566)
* **Prometheus**: [http://localhost:9090](http://localhost:9090)
* **Grafana**: [http://localhost:3000](http://localhost:3000)
  - *Dashboard de Cache*: [http://localhost:3000/d/ae4ccf58-5a46-44ba-8f8d-0b15d2912b80/cache-number?orgId=1&refresh=5s](http://localhost:3000/d/ae4ccf58-5a46-44ba-8f8d-0b15d2912b80/cache-number?orgId=1&refresh=5s)
## Inicialização do banco de dados

Migrations roda automaticamente ao subir o container da API.

## Testando a API

Exemplo de requisição:

```bash
curl http://localhost:8080/courses
```

## Executando testes

Rode os testes localmente no seu host

```bash
dotnet test
```

## Encerrando os containers

```bash
docker compose down
```

Isso remove os containers, mas mantém volumes e dados persistidos.
