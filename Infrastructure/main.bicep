param location string = resourceGroup().location
param sqlServerName string
param sqlAdminUser string
@secure()
param sqlAdminPassword string
param databaseName string
param appServiceName string
param apiServiceName string
@secure()
param jwtSecretKey string
param frontendServiceName string

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminUser
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
  }
}

// Database
resource sqlDb 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
}

// Firewall
resource allowAzure 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// App Service Plan (shared)
resource plan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: '${appServiceName}-plan'
  location: resourceGroup().location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
}

// API Web App
resource apiApp 'Microsoft.Web/sites@2022-03-01' = {
  name: apiServiceName
  location: resourceGroup().location
  properties: {
    serverFarmId: plan.id
    siteConfig: {
      appSettings: [
        {
          name: 'JwtSettings__SecretKey'
          value: jwtSecretKey
        }
        {
          name: 'ConnectionStrings__DefaultConnection'
          value: 'Server=tcp:${sqlServer.name}.database.windows.net,1433;Initial Catalog=${databaseName};User ID=${sqlAdminUser};Password=${sqlAdminPassword};Encrypt=True;'
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
      ]
      ipSecurityRestrictions: [
        {
          ipAddress: 'AppService'
          tag: 'ServiceTag'
          action: 'Allow'
          priority: 200
          name: 'AllowAzureAppServices'
        }
        {
          ipAddress: 'Any'
          action: 'Deny'
        }
      ]
    }
  }
}

// Frontend Web App
resource frontendApp 'Microsoft.Web/sites@2022-03-01' = {
  name: frontendServiceName
  location: resourceGroup().location
  properties: {
    serverFarmId: plan.id
    siteConfig: {
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
      ]
    }
  }
}
