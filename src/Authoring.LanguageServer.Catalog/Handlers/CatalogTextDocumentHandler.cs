using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Authoring.LanguageServer.Catalog.Handlers;

internal sealed class CatalogTextDocumentHandler : TextDocumentSyncHandlerBase
{
    private readonly ILanguageServerFacade _facade;

    public CatalogTextDocumentHandler(ILanguageServerFacade facade) => _facade = facade;

    public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Full;

    public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri) =>
        new(uri, "catalog");

    protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(
        TextSynchronizationCapability capability,
        ClientCapabilities clientCapabilities) =>
        new()
        {
            DocumentSelector = CatalogLspHelpers.DocumentSelector,
            Change = Change,
            Save = new SaveOptions { IncludeText = true },
        };

    public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken) =>
        PublishAsync(request.TextDocument.Uri, request.TextDocument.Text);

    public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
    {
        var text = request.ContentChanges.LastOrDefault()?.Text ?? string.Empty;
        return PublishAsync(request.TextDocument.Uri, text);
    }

    public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken) =>
        PublishAsync(request.TextDocument.Uri, request.Text);

    public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken) =>
        Unit.Task;

    private Task<Unit> PublishAsync(DocumentUri uri, string? text)
    {
        _facade.TextDocument.PublishDiagnostics(
            new PublishDiagnosticsParams
            {
                Uri = uri,
                Diagnostics = new Container<Diagnostic>(CatalogStubDiagnostics.Analyze(uri, text)),
            });

        return Unit.Task;
    }
}
