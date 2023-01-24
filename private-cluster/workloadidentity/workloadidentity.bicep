param resourcename string
param location string 

param serviceAccountNamespace string = 'wli'
param serviceAccount string = 'sa-wli'

var name = '${resourcename}-dev'

var akvRoleDefId = '4633458b-17de-408a-b874-0445c86b69e6' // Secrets user

resource akv 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: 'akv-${name}'
}

resource aks 'Microsoft.ContainerService/managedClusters@2022-07-02-preview' existing = {
  name: 'aks-${name}'
}

resource wliumi 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: 'umi-wli-${name}'
  location: location
}

resource wlifederate 'Microsoft.ManagedIdentity/userAssignedIdentities/federatedIdentityCredentials@2022-01-31-preview' = {
  name: 'fe-umi-wli-${name}'
  parent: wliumi
  properties: {
    audiences: [
      'api://AzureADTokenExchange'
    ]
    issuer: aks.properties.oidcIssuerProfile.issuerURL
    subject: 'system:serviceaccount:${serviceAccountNamespace}:${serviceAccount}'
  }
}

resource setAkvRbac 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  scope: akv
  name: guid(wliumi.id, akvRoleDefId, name)
  properties: {
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', akvRoleDefId)
    principalId: wliumi.properties.principalId
    principalType: 'ServicePrincipal'
  }
}


resource secret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: akv
  name: 'wli-secret'
  properties: {
    value: 'This-is-my-WorkloadIdentity-Secret'
  }
}



