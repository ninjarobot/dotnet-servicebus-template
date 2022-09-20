import React, {Children, Component} from 'react'

export function Template({childrenContent, asyncapi}) {
    return `{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "comment": "${ asyncapi.info().title() }",
  "contentVersion": "1.0.0.0",
  "outputs": {},
  "parameters": {},
  "resources": [
    ${ childrenContent }
  ]
}`
}