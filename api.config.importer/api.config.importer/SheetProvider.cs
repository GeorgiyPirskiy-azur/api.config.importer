using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace api.config.importer
{
    internal sealed class SheetProvider : ISheetProvider
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly Dictionary<string, ISheet> _sheets = new Dictionary<string, ISheet>();
        private readonly string _spreadsheetId;
        private readonly SheetsService _sheetsService;

        private bool _disposed;

        public SheetProvider(string spreadsheetId, SheetsService sheetsService)
        {
            _spreadsheetId = spreadsheetId;
            _sheetsService = sheetsService;
        }

        private async Task<ISheet> GetSheetAsync(
            string name,
            CancellationToken token)
        {
            var sheetNotFound = !_sheets.TryGetValue(name, out var sheet);
            if (sheetNotFound)
            {
                using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token, token);
                sheet = await LoadSheetAsync(
                    name,
                    _spreadsheetId,
                    _sheetsService,
                    tokenSource.Token);
                _sheets.Add(name, sheet);
            }
            return sheet;
        }

        private void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _tokenSource.Cancel();
            _sheetsService.Dispose();
        }

        private static async Task<ISheet> LoadSheetAsync(
            string name,
            string spreadsheetId,
            SheetsService sheetsService,
            CancellationToken token)
        {
            var request = sheetsService.Spreadsheets.Values.Get(spreadsheetId, name);
            request.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.MajorDimensionEnum.ROWS;
            request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE;
            var response = await request.ExecuteAsync(token);
            return new Sheet(name, response.Values);
        }

        #region ISheetProvider

        Task<ISheet> ISheetProvider.GetSheet(string name, CancellationToken token)
        {
            return GetSheetAsync(name, token);
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose()
        {
            Dispose();
        }

        #endregion
    }

    public static partial class Factory
    {
        #region Constants

        private const string ApplicationName = "Google Sheets API .NET Quickstart";

        #endregion

        private static readonly string[] _scopes = { SheetsService.Scope.SpreadsheetsReadonly };

        public static async Task<ISheetProvider> CreateSheetProviderAsync(
            string spreadsheetId,
            string userName,
            string credentialFileName,
            string accessTokenPath,
            CancellationToken token)
        {
            var secrets = GoogleClientSecrets.FromFile(credentialFileName).Secrets;
            var fileDataStore = new FileDataStore(accessTokenPath, true);
            var credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                _scopes,
                userName,
                token,
                fileDataStore);
            var initializer = new BaseClientService.Initializer()
            {
                ApplicationName = ApplicationName,
                HttpClientInitializer = credentials
            };
            var sheetsService = new SheetsService(initializer);
            return new SheetProvider(spreadsheetId, sheetsService);
        }
    }
}
