﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Immutable
Imports System.Threading
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Completion.Providers
Imports Microsoft.CodeAnalysis.Options
Imports Microsoft.CodeAnalysis.Shared.Extensions.ContextQuery
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Completion.Providers
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Microsoft.CodeAnalysis.VisualBasic.Completion.Providers
    Friend NotInheritable Class ExtensionMethodImportCompletionProvider
        Inherits AbstractExtensionMethodImportCompletionProvider

        Friend Overrides Function IsInsertionTrigger(text As SourceText, characterPosition As Integer, options As OptionSet) As Boolean
            Return CompletionUtilities.IsDefaultTriggerCharacterOrParen(text, characterPosition, options)
        End Function

        Protected Overrides Function CreateContextAsync(document As Document, position As Integer, cancellationToken As CancellationToken) As Task(Of SyntaxContext)
            Return ImportCompletionProviderHelper.CreateContextAsync(document, position, cancellationToken)
        End Function

        Protected Overrides Function GetImportedNamespaces(location As SyntaxNode, semanticModel As SemanticModel, cancellationToken As CancellationToken) As ImmutableArray(Of String)
            Return ImportCompletionProviderHelper.GetImportedNamespaces(location, semanticModel, cancellationToken)
        End Function

        Protected Overrides Function IsInImportsDirectiveAsync(document As Document, position As Integer, cancellationToken As CancellationToken) As Task(Of Boolean)
            Return ImportCompletionProviderHelper.IsInImportsDirectiveAsync(document, position, cancellationToken)
        End Function

        Protected Overrides Function TryGetReceiverTypeSymbol(syntaxContext As SyntaxContext, ByRef receiverTypeSymbol As ITypeSymbol) As Boolean
            If syntaxContext.TargetToken.Parent.Kind = SyntaxKind.SimpleMemberAccessExpression Then
                Dim memberAccessNode = CType(syntaxContext.TargetToken.Parent, MemberAccessExpressionSyntax)

                Dim symbol = syntaxContext.SemanticModel.GetSymbolInfo(memberAccessNode.Expression).Symbol

                If symbol Is Nothing Or symbol.Kind <> SymbolKind.NamedType And symbol.Kind <> SymbolKind.TypeParameter Then
                    receiverTypeSymbol = syntaxContext.SemanticModel.GetTypeInfo(memberAccessNode.Expression).Type
                    Return receiverTypeSymbol IsNot Nothing
                End If
            End If

            receiverTypeSymbol = Nothing
            Return False
        End Function
    End Class
End Namespace
