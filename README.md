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
