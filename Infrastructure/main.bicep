param location string = resourceGroup().location
param sqlServerName string
param sqlAdminUser string
@secure()
param sqlAdminPassword string
param databaseName string
param appServiceName string

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

// // App Service Plan
// resource plan 'Microsoft.Web/serverfarms@2022-03-01' = {
//   name: '${appServiceName}-plan'
//   location: location
//   sku: {
//     name: 'B1'
//     tier: 'Basic'
//   }
// }

// // Web App
// resource app 'Microsoft.Web/sites@2022-03-01' = {
//   name: appServiceName
//   location: location
//   properties: {
//     serverFarmId: plan.id
//   }
// }
