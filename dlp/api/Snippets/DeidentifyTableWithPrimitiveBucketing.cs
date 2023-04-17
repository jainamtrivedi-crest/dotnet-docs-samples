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

// [START dlp_deindentify_table_primitive_bucketing]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class DeidentifyTableWithPrimitiveBucketing
{
    public static Table DeidentifyData(string projectId, Table tableToInspect = null)
    {
        // Instantiate dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the table if null.
        var table = tableToInspect;

        if (table == null)
        {
            var row1 = new Value[] { new Value { IntegerValue = 1 }, new Value { IntegerValue = 100 } };
            var row2 = new Value[] { new Value { IntegerValue = 2 }, new Value { IntegerValue = 61 } };
            var row3 = new Value[] { new Value { IntegerValue = 3 }, new Value { IntegerValue = 22 } };

            table = new Table
            {
                Headers = { new FieldId { Name = "user_id" }, new FieldId { Name = "score" } },
                Rows =
                {
                    new Table.Types.Row { Values = { row1 } },
                    new Table.Types.Row { Values = { row2 } },
                    new Table.Types.Row { Values = { row3 } }
                }
            };
        }

        // Construct the table content item.
        var contentItem = new ContentItem { Table = table };

        // Specify how the content should be de-identified.
        var bucketingConfig = new BucketingConfig
        {
            Buckets =
            {
                new BucketingConfig.Types.Bucket
                {
                    Min = new Value { IntegerValue = 0 },
                    Max = new Value { IntegerValue = 25 },
                    ReplacementValue = new Value { StringValue = "Low" }
                },
                new BucketingConfig.Types.Bucket
                {
                    Min = new Value { IntegerValue = 25 },
                    Max = new Value { IntegerValue = 75 },
                    ReplacementValue = new Value { StringValue = "Medium" }
                },
                new BucketingConfig.Types.Bucket
                {
                    Min = new Value { IntegerValue = 75 },
                    Max = new Value { IntegerValue = 100 },
                    ReplacementValue = new Value { StringValue = "High" }
                }
            }
        };

        // Associate the encryption with the specified field.
        var fieldTransformation = new FieldTransformation
        {
            PrimitiveTransformation = new PrimitiveTransformation
            {
                BucketingConfig = bucketingConfig
            }
        };

        // Construct the deidentify config.
        var deidentifyConfig = new DeidentifyConfig
        {
            RecordTransformations = new RecordTransformations
            {
                FieldTransformations = { fieldTransformation }
            }
        };

        // Construct the request.
        var request = new DeidentifyContentRequest
        {
            Parent = new LocationName(projectId, "global").ToString(),
            DeidentifyConfig = deidentifyConfig,
            Item = contentItem,
        };

        // Call the API.
        var response = dlp.DeidentifyContent(request);

        // Inspect the response.
        Console.WriteLine(response.Item.Table);

        return response.Item.Table;
    }
}

// [END dlp_deindentify_table_primitive_bucketing]
