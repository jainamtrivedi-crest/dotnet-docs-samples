// Copyright (c) 2023 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START dlp_deidentify_table_with_multiple_crypto_hash]

using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class DeidentifyTableWithTwoCryptoHash
{
    public static Table Deidentify(
        string projectId,
        Table tableToDeidentify = null,
        IEnumerable<InfoType> infoTypes = null)
    {
        // Instantiate the client.
        var dlp = DlpServiceClient.Create();

        // Construct the table if null.
        var table = tableToDeidentify;

        if (table == null)
        {
            var row1 = new Value[] { new Value { StringValue = "user1@example.org" }, new Value { StringValue = "my email is user1@example.org and phone is 858-555-0222" } };
            var row2 = new Value[] { new Value { StringValue = "abbyabernathy1" }, new Value { StringValue = "my userid is abbyabernathy1 and my email is aabernathy@example.com" } };

            table = new Table
            {
                Headers = { new FieldId { Name = "User ID" }, new FieldId { Name = "comments" } },
                Rows =
                {
                    new Table.Types.Row { Values = { row1 } },
                    new Table.Types.Row { Values = { row2 } }
                }
            };
        }

        // Construct the table content item.
        var contentItem = new ContentItem { Table = table };

        // Construct the crypto hash config for primitive transformation using transient crypto key.
        var cryptoHashConfig1 = new CryptoHashConfig
        {
            CryptoKey = new CryptoKey
            {
                Transient = new TransientCryptoKey
                {
                    Name = "[TRANSIENT-CRYPTO-KEY-1]"
                }
            }
        };

        // Construct the crypto hash config for infotype transformation using transient crypto key.
        var cryptoHashConfig2 = new CryptoHashConfig
        {
            CryptoKey = new CryptoKey
            {
                Transient = new TransientCryptoKey
                {
                    Name = "[TRANSIENT-CRYPTO-KEY-2]"
                }
            }
        };

        // Construct the deidentify config using crypto hash configs.
        var deidentifyConfig = new DeidentifyConfig
        {
            RecordTransformations = new RecordTransformations
            {
                FieldTransformations =
                {
                    new FieldTransformation
                    {
                        Fields = { new FieldId[] { new FieldId { Name = "User ID" } } },
                        PrimitiveTransformation = new PrimitiveTransformation
                        {
                            CryptoHashConfig = cryptoHashConfig1
                        }
                    },
                    new FieldTransformation
                    {
                        Fields = { new FieldId[] { new FieldId { Name = "comments" } } },
                        InfoTypeTransformations =  new InfoTypeTransformations
                        {
                            Transformations =
                            {
                                new InfoTypeTransformations.Types.InfoTypeTransformation
                                {
                                    PrimitiveTransformation = new PrimitiveTransformation
                                    {
                                        CryptoHashConfig = cryptoHashConfig2
                                    },
                                    InfoTypes = { infoTypes ?? new InfoType[] { new InfoType { Name = "EMAIL_ADDRESS" }, new InfoType { Name = "PHONE_NUMBER" } } }
                                }
                            }
                        }
                    }
                }
            }
        };

        // Construct the inspect config.
        var inspectConfig = new InspectConfig
        {
            InfoTypes = { infoTypes ?? new InfoType[] { new InfoType { Name = "EMAIL_ADDRESS" }, new InfoType { Name = "PHONE_NUMBER" } } },
            IncludeQuote = true
        };

        // Construct the request.
        var request = new DeidentifyContentRequest
        {
            Parent = new LocationName(projectId, "global").ToString(),
            DeidentifyConfig = deidentifyConfig,
            Item = contentItem,
            InspectConfig = inspectConfig
        };

        // Call the API.
        var response = dlp.DeidentifyContent(request);

        // Inspect the table.
        Console.WriteLine(response.Item.Table);

        return response.Item.Table;
    }
}

// [END dlp_deidentify_table_with_multiple_crypto_hash]
