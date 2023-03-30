﻿// Copyright 2023 Google Inc.
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

// [START dlp_deidentify_redact]

using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class DeidentifyDataUsingRedactWithMatchedInputValues
{
    public static DeidentifyContentResponse Deidentify(
        string projectId,
        string text,
        IEnumerable<InfoType> infoTypes = null)
    {
        // Instantiate the client.
        var dlp = DlpServiceClient.Create();

        // Construct inspect config.
        var inspectConfig = new InspectConfig
        {
            InfoTypes = { infoTypes ?? new InfoType[] { new InfoType { Name = "EMAIL_ADDRESS" } } },
        };

        // Construct redact config.
        var redactConfig = new RedactConfig();

        // Construct deidentify config using redact config.
        var deidentifyConfig = new DeidentifyConfig
        {
            InfoTypeTransformations = new InfoTypeTransformations
            {
                Transformations =
                {
                    new InfoTypeTransformations.Types.InfoTypeTransformation
                    {
                        PrimitiveTransformation = new PrimitiveTransformation
                        {
                            RedactConfig = redactConfig
                        }
                    }
                }
            }
        };

        // Construct a request.
        var request = new DeidentifyContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            DeidentifyConfig = deidentifyConfig,
            InspectConfig = inspectConfig,
            Item = new ContentItem { Value = text }
        };

        // Call the API.
        var response = dlp.DeidentifyContent(request);

        // Check the deidentified content.
        Console.WriteLine($"Deidentified content: {response.Item.Value}");
        return response;
    }
}

// [END dlp_deidentify_redact]
