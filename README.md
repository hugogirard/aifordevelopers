# Introduction

The purpose of this lab is to show an end to end example of knowledge mining for structured documents (PDF) and leveraging Azure Open AI to consult on your own data.

This sample will provide CI/CD to train a custom model in Document Intelligent, index all documents provided in Azure AI Search and call a custom skillsets that will leverage the trained model in Document Intelligent.


![image](./docs/ai-demo.png)

.\modelUtility.exe "CreateModelDocument" "PurchaseOrder" "Purchase Order to extract key information"


# REST API Azure Search Data Plane

https://learn.microsoft.com/en-us/rest/api/searchservice/operation-groups?view=rest-searchservice-2023-11-01
