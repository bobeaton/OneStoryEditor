using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AiChorus
{
    public class GoogleSheetHandler
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "OseProjectData";

        public static void UpdateGoogleSheet(Dictionary<string, List<OseProjectData>> mapProjectsToStoryInfo, string spreadsheetId, 
                                             string sheetsClientId, string sheetsClientSecret)
        {
            var service = Authenticate(sheetsClientId, sheetsClientSecret);

            var oseProjectDataRows = mapProjectsToStoryInfo.Values.SelectMany(l => l).ToList();
            var firstOseProjectDataRow = oseProjectDataRows.First();

            // the maximal range would be the max # of columns and who knows how many rows.
            //  But if the user has their own columns interspersed in between, there might be
            //  more. So let's go with A-Z and then remove any at the end that are blank
            var range = $"{typeof(OseProjectData).Name}!A1:Z";
            var getRequest = service.Spreadsheets.Values.Get(spreadsheetId, range);
            var getResponse = getRequest.Execute();

            var numOfRows = getResponse.Values.Count;
            if (numOfRows <= 0)
            {
                throw new ApplicationException($"Now header row found! You want something like: {firstOseProjectDataRow.KeyToString()} (or ignore ones you don't care about). Then we can fill the spreadsheet with the data for whichever columns you include.");
            }

            var header = getResponse.Values.First();
            var highestColumnFound = DetermineColumnRange(firstOseProjectDataRow, header);
            int numOfRowsInUpdate = oseProjectDataRows.Count + 1;   // add 1 for the header
            if (numOfRows > numOfRowsInUpdate)
            {
                // this means that there are more rows in the current spreadsheet than what we will be overwriting, 
                //  so we have to clobber the other rows below it
                range = $"{typeof(OseProjectData).Name}!A{numOfRowsInUpdate}:{(char)('A' + highestColumnFound)}{numOfRows}";
                Console.WriteLine($"We'll want to clear this range: {range}");
                var clearRequest = service.Spreadsheets.Values.Clear(null, spreadsheetId, range);
                var clearResponse = clearRequest.Execute();
            }

            // now we can set the data
            var updateData = GenerateUpdateValueRange(header, highestColumnFound, oseProjectDataRows);
            var requestBody = new BatchUpdateValuesRequest
            {
                ValueInputOption = "USER_ENTERED",
                Data = updateData
            };

            var updateRequest = service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);
            var updateResponse = updateRequest.Execute();
        }

        private static List<ValueRange> GenerateUpdateValueRange(IList<object> header, 
                                        int highestColumnFound, List<OseProjectData> oseProjectDataRows)
        {
            var firstOseProjectDataRow = oseProjectDataRows.First();
            var valueUpdates = new List<ValueRange>();

            // to leave open the possibility that the user might have their own columns between ours, we might
            //  have to do the ranges in chunks (e.g. A1:B87, D1:G87, etc)
            int i = 0, firstColumn = 0;
            for (; i <= highestColumnFound; i++)
            {
                var accumulatingColumns = false;
                for (; i < header.Count; i++)
                {
                    var columnName = header[i].ToString();
                    if (!firstOseProjectDataRow.ContainsKey(columnName))
                    {
                        if (!accumulatingColumns)
                            firstColumn++;
                        break;
                    }
                    else
                        accumulatingColumns = true;
                }

                if (i <= firstColumn)
                    continue;
                else
                    i--;

                var range = $"{typeof(OseProjectData).Name}!{(char)('A' + firstColumn)}2:{(char)('A' + i)}{oseProjectDataRows.Count + 1}";
                var rows = new List<IList<object>>();

                foreach (var oseProjectDataRow in oseProjectDataRows)
                {
                    var values = new List<object>();
                    for (var ii = firstColumn; ii <= i; ii++)
                    {
                        var columnName = header[ii].ToString();
                        var value = oseProjectDataRow[columnName];
                        values.Add(value);
                    }
                    rows.Add(values);
                }

                var valueUpdate = new ValueRange { Range = range, Values = rows };
                valueUpdates.Add(valueUpdate);
                firstColumn = i + 1;
            }

            return valueUpdates;
        }

        private static int DetermineColumnRange(OseProjectData firstOseProjectDataRow, IList<object> header)
        {
            int highestColumnFound = -1;
            for (var i = 0; i < header.Count; i++)
            {
                // if the header is blank, then we're done
                if (header[i] == null)
                    break;

                // otherwise, let's see if the columnName is one we know about
                var columnName = header[i].ToString();
                if (firstOseProjectDataRow.ContainsKey(columnName))
                {
                    // this is a column that we're going to overwrite with our new data... so let's clear out all the old data
                    highestColumnFound = i;
                }
                else
                {
                    Console.WriteLine($"Encountered Unknown column name {columnName}, which may or may not be a mistake! You want the columns to have names like: {firstOseProjectDataRow.KeyToString()} (or ignore ones you don't care about). Then we can fill the spreadsheet with the data for whichever columns you include.");
                }
            }

            if (highestColumnFound < 0) // this might mean that the user doesn't have the spreadsheet configured properly
                throw new ApplicationException($"Encountered Unknown column names {String.Join(",", header)}! You want the columns to have names like: {firstOseProjectDataRow.KeyToString()} (or ignore ones you don't care about). Then we can fill the spreadsheet with the data for whichever columns you include.");

            return highestColumnFound;
        }

        private static SheetsService Authenticate(string sheetsClientId, string sheetsClientSecret)
        {
            string credPath = "OseProjectDataToken.json";
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = sheetsClientId,
                ClientSecret = sheetsClientSecret
            }, Scopes, Environment.UserName, CancellationToken.None, new FileDataStore(credPath, true)).Result;
            Console.WriteLine("Credential file saved to: " + credPath);

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }
    }
}