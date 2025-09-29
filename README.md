## Configuração do appsettings.json

O arquivo `appsettings.json` contém as configurações de banco, Identity e outros parâmetros do aplicativo.

> **Importante:** Nunca commit o `appsettings.json` com senhas reais. Use valores de teste ou variáveis de ambiente.

### Exemplo de template:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=db;port=3306;database=mydb;user=root;password=root"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

# Projeto API de Cursos (.NET Core)

Este projeto é uma **API REST em .NET Core** para gerenciar cursos.
O ambiente utiliza **Docker Compose** para subir a aplicação junto com serviços de suporte.

## Serviços

* **API** — aplicação principal em .NET Core.
* **MySQL** — banco de dados relacional.
* **Redis** — cache em memória.
* **LocalStack** — simulação de serviços AWS (ex.: SQS, S3).

## Pré-requisitos

* [Docker](https://docs.docker.com/get-docker/)
* [Docker Compose](https://docs.docker.com/compose/install/)
### Opcional - para rodar localmente sem container
* [SDK .NET Core 8.0](https://dotnet.microsoft.com/download)
* renomear appsettings.example.json para appsettings.json

## Subindo o ambiente

```bash
docker compose up -d
```

Isso iniciará todos os containers necessários.

## Acesso aos serviços

* **API**: [http://localhost:8080](http://localhost:8080)
* **Swagger**: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)
* **MySQL**: localhost:3306 (`user: root`, `password: root`)
* **Redis**: localhost:6379
* **LocalStack**: [http://localhost:4566](http://localhost:4566)

## Inicialização do banco de dados

Migrations roda automaticamente ao subir o container da API:

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
