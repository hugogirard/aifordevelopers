# Introduction

The purpose of this lab is to show an end to end example of knowledge mining for structured documents (PDF) and leveraging Azure Open AI to consult on your own data.

This sample will provide CI/CD to train a custom model in Document Intelligent, index all documents provided in Azure AI Search and call a custom skillsets that will leverage the trained model in Document Intelligent.

# Architecture

Here a diagram that represent the architecture deployed in this GitHub repository.

![image](./docs/ai-demo.png)

.\modelUtility.exe "CreateModelDocument" "PurchaseOrder" "Purchase Order to extract key information"

# How to deploy the Azure Resources

First, **fork** this repository

NExt, you will need to create some [GitHub repository secrets](https://docs.github.com/en/codespaces/managing-codespaces-for-your-organization/managing-encrypted-secrets-for-your-repository-and-organization-for-codespaces#adding-secrets-for-a-repository) first.  Here the list of secrets you will need to create.

| Secret Name | Value | Link
|-------------|-------|------|
| AZURE_CREDENTIALS | The service principal credentials needed in the Github Action | [GitHub Action](https://github.com/marketplace/actions/azure-login)
| AZURE_SUBSCRIPTION | The subscription ID where the resources will be created |
| PA_TOKEN | Needed to create GitHub repository secret within the GitHub action |  [Github Action](https://github.com/gliech/create-github-secret-action) |
| DATASOURCE_NAME | The name of the datasource that will be created in Azure AI Search (can be dtTest) |
| INDEX_NAME | The name of the index that will be created in Azure AI Search - here write **order** |
| INDEXER_NAME | The name of the indexer that will be created in Azure AI Search - here write **indexer** |

## Run Create Azure Resources GitHub Action

Now you can go to the Actions tab and Run the **Create Azure Resources** [GitHub Actions](https://docs.github.com/en/actions).

## Create more GitHub Action Secrets

Once this is done you will need to create more GitHub Actions Secrets.  You need to retrieve some values in the Azure Portal.

# REST API Azure Search Data Plane

https://learn.microsoft.com/en-us/rest/api/searchservice/operation-groups?view=rest-searchservice-2023-11-01
