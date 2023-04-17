// Copyright 2023 Google Inc.
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

// [START dlp_inspect_gcs_send_to_scc]

using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using static Google.Cloud.Dlp.V2.InspectConfig.Types;

public class CreateGcsJobWithSCCIntegration
{
    public static DlpJob SendGcsData(
        string projectId,
        string gcsUri,
        Likelihood minLikelihood = Likelihood.Unlikely,
        IEnumerable<InfoType> infoTypes = null)
    {
        // Instantiate the dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the storage config.
        var storageConfig = new StorageConfig
        {
            CloudStorageOptions = new CloudStorageOptions
            {
                FileSet = new CloudStorageOptions.Types.FileSet
                {
                    Url = gcsUri
                }
            }
        };

        // Construct the inspect config.
        var inspectConfig = new InspectConfig
        {
            InfoTypes = { infoTypes ?? new InfoType[] { new InfoType { Name = "EMAIL_ADDRESS" }, new InfoType { Name = "PERSON_NAME" } } },
            IncludeQuote = true,
            MinLikelihood = minLikelihood,
            Limits = new FindingLimits
            {
                MaxFindingsPerRequest = 100
            }
        };

        // Construct the SCC action which will be performed after inspecting the source.
        var actions = new Google.Cloud.Dlp.V2.Action[]
        {
            new Google.Cloud.Dlp.V2.Action
            {
                PublishSummaryToCscc = new Google.Cloud.Dlp.V2.Action.Types.PublishSummaryToCscc { }
            }
        };

        // Construct the inspect job config using storage config, inspect config and action.
        var inspectJob = new InspectJobConfig
        {
            StorageConfig = storageConfig,
            InspectConfig = inspectConfig,
            Actions = { actions }
        };

        // Construct the request.
        var request = new CreateDlpJobRequest
        {
            InspectJob = inspectJob,
            Parent = new LocationName(projectId, "global").ToString()
        };

        // Call the API.
        var response = dlp.CreateDlpJob(request);

        // Parse the response.
        Console.WriteLine($"Job Name: {response.Name}");

        return response;
    }
}

// [END dlp_inspect_gcs_send_to_scc]
